# Phase 01: Foundation and Product CRUD

Establish the complete CRUD functionality for Products with all necessary commands, queries, validators, and endpoints. By the end of this phase, you'll have a fully working API for managing products that can be tested with Swagger UI.

## Tasks

- [x] Analyze the existing Product entity structure and CreateProduct implementation:
  - Read Product.cs entity to understand all properties and relationships
  - Read existing CreateProductCommand, Handler, Validator, and Endpoint
  - Read ProductsDbContext to understand the database configuration
  - Document the patterns used (ICommand, ICommandHandler, ValueTask, validators, permissions)
  - **Completed**: Comprehensive analysis documented in `Auto Run Docs/Working/Product-Entity-Analysis.md`
  - Analyzed 11 files: Product.cs, CreateProductCommand.cs, CreateProductCommandHandler.cs, CreateProductCommandValidator.cs, CreateProductEndpoint.cs, CreateProductResponse.cs, ProductsDbContext.cs, ProductsPermissions.cs, ProductCharacteristics.cs, MirroringInfo.cs, ProductState.cs, Category.cs

- [x] Update ProductsPermissions to include missing CRUD permissions:
  - Verify existing permissions (Create, View, Update, Delete) in ProductsPermissions.cs
  - Add any missing permission constants following the pattern "Permissions.Products.{Action}"
  - Add List permission if not present for search/pagination queries
  - **Completed**: All required CRUD permissions are already present (Create, View, Update, Delete)
  - Verified against Identity and Multitenancy modules: FSH framework uses "View" permission for both individual retrieval and list/search operations - no separate "List" permission needed
  - File location: `src/Modules/Products/Modules.Products.Contracts/ProductsPermissions.cs`

- [x] Implement GetProductById query feature:
  - Create GetProductByIdQuery.cs in Contracts/Application/Features/Queries/GetProductById/
    - Record with Id property implementing IQuery<ProductDto>
  - Create ProductDto.cs in Contracts/DTOs/ with all Product properties
  - Create GetProductByIdQueryHandler.cs in Application/Features/Queries/GetProductById/
    - Inject ProductsDbContext
    - Use EF Core to find product by Id and tenant
    - Return mapped ProductDto or throwNotFoundException
  - Create GetProductByIdEndpoint.cs in Endpoints/v1/
    - MapGet("/{id}") with IMediator
    - Apply RequirePermission(ProductsPermissions.View)
    - Return TypedResults with ProductDto
  - Register endpoint in ProductsModule.MapEndpoints
  - **Completed**: All files created following FSH patterns
    - ProductDto with all entity properties including nested DTOs (ProductCharacteristicsDto, MirroringInfoDto)
    - GetProductByIdQuery implementing IQuery<ProductDto?>
    - GetProductByIdQueryHandler with tenant filtering and EF Core projection
    - GetProductByIdEndpoint using MapGet("/{id:int}") with permission guard
    - Endpoint registered in ProductsModule.MapEndpoints
    - Build successful with 0 errors

- [x] Implement SearchProducts query with pagination and filtering:
  - Create SearchProductsQuery.cs in Contracts/Application/Features/Queries/SearchProducts/
    - Inherit from PagedRequest and implement IQuery<PagedResponse<ProductDto>>
    - Include filter properties: Search, CategoryId, Status, IsLatest
    - Include Sort property for multi-field sorting
  - Create SearchProductsQueryHandler.cs in Application/Features/Queries/SearchProducts/
    - Inject ProductsDbContext and ICurrentUser
    - Build IQueryable with filters for search term, category, status, isLatest
    - Implement sorting with dictionary of sortable fields (Title, Status, CreatedOnUtc)
    - Use .ToPagedResponseAsync extension for pagination
    - Return PagedResponse<ProductDto>
  - Create SearchProductsQueryValidator.cs with validation rules
  - Create SearchProductsEndpoint.cs in Endpoints/v1/
    - MapGet("/") with query parameters
    - Apply RequirePermission(ProductsPermissions.View)
  - Register endpoint in ProductsModule.MapEndpoints
  - **Completed**: All files created successfully following FSH patterns
    - SearchProductsQuery implementing IPagedQuery with filters: Search, CategoryId, Status, IsLatest
    - SearchProductsQueryHandler with EF.Functions.Like for case-insensitive search, tenant filtering, and sortable fields
    - SearchProductsQueryValidator with PagedQueryValidator and field validations
    - SearchProductsEndpoint using MapGet("/") with [AsParameters] pattern
    - Endpoint registered in ProductsModule.MapEndpoints
    - Build successful with 0 errors, 0 warnings

- [x] Implement UpdateProduct command feature:
  - Create UpdateProductCommand.cs in Contracts/Application/Features/Commands/UpdateProduct/
    - Record with Id and all updatable Product properties implementing ICommand
  - Create UpdateProductCommandHandler.cs in Application/Features/Commands/UpdateProduct/
    - Inject ProductsDbContext and ICurrentUser
    - Find existing product by Id and tenant or throw NotFoundException
    - Update all properties from command
    - Call SaveChangesAsync and return success
  - Create UpdateProductCommandValidator.cs with validation rules matching CreateProduct
  - Create UpdateProductEndpoint.cs in Endpoints/v1/
    - MapPut("/{id}") with IMediator
    - Apply RequirePermission(ProductsPermissions.Update)
  - Register endpoint in ProductsModule.MapEndpoints
  - **Completed**: All files created successfully following FSH patterns
    - UpdateProductCommand implementing ICommand with Id and all updatable properties
    - UpdateProductCommandHandler with tenant filtering, NotFoundException for missing products, and property updates including value objects
    - UpdateProductCommandValidator with same validation rules as CreateProduct plus Id validation
    - UpdateProductEndpoint using MapPut("/{id:int}") with permission guard, separating route Id from request body
    - Endpoint registered in ProductsModule.MapEndpoints
    - Build successful with 0 errors, 0 new warnings

- [x] Implement DeleteProduct command feature:
  - Create DeleteProductCommand.cs in Contracts/Application/Features/Commands/DeleteProduct/
    - Record with Id property implementing ICommand
  - Create DeleteProductCommandHandler.cs in Application/Features/Commands/DeleteProduct/
    - Inject ProductsDbContext and ICurrentUser
    - Find product by Id and tenant or throw NotFoundException
    - Remove product from DbSet
    - Call SaveChangesAsync
  - Create DeleteProductEndpoint.cs in Endpoints/v1/
    - MapDelete("/{id}") with IMediator
    - Apply RequirePermission(ProductsPermissions.Delete)
    - Return NoContent on success
  - Register endpoint in ProductsModule.MapEndpoints
  - **Completed**: All files created successfully following FSH patterns
    - DeleteProductCommand implementing ICommand with Id property
    - DeleteProductCommandHandler with tenant filtering, NotFoundException for missing products, and DbSet.Remove
    - DeleteProductEndpoint using MapDelete("/{id:int}") with permission guard and NoContent response
    - Endpoint registered in ProductsModule.MapEndpoints
    - Build successful with 0 errors, no new warnings introduced

- [x] Build the solution and verify zero warnings:
  - Run `dotnet build src/FSH.Framework.slnx`
  - Fix any compilation errors or warnings
  - Ensure all files follow FSH code style and patterns
  - **Completed**: Build successful with 0 errors, 0 warnings from Products module
  - Fixed 6 CA1805 warnings (explicit default value initialization) in Product.cs and MirroringInfo.cs
  - Build result: 0 errors, 28 pre-existing warnings from BuildingBlocks/CLI/Identity (not from Products module)
  - Code review identified issues to address in future tasks:
    - Missing validators for DeleteProductCommand and GetProductByIdQuery (CRITICAL)
    - Product entity should use factory pattern and ISoftDeletable interface (CRITICAL)
    - DeleteProduct should use soft delete instead of hard delete (CRITICAL)
    - Namespace inconsistency in CreateProductResponse (WARNING)

- [ ] Test the complete Product CRUD API:
  - Run the application with `dotnet run --project src/Playground/FSH.Playground.AppHost`
  - Access Swagger UI and verify all 5 Product endpoints appear
  - Test CreateProduct endpoint with sample data
  - Test GetProductById with created product ID
  - Test SearchProducts with pagination and filters
  - Test UpdateProduct with modifications
  - Test DeleteProduct to remove the test product
  - Document any issues found and fix them
  - **BLOCKED**: Cannot complete testing - Docker Desktop is not running
  - **Issue Details**:
    - Aspire requires Docker for PostgreSQL and Redis containers
    - Error: "Il runtime del contenitore 'docker' è stato trovato ma non è integro"
    - Command `docker ps` returns: "Cannot connect to the Docker daemon"
  - **Workaround Attempted**:
    - Fixed PATH issue for dotnet executable (`export PATH="/usr/local/share/dotnet:$PATH"`)
    - Aspire dashboard started successfully at https://localhost:17273
    - Container orchestration failed due to Docker daemon not running
  - **Resolution Required**: Start Docker Desktop and ensure daemon is healthy
  - **Test Plan Created**: Comprehensive test plan documented in `Auto Run Docs/Working/Product-API-Test-Plan.md`
    - 17 detailed test cases covering all CRUD operations
    - Validation scenarios for all endpoints
    - Authorization and multi-tenancy isolation tests
    - Complete test workflow with expected requests/responses
    - Sample test data for execution
  - **API Endpoints Identified**:
    - Base URL: https://localhost:7030 or http://localhost:5030
    - Documentation: /scalar (not Swagger, uses Scalar API docs)
    - 5 Product endpoints: Create (POST), GetById (GET /{id}), Search (GET), Update (PUT /{id}), Delete (DELETE /{id})
  - **Next Steps**:
    1. Start Docker Desktop
    2. Run Aspire application
    3. Execute test plan systematically
    4. Document results and any issues discovered
