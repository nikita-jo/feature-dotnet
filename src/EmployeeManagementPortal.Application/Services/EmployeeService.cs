using EmployeeManagementPortal.Application.Common;
using EmployeeManagementPortal.Application.DTOs;
using EmployeeManagementPortal.Application.Interfaces;
using EmployeeManagementPortal.Application.Mappers;
using EmployeeManagementPortal.Application.Validators;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace EmployeeManagementPortal.Application.Services;

/// <summary>
/// Orchestrates employee use cases: validates input, enforces uniqueness,
/// delegates persistence to <see cref="IEmployeeRepository"/>, and logs key events.
/// </summary>
public sealed class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _repository;
    private readonly IValidator<CreateEmployeeDto> _createValidator;
    private readonly IValidator<UpdateEmployeeDto> _updateValidator;
    private readonly ILogger<EmployeeService> _logger;
    private readonly TimeProvider _timeProvider;

    public EmployeeService(
        IEmployeeRepository repository,
        IValidator<CreateEmployeeDto> createValidator,
        IValidator<UpdateEmployeeDto> updateValidator,
        ILogger<EmployeeService> logger,
        TimeProvider timeProvider)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _createValidator = createValidator ?? throw new ArgumentNullException(nameof(createValidator));
        _updateValidator = updateValidator ?? throw new ArgumentNullException(nameof(updateValidator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
    }

    /// <inheritdoc />
    public async Task<Result<EmployeeDto>> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<EmployeeDto>.Failure("Employee id must be greater than zero.");
        }

        var entity = await _repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (entity is null)
        {
            return Result<EmployeeDto>.Failure($"Employee with id {id} was not found.");
        }

        return Result<EmployeeDto>.Success(EmployeeMapper.ToDto(entity));
    }

    /// <inheritdoc />
    public async Task<Result<IReadOnlyList<EmployeeDto>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAllAsync(cancellationToken).ConfigureAwait(false);
        IReadOnlyList<EmployeeDto> dtos = entities.Select(EmployeeMapper.ToDto).ToList();
        return Result<IReadOnlyList<EmployeeDto>>.Success(dtos);
    }

    /// <inheritdoc />
    public async Task<Result<EmployeeDto>> CreateAsync(CreateEmployeeDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var validation = await _createValidator.ValidateAsync(dto, cancellationToken).ConfigureAwait(false);
        if (!validation.IsValid)
        {
            return Result<EmployeeDto>.Failure(validation.Errors.Select(e => e.ErrorMessage).ToArray());
        }

        var existingByEmail = await _repository.GetByEmailAsync(dto.Email, cancellationToken).ConfigureAwait(false);
        if (existingByEmail is not null)
        {
            return Result<EmployeeDto>.Failure("An employee with this email already exists.");
        }

        var existingByCode = await _repository.GetByEmployeeCodeAsync(dto.EmployeeCode, cancellationToken)
            .ConfigureAwait(false);
        if (existingByCode is not null)
        {
            return Result<EmployeeDto>.Failure("An employee with this code already exists.");
        }

        var entity = EmployeeMapper.ToEntity(dto);
        entity.CreatedAt = _timeProvider.GetUtcNow().UtcDateTime;

        var saved = await _repository.AddAsync(entity, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Employee created. Id={Id}, Code={Code}", saved.Id, saved.EmployeeCode);

        return Result<EmployeeDto>.Success(EmployeeMapper.ToDto(saved));
    }

    /// <inheritdoc />
    public async Task<Result<EmployeeDto>> UpdateAsync(UpdateEmployeeDto dto, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dto);

        var validation = await _updateValidator.ValidateAsync(dto, cancellationToken).ConfigureAwait(false);
        if (!validation.IsValid)
        {
            return Result<EmployeeDto>.Failure(validation.Errors.Select(e => e.ErrorMessage).ToArray());
        }

        var entity = await _repository.GetByIdAsync(dto.Id, cancellationToken).ConfigureAwait(false);
        if (entity is null)
        {
            return Result<EmployeeDto>.Failure($"Employee with id {dto.Id} was not found.");
        }

        var emailOwner = await _repository.GetByEmailAsync(dto.Email, cancellationToken).ConfigureAwait(false);
        if (emailOwner is not null && emailOwner.Id != dto.Id)
        {
            return Result<EmployeeDto>.Failure("Another employee already uses this email.");
        }

        var codeOwner = await _repository.GetByEmployeeCodeAsync(dto.EmployeeCode, cancellationToken)
            .ConfigureAwait(false);
        if (codeOwner is not null && codeOwner.Id != dto.Id)
        {
            return Result<EmployeeDto>.Failure("Another employee already uses this code.");
        }

        EmployeeMapper.ApplyUpdate(entity, dto);
        entity.UpdatedAt = _timeProvider.GetUtcNow().UtcDateTime;

        await _repository.UpdateAsync(entity, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Employee updated. Id={Id}", entity.Id);

        return Result<EmployeeDto>.Success(EmployeeMapper.ToDto(entity));
    }

    /// <inheritdoc />
    public async Task<Result<bool>> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        if (id <= 0)
        {
            return Result<bool>.Failure("Employee id must be greater than zero.");
        }

        var entity = await _repository.GetByIdAsync(id, cancellationToken).ConfigureAwait(false);
        if (entity is null)
        {
            return Result<bool>.Failure($"Employee with id {id} was not found.");
        }

        await _repository.DeleteAsync(entity, cancellationToken).ConfigureAwait(false);
        _logger.LogInformation("Employee deleted. Id={Id}", id);

        return Result<bool>.Success(true);
    }
}