---
type: test-plan
title: Product API CRUD Testing Plan
created: 2026-02-05
tags:
  - testing
  - products
  - api
  - crud
related:
  - "[[Product-Entity-Analysis]]"
---

# Product API CRUD Testing Plan

## Executive Summary

This document provides a comprehensive test plan for the complete Product CRUD API implementation. The API includes 5 endpoints covering all CRUD operations with proper authentication, authorization, validation, and multi-tenancy support.

## Prerequisites

### Environment Requirements

1. **Docker Desktop**: Must be running for database and cache dependencies
   - PostgreSQL container for data persistence
   - Redis container for caching

2. **.NET 10 SDK**: Installed at `/usr/local/share/dotnet/dotnet`

3. **Application Start Command**:
   ```bash
   export PATH="/usr/local/share/dotnet:$PATH"
   dotnet run --project src/Playground/FSH.Playground.AppHost
   ```

### Access Points

- **Aspire Dashboard**: https://localhost:17273
- **API Base URL**: https://localhost:7030 (HTTPS) or http://localhost:5030 (HTTP)
- **API Documentation**: https://localhost:7030/scalar
- **Authentication**: Required via JWT bearer token
- **Multi-tenancy**: All operations are tenant-scoped

## API Endpoints Overview

### 1. Create Product
- **Method**: POST
- **Path**: `/api/v1/products`
- **Permission**: `Permissions.Products.Create`
- **Request Body**: `CreateProductCommand`
- **Response**: `201 Created` with `CreateProductResponse` (contains `Id`)

### 2. Get Product By Id
- **Method**: GET
- **Path**: `/api/v1/products/{id:int}`
- **Permission**: `Permissions.Products.View`
- **Response**: `200 OK` with `ProductDto` or `404 Not Found`

### 3. Search Products
- **Method**: GET
- **Path**: `/api/v1/products`
- **Permission**: `Permissions.Products.View`
- **Query Parameters**:
  - `PageNumber` (int, default: 1)
  - `PageSize` (int, default: 10)
  - `Search` (string, optional) - searches Title and Description
  - `CategoryId` (int?, optional)
  - `Status` (ProductState enum, optional)
  - `IsLatest` (bool?, optional)
  - `Sort` (string, optional) - e.g., "Title ASC", "CreatedOnUtc DESC"
- **Response**: `200 OK` with `PagedResponse<ProductDto>`

### 4. Update Product
- **Method**: PUT
- **Path**: `/api/v1/products/{id:int}`
- **Permission**: `Permissions.Products.Update`
- **Request Body**: `UpdateProductCommand` (Id is in route)
- **Response**: `200 OK` or `404 Not Found`

### 5. Delete Product
- **Method**: DELETE
- **Path**: `/api/v1/products/{id:int}`
- **Permission**: `Permissions.Products.Delete`
- **Response**: `204 No Content` or `404 Not Found`

## Test Scenarios

### Test Case 1: Create Product - Success

**Objective**: Verify that a product can be created successfully with valid data.

**Prerequisites**:
- User authenticated with valid JWT token
- User has `Permissions.Products.Create` permission
- User is associated with a tenant

**Request**:
```http
POST /api/v1/products
Content-Type: application/json
Authorization: Bearer {token}

{
  "title": "Test Product Alpha",
  "description": "A comprehensive test product for API validation",
  "categoryId": 1,
  "unitPrice": 99.99,
  "productCharacteristics": {
    "brand": "TestBrand",
    "model": "TB-001",
    "technology": "Advanced Testing Tech",
    "certification": "ISO-9001"
  },
  "mirroringInfo": {
    "mirroringType": "Full",
    "latestVersion": "1.0.0"
  },
  "isLatest": true,
  "status": 1
}
```

**Expected Response**:
- Status: `201 Created`
- Location header: `/api/v1/products/{newId}`
- Body:
```json
{
  "id": 123
}
```

**Validation Checklist**:
- [ ] Response status is 201
- [ ] Response contains valid product ID
- [ ] Location header points to the new resource
- [ ] Product is created in database with correct tenant association

### Test Case 2: Create Product - Validation Errors

**Objective**: Verify that invalid data is rejected with appropriate validation messages.

**Test Variations**:

a) **Missing Required Title**:
```json
{
  "description": "Missing title test",
  "categoryId": 1,
  "unitPrice": 50.00
}
```
Expected: `400 Bad Request` with validation error for Title

b) **Invalid Unit Price (negative)**:
```json
{
  "title": "Invalid Price Test",
  "categoryId": 1,
  "unitPrice": -10.00
}
```
Expected: `400 Bad Request` with validation error for UnitPrice

c) **Invalid Category**:
```json
{
  "title": "Invalid Category Test",
  "categoryId": 999,
  "unitPrice": 50.00
}
```
Expected: `400 Bad Request` (if category validation is enforced)

### Test Case 3: Get Product By Id - Success

**Objective**: Retrieve a specific product by its ID.

**Prerequisites**:
- Product with known ID exists in database for current tenant
- User has `Permissions.Products.View` permission

**Request**:
```http
GET /api/v1/products/{id}
Authorization: Bearer {token}
```

**Expected Response**:
- Status: `200 OK`
- Body:
```json
{
  "id": 123,
  "title": "Test Product Alpha",
  "description": "A comprehensive test product for API validation",
  "categoryId": 1,
  "unitPrice": 99.99,
  "productCharacteristics": {
    "brand": "TestBrand",
    "model": "TB-001",
    "technology": "Advanced Testing Tech",
    "certification": "ISO-9001"
  },
  "mirroringInfo": {
    "mirroringType": "Full",
    "latestVersion": "1.0.0"
  },
  "isLatest": true,
  "status": 1,
  "createdBy": "{userId}",
  "createdOnUtc": "2026-02-05T...",
  "lastModifiedBy": null,
  "lastModifiedOnUtc": null
}
```

**Validation Checklist**:
- [ ] All product properties are returned correctly
- [ ] Nested objects (ProductCharacteristics, MirroringInfo) are properly serialized
- [ ] Audit fields (CreatedBy, CreatedOnUtc) are populated
- [ ] Product belongs to the current user's tenant

### Test Case 4: Get Product By Id - Not Found

**Objective**: Verify proper handling of non-existent product IDs.

**Request**:
```http
GET /api/v1/products/999999
Authorization: Bearer {token}
```

**Expected Response**:
- Status: `404 Not Found`
- Body should contain error details

**Validation Checklist**:
- [ ] Status is 404
- [ ] Error message is informative
- [ ] No sensitive information is leaked

### Test Case 5: Get Product By Id - Tenant Isolation

**Objective**: Verify that users cannot access products from other tenants.

**Prerequisites**:
- Product exists but belongs to a different tenant

**Request**:
```http
GET /api/v1/products/{otherTenantProductId}
Authorization: Bearer {token}
```

**Expected Response**:
- Status: `404 Not Found` (product should not be visible to this tenant)

### Test Case 6: Search Products - Default Pagination

**Objective**: Verify default pagination behavior.

**Request**:
```http
GET /api/v1/products
Authorization: Bearer {token}
```

**Expected Response**:
- Status: `200 OK`
- Body:
```json
{
  "data": [...],
  "currentPage": 1,
  "totalPages": X,
  "totalCount": Y,
  "pageSize": 10,
  "hasPreviousPage": false,
  "hasNextPage": true/false
}
```

**Validation Checklist**:
- [ ] Default page is 1
- [ ] Default page size is 10
- [ ] Only products from current tenant are returned
- [ ] Pagination metadata is accurate

### Test Case 7: Search Products - With Filters

**Objective**: Verify search and filtering functionality.

**Test Variations**:

a) **Text Search**:
```http
GET /api/v1/products?Search=Alpha
Authorization: Bearer {token}
```
Expected: Products with "Alpha" in Title or Description

b) **Category Filter**:
```http
GET /api/v1/products?CategoryId=1
Authorization: Bearer {token}
```
Expected: Only products in category 1

c) **Status Filter**:
```http
GET /api/v1/products?Status=1
Authorization: Bearer {token}
```
Expected: Only products with status Active (1)

d) **IsLatest Filter**:
```http
GET /api/v1/products?IsLatest=true
Authorization: Bearer {token}
```
Expected: Only products marked as latest version

e) **Combined Filters**:
```http
GET /api/v1/products?Search=Test&CategoryId=1&IsLatest=true&PageSize=5
Authorization: Bearer {token}
```
Expected: Products matching ALL criteria

### Test Case 8: Search Products - Sorting

**Objective**: Verify sorting functionality.

**Test Variations**:

a) **Sort by Title Ascending**:
```http
GET /api/v1/products?Sort=Title ASC
Authorization: Bearer {token}
```

b) **Sort by Created Date Descending**:
```http
GET /api/v1/products?Sort=CreatedOnUtc DESC
Authorization: Bearer {token}
```

c) **Sort by Status**:
```http
GET /api/v1/products?Sort=Status ASC
Authorization: Bearer {token}
```

**Validation Checklist**:
- [ ] Results are ordered correctly
- [ ] Invalid sort fields are rejected
- [ ] Sort direction (ASC/DESC) is respected

### Test Case 9: Update Product - Success

**Objective**: Verify that a product can be updated successfully.

**Prerequisites**:
- Product exists with known ID
- User has `Permissions.Products.Update` permission

**Request**:
```http
PUT /api/v1/products/123
Content-Type: application/json
Authorization: Bearer {token}

{
  "id": 123,
  "title": "Updated Test Product Alpha",
  "description": "Updated description",
  "categoryId": 2,
  "unitPrice": 149.99,
  "productCharacteristics": {
    "brand": "UpdatedBrand",
    "model": "TB-002",
    "technology": "Next-Gen Testing Tech",
    "certification": "ISO-9001-2024"
  },
  "mirroringInfo": {
    "mirroringType": "Partial",
    "latestVersion": "2.0.0"
  },
  "isLatest": true,
  "status": 1
}
```

**Expected Response**:
- Status: `200 OK`

**Validation Checklist**:
- [ ] All fields are updated correctly
- [ ] LastModifiedBy and LastModifiedOnUtc are set
- [ ] CreatedBy and CreatedOnUtc remain unchanged
- [ ] Product remains in same tenant

### Test Case 10: Update Product - Validation Errors

**Objective**: Verify validation on update operations.

**Test Variations**:

a) **Route ID Mismatch**:
```http
PUT /api/v1/products/123
Body: { "id": 456, ... }
```
Expected: `400 Bad Request` - ID mismatch

b) **Invalid Data**:
Same validation rules as Create should apply

### Test Case 11: Update Product - Not Found

**Objective**: Verify handling of updates to non-existent products.

**Request**:
```http
PUT /api/v1/products/999999
Content-Type: application/json
Authorization: Bearer {token}

{
  "id": 999999,
  "title": "Non-existent Product",
  ...
}
```

**Expected Response**:
- Status: `404 Not Found`

### Test Case 12: Update Product - Tenant Isolation

**Objective**: Verify users cannot update products from other tenants.

**Prerequisites**:
- Product exists but belongs to different tenant

**Request**:
```http
PUT /api/v1/products/{otherTenantProductId}
Authorization: Bearer {token}
```

**Expected Response**:
- Status: `404 Not Found`

### Test Case 13: Delete Product - Success

**Objective**: Verify that a product can be deleted successfully.

**Prerequisites**:
- Product exists with known ID
- User has `Permissions.Products.Delete` permission

**Request**:
```http
DELETE /api/v1/products/123
Authorization: Bearer {token}
```

**Expected Response**:
- Status: `204 No Content`

**Validation Checklist**:
- [ ] Response has no body
- [ ] Product is removed from database
- [ ] Subsequent GET returns 404

**Note**: Based on code review findings, this currently performs a **hard delete**. Future enhancement should implement soft delete using `ISoftDeletable` interface.

### Test Case 14: Delete Product - Not Found

**Objective**: Verify handling of delete for non-existent products.

**Request**:
```http
DELETE /api/v1/products/999999
Authorization: Bearer {token}
```

**Expected Response**:
- Status: `404 Not Found`

### Test Case 15: Delete Product - Tenant Isolation

**Objective**: Verify users cannot delete products from other tenants.

**Prerequisites**:
- Product exists but belongs to different tenant

**Request**:
```http
DELETE /api/v1/products/{otherTenantProductId}
Authorization: Bearer {token}
```

**Expected Response**:
- Status: `404 Not Found`

## Authorization Test Cases

### Test Case 16: Unauthorized Access

**Objective**: Verify that all endpoints require authentication.

**Test for Each Endpoint**:
```http
{METHOD} /api/v1/products{/id}
# No Authorization header
```

**Expected Response**:
- Status: `401 Unauthorized`

### Test Case 17: Insufficient Permissions

**Objective**: Verify that endpoints enforce permission requirements.

**Test Variations**:
- User with `View` permission attempting `Create` → `403 Forbidden`
- User with `View` permission attempting `Update` → `403 Forbidden`
- User with `View` permission attempting `Delete` → `403 Forbidden`

## Complete Test Workflow

This workflow tests all CRUD operations in sequence:

1. **Authenticate**: Obtain JWT token
2. **Create Product**: Store returned ID
3. **Get By ID**: Verify created product
4. **Search All**: Verify product appears in list
5. **Search with Filter**: Find product by search term
6. **Update Product**: Modify some fields
7. **Get By ID**: Verify updates applied
8. **Delete Product**: Remove test product
9. **Get By ID**: Verify 404 response
10. **Search All**: Verify product no longer in list

## Known Issues and Future Enhancements

Based on code review findings in Phase 01:

### CRITICAL Issues to Address
1. **Missing Validators**:
   - `DeleteProductCommand` needs a validator (even if just for Id validation)
   - `GetProductByIdQuery` needs a validator

2. **Soft Delete Not Implemented**:
   - Product entity should implement `ISoftDeletable`
   - DeleteProduct should mark as deleted rather than removing
   - Queries should filter out soft-deleted products

3. **Entity Factory Pattern**:
   - Product entity should use static factory method instead of public constructor
   - Update handler uses direct property assignment instead of Update methods

### WARNING-Level Issues
4. **Namespace Inconsistency**:
   - `CreateProductResponse.cs` uses incorrect namespace

## Testing Blockers Encountered

### Docker Not Running

**Issue**: Aspire requires Docker Desktop to be running to start PostgreSQL and Redis containers.

**Error Message**:
```
Il runtime del contenitore 'docker' è stato trovato ma non è integro.
Assicurati che Docker sia in esecuzione e che il demone Docker sia accessibile.
```

**Resolution Required**:
1. Start Docker Desktop
2. Ensure Docker daemon is healthy
3. Re-run the Aspire application

**Verification**:
```bash
docker ps  # Should list running containers without error
```

### Environment Path Issue

**Issue**: The `dotnet` command was not in the PATH for subprocess executions.

**Resolution**: Use full path or set PATH environment variable:
```bash
export PATH="/usr/local/share/dotnet:$PATH"
dotnet run --project src/Playground/FSH.Playground.AppHost
```

## Test Execution Checklist

Before executing tests:
- [ ] Docker Desktop is running and healthy
- [ ] Aspire application starts successfully
- [ ] All containers are running (postgres, redis, API, Blazor)
- [ ] Can access Scalar documentation at https://localhost:7030/scalar
- [ ] Have valid authentication credentials
- [ ] User has all necessary permissions (Create, View, Update, Delete)

During test execution:
- [ ] Document all test results
- [ ] Capture screenshots of successful tests
- [ ] Note any unexpected behaviors
- [ ] Verify tenant isolation for multi-tenant scenarios
- [ ] Check database state after operations
- [ ] Monitor application logs for errors

After test execution:
- [ ] Clean up test data
- [ ] Document any bugs found
- [ ] Update task file with test results
- [ ] Create follow-up tasks for issues discovered

## Appendix: Sample Test Data

### Valid Product 1
```json
{
  "title": "Enterprise Widget Pro",
  "description": "Professional-grade widget for enterprise applications",
  "categoryId": 1,
  "unitPrice": 299.99,
  "productCharacteristics": {
    "brand": "ProTech",
    "model": "EWP-2024",
    "technology": "Cloud-Native Architecture",
    "certification": "SOC2 Type II"
  },
  "mirroringInfo": {
    "mirroringType": "Full",
    "latestVersion": "3.5.0"
  },
  "isLatest": true,
  "status": 1
}
```

### Valid Product 2
```json
{
  "title": "Budget Widget Lite",
  "description": "Affordable widget for small businesses",
  "categoryId": 1,
  "unitPrice": 49.99,
  "productCharacteristics": {
    "brand": "ValueTech",
    "model": "BWL-2024",
    "technology": "Standard Implementation",
    "certification": "Basic"
  },
  "mirroringInfo": {
    "mirroringType": "Partial",
    "latestVersion": "1.2.0"
  },
  "isLatest": true,
  "status": 1
}
```

### Valid Product 3 (Different Category)
```json
{
  "title": "Premium Gadget Suite",
  "description": "Complete suite of premium gadgets",
  "categoryId": 2,
  "unitPrice": 899.99,
  "productCharacteristics": {
    "brand": "LuxuryTech",
    "model": "PGS-2024",
    "technology": "AI-Powered Automation",
    "certification": "ISO 27001"
  },
  "mirroringInfo": {
    "mirroringType": "Full",
    "latestVersion": "5.0.0"
  },
  "isLatest": true,
  "status": 1
}
```

## Conclusion

This comprehensive test plan covers all aspects of the Product CRUD API including:
- All 5 endpoints (Create, Get, Search, Update, Delete)
- Validation scenarios
- Authorization and authentication
- Multi-tenancy isolation
- Pagination and filtering
- Error handling

Once Docker is running and the environment is properly configured, execute these tests systematically to verify the complete functionality of the Product API.
