# Forge-Api

`Forge-Api` is a .NET 10 solution centered on the `Forge` SDK libraries, a database/migrations project, and an early `LeadForge.Api` web application. The repository currently combines domain models, contracts, services, EF Core persistence, PostgreSQL migrations/seeders, and dedicated NUnit test projects.

## Solution Layout

- `Forge.Contracts`: DTOs, enums, and shared abstractions.
- `Forge.Domain`: entities, domain exceptions, and repository interfaces.
- `Forge.Services.Abstractions`: service interfaces for auth, organizations, permissions, roles, and the service manager.
- `Forge.Services`: service implementations built on repository abstractions.
- `Forge.Persistence`: `RepositoryDbContext`, EF Core configurations, repository implementations, and `PaginatedList<T>`.
- `Forge.Database`: design-time EF Core context factory, migrations, and enum-driven seeders.
- `Forge.Tests.Contracts`: contract and enum tests.
- `Forge.Tests.Domain`: entity and exception tests.
- `Forge.Tests.Services`: service tests.
- `Forge.Tests.Persistence`: persistence and repository tests.
- `Forge.Tests.Database`: database factory tests.
- `LeadForge.Api`: minimal ASP.NET Core app.
- `LeadForge.Domain`: placeholder .NET class library for the `LeadForge` area.

## Current Behavior

- `AuthService.Register` checks unique login and email, loads default status `UserStatus.Active`, creates a user, and adds default role `UserRole.User`.
- `AuthService.Login` loads a user by login or email and throws `InvalidCredentialsException` on missing user or password mismatch.
- `OrganizationService.Create(CreateOrganizationDto, User, ...)` rejects duplicate organization names, creates the organization, and still has a TODO for adding the owner membership.
- `RoleService.GetAll` currently returns user roles only.
- `Forge.Database` seeders repopulate `Permission`, `Role`, and `Status` rows from the contract enums and clear `DeletedAt` when records already exist.
- `LeadForge.Api` currently maps only `GET /` to `"Hello World!"`.

## Requirements

- .NET SDK 10.0
- Docker, if you want the local PostgreSQL/nginx stack
- Access to NuGet package sources for restore

## Restore

```bash
dotnet restore Forge-Api.sln
```

## Build

```bash
dotnet build Forge-Api.sln --no-restore
```

## Test

Run the full solution:

```bash
dotnet test Forge-Api.sln --no-restore -v minimal
```

Run individual test projects:

```bash
dotnet test Forge.Tests.Domain/Forge.Tests.Domain.csproj --no-restore -v minimal
dotnet test Forge.Tests.Contracts/Forge.Tests.Contracts.csproj --no-restore -v minimal
dotnet test Forge.Tests.Services/Forge.Tests.Services.csproj --no-restore -v minimal
dotnet test Forge.Tests.Persistence/Forge.Tests.Persistence.csproj --no-restore -v minimal
dotnet test Forge.Tests.Database/Forge.Tests.Database.csproj --no-restore -v minimal
```

## Run The API

```bash
dotnet run --project LeadForge.Api/LeadForge.Api.csproj
```

Default launch settings expose:

- `http://localhost:5117`
- `https://localhost:7279`

## Local Infrastructure

The repository includes `compose.yaml` plus a small `Makefile` wrapper.

Start the local stack:

```bash
make up
```

Useful commands:

```bash
make down
make restart
make logs
make ps
```

Services:

- `postgres`: `postgres:18.3-alpine`, default port `5432`
- `nginx`: `nginx:1.29.6-alpine`, default port `8080`

Default PostgreSQL container settings:

- database: `forge`
- user: `forge`
- password: `secret`

## Repository Conventions

- Keep entities in `Forge.Domain/Entities`.
- Keep exceptions under `Forge.Domain/Exceptions/<Area>`.
- Keep repository interfaces in `Forge.Domain/Repositories`.
- Keep request DTOs in `Forge.Contracts/Data/<Area>`.
- Keep contract enums in `Forge.Contracts/Types/<Area>`.
- Keep EF Core configurations in `Forge.Persistence/Configurations`.
- Keep repository implementations in `Forge.Persistence/Repositories`.
- Keep migrations and seeders in `Forge.Database`.
- Add or update dedicated tests for every changed production type.
- Keep `README.md` and `AGENTS.md` aligned with the actual codebase when structure or conventions change.
