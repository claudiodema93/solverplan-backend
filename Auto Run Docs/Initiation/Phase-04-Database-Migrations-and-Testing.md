# Phase 04: Database Migrations and Testing

Create and apply EF Core migrations for the new CRUD operations, write unit tests for critical handlers and validators, and perform integration testing to ensure the entire Products module works correctly.

## Tasks

- [x] Review and update EF Core configurations:
  - Read ProductsDbContext.cs and verify DbSet properties for Product, Category, Issue
  - Check Infrastructure/Data/Configurations/ for entity configurations
  - Create CategoryConfiguration.cs if missing with proper entity mapping
  - Create IssueConfiguration.cs if missing with foreign key to Product
  - Ensure all navigation properties are configured correctly
  - Configure cascade delete behavior for Issue when Product is deleted

  **Completion Notes:**
  - ✅ ProductsDbContext has all three DbSet properties: Products, Categories, Issues
  - ✅ All configuration files exist and are properly structured:
    - CategoryConfiguration.cs: Configures table name, indexes, properties with proper max lengths
    - IssueConfiguration.cs: Configures all properties including relationship to Product
    - ProductConfiguration.cs: Configures all properties including value objects and relationship to Category
  - ✅ Navigation properties verified:
    - Product.Category (nullable) matches ProductConfiguration foreign key setup
    - Issue.Product (nullable) matches IssueConfiguration foreign key setup
  - ✅ Cascade delete already configured: IssueConfiguration.cs:50 sets OnDelete(DeleteBehavior.Cascade) for Issue when Product is deleted
  - ✅ Product-Category relationship uses SetNull on delete (ProductConfiguration.cs:83)
  - All configurations follow FSH patterns with tenant isolation indexes and audit field configurations

- [x] Create and apply EF Core migration for CRUD operations:
  - Run `dotnet ef migrations add AddProductsCRUD --project src/Modules/Products/Modules.Products --startup-project src/Playground/FSH.Playground.AppHost --context ProductsDbContext`
  - Review generated migration file to ensure:
    - All Category columns are included
    - All Issue columns are included with foreign key to Products
    - Proper indexes are created for tenant isolation and foreign keys
  - Apply migration with `dotnet ef database update --project src/Modules/Products/Modules.Products --startup-project src/Playground/FSH.Playground.AppHost --context ProductsDbContext`
  - Verify migration applied successfully

  **Completion Notes:**
  - ✅ Migration already exists: `20260206191857_Initial_Products` in src/Playground/Migrations.PostgreSQL/Products/
  - ✅ Reviewed migration file and verified all required elements:
    - **Categories table**: Id, Name, TenantId, audit fields (CreatedOnUtc, CreatedBy, LastModifiedOnUtc, LastModifiedBy)
    - **Products table**: 24+ columns including:
      - Basic properties (Id, Title, Revision, Status, Description, Private, CustomerId, Variant, Version)
      - Optional properties (CategoryId, Keywords, Language, Subject, Notes, HashSha256)
      - Value objects (IsAssembly, IsJobWork, IsManufacturable, IsCommercial, IsSellable, IsQualityCheckRequired, IsMirrored, MirrorSourceTitle, MirrorSourceRevision, MirrorCopyProduct, IsLatest)
      - Audit properties (TenantId, CreatedOnUtc, CreatedBy, LastModifiedOnUtc, LastModifiedBy)
    - **Issues table**: Id, ProductId (FK), Title, Description, Severity, Status, ResolutionNotes, TenantId, audit fields
  - ✅ Foreign keys verified:
    - Products.CategoryId → Categories.Id with ON DELETE SET NULL (line 84)
    - Issues.ProductId → Products.Id with ON DELETE CASCADE (line 115)
  - ✅ Indexes verified for tenant isolation and performance:
    - IX_Categories_TenantId (line 119)
    - IX_Products_TenantId (line 143)
    - IX_Products_CategoryId (line 137)
    - IX_Issues_TenantId (line 131)
    - IX_Issues_ProductId (line 125)
  - ✅ Applied migration successfully to database using Aspire-orchestrated PostgreSQL container
  - ✅ Verified migration application with `dotnet ef migrations list` showing `20260206191857_Initial_Products` as applied
  - Database connection used dynamic port (54717) from Aspire container orchestration

- [ ] Write unit tests for Product command validators:
  - Create Tests/Products/Validators/ folder structure
  - Create CreateProductCommandValidatorTests.cs
    - Test required field validation (Title)
    - Test string length constraints
    - Test enum value validation
  - Create UpdateProductCommandValidatorTests.cs with similar tests
  - Run tests with `dotnet test src/FSH.Framework.slnx --filter Category=Products`

- [ ] Write unit tests for Category and Issue validators:
  - Create CreateCategoryCommandValidatorTests.cs
    - Test Name is required
    - Test Name max length
  - Create CreateIssueCommandValidatorTests.cs
    - Test ProductId is required
    - Test Title and Description are required
    - Test enum validations for Severity and Status
  - Run tests and ensure all pass

- [ ] Write integration tests for Product CRUD workflow:
  - Create Tests/Products/Integration/ProductCrudTests.cs
  - Test complete workflow:
    - Create product via API
    - Get product by ID
    - Update product
    - Search products with filters
    - Delete product
  - Use WebApplicationFactory for integration testing
  - Mock authentication and tenant context
  - Run integration tests and verify all scenarios pass

- [ ] Write integration tests for Category and Issue relationships:
  - Create CategoryCrudTests.cs
    - Test CRUD operations
    - Test deletion prevents when products reference category
  - Create IssueCrudTests.cs
    - Test creating issue for a product
    - Test updating issue status and resolution notes
    - Test cascade delete when product is deleted
  - Run all integration tests

- [ ] Perform manual end-to-end testing:
  - Run `dotnet run --project src/Playground/FSH.Playground.AppHost`
  - Access Swagger UI at the application URL
  - Authenticate with test credentials
  - Execute complete workflow:
    - Create 3 categories (e.g., "Electronics", "Furniture", "Materials")
    - Create 5 products across different categories
    - Create 8-10 issues with various severities and statuses
    - Test search/filter operations with pagination
    - Test update operations
    - Test delete with referential integrity checks
  - Document any bugs or UX issues found

- [ ] Build solution with zero warnings and run all tests:
  - Run `dotnet build src/FSH.Framework.slnx` and verify zero warnings
  - Run `dotnet test src/FSH.Framework.slnx` and ensure all tests pass
  - Generate test coverage report if coverage tools are configured
  - Document final test results and coverage metrics
