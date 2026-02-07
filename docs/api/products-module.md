---
type: reference
title: Products Module API Documentation
created: 2026-02-07
tags:
  - api
  - products
  - crud
  - categories
  - issues
related:
  - "[[Category]]"
  - "[[Product]]"
  - "[[Issue]]"
  - "[[FSH-Patterns]]"
---

# Products Module API Documentation

## Overview

The Products module provides comprehensive CRUD operations for managing products, categories, and issues in a multi-tenant environment. It follows the FullStackHero (FSH) Vertical Slice Architecture pattern with Mediator-based command/query handling.

**Key Features:**
- Multi-tenant isolation with automatic tenant scoping
- Full CRUD operations for Categories, Products, and Issues
- Advanced search and filtering with pagination
- Audit tracking (created/modified timestamps and users)
- Soft delete support with optimistic concurrency
- Permission-based authorization

**Base URL Pattern:**
- Categories: `/api/v1/categories`
- Products: `/api/v1/products`
- Issues: `/api/v1/issues`

## Authentication & Permissions

All endpoints require authentication via JWT bearer token and specific permissions.

### Permission Structure

**Category Permissions:**
- `Permissions.Products.Categories.Create` - Create new categories
- `Permissions.Products.Categories.View` - View individual categories
- `Permissions.Products.Categories.Update` - Update existing categories
- `Permissions.Products.Categories.Delete` - Delete categories
- `Permissions.Products.Categories.List` - Search and list categories

**Product Permissions:**
- `Permissions.Products.Create` - Create new products
- `Permissions.Products.View` - View individual products
- `Permissions.Products.Update` - Update existing products
- `Permissions.Products.Delete` - Delete products
- `Permissions.Products.View` - Search and list products (uses View permission)

**Issue Permissions:**
- `Permissions.Products.Issues.Create` - Create new issues
- `Permissions.Products.Issues.View` - View individual issues
- `Permissions.Products.Issues.Update` - Update existing issues
- `Permissions.Products.Issues.Delete` - Delete issues
- `Permissions.Products.Issues.List` - Search and list issues

## Entities

### Category

Represents a product category for organizational purposes.

**Schema:**
```json
{
  "id": 1,
  "name": "Electronics",
  "tenantId": "tenant-abc-123",
  "createdOnUtc": "2024-01-15T10:30:00Z",
  "createdBy": "user-123",
  "lastModifiedOnUtc": "2024-02-01T14:20:00Z",
  "lastModifiedBy": "user-456"
}
```

**Properties:**
- `id` (integer): Unique identifier
- `name` (string, required): Category name
- `tenantId` (string): Tenant identifier (automatically set)
- `createdOnUtc` (datetime): Creation timestamp
- `createdBy` (string, nullable): User who created the category
- `lastModifiedOnUtc` (datetime, nullable): Last modification timestamp
- `lastModifiedBy` (string, nullable): User who last modified the category

### Product

Represents a product with comprehensive metadata and versioning support.

**Schema:**
```json
{
  "id": 101,
  "title": "Widget Pro 2000",
  "revision": 2,
  "status": 1,
  "description": "High-performance industrial widget",
  "private": false,
  "customerId": "CUST-001",
  "variant": "Standard",
  "version": 1,
  "categoryId": 1,
  "categoryName": "Electronics",
  "keywords": "widget,industrial,performance",
  "language": "en-US",
  "subject": "Manufacturing",
  "notes": "Internal notes here",
  "hashSha256": "abc123...",
  "characteristics": {
    "isAssembly": false,
    "isJobWork": false,
    "isManufacturable": true,
    "isCommercial": true,
    "isSellable": true,
    "isQualityCheckRequired": true
  },
  "isLatest": true,
  "mirroring": {
    "isMirrored": false,
    "sourceProductTitle": null,
    "sourceProductRevision": null,
    "copyProduct": false
  },
  "tenantId": "tenant-abc-123",
  "createdOnUtc": "2024-01-15T10:30:00Z",
  "createdBy": "user-123",
  "lastModifiedOnUtc": "2024-02-01T14:20:00Z",
  "lastModifiedBy": "user-456"
}
```

**Properties:**
- `id` (integer): Unique identifier
- `title` (string, required): Product title
- `revision` (integer): Revision number
- `status` (enum): Product state - `0` (Draft), `1` (Active), `2` (Obsolete)
- `description` (string, nullable): Product description
- `private` (boolean): Whether product is private
- `customerId` (string, nullable): Customer identifier
- `variant` (string): Product variant identifier
- `version` (integer): Version number
- `categoryId` (integer, nullable): Associated category ID
- `categoryName` (string, nullable): Associated category name
- `keywords` (string, nullable): Search keywords
- `language` (string, nullable): Language code
- `subject` (string, nullable): Subject or topic
- `notes` (string, nullable): Internal notes
- `hashSha256` (string, nullable): SHA-256 hash for verification
- `characteristics` (object, nullable): Product characteristics
  - `isAssembly` (boolean): Is an assembly
  - `isJobWork` (boolean): Is job work
  - `isManufacturable` (boolean): Can be manufactured
  - `isCommercial` (boolean): Is commercial
  - `isSellable` (boolean): Can be sold
  - `isQualityCheckRequired` (boolean): Requires quality check
- `isLatest` (boolean): Is the latest version
- `mirroring` (object, nullable): Mirroring information
  - `isMirrored` (boolean): Is mirrored from another source
  - `sourceProductTitle` (string, nullable): Source product title
  - `sourceProductRevision` (string, nullable): Source product revision
  - `copyProduct` (boolean): Copy product during mirroring
- `tenantId` (string): Tenant identifier
- `createdOnUtc` (datetime): Creation timestamp
- `createdBy` (string, nullable): User who created the product
- `lastModifiedOnUtc` (datetime, nullable): Last modification timestamp
- `lastModifiedBy` (string, nullable): User who last modified the product

### Issue

Represents a product issue or defect with severity and status tracking.

**Schema:**
```json
{
  "id": 201,
  "productId": 101,
  "productTitle": "Widget Pro 2000",
  "title": "Performance degradation under load",
  "description": "Product shows performance issues when processing more than 100 items",
  "severity": 2,
  "status": 0,
  "resolutionNotes": null,
  "tenantId": "tenant-abc-123",
  "createdOnUtc": "2024-02-01T10:30:00Z",
  "createdBy": "user-789",
  "lastModifiedOnUtc": null,
  "lastModifiedBy": null
}
```

**Properties:**
- `id` (integer): Unique identifier
- `productId` (integer, required): Associated product ID
- `productTitle` (string): Associated product title
- `title` (string, required): Issue title
- `description` (string, required): Detailed issue description
- `severity` (enum): Severity level - `0` (Low), `1` (Medium), `2` (High), `3` (Critical)
- `status` (enum): Issue status - `0` (Open), `1` (Close), `2` (Cancelled), `3` (Resolved)
- `resolutionNotes` (string, nullable): Notes about the resolution
- `tenantId` (string): Tenant identifier
- `createdOnUtc` (datetime): Creation timestamp
- `createdBy` (string, nullable): User who created the issue
- `lastModifiedOnUtc` (datetime, nullable): Last modification timestamp
- `lastModifiedBy` (string, nullable): User who last modified the issue

## Endpoints

### Category Endpoints

#### 1. Create Category

Creates a new category within the current tenant.

**Endpoint:** `POST /api/v1/categories`
**Permission:** `Permissions.Products.Categories.Create`

**Request Body:**
```json
{
  "name": "Electronics"
}
```

**Response:** `201 Created`
```json
{
  "id": 1
}
```

**Validation Rules:**
- `name` is required and must not be empty
- Category names must be unique within the tenant

---

#### 2. Get Category by ID

Retrieves a single category by its identifier.

**Endpoint:** `GET /api/v1/categories/{id}`
**Permission:** `Permissions.Products.Categories.View`

**Path Parameters:**
- `id` (integer, required): Category identifier

**Response:** `200 OK`
```json
{
  "id": 1,
  "name": "Electronics",
  "tenantId": "tenant-abc-123",
  "createdOnUtc": "2024-01-15T10:30:00Z",
  "createdBy": "user-123",
  "lastModifiedOnUtc": "2024-02-01T14:20:00Z",
  "lastModifiedBy": "user-456"
}
```

**Error Responses:**
- `404 Not Found`: Category not found or does not belong to current tenant

---

#### 3. Update Category

Updates an existing category.

**Endpoint:** `PUT /api/v1/categories/{id}`
**Permission:** `Permissions.Products.Categories.Update`

**Path Parameters:**
- `id` (integer, required): Category identifier

**Request Body:**
```json
{
  "id": 1,
  "name": "Consumer Electronics"
}
```

**Response:** `204 No Content`

**Validation Rules:**
- `id` is required and must match the path parameter
- `name` is required and must not be empty
- Category names must be unique within the tenant

**Error Responses:**
- `404 Not Found`: Category not found or does not belong to current tenant

---

#### 4. Delete Category

Deletes a category. Categories with associated products cannot be deleted.

**Endpoint:** `DELETE /api/v1/categories/{id}`
**Permission:** `Permissions.Products.Categories.Delete`

**Path Parameters:**
- `id` (integer, required): Category identifier

**Response:** `204 No Content`

**Error Responses:**
- `404 Not Found`: Category not found or does not belong to current tenant
- `400 Bad Request`: Category has associated products and cannot be deleted

---

#### 5. Search Categories

Retrieves a paginated list of categories with optional filtering and sorting.

**Endpoint:** `GET /api/v1/categories`
**Permission:** `Permissions.Products.Categories.View`

**Query Parameters:**
- `pageNumber` (integer, optional, default: 1): 1-based page number
- `pageSize` (integer, optional, default: 10): Number of items per page
- `sort` (string, optional): Multi-column sort expression (e.g., `"Name,-CreatedOnUtc"`)
  - Prefix with `-` for descending order
  - Multiple columns separated by comma
- `search` (string, optional): Search term to filter by category name

**Response:** `200 OK`
```json
{
  "data": [
    {
      "id": 1,
      "name": "Electronics",
      "tenantId": "tenant-abc-123",
      "createdOnUtc": "2024-01-15T10:30:00Z",
      "createdBy": "user-123",
      "lastModifiedOnUtc": "2024-02-01T14:20:00Z",
      "lastModifiedBy": "user-456"
    },
    {
      "id": 2,
      "name": "Furniture",
      "tenantId": "tenant-abc-123",
      "createdOnUtc": "2024-01-16T09:15:00Z",
      "createdBy": "user-123",
      "lastModifiedOnUtc": null,
      "lastModifiedBy": null
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 2,
  "totalPages": 1,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

---

### Product Endpoints

#### 6. Create Product

Creates a new product with comprehensive details.

**Endpoint:** `POST /api/v1/products`
**Permission:** `Permissions.Products.Create`

**Request Body:**
```json
{
  "title": "Widget Pro 2000",
  "revision": 1,
  "description": "High-performance industrial widget",
  "status": 1,
  "private": false,
  "customerId": "CUST-001",
  "variant": "Standard",
  "version": 1,
  "categoryId": 1,
  "keywords": "widget,industrial,performance",
  "language": "en-US",
  "subject": "Manufacturing",
  "notes": "Internal notes",
  "hashSha256": null,
  "isLatest": true,
  "characteristics": {
    "isAssembly": false,
    "isJobWork": false,
    "isManufacturable": true,
    "isCommercial": true,
    "isSellable": true,
    "isQualityCheckRequired": true
  },
  "mirroring": null
}
```

**Response:** `201 Created`
```json
{
  "id": 101
}
```

**Validation Rules:**
- `title` is required (min: 1 char, max: 200 chars)
- `revision` must be >= 0
- `status` must be a valid ProductState enum value (0, 1, or 2)
- `variant` has max length of 100 characters
- `version` must be >= 0
- `categoryId` must reference an existing category if provided
- `keywords`, `language`, `subject`, `notes` have max length of 500 characters

---

#### 7. Get Product by ID

Retrieves a single product by its identifier with related category information.

**Endpoint:** `GET /api/v1/products/{id}`
**Permission:** `Permissions.Products.View`

**Path Parameters:**
- `id` (integer, required): Product identifier

**Response:** `200 OK`
```json
{
  "id": 101,
  "title": "Widget Pro 2000",
  "revision": 2,
  "status": 1,
  "description": "High-performance industrial widget",
  "private": false,
  "customerId": "CUST-001",
  "variant": "Standard",
  "version": 1,
  "categoryId": 1,
  "categoryName": "Electronics",
  "keywords": "widget,industrial,performance",
  "language": "en-US",
  "subject": "Manufacturing",
  "notes": "Internal notes",
  "hashSha256": null,
  "characteristics": {
    "isAssembly": false,
    "isJobWork": false,
    "isManufacturable": true,
    "isCommercial": true,
    "isSellable": true,
    "isQualityCheckRequired": true
  },
  "isLatest": true,
  "mirroring": {
    "isMirrored": false,
    "sourceProductTitle": null,
    "sourceProductRevision": null,
    "copyProduct": false
  },
  "tenantId": "tenant-abc-123",
  "createdOnUtc": "2024-01-15T10:30:00Z",
  "createdBy": "user-123",
  "lastModifiedOnUtc": "2024-02-01T14:20:00Z",
  "lastModifiedBy": "user-456"
}
```

**Error Responses:**
- `404 Not Found`: Product not found or does not belong to current tenant

---

#### 8. Update Product

Updates an existing product.

**Endpoint:** `PUT /api/v1/products/{id}`
**Permission:** `Permissions.Products.Update`

**Path Parameters:**
- `id` (integer, required): Product identifier

**Request Body:**
```json
{
  "id": 101,
  "title": "Widget Pro 2000 - Enhanced",
  "revision": 2,
  "description": "Enhanced high-performance industrial widget",
  "status": 1,
  "private": false,
  "customerId": "CUST-001",
  "variant": "Standard",
  "version": 1,
  "categoryId": 1,
  "keywords": "widget,industrial,performance,enhanced",
  "language": "en-US",
  "subject": "Manufacturing",
  "notes": "Updated with new features",
  "hashSha256": null,
  "isLatest": true,
  "characteristics": {
    "isAssembly": false,
    "isJobWork": false,
    "isManufacturable": true,
    "isCommercial": true,
    "isSellable": true,
    "isQualityCheckRequired": true
  },
  "mirroring": null
}
```

**Response:** `204 No Content`

**Validation Rules:** Same as Create Product

**Error Responses:**
- `404 Not Found`: Product not found or does not belong to current tenant

---

#### 9. Delete Product

Deletes a product and all its associated issues.

**Endpoint:** `DELETE /api/v1/products/{id}`
**Permission:** `Permissions.Products.Delete`

**Path Parameters:**
- `id` (integer, required): Product identifier

**Response:** `204 No Content`

**Error Responses:**
- `404 Not Found`: Product not found or does not belong to current tenant

**Note:** Deleting a product will cascade delete all associated issues.

---

#### 10. Search Products

Retrieves a paginated list of products with advanced filtering and sorting.

**Endpoint:** `GET /api/v1/products`
**Permission:** `Permissions.Products.View`

**Query Parameters:**
- `pageNumber` (integer, optional, default: 1): 1-based page number
- `pageSize` (integer, optional, default: 10): Number of items per page
- `sort` (string, optional): Multi-column sort expression (e.g., `"Title,-CreatedOnUtc"`)
  - Prefix with `-` for descending order
  - Multiple columns separated by comma
- `search` (string, optional): Search term to filter by title, description, keywords, or subject
- `categoryId` (integer, optional): Filter by category ID
- `status` (integer, optional): Filter by product status (0=Draft, 1=Active, 2=Obsolete)
- `isLatest` (boolean, optional): Filter by whether product is the latest version

**Example Request:**
```
GET /api/v1/products?search=widget&categoryId=1&status=1&isLatest=true&pageNumber=1&pageSize=20&sort=Title,-CreatedOnUtc
```

**Response:** `200 OK`
```json
{
  "data": [
    {
      "id": 101,
      "title": "Widget Pro 2000",
      "revision": 2,
      "status": 1,
      "description": "High-performance industrial widget",
      "private": false,
      "customerId": "CUST-001",
      "variant": "Standard",
      "version": 1,
      "categoryId": 1,
      "categoryName": "Electronics",
      "keywords": "widget,industrial,performance",
      "language": "en-US",
      "subject": "Manufacturing",
      "notes": null,
      "hashSha256": null,
      "characteristics": {
        "isAssembly": false,
        "isJobWork": false,
        "isManufacturable": true,
        "isCommercial": true,
        "isSellable": true,
        "isQualityCheckRequired": true
      },
      "isLatest": true,
      "mirroring": {
        "isMirrored": false,
        "sourceProductTitle": null,
        "sourceProductRevision": null,
        "copyProduct": false
      },
      "tenantId": "tenant-abc-123",
      "createdOnUtc": "2024-01-15T10:30:00Z",
      "createdBy": "user-123",
      "lastModifiedOnUtc": "2024-02-01T14:20:00Z",
      "lastModifiedBy": "user-456"
    }
  ],
  "pageNumber": 1,
  "pageSize": 20,
  "totalCount": 1,
  "totalPages": 1,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

**Filtering Behavior:**
- `search`: Performs case-insensitive search across title, description, keywords, and subject fields
- `categoryId`: Exact match filter
- `status`: Exact match filter (enum values: 0=Draft, 1=Active, 2=Obsolete)
- `isLatest`: Exact match filter for latest version flag
- All filters are combined with AND logic
- All results are automatically scoped to the current tenant

---

### Issue Endpoints

#### 11. Create Issue

Creates a new issue associated with a product.

**Endpoint:** `POST /api/v1/issues`
**Permission:** `Permissions.Products.Issues.Create`

**Request Body:**
```json
{
  "productId": 101,
  "title": "Performance degradation under load",
  "description": "Product shows performance issues when processing more than 100 items",
  "severity": 2,
  "status": 0
}
```

**Response:** `201 Created`
```json
{
  "id": 201
}
```

**Validation Rules:**
- `productId` is required and must reference an existing product
- `title` is required (min: 1 char, max: 200 chars)
- `description` is required (min: 1 char, max: 2000 chars)
- `severity` must be a valid IssueSeverity enum value (0-3) if provided
- `status` must be a valid IssueState enum value (0-3) if provided

---

#### 12. Get Issue by ID

Retrieves a single issue by its identifier with related product information.

**Endpoint:** `GET /api/v1/issues/{id}`
**Permission:** `Permissions.Products.Issues.View`

**Path Parameters:**
- `id` (integer, required): Issue identifier

**Response:** `200 OK`
```json
{
  "id": 201,
  "productId": 101,
  "productTitle": "Widget Pro 2000",
  "title": "Performance degradation under load",
  "description": "Product shows performance issues when processing more than 100 items",
  "severity": 2,
  "status": 0,
  "resolutionNotes": null,
  "tenantId": "tenant-abc-123",
  "createdOnUtc": "2024-02-01T10:30:00Z",
  "createdBy": "user-789",
  "lastModifiedOnUtc": null,
  "lastModifiedBy": null
}
```

**Error Responses:**
- `404 Not Found`: Issue not found or does not belong to current tenant

---

#### 13. Update Issue

Updates an existing issue.

**Endpoint:** `PUT /api/v1/issues/{id}`
**Permission:** `Permissions.Products.Issues.Update`

**Path Parameters:**
- `id` (integer, required): Issue identifier

**Request Body:**
```json
{
  "id": 201,
  "title": "Performance degradation under load - RESOLVED",
  "description": "Product showed performance issues when processing more than 100 items. Fixed by optimizing database queries.",
  "severity": 2,
  "status": 3,
  "resolutionNotes": "Optimized database queries and added caching"
}
```

**Response:** `204 No Content`

**Validation Rules:**
- `id` is required and must match the path parameter
- `title` is required (min: 1 char, max: 200 chars)
- `description` is required (min: 1 char, max: 2000 chars)
- `severity` must be a valid IssueSeverity enum value (0-3)
- `status` must be a valid IssueState enum value (0-3)
- `resolutionNotes` has max length of 2000 characters

**Error Responses:**
- `404 Not Found`: Issue not found or does not belong to current tenant

---

#### 14. Delete Issue

Deletes an issue.

**Endpoint:** `DELETE /api/v1/issues/{id}`
**Permission:** `Permissions.Products.Issues.Delete`

**Path Parameters:**
- `id` (integer, required): Issue identifier

**Response:** `204 No Content`

**Error Responses:**
- `404 Not Found`: Issue not found or does not belong to current tenant

---

#### 15. Search Issues

Retrieves a paginated list of issues with advanced filtering and sorting.

**Endpoint:** `GET /api/v1/issues`
**Permission:** `Permissions.Products.Issues.List`

**Query Parameters:**
- `pageNumber` (integer, optional, default: 1): 1-based page number
- `pageSize` (integer, optional, default: 10): Number of items per page
- `sort` (string, optional): Multi-column sort expression (e.g., `"-Severity,CreatedOnUtc"`)
  - Prefix with `-` for descending order
  - Multiple columns separated by comma
- `search` (string, optional): Search term to filter by title or description
- `productId` (integer, optional): Filter by specific product ID
- `severity` (integer, optional): Filter by severity level (0=Low, 1=Medium, 2=High, 3=Critical)
- `status` (integer, optional): Filter by status (0=Open, 1=Close, 2=Cancelled, 3=Resolved)

**Example Request:**
```
GET /api/v1/issues?search=performance&productId=101&severity=2&status=0&pageNumber=1&pageSize=10&sort=-Severity,CreatedOnUtc
```

**Response:** `200 OK`
```json
{
  "data": [
    {
      "id": 201,
      "productId": 101,
      "productTitle": "Widget Pro 2000",
      "title": "Performance degradation under load",
      "description": "Product shows performance issues when processing more than 100 items",
      "severity": 2,
      "status": 0,
      "resolutionNotes": null,
      "tenantId": "tenant-abc-123",
      "createdOnUtc": "2024-02-01T10:30:00Z",
      "createdBy": "user-789",
      "lastModifiedOnUtc": null,
      "lastModifiedBy": null
    }
  ],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 1,
  "totalPages": 1,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

**Filtering Behavior:**
- `search`: Performs case-insensitive search across title and description fields
- `productId`: Exact match filter
- `severity`: Exact match filter (enum values: 0=Low, 1=Medium, 2=High, 3=Critical)
- `status`: Exact match filter (enum values: 0=Open, 1=Close, 2=Cancelled, 3=Resolved)
- All filters are combined with AND logic
- All results are automatically scoped to the current tenant

---

## Pagination

All search endpoints return paginated results using the `PagedResponse<T>` wrapper.

**Pagination Parameters:**
- `pageNumber`: 1-based page number (default: 1, minimum: 1)
- `pageSize`: Items per page (default: 10, configurable maximum)

**Pagination Response Structure:**
```json
{
  "data": [...],
  "pageNumber": 1,
  "pageSize": 10,
  "totalCount": 45,
  "totalPages": 5,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

## Sorting

All search endpoints support multi-column sorting via the `sort` query parameter.

**Sorting Syntax:**
- Single column: `"Name"` (ascending)
- Descending: `"-Name"` (prefix with `-`)
- Multiple columns: `"Name,-CreatedOnUtc"` (comma-separated)

**Common Sort Fields:**
- **Categories**: `Name`, `CreatedOnUtc`
- **Products**: `Title`, `Status`, `CreatedOnUtc`, `Revision`, `Version`
- **Issues**: `Title`, `Severity`, `Status`, `CreatedOnUtc`

## Error Responses

All endpoints follow consistent error response patterns:

**404 Not Found:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Category with id 123 not found."
}
```

**400 Bad Request:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "Bad Request",
  "status": 400,
  "detail": "Cannot delete category because it has associated products.",
  "errors": {
    "Name": ["The Name field is required."]
  }
}
```

**401 Unauthorized:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7235#section-3.1",
  "title": "Unauthorized",
  "status": 401,
  "detail": "Authentication required."
}
```

**403 Forbidden:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.3",
  "title": "Forbidden",
  "status": 403,
  "detail": "You do not have permission to perform this action."
}
```

## Multi-Tenancy

All endpoints automatically scope data to the current tenant based on the authenticated user's tenant context. This is enforced at the database query level to ensure data isolation.

**Tenant Isolation Features:**
- All queries automatically filter by `TenantId`
- Cross-tenant access is not possible
- Tenant context is extracted from JWT claims
- No explicit tenant parameter needed in API calls

## Data Validation

All command/mutation endpoints include comprehensive validation:

**Common Validation Rules:**
- Required fields are validated for presence
- String length limits are enforced
- Enum values are validated against allowed values
- Foreign key relationships are verified (CategoryId, ProductId)
- Unique constraints are enforced (Category names within tenant)

**Validation Error Response:**
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Title": ["Title is required and cannot be empty."],
    "Description": ["Description must not exceed 2000 characters."]
  }
}
```

## Best Practices

1. **Always use pagination** for search endpoints to avoid loading large datasets
2. **Filter before sorting** to improve query performance
3. **Use specific permissions** rather than granting broad access
4. **Check for 404 responses** when working with foreign key relationships
5. **Handle validation errors** gracefully with user-friendly messages
6. **Use IsLatest flag** to query only the current version of products
7. **Leverage multi-column sorting** for complex result ordering
8. **Search is case-insensitive** and performs partial matching

## Related Documentation

- [[FSH-Patterns]] - FullStackHero architecture patterns
- [[Vertical-Slice-Architecture]] - Understanding the vertical slice approach
- [[Mediator-Pattern]] - Command/Query handling with Mediator
- [[Multi-Tenancy]] - Multi-tenant data isolation
- [[Permission-System]] - Authorization and permissions
