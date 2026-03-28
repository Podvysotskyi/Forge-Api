# Forge-Api Agent Guide

## Solution Layout
- `Forge.Contracts`: request DTOs, public enums, and shared abstractions.
- `Forge.Domain`: domain entities, exceptions, and repository interfaces.
- `Forge.Services.Abstractions`: service interfaces that depend on `Forge.Contracts` and `Forge.Domain`.
- `Forge.Services`: service implementations that orchestrate repositories and return domain entities.
- `Forge.Persistence`: Entity Framework Core persistence layer, `RepositoryDbContext`, entity configurations, repository implementations, and pagination helpers.
- `Forge.Database`: design-time EF Core factory, migrations, and seeders for enum-backed data.
- `Forge.Tests.Contracts`: tests for `Forge.Contracts`.
- `Forge.Tests.Domain`: tests for `Forge.Domain`.
- `Forge.Tests.Services`: tests for `Forge.Services`.
- `Forge.Tests.Persistence`: tests for `Forge.Persistence`.
- `Forge.Tests.Database`: tests for `Forge.Database`.
- `LeadForge.Api`: minimal ASP.NET Core web app.
- `LeadForge.Domain`: minimal class library for the `LeadForge` area.

## Target Framework And Tooling
- All projects currently target `net10.0`.
- Test projects use `NUnit`.
- `Forge.Persistence` uses `Npgsql.EntityFrameworkCore.PostgreSQL`.
- `Forge.Database` uses `Microsoft.EntityFrameworkCore.Design` for migrations and design-time context creation.
- Local infrastructure is defined in `compose.yaml` and wrapped by the root `Makefile`.

## Domain Structure
- Prefer file-scoped namespaces in handwritten code where practical.
- Keep domain entities in `Forge.Domain/Entities`.
- Keep exception classes grouped by domain area under `Forge.Domain/Exceptions`.
- Keep repository interfaces in `Forge.Domain/Repositories`.
- Current entities:
  - `Organization`
  - `OrganizationUser`
  - `Permission`
  - `Role`
  - `Status`
  - `User`

## Relationship Notes
- `Organization` has an optional `UserId` and nullable `User` navigation.
- `Organization` also exposes `OrganizationUsers`.
- `User` has nullable `Organization` and `Status` navigations.
- `User` also exposes `Roles`, `Permissions`, and `OrganizationUsers`.
- `OrganizationUser` contains `OrganizationId`, `UserId`, `StatusId`, `RoleId`, timestamps, `DeletedAt`, and nullable navigations to `Organization`, `User`, `Status`, and `Role`.
- `OrganizationUser` also exposes a `Permissions` collection.
- `Permission` exposes `Users`, `Roles`, and `OrganizationUsers`.
- `Status` exposes `Users` and `OrganizationUsers`.
- `User.IsActive` depends on `Status` being loaded and throws `UserStatusIsNotLoadedException` otherwise.

## Contracts Structure
- Keep request DTOs in `Forge.Contracts/Data/<Area>`.
- Keep public enum contracts in `Forge.Contracts/Types/<Area>`.
- Keep collection abstractions like `IPaginatedList<T>` at the project root unless there is a clear grouping reason.
- Current request DTOs:
  - `LoginDto`
  - `RegisterDto`
  - `CreateOrganizationDto`
- Current contract enums:
  - `UserPermission`
  - `UserRole`
  - `OrganizationUserRole`
  - `UserStatus`
  - `OrganizationUserStatus`

## Services And Abstractions
- `Forge.Services.Abstractions` currently contains:
  - `IAuthService`
  - `IOrganizationService`
  - `IPasswordService`
  - `IPermissionService`
  - `IRoleService`
  - `IServiceManager`
- Service interfaces currently consume contract DTOs for input and return domain entities.
- `IOrganizationService` exposes `GetAll`, `Create(CreateOrganizationDto, User, ...)`, and `Create(User, ...)`.
- `ServiceManager` lazily composes the concrete services and delegates persistence to `IRepositoryManager.SaveChangesAsync`.
- `RoleService.GetAll` currently returns user roles via `IRoleRepository.UserRoles(...)`.
- `PermissionService.GetAll` returns all permissions.
- `AuthService.Register` returns the created `User` entity and does not create an organization.
- `OrganizationService.Create(CreateOrganizationDto, User, ...)` still contains a TODO for adding the creator as organization owner.

## Repository And Persistence Notes
- `IRepositoryManager` exposes organization, permission, role, user, and status repositories plus `SaveChangesAsync`.
- `IOrganizationRepository` exposes read helpers, `NameExists`, two `Create(...)` overloads, `Delete(...)`, and `LoadUsers(...)`.
- `IRoleRepository` includes `UserRoles(...)`, `OrganizationUserRoles(...)`, and `Find(...)` overloads for both role enums.
- `IStatusRepository` includes `GetUserStatuses(...)`, `GetOrganizationUserStatuses(...)`, and `Find(...)` overloads for both status enums.
- `IUserRepository` includes `LoadStatus(User, ...)`, plus role and permission loading helpers.
- Keep EF Core entity type configurations in `Forge.Persistence/Configurations`.
- Keep repository implementations in `Forge.Persistence/Repositories`.
- Repositories should inherit `Forge.Persistence/Repositories/Repository.cs` (`Repository<TEntity>`).
- Use the base helpers in repositories:
  - `Entities` for `DbSet<TEntity>` access.
  - `Entry(...)` for tracked navigation loading; do not call `DbContext.Entry(...)` directly in concrete repositories.
- `RepositoryDbContext` should stay responsible for applying persistence configurations from the `Forge.Persistence` assembly.
- `PaginatedList<T>` lives in `Forge.Persistence` and implements `IPaginatedList<T>`.

## Database Notes
- Keep migrations in `Forge.Database/Migrations`.
- Keep seeders in `Forge.Database/Seeders/<Area>`.
- The database project currently seeds permissions, roles, and statuses from the corresponding contract enums.
- `DatabaseContextFactory` is the design-time entry point for EF Core tooling and builds a `RepositoryDbContext` configured for PostgreSQL with migrations in `Forge.Database`.

## API Notes
- `LeadForge.Api` is currently a minimal app with a single `GET /` endpoint returning `"Hello World!"`.
- `LeadForge.Api` currently references `LeadForge.Domain` via a file reference in the project file rather than a project reference.
- Treat the `LeadForge.*` projects as early-stage placeholders unless the user asks to expand them.

## Testing
- Use one dedicated test class per production class, enum, or exception where feasible.
- Domain tests belong in `Forge.Tests.Domain`.
- Contracts tests belong in `Forge.Tests.Contracts`.
- Services tests belong in `Forge.Tests.Services`.
- Persistence tests belong in `Forge.Tests.Persistence`.
- Database tests belong in `Forge.Tests.Database`.
- Keep test namespaces aligned with their project and folder structure.
- If a type is renamed, rename its test file and test class to match.
- Run domain tests with `dotnet test Forge.Tests.Domain/Forge.Tests.Domain.csproj --no-restore -v minimal`.
- Run contracts tests with `dotnet test Forge.Tests.Contracts/Forge.Tests.Contracts.csproj --no-restore -v minimal`.
- Run services tests with `dotnet test Forge.Tests.Services/Forge.Tests.Services.csproj --no-restore -v minimal`.
- Run persistence tests with `dotnet test Forge.Tests.Persistence/Forge.Tests.Persistence.csproj --no-restore -v minimal`.
- Run database tests with `dotnet test Forge.Tests.Database/Forge.Tests.Database.csproj --no-restore -v minimal`.

## Tooling
- The root `Makefile` wraps `docker compose` commands for the local infrastructure.
- Container definitions live in `compose.yaml` and `docker/`.
- No project-level MCP configuration file is currently checked in under `.ai/mcp/mcp.json`.
- If MCP servers are added, removed, or changed in the repository later, keep this file aligned.

## When Making Changes
- Update or add dedicated tests for every new domain class, enum, exception, contract, repository, configuration, service, migration-related helper, or database seeder behavior.
- If a contract is added, add its tests in `Forge.Tests.Contracts`.
- If a domain type is added, add its tests in `Forge.Tests.Domain`.
- If a service is added or changed, add or update tests in `Forge.Tests.Services`.
- If a persistence type is added or changed, add or update tests in `Forge.Tests.Persistence`.
- If a database type is added or changed, add or update tests in `Forge.Tests.Database`.
- Keep `README.md` and `AGENTS.md` aligned with the actual codebase when project structure, conventions, tooling, or infrastructure change.
