# once-server

ASP.NET Core backend for the **Once** platform — bank-employee AI training and
guarantee-workflow services. Built on **Clean Architecture** with the
**Result pattern** end-to-end, JWT auth and Hangfire background jobs.

---

## Table of contents

- [What it does](#what-it-does)
- [Tech stack](#tech-stack)
- [Architecture](#architecture)
- [Project layout](#project-layout)
- [Getting started](#getting-started)
- [Configuration](#configuration)
- [Database & migrations](#database--migrations)
- [Background jobs](#background-jobs)
- [API surface](#api-surface)
- [Code conventions](#code-conventions)
- [Testing](#testing)
- [Docker & deployment](#docker--deployment)
- [Adding a new feature](#adding-a-new-feature)
- [Troubleshooting](#troubleshooting)

---

## What it does

Backend for an AI-assisted learning platform aimed at bank staff:

| Capability | Where it lives |
|---|---|
| Authentication (email/password, refresh tokens, JWT) | `Once.Application/Services/Auth/` |
| User & role management | `Once.Application/Services/Users/` |
| Permission-based authorization | `Once.Infrastructure/Authentication/` |
| Background jobs (sync, mailers, schedulers) | `Once.Application/Jobs/` |
| Reports / printing | DevExpress (optional) |
| Real-time notifications | SignalR Hubs (Infrastructure layer) |

The frontend that consumes this API is **once-client** (React 19 + Vite).

---

## Tech stack

| Layer | Choice |
|---|---|
| Runtime | **.NET 10** (`mcr.microsoft.com/dotnet/aspnet:10.0`) |
| Language | **C# 13** |
| HTTP | ASP.NET Core minimal APIs / controllers |
| ORM | **EF Core 8** (PostgreSQL provider, snake_case naming) |
| Database | **PostgreSQL 14+** |
| Auth | JWT Bearer + permission-based authorization |
| Background jobs | **Hangfire** (PostgreSQL storage) |
| API docs | Swagger / OpenAPI (with custom CSS in `wwwroot/swagger-ui/`) |
| Reporting | **DevExpress 24.2** printing |
| Bot integration | **Telegram.Bot** |
| Validation | FluentValidation |
| Globalization | `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false` + `Asia/Tashkent` TZ |

---

## Architecture

Clean Architecture — strict outward → inward dependency:

```
Once.Domain  ◀── Once.Infrastructure  ◀── Once.Application  ◀── Once.Api
   (pure)         (EF / external)            (business)          (HTTP)
```

| Layer | Depends on | Holds |
|---|---|---|
| **Domain** | nothing | Entities, enums, value types, `Result<T>`, `Error`, `PagedList`, abstractions |
| **Infrastructure** | Domain | `AppDbContext`, migrations, JWT/auth config, brokers, SignalR hubs, seeders, 3rd-party integrations |
| **Application** | Domain + Infrastructure | Service interfaces + implementations, background jobs, DTOs (`*Request` / `*Response` / `*FilterRequest`), validators, `*Errors` static classes |
| **Api** | all of the above | Controllers, middleware, filters, global exception handler, DI wiring (`Dependencies.cs`), `Program.cs` |

Each layer does exactly its own job — never mix them.

---

## Project layout

```
once-server/
├── src/
│   ├── Once.Domain/                core abstractions, entities, enums
│   │   ├── Abstractions/           Result<T>, Error, PagedList, DataQueryRequest
│   │   ├── Entities/
│   │   │   └── Common/             ModelBase, AuditableModelBase
│   │   ├── Enums/
│   │   └── Utils/
│   ├── Once.Infrastructure/        data access, auth, 3rd-party
│   │   ├── Persistence/
│   │   │   ├── AppDbContext.cs
│   │   │   └── Migrations/
│   │   ├── Authentication/         JWT, permission handlers
│   │   ├── Brokers/                external service clients
│   │   ├── Hubs/                   SignalR hubs
│   │   ├── Extensions/Seed/        DB seeders
│   │   └── Dependencies.cs
│   ├── Once.Application/           business logic
│   │   ├── Services/
│   │   │   └── {Domain}/
│   │   │       ├── Contracts/      *Request / *Response / *FilterRequest / *Validator
│   │   │       ├── {Domain}Errors.cs
│   │   │       ├── I{Domain}Service.cs
│   │   │       └── {Domain}Service.cs
│   │   ├── Jobs/                   background jobs ({Name}Job.cs)
│   │   └── Dependencies.cs
│   └── Once.Api/                   entry point
│       ├── Controllers/
│       │   ├── Common/             AuthorizedController
│       │   └── {Scope}/            {Domain}sController.cs
│       ├── Filters/
│       ├── Middlewares/            GlobalExceptionHandlerMiddleware, …
│       ├── Extensions/             ResultExtensions, etc.
│       ├── Converters/
│       ├── Resources/              i18n (.resx) for localized errors
│       ├── wwwroot/swagger-ui/     custom Swagger CSS
│       ├── Dependencies.cs         DI registration
│       ├── Program.cs              host bootstrap
│       └── appsettings.{Env}.json
├── tests/                          unit + integration tests
├── deploy/                         production compose + deploy assets
├── Dockerfile                      multi-stage build (sdk → publish → runtime)
└── Once.sln
```

---

## Getting started

### Prerequisites

- **.NET SDK 10+**
- **PostgreSQL 14+** running locally (or pointed at via the connection string)
- **DevExpress 24.2** (only needed for report generation; optional for local dev)

### 1. Restore

```bash
git clone <repo-url>
cd once-server
dotnet restore Once.sln
```

### 2. Configure

Copy the example and fill in your values:

```bash
cp src/Once.Api/appsettings.Development.json.example src/Once.Api/appsettings.Development.json
```

Minimum required keys:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=once;Username=postgres;Password=postgres"
  },
  "Jwt": {
    "SecretKey": "<at-least-32-bytes-of-random-data>",
    "Issuer": "once-server",
    "Audience": "once-client",
    "AccessTokenLifetimeMinutes": 60,
    "RefreshTokenLifetimeDays": 30
  }
}
```

### 3. Apply migrations

```bash
dotnet ef database update \
  --project src/Once.Infrastructure \
  --startup-project src/Once.Api
```

### 4. Run

```bash
dotnet run --project src/Once.Api
```

- API: <http://localhost:5000>
- Swagger UI: <http://localhost:5000/swagger>
- Hangfire dashboard: <http://localhost:5000/hangfire> (requires auth)
- Health: <http://localhost:5000/health>

---

## Configuration

`appsettings.{Environment}.json` files override the base `appsettings.json`.
Environment is selected by `ASPNETCORE_ENVIRONMENT` (defaults to `Production`
in the Docker image, `Development` when run via `dotnet run`).

Any setting can also be overridden via environment variables — replace `:` with
`__`:

```bash
ConnectionStrings__DefaultConnection="Host=db;Database=once;..."
Jwt__SecretKey="..."
```

Common settings:

| Section | Key | Purpose |
|---|---|---|
| `ConnectionStrings` | `DefaultConnection` | PostgreSQL DSN |
| `Jwt` | `SecretKey` / `Issuer` / `Audience` / lifetimes | Token issuance |
| `Cors` | `AllowedOrigins` | Comma-separated origins |
| `Hangfire` | `WorkerCount` / `Queues` | Job runner |
| `Telegram` | `BotToken` / `ChatId` | Notifications |
| `Logging` | `LogLevel.*` | Standard MEL config |

---

## Database & migrations

- **PostgreSQL + EF Core 8**, snake_case naming convention.
- All entities derive from `ModelBase` or `AuditableModelBase` — `Id`, `CreatedAt`,
  `UpdatedAt`, `IsDeleted` come for free.
- `SaveChangesAsync` in `AppDbContext` stamps `CreatedAt` / `UpdatedAt`
  automatically.
- `MultiLanguageField` columns are stored as JSONB.
- Migrations live in `src/Once.Infrastructure/Persistence/Migrations/`.

```bash
# Add a new migration
dotnet ef migrations add <Name> \
  --project src/Once.Infrastructure \
  --startup-project src/Once.Api

# Revert the last migration (only useful before it ships)
dotnet ef migrations remove \
  --project src/Once.Infrastructure \
  --startup-project src/Once.Api

# Apply
dotnet ef database update \
  --project src/Once.Infrastructure \
  --startup-project src/Once.Api
```

---

## Background jobs

Hangfire runs in-process. Recurring jobs are registered in
`Once.Application/Jobs/JobsRegistrar.cs` (called from `Dependencies.cs`).

```csharp
// Pattern: one interface, one implementation, ExecuteAsync(CancellationToken)
public interface IBankSyncJob
{
    Task ExecuteAsync(CancellationToken cancellationToken);
}

public sealed class BankSyncJob(IBankService bankService) : IBankSyncJob
{
    public Task ExecuteAsync(CancellationToken ct) =>
        bankService.SyncAsync(ct);
}
```

Dashboard: `/hangfire` — locked behind the same JWT scheme as the API.

---

## API surface

Controllers are grouped under `Controllers/{Scope}/`:

| Scope | Controllers | Purpose |
|---|---|---|
| `Auth` | `AuthController` | Login, refresh, logout |
| `Management` | `UsersController` | User CRUD, role assignment |
| `Common` | `AuthorizedController` | Base class providing `CurrentUserId` etc. |

Naming is load-bearing — `*Request`, `*Response`, `*FilterRequest`, paginated
results return `PagedList<T>`. Controllers are XML-documented (`/// <summary>`)
so Swagger shows real descriptions.

Standard controller body:

```csharp
/// <summary>Get user by id.</summary>
[HttpGet("{id:long}")]
public async Task<IResult> GetByIdAsync(long id, CancellationToken ct)
{
    var result = await userService.GetByIdAsync(id, ct);
    return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails();
}
```

---

## Code conventions

These are non-negotiable across the codebase (see `.claude/rules/` for the
authoritative list):

- **Result pattern only** — services return `Result` / `Result<T>`. **Never
  throw** for business errors. Each domain has a static `{Domain}Errors` class:
  ```csharp
  public static class UserErrors
  {
      public static Error NotFound      => Error.NotFound("User.NotFound");
      public static Error AlreadyExists => Error.Conflict("User.AlreadyExists");
  }
  ```
- **Soft delete only** — set `IsDeleted = true`. **Never** call `Remove()`.
- **All read queries** include `.AsNoTracking()` *and* `.Where(x => !x.IsDeleted)`.
- **`SaveChangesAsync`** lives only in the **Service layer** — never in
  Controllers, never in Repositories.
- **No `.First()`** — use `.SingleOrDefaultAsync()`; explicit null check
  returns the matching `*Errors` value.
- **No `int` for entity IDs** — use `long`.
- **Navigation properties** are initialised: `= null!` or `= new()`.
- **XML doc comments** on Controller methods only — never on services.
- **Business logic stays in Services** — controllers do `service.X(); return result.ToProblemDetails()`.
- **No `DbContext` in controllers** — only services touch the DB.
- **Primary constructor injection** throughout — `public sealed class UserService(IUserRepository repo, …)`.
- **NSubstitute** for mocking in tests — mock interfaces, never domain types.

---

## Testing

```bash
dotnet test                                                  # all tests
dotnet test --filter "FullyQualifiedName~UnitTests"          # unit only
dotnet test --filter "FullyQualifiedName~IntegrationTests"   # integration (Docker required)
```

Layout:

```
tests/
├── Once.Application.UnitTests/    service-level unit tests
│   └── Services/{Domain}/         mirrors src/Once.Application/Services/
├── Once.Api.IntegrationTests/     HTTP-level tests against Testcontainers
│   ├── Fixtures/                  CustomWebApplicationFactory, TestAuthHandler
│   └── Controllers/               one file per controller
└── Once.Domain.UnitTests/         pure domain tests
```

**Partial class per method** — `{TestClass}.{MethodUnderTest}.cs` for each
tested method, plus a base file with fields, ctor and helpers.

**Naming** — `MethodName_ExpectedResult_WhenCondition`:
```csharp
GetByIdAsync_ReturnsNotFound_WhenUserDoesNotExist
AddUserAsync_ReturnsConflict_WhenUserAlreadyExists
```

**Structure** — Arrange / Act / Assert with blank-line separators. FluentAssertions
for readable assertions. Integration tests use Testcontainers with a real
PostgreSQL plus a fake auth handler.

---

## Docker & deployment

### Local Docker

```bash
docker build -t once-server .
docker run -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Port=5432;Database=once;Username=postgres;Password=postgres" \
  -e Jwt__SecretKey="$(openssl rand -hex 32)" \
  --add-host=host.docker.internal:host-gateway \
  once-server
```

The image is multi-stage (`mcr.microsoft.com/dotnet/sdk:10.0` → restore →
publish → `mcr.microsoft.com/dotnet/aspnet:10.0`) with the runtime locale set
to `Asia/Tashkent` and the full glyph set + DevExpress fonts pre-installed.

Healthcheck: `GET /health` on port `8080`.

### CI/CD

`.github/workflows/deploy.yml` builds the image, pushes it to **GHCR**
(`ghcr.io/<owner>/once-server`) on push to `main`, then SSHes into the
production host and runs `docker compose pull && up -d` against the compose
file under `deploy/`.

Required GitHub secrets:

| Secret | Purpose |
|---|---|
| `DEPLOY_HOST` | Production server IP/hostname |
| `DEPLOY_USER` | SSH user (the `cicduser` provisioned by `devops-tools`) |
| `DEPLOY_SSH_KEY` | Private key for that user |
| `DEPLOY_PORT` | SSH port (default 22) |
| `GITHUB_TOKEN` | Provided automatically for GHCR push |

Front the container with `setup-nginx-ssl.sh` (in `devops-tools`) so that
`https://api.<your-domain>` proxies to the container's published port
(default mapping: host `44010` → container `8080`).

---

## Adding a new feature

1. **Domain** — add the entity under `src/Once.Domain/Entities/{Domain}.cs`,
   plus any enums.
2. **Migration** — `dotnet ef migrations add Add{Domain}` (review the
   generated file).
3. **Application** — under `Services/{Domain}/`:
   - `Contracts/Create{Domain}Request.cs`, `Get{Domain}FilterRequest.cs`,
     `{Domain}Response.cs`, `Create{Domain}RequestValidator.cs`
   - `{Domain}Errors.cs`
   - `I{Domain}Service.cs` + `{Domain}Service.cs`
4. **DI** — register the service in `Once.Application/Dependencies.cs` as
   `services.AddScoped<I{Domain}Service, {Domain}Service>();`.
5. **API** — controller at `Controllers/{Scope}/{Domains}Controller.cs`,
   inheriting from `AuthorizedController`. XML-doc each action. Body is
   `service.X(...); return result.IsSuccess ? Results.Ok(...) : result.ToProblemDetails();`.
6. **Tests** — partial-class-per-method unit tests under
   `tests/Once.Application.UnitTests/Services/{Domain}/`, and HTTP integration
   tests under `tests/Once.Api.IntegrationTests/Controllers/`.

---

## Troubleshooting

| Symptom | Likely cause |
|---|---|
| `Npgsql.PostgresException: FATAL: password authentication failed` | Wrong creds in `ConnectionStrings:DefaultConnection`. Note env-var form uses `__`. |
| 401 on Swagger calls | Click **Authorize** and paste the JWT (without the `Bearer ` prefix). |
| `Could not find type or namespace 'DevExpress'` | DevExpress is optional. Comment out the reporting controllers or install DevExpress 24.2 locally. |
| Hangfire dashboard `404` | `app.UseHangfireDashboard("/hangfire")` requires the user to be authenticated as an admin. |
| Migrations not applied in Docker | The container does **not** auto-apply migrations — run `dotnet ef database update` against the target DB, or add a one-shot init job. |
| Wrong timezone in logs | Set `TZ` in the container; the Dockerfile defaults to `Asia/Tashkent`. |
