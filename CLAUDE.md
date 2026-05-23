# Guarantee Server — Project Guide

## Overview

Financial guarantee server (ASP.NET Core 8, C#) with Clean Architecture. Manages credit scoring, bank integrations, document processing, and payment workflows for Uzbekistan's guarantee system.

## Architecture

```
Guarantee.Api            → Controllers, filters, middleware, DI config
Guarantee.Application    → Services, background jobs, DTOs/contracts
Guarantee.Infrastructure → Repositories, DbContext, auth, external integrations
Guarantee.Domain         → Entities, enums, abstractions (Result, Error, PagedList)
```

## Build & Run

```bash
dotnet build
dotnet run --project src/Guarantee.Api
```

## Testing

```bash
dotnet test                                          # all tests
dotnet test --filter "FullyQualifiedName~UnitTests"  # unit tests only
dotnet test --filter "FullyQualifiedName~IntegrationTests"  # integration tests (requires Docker)
```

### Test Project Structure

```
tests/
├── Guarantee.Application.UnitTests/    → Service-level unit tests
│   └── Services/{Domain}/             → Mirrors src/Guarantee.Application/Services/
├── Guarantee.Api.IntegrationTests/     → HTTP-level integration tests
│   ├── Fixtures/                       → WebApplicationFactory, TestAuthHandler
│   └── Controllers/                   → One file per controller
└── Guarantee.Domain.UnitTests/         → Domain logic tests
```

### Testing Conventions

**Partial classes — one file per method:**
Each test class uses `partial class`. The main file holds only configuration (fields, constructor, helpers). Each tested method gets its own file:
```
Services/Banks/
├── BankServiceTests.cs                          → partial: fields, ctor, helpers
├── BankServiceTests.GetAllBanksAsync.cs          → partial: tests for GetAllBanksAsync
├── BankServiceTests.GetBankByIdAsync.cs           → partial: tests for GetBankByIdAsync
├── BankServiceTests.AddBankAsync.cs               → partial: tests for AddBankAsync
└── ...
```
File naming: `{TestClass}.{MethodUnderTest}.cs`

**Naming:** `MethodName_ExpectedResult_WhenCondition`
```csharp
GetBankByIdAsync_ReturnsNotFound_WhenBankDoesNotExist
AddBankAsync_ReturnsConflict_WhenBankAlreadyExists
```

**Structure:** Arrange-Act-Assert (AAA) with blank line separators
```csharp
[Fact]
public async Task GetBankByIdAsync_ReturnsBank_WhenFound()
{
    // Arrange
    var bank = new Bank { Id = 1, Tin = 123456789 };
    _bankRepository.GetByIdAsync(1).Returns(bank);

    // Act
    var result = await _sut.GetBankByIdAsync(1);

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Data.Should().NotBeNull();
    result.Data!.Id.Should().Be(1);
}
```

**Mocking:** NSubstitute — mock interfaces, not implementations
- Mock `IRepositoryBase<T, TId>` for data access
- Mock `IHamkorBroker`, `IBankBroker` for external services
- Mock `IHttpContextAccessorService` for user context
- Never mock `Result<T>`, `Error`, or domain entities

**Integration tests:** Testcontainers (real PostgreSQL in Docker) + fake auth handler
- Use `CustomWebApplicationFactory` with real DB, mocked external services
- Test full HTTP pipeline: routing → auth → controller → service → DB

### What to Test

**Unit tests (service layer):**
- Happy path for each method
- Every error branch (NotFound, Conflict, Validation, PermissionDenied)
- External service failure handling
- Mapping correctness (entity → response)

**Integration tests (controller layer):**
- HTTP status codes (200, 400, 404, 409)
- Response body shape/content
- Auth/permission enforcement
- Request validation

### What NOT to Test

- Private methods directly (test through public API)
- EF Core migrations
- Third-party library internals
- Trivial getters/setters

## Code Patterns

### Result Pattern
All service methods return `Result` or `Result<T>`. Never throw exceptions for business logic.
```csharp
public async Task<Result<BankResponse>> GetBankByIdAsync(int bankId)
{
    var bank = await bankRepository.GetByIdAsync(bankId);
    if (bank is null) return BankErrors.NotFound;  // implicit conversion
    return MapToResponse(bank);                     // implicit conversion
}
```

### Error Definitions
Each service domain has a static `*Errors` class:
```csharp
public static class BankErrors
{
    public static Error NotFound => Error.NotFound("Bank.NotFound");
    public static Error AlreadyExists => Error.Conflict("Bank.AlreadyExists");
}
```

### Controller Pattern
Controllers delegate to services and convert results:
```csharp
var result = await service.MethodAsync(request);
return result.IsSuccess ? Results.Ok(result.Data) : result.ToProblemDetails(localizationService);
```

### Repository
Generic `IRepositoryBase<T, TId>` with auto-save. Supports `IQueryable<T>` for LINQ queries.

### DI
All services registered as Scoped in `Dependencies.cs`. Primary constructor injection throughout.

## Database

- PostgreSQL with EF Core 8
- Snake_case naming convention
- JSONB for `MultiLanguageField`
- Auto-timestamps: `CreatedAt`, `UpdatedAt` in `SaveChangesAsync`
- Migrations in `Guarantee.Infrastructure/Migrations/`

## Key Directories

- Controllers: `src/Guarantee.Api/Controllers/`
- Services: `src/Guarantee.Application/Services/{Domain}/`
- Entities: `src/Guarantee.Domain/Entities/`
- Enums: `src/Guarantee.Domain/Enums/`
- Abstractions: `src/Guarantee.Domain/Abstractions/`
- Repositories: `src/Guarantee.Infrastructure/Repositories/`
- Auth: `src/Guarantee.Infrastructure/Authentication/`
- Jobs: `src/Guarantee.Application/Jobs/`
- DI Config: `src/Guarantee.Api/Dependencies.cs`
