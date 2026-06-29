# Employee Management Portal — Architecture

This project follows **Clean Architecture** with strict separation of concerns. Dependencies only
point inward (Infrastructure → Application → Domain), keeping the Domain layer free of frameworks.

## High-level overview

```mermaid
flowchart LR
    subgraph Presentation
        A[EmployeeManagementPortal.Web<br/>MVC Controllers + Razor Views]
    end
    subgraph Application
        B[EmployeeManagementPortal.Application<br/>Services, DTOs, Validators, Mappers]
    end
    subgraph Domain
        C[EmployeeManagementPortal.Domain<br/>Entities + Base Entity]
    end
    subgraph Infrastructure
        D[EmployeeManagementPortal.Infrastructure<br/>EF Core DbContext + Repositories]
    end
    A --> B
    B --> C
    D --> B
    D --> C
    D -.SQL Server / LocalDB.-> E[(Database)]
```

## Layer responsibilities

| Layer | Project | Responsibility |
|---|---|---|
| Domain | `EmployeeManagementPortal.Domain` | Pure entities and base classes; zero dependencies. |
| Application | `EmployeeManagementPortal.Application` | Use cases, DTOs, validators, mappers, service contracts. |
| Infrastructure | `EmployeeManagementPortal.Infrastructure` | EF Core `DbContext`, repository implementations, dependency wiring. |
| Web | `EmployeeManagementPortal.Web` | MVC controllers, Razor views, DI composition root. |
| Tests | `EmployeeManagementPortal.Tests` | xUnit tests for all of the above. |