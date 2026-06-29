using EmployeeManagementPortal.Application.Common;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace EmployeeManagementPortal.Web.Extensions;

/// <summary>
/// Helpers that bridge Application-layer <see cref="Result{T}"/> failures to MVC ModelState.
/// Keeps controllers thin and consistent.
/// </summary>
public static class ResultExtensions
{
    /// <summary>
    /// Copies each error in the result into ModelState under the supplied key prefix.
    /// Returns true if any errors were added (useful for guarding re-render).
    /// </summary>
    public static bool AddErrorsToModelState<T>(this Result<T> result, ModelStateDictionary modelState, string keyPrefix = "")
    {
        ArgumentNullException.ThrowIfNull(modelState);

        if (result.IsSuccess || result.Errors.Count == 0)
        {
            return false;
        }

        foreach (var error in result.Errors)
        {
            modelState.AddModelError(string.IsNullOrWhiteSpace(keyPrefix) ? string.Empty : keyPrefix, error);
        }

        return true;
    }
}