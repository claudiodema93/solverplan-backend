---
type: analysis
title: Product Entity and CreateProduct Implementation Analysis
created: 2026-02-05
tags:
  - products
  - entity-analysis
  - fsh-patterns
related:
  - "[[Phase-01-Foundation-and-Product-CRUD]]"
---

# Product Entity and CreateProduct Implementation Analysis

## Overview
This document provides a comprehensive analysis of the Product entity structure, the CreateProduct implementation, and the architectural patterns used in the FSH (FullStackHero) framework for the Products module.

---

## Product Entity Structure

**Location:** `src/Modules/Products/Modules.Products/Domain/Entities/Product.cs`

### Entity Definition
```csharp
public class Product : BaseEntity<int>, IHasTenant, IAuditableEntity
```

The Product entity inherits from `BaseEntity<int>`, implementing both multi-tenancy (`IHasTenant`) and audit tracking (`IAuditableEntity`).

### Properties

#### Core Product Information
- **Title** (required string): Product name/title (max 256 characters)
- **Description** (nullable string): Detailed description (max 2000 characters)
- **Revision** (int): Revision number, increments with each revision (default: 0)
- **Version** (int): Version tracking for product evolution (default: 0)
- **Variant** (string): Different configurations/models identifier (default: empty string)

#### Status and Visibility
- **Status** (ProductState enum): Current state - Draft (0), Active (1), or Obsolete (2)
- **Private** (bool): Visibility flag for public/private access (default: false)
- **IsLatest** (bool): Indicates if this is the latest version (default: false)

#### Relationships
- **CategoryId** (nullable int): Foreign key to Category entity
- **Category** (Category navigation property): EF Core navigation to Category
- **CustomerId** (nullable string): Customer-specific product association

#### Metadata and Documentation
- **Keywords** (nullable string): Search/categorization keywords (max 500 characters)
- **Language** (nullable string): Product documentation language (max 50 characters)
- **Subject** (nullable string): Main topic/subject (max 200 characters)
- **Notes** (nullable string): Additional comments (max 2000 characters)
- **HashSha256** (nullable string): SHA-256 hash for integrity verification (64 hex chars)

#### Complex Value Objects
- **Characteristics** (ProductCharacteristics): Flags for product type and capabilities
- **Mirroring** (MirroringInfo): Product mirroring/copying information

#### Multi-tenancy and Auditing (from interfaces)
- **TenantId** (string, required): Tenant identifier for multi-tenant isolation
- **CreatedOnUtc** (DateTimeOffset): Creation timestamp
- **CreatedBy** (nullable string): Creator user ID
- **LastModifiedOnUtc** (nullable DateTimeOffset): Last modification timestamp
- **LastModifiedBy** (nullable string): Last modifier user ID

---

## Value Objects

### ProductCharacteristics
**Location:** `src/Modules/Products/Modules.Products/Domain/ValueObjects/ProductCharacteristics.cs`

A sealed value object containing boolean flags:
- **IsAssembly**: Product composed of multiple parts
- **IsJobWork**: Outsourced manufacturing
- **IsManufacturable**: Can be manufactured
- **IsCommercial**: For commercial purposes
- **IsSellable**: Can be sold to customers
- **IsQualityCheckRequired**: Requires quality checks

**Pattern Used:**
- Immutable with `private init` setters
- Static factory methods: `Create()` and `Default()`
- Inherits from `ValueObject` base class
- Implements equality comparison via `GetEqualityComponents()`

### MirroringInfo
**Location:** `src/Modules/Products/Modules.Products/Domain/ValueObjects/MirroringInfo.cs`

A sealed value object for product mirroring:
- **IsMirrored**: Whether product is mirrored from another
- **SourceProductTitle**: Original product title
- **SourceProductRevision**: Original product revision
- **CopyProduct**: Whether to copy product data

**Pattern Used:**
- Same immutability and factory pattern as ProductCharacteristics
- Custom `Equals()` and `GetHashCode()` implementations
- `IsEmpty` convenience property

---

## ProductState Enum
**Location:** `src/Modules/Products/Modules.Products.Contracts/Domain/Enums/ProductState.cs`

```csharp
public enum ProductState
{
    Draft = 0,
    Active = 1,
    Obsolete = 2
}
```

---

## CreateProduct Implementation

### Command (Contract)
**Location:** `src/Modules/Products/Modules.Products.Contracts/Application/Features/Commands/CreateProduct/CreateProductCommand.cs`

```csharp
public sealed record CreateProductCommand(...) : ICommand<CreateProductResponse>;
```

**Key Patterns:**
- **Record type** for immutability
- **ICommand<T>** interface (NOT IRequest - this is Mediator library, NOT MediatR)
- All Product properties included as parameters with appropriate defaults
- Nested DTOs: `ProductCharacteristicsDto` and `MirroringInfoDto` for value objects
- Located in Contracts project for clean separation

### Handler
**Location:** `src/Modules/Products/Modules.Products/Application/Features/Commands/CreateProduct/CreateProductCommandHandler.cs`

```csharp
public sealed class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, CreateProductResponse>
{
    public async ValueTask<CreateProductResponse> Handle(...)
}
```

**Key Patterns:**
- **ICommandHandler<TCommand, TResponse>** interface
- **ValueTask<T>** return type (NOT Task<T>)
- Constructor injection: `ProductsDbContext`, `ICurrentUser`
- Tenant retrieval via `currentUser.GetTenant()`
- Maps DTOs to domain value objects using static factory methods
- Creates entity with required `TenantId`
- Uses EF Core: `AddAsync()` and `SaveChangesAsync()`
- Returns simple response with generated ID

### Validator
**Location:** `src/Modules/Products/Modules.Products/Application/Features/Commands/CreateProduct/CreateProductCommandValidator.cs`

```csharp
public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
```

**Key Patterns:**
- Inherits from FluentValidation's `AbstractValidator<T>`
- Comprehensive validation rules:
  - Required fields (Title)
  - Maximum length constraints (Title: 256, Description: 2000, etc.)
  - Range validations (Revision >= 0, Version >= 0)
  - Conditional validations using `.When()`
  - Regex validation for HashSha256 (64 hex characters)
  - Business rule validation (SourceProductTitle required when IsMirrored)

### Endpoint
**Location:** `src/Modules/Products/Modules.Products/Endpoints/v1/CreateProductEndpoint.cs`

```csharp
public static class CreateProductEndpoint
{
    public static RouteHandlerBuilder Map(this IEndpointRouteBuilder endpoints)
}
```

**Key Patterns:**
- Static class with static `Map()` method returning `RouteHandlerBuilder`
- **MapPost("/")** for HTTP POST
- `[FromBody]` attribute for command binding
- `[FromServices]` attribute for IMediator dependency injection
- **IMediator.Send()** to dispatch command
- **TypedResults.Ok()** for response
- **Fluent configuration:**
  - `.WithName("CreateProduct")` for endpoint naming
  - `.WithSummary()` for OpenAPI documentation
  - **`.RequirePermission(ProductsPermissions.Create)`** for authorization
  - `.WithDescription()` for detailed docs
  - `.Produces<T>()` for response type documentation

### Response
**Location:** `src/Modules/Products/Modules.Products.Contracts/Application/Features/Commands/CreateProduct/CreateProductResponse.cs`

```csharp
public sealed record CreateProductResponse(int Id);
```

Simple record with the created product ID.

---

## ProductsDbContext

**Location:** `src/Modules/Products/Modules.Products/Infrastructure/Data/ProductsDbContext.cs`

```csharp
public sealed class ProductsDbContext : BaseDbContext
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductsDbContext).Assembly);
    }
}
```

**Key Patterns:**
- Inherits from `BaseDbContext` (provides multi-tenancy, auditing, domain events)
- Constructor injects: `IMultiTenantContextAccessor`, `DbContextOptions`, `IOptions<DatabaseOptions>`, `IHostEnvironment`
- DbSet property for Products
- Configuration applied from assembly (looks for `IEntityTypeConfiguration<T>` implementations)

---

## ProductsPermissions

**Location:** `src/Modules/Products/Modules.Products.Contracts/ProductsPermissions.cs`

```csharp
public static class ProductsPermissions
{
    public const string Create = "Permissions.Products.Create";
    public const string View = "Permissions.Products.View";
    public const string Update = "Permissions.Products.Update";
    public const string Delete = "Permissions.Products.Delete";
}
```

**Pattern:** Static class with const string permissions following format: `"Permissions.{Module}.{Action}"`

**Note:** No explicit "List" permission is defined - typically View permission covers both single item retrieval and list/search operations.

---

## FSH Architectural Patterns Summary

### 1. Vertical Slice Architecture
Each feature is self-contained with:
- Command/Query (in Contracts)
- Handler (in Application)
- Validator (in Application)
- Endpoint (in Endpoints)

### 2. CQRS with Mediator Library
- **ICommand<TResponse>** for commands that modify state
- **IQuery<TResponse>** for queries (to be implemented)
- **ICommandHandler<TCommand, TResponse>** for command handlers
- **IQueryHandler<TQuery, TResponse>** for query handlers
- **ValueTask<T>** return types (not Task<T>)
- **IMediator** for dispatching (NOT MediatR's IMediator)

### 3. Multi-tenancy
- `IHasTenant` interface with `TenantId` property
- `ICurrentUser.GetTenant()` for retrieving current tenant
- Automatic tenant filtering in BaseDbContext

### 4. Audit Trail
- `IAuditableEntity` interface
- Automatic tracking via BaseDbContext interceptors
- Properties: CreatedOnUtc, CreatedBy, LastModifiedOnUtc, LastModifiedBy

### 5. Domain-Driven Design
- Value Objects (ProductCharacteristics, MirroringInfo)
- Entities (Product, Category)
- Domain enums (ProductState)
- Separation of domain and DTOs

### 6. Clean Architecture Layers
- **Domain**: Entities, Value Objects, Enums
- **Contracts**: Commands, Queries, DTOs, Responses, Permissions
- **Application**: Handlers, Validators
- **Infrastructure**: DbContext, Persistence configurations
- **Endpoints**: HTTP endpoint mappings

### 7. Validation
- FluentValidation's `AbstractValidator<T>`
- Separate validator classes
- Automatic validation pipeline integration

### 8. Permission-Based Authorization
- Static permission constants
- `.RequirePermission()` extension on endpoints
- Format: "Permissions.{Module}.{Action}"

### 9. API Documentation
- Fluent endpoint configuration
- `.WithName()`, `.WithSummary()`, `.WithDescription()`
- `.Produces<T>()` for OpenAPI/Swagger

---

## File Organization

```
Products/
├── Modules.Products.Contracts/           # Public contracts
│   ├── Domain/
│   │   └── Enums/
│   │       └── ProductState.cs
│   ├── Application/
│   │   └── Features/
│   │       └── Commands/
│   │           └── CreateProduct/
│   │               ├── CreateProductCommand.cs
│   │               └── CreateProductResponse.cs (in v1 subfolder)
│   └── ProductsPermissions.cs
│
├── Modules.Products/                      # Internal implementation
│   ├── Domain/
│   │   ├── Entities/
│   │   │   ├── Product.cs
│   │   │   └── Category.cs
│   │   └── ValueObjects/
│   │       ├── ProductCharacteristics.cs
│   │       └── MirroringInfo.cs
│   ├── Application/
│   │   └── Features/
│   │       └── Commands/
│   │           └── CreateProduct/
│   │               ├── CreateProductCommandHandler.cs
│   │               └── CreateProductCommandValidator.cs
│   ├── Infrastructure/
│   │   └── Data/
│   │       └── ProductsDbContext.cs
│   └── Endpoints/
│       └── v1/
│           └── CreateProductEndpoint.cs
```

---

## Critical Implementation Rules

1. ✅ Use `Mediator` library, NOT `MediatR`
2. ✅ Use `ICommand<T>` / `IQuery<T>`, NOT `IRequest<T>`
3. ✅ Return `ValueTask<T>`, NOT `Task<T>`
4. ✅ DTOs in Contracts project
5. ✅ Every command has a validator (AbstractValidator<T>)
6. ✅ Every endpoint uses `.RequirePermission()`
7. ✅ Records for immutable commands/queries/responses
8. ✅ Sealed classes for handlers and validators
9. ✅ Static endpoint mapping methods
10. ✅ Multi-tenant aware (TenantId from ICurrentUser)

---

## Next Steps for CRUD Implementation

Based on this analysis, the remaining CRUD operations should follow identical patterns:

1. **GetProductById**: IQuery<ProductDto> with single ID parameter
2. **SearchProducts**: IQuery<PagedResponse<ProductDto>> inheriting from PagedRequest
3. **UpdateProduct**: ICommand with ID + all updatable properties
4. **DeleteProduct**: ICommand with single ID parameter

All will follow:
- Same file structure
- Same handler pattern with ValueTask
- Same validator pattern
- Same endpoint pattern with appropriate HTTP verbs
- Same permission requirements
