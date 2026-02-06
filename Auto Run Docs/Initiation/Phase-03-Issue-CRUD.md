# Phase 03: Issue CRUD

Implement complete CRUD functionality for Issue entities. Issues are related to Products and include severity and status tracking, making them slightly more complex than Categories.

## Tasks

- [x] Add Issue permissions to ProductsPermissions:
  - Add nested Issues class in ProductsPermissions.cs
  - Define Create, View, Update, Delete, and List permission constants
  - Follow pattern: "Permissions.Products.Issues.{Action}"
  - **Completed**: Added Issues nested class with all 5 permission constants following the same pattern as Categories.

- [ ] Implement CreateIssue command feature:
  - Create CreateIssueCommand.cs in Contracts/Application/Features/Commands/CreateIssue/
    - Record with ProductId, Title, Description, Severity, Status properties implementing ICommand<int>
  - Create CreateIssueResponse.cs with Id property
  - Create CreateIssueCommandHandler.cs in Application/Features/Commands/CreateIssue/
    - Inject ProductsDbContext and ICurrentUser
    - Verify ProductId exists and belongs to current tenant
    - Create new Issue entity with TenantId
    - Set default Severity to Medium and Status to Open if not provided
    - Save to database and return Id
  - Create CreateIssueCommandValidator.cs
    - Validate ProductId is required
    - Validate Title is required with max length
    - Validate Description is required
    - Validate Severity and Status are valid enum values
  - Create CreateIssueEndpoint.cs in Endpoints/v1/
    - MapPost("/issues") with IMediator
    - Apply RequirePermission(ProductsPermissions.Issues.Create)
  - Register endpoint in ProductsModule with issues group

- [ ] Implement GetIssueById query feature:
  - Create IssueDto.cs in Contracts/DTOs/
    - Properties: Id, ProductId, ProductTitle, Title, Description, Severity, Status, ResolutionNotes
    - Include audit fields: TenantId, CreatedOnUtc, CreatedBy, LastModifiedOnUtc, LastModifiedBy
  - Create GetIssueByIdQuery.cs in Contracts/Application/Features/Queries/GetIssueById/
    - Record with Id property implementing IQuery<IssueDto>
  - Create GetIssueByIdQueryHandler.cs in Application/Features/Queries/GetIssueById/
    - Inject ProductsDbContext and ICurrentUser
    - Find issue by Id and tenant with Include for Product navigation
    - Map to IssueDto including ProductTitle from related Product
    - Return dto or throw NotFoundException
  - Create GetIssueByIdEndpoint.cs in Endpoints/v1/
    - MapGet("/issues/{id}") with IMediator
    - Apply RequirePermission(ProductsPermissions.Issues.View)
  - Register endpoint in ProductsModule

- [ ] Implement SearchIssues query with filtering and pagination:
  - Create SearchIssuesQuery.cs in Contracts/Application/Features/Queries/SearchIssues/
    - Inherit from PagedRequest implementing IQuery<PagedResponse<IssueDto>>
    - Include filter properties: Search, ProductId, Severity, Status
    - Include Sort property for ordering
  - Create SearchIssuesQueryHandler.cs in Application/Features/Queries/SearchIssues/
    - Inject ProductsDbContext and ICurrentUser
    - Build query with Include for Product navigation
    - Filter by tenant and optional filters (search on Title/Description, ProductId, Severity, Status)
    - Implement sorting by Title, Severity, Status, CreatedOnUtc
    - Map to IssueDto with ProductTitle
    - Use ToPagedResponseAsync for pagination
  - Create SearchIssuesQueryValidator.cs
  - Create SearchIssuesEndpoint.cs in Endpoints/v1/
    - MapGet("/issues") with query parameters
    - Apply RequirePermission(ProductsPermissions.Issues.View)
  - Register endpoint in ProductsModule

- [ ] Implement UpdateIssue command feature:
  - Create UpdateIssueCommand.cs in Contracts/Application/Features/Commands/UpdateIssue/
    - Record with Id, Title, Description, Severity, Status, ResolutionNotes properties implementing ICommand
  - Create UpdateIssueCommandHandler.cs in Application/Features/Commands/UpdateIssue/
    - Inject ProductsDbContext and ICurrentUser
    - Find issue by Id and tenant
    - Update all properties from command
    - Save changes
  - Create UpdateIssueCommandValidator.cs with same rules as Create
  - Create UpdateIssueEndpoint.cs in Endpoints/v1/
    - MapPut("/issues/{id}") with IMediator
    - Apply RequirePermission(ProductsPermissions.Issues.Update)
  - Register endpoint in ProductsModule

- [ ] Implement DeleteIssue command feature:
  - Create DeleteIssueCommand.cs in Contracts/Application/Features/Commands/DeleteIssue/
    - Record with Id property implementing ICommand
  - Create DeleteIssueCommandHandler.cs in Application/Features/Commands/DeleteIssue/
    - Inject ProductsDbContext and ICurrentUser
    - Find issue by Id and tenant
    - Remove issue from DbSet
    - Save changes
  - Create DeleteIssueEndpoint.cs in Endpoints/v1/
    - MapDelete("/issues/{id}") with IMediator
    - Apply RequirePermission(ProductsPermissions.Issues.Delete)
  - Register endpoint in ProductsModule

- [ ] Build and test Issue CRUD functionality:
  - Run `dotnet build src/FSH.Framework.slnx` and ensure zero warnings
  - Run application and access Swagger UI
  - Test complete CRUD workflow for issues:
    - Create a test product first
    - Create 3-4 test issues with different severities and statuses
    - Get issue by ID and verify ProductTitle is included
    - Search issues by ProductId
    - Search issues by Severity and Status
    - Update an issue status from Open to Resolved with resolution notes
    - Delete a resolved issue
    - Verify cascade behavior when deleting a product with issues
  - Document test results and any edge cases found
