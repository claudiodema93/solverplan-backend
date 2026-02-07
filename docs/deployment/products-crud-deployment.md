---
type: guide
title: Products Module CRUD Deployment Guide
created: 2026-02-07
tags:
  - deployment
  - migration
  - products
  - database
  - permissions
related:
  - "[[Database-Migrations]]"
  - "[[Permissions-Setup]]"
  - "[[Products-Module]]"
  - "[[Multi-Tenancy]]"
---

# Products Module CRUD Deployment Guide

This guide provides a comprehensive checklist for deploying the Products module CRUD functionality to production environments. Follow these steps carefully to ensure a smooth deployment with minimal risk.

## Overview

The Products module introduces three new entities (Categories, Products, Issues) with 15 endpoints, 11 permissions, and 9 database indexes. This deployment requires:

- Database schema changes (new tables, indexes, relationships)
- Permission system updates (new permissions for Categories, Products, Issues)
- Multi-tenant data isolation verification
- API endpoint registration

## Pre-Deployment Checklist

Complete these tasks **before** starting the deployment:

### 1. Environment Verification

- [ ] Confirm target environment (Staging, Production, etc.)
- [ ] Verify database connection strings are correctly configured
- [ ] Ensure sufficient database storage space (estimate: +50MB per 10k products)
- [ ] Confirm application downtime window (recommended: 10-15 minutes)
- [ ] Notify users of upcoming deployment and potential downtime

### 2. Backup Procedures

- [ ] Create full database backup:
  ```bash
  # SQL Server example
  BACKUP DATABASE [YourDatabaseName]
  TO DISK = 'C:\Backups\YourDatabase_PreProductsDeployment_20260207.bak'
  WITH FORMAT, COMPRESSION;
  ```

- [ ] Export current permission configuration:
  ```bash
  # Query to export permissions
  SELECT * FROM Permissions WHERE Resource LIKE '%Products%' OR Resource LIKE '%Categories%' OR Resource LIKE '%Issues%';
  ```

- [ ] Document current schema version:
  ```bash
  # Check migration history
  dotnet ef migrations list --project src/Modules/Products/Products.Infrastructure
  ```

### 3. Code Verification

- [ ] Confirm Products module branch is merged to target branch (e.g., `develop` or `main`)
- [ ] Verify build succeeds with zero warnings:
  ```bash
  dotnet build src/FSH.Framework.slnx
  # Expected: 0 errors, 0 warnings in Products module
  ```

- [ ] Verify all 220 Products module tests pass:
  ```bash
  dotnet test src/FSH.Framework.slnx --filter "FullyQualifiedName~Products"
  # Expected: 220 passed, 23 skipped (integration tests)
  ```

- [ ] Run architecture validation:
  ```bash
  dotnet test src/Tests/Architecture/Architecture.Tests.csproj
  ```

### 4. Deployment Package

- [ ] Build release package:
  ```bash
  dotnet publish src/Playground/FSH.Playground.AppHost \
    --configuration Release \
    --output ./publish
  ```

- [ ] Verify package includes Products module assemblies:
  - `Products.Domain.dll`
  - `Products.Application.dll`
  - `Products.Infrastructure.dll`

- [ ] Create deployment artifact with version tag (e.g., `v1.0.0-products-crud`)

## Migration Steps

Follow these steps in order during the deployment window:

### Step 1: Application Shutdown

```bash
# Stop the application (example for systemd)
sudo systemctl stop fsh-playground

# Verify application is stopped
sudo systemctl status fsh-playground
```

### Step 2: Database Migration

```bash
# Navigate to solution directory
cd /path/to/solverplan-backend

# Apply Products module migrations
dotnet ef database update \
  --project src/Modules/Products/Products.Infrastructure \
  --startup-project src/Playground/FSH.Playground.AppHost \
  --context ProductsDbContext \
  --verbose

# Verify migration success
# Expected output: "Done." or "Applying migration '[MigrationName]'."
```

**Migration includes:**
- Creation of `Products.Categories` table
- Creation of `Products.Products` table
- Creation of `Products.Issues` table
- Addition of 9 performance indexes
- Foreign key constraints (Product → Category, Issue → Product)
- Soft-delete support (IsDeleted column on all tables)
- Audit fields (CreatedBy, CreatedOnUtc, LastModifiedBy, LastModifiedOnUtc)
- Multi-tenancy support (TenantId column on all tables)

### Step 3: Verify Database Schema

```sql
-- Verify tables were created
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_SCHEMA = 'Products'
ORDER BY TABLE_NAME;
-- Expected: Categories, Issues, Products

-- Verify indexes were created
SELECT
    i.name AS IndexName,
    t.name AS TableName,
    COL_NAME(ic.object_id, ic.column_id) AS ColumnName
FROM sys.indexes i
INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
INNER JOIN sys.tables t ON i.object_id = t.object_id
WHERE t.name IN ('Categories', 'Products', 'Issues')
ORDER BY t.name, i.name;
-- Expected: 9 indexes across Products and Issues tables

-- Verify foreign key constraints
SELECT
    fk.name AS ForeignKeyName,
    tp.name AS ParentTable,
    cp.name AS ParentColumn,
    tr.name AS ReferencedTable,
    cr.name AS ReferencedColumn
FROM sys.foreign_keys fk
INNER JOIN sys.tables tp ON fk.parent_object_id = tp.object_id
INNER JOIN sys.tables tr ON fk.referenced_object_id = tr.object_id
INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
INNER JOIN sys.columns cp ON fkc.parent_column_id = cp.column_id AND fkc.parent_object_id = cp.object_id
INNER JOIN sys.columns cr ON fkc.referenced_column_id = cr.column_id AND fkc.referenced_object_id = cr.object_id
WHERE tp.name IN ('Products', 'Issues');
-- Expected: FK_Products_Categories_CategoryId, FK_Issues_Products_ProductId
```

### Step 4: Deploy Application

```bash
# Copy new application files
cp -r ./publish/* /var/www/fsh-playground/

# Verify file permissions
chown -R www-data:www-data /var/www/fsh-playground/

# Start the application
sudo systemctl start fsh-playground

# Verify application started successfully
sudo systemctl status fsh-playground

# Check application logs
sudo journalctl -u fsh-playground -n 50 --no-pager
# Look for: "Application started" or "Now listening on: https://..."
```

### Step 5: Warm-up Application

```bash
# Wait for application to fully initialize (30-60 seconds)
sleep 60

# Health check
curl -k https://localhost:5001/health
# Expected: HTTP 200 with "Healthy" status
```

## Permission Setup Instructions

### Default Permissions

The Products module introduces 11 new permissions across 3 resources:

**Categories Permissions:**
- `Permissions.Products.Categories.View` - View categories
- `Permissions.Products.Categories.Search` - Search categories with filters
- `Permissions.Products.Categories.Create` - Create new categories
- `Permissions.Products.Categories.Update` - Update existing categories
- `Permissions.Products.Categories.Delete` - Delete categories (blocked if products assigned)

**Products Permissions:**
- `Permissions.Products.Products.View` - View product details
- `Permissions.Products.Products.Search` - Search products with filters
- `Permissions.Products.Products.Create` - Create new products
- `Permissions.Products.Products.Update` - Update existing products
- `Permissions.Products.Products.Delete` - Delete products (soft-delete only)

**Issues Permissions:**
- `Permissions.Products.Issues.View` - View issue details
- `Permissions.Products.Issues.Search` - Search issues with filters
- `Permissions.Products.Issues.Create` - Create new issues
- `Permissions.Products.Issues.Update` - Update existing issues
- `Permissions.Products.Issues.Delete` - Delete issues (soft-delete only)

### Recommended Role Assignments

Assign permissions based on your organizational roles:

**Administrator Role** (Full Access):
```
All 11 permissions:
- Permissions.Products.Categories.* (all 5)
- Permissions.Products.Products.* (all 5)
- Permissions.Products.Issues.* (all 5)
```

**Product Manager Role** (Manage Products and Categories):
```
9 permissions:
- Permissions.Products.Categories.* (all 5)
- Permissions.Products.Products.* (all 5)
- Permissions.Products.Issues.View (read-only for issues)
```

**Support Agent Role** (Manage Issues Only):
```
5 permissions:
- Permissions.Products.Products.View
- Permissions.Products.Products.Search
- Permissions.Products.Issues.* (all 5)
```

**Read-Only Role** (View Only):
```
3 permissions:
- Permissions.Products.Categories.View
- Permissions.Products.Products.View
- Permissions.Products.Issues.View
```

### Permission Assignment via API

```bash
# Example: Assign all Products permissions to Admin role
curl -X POST https://your-api-url/api/v1/identity/roles/{roleId}/permissions \
  -H "Authorization: Bearer {your-token}" \
  -H "Content-Type: application/json" \
  -d '{
    "permissions": [
      "Permissions.Products.Categories.View",
      "Permissions.Products.Categories.Search",
      "Permissions.Products.Categories.Create",
      "Permissions.Products.Categories.Update",
      "Permissions.Products.Categories.Delete",
      "Permissions.Products.Products.View",
      "Permissions.Products.Products.Search",
      "Permissions.Products.Products.Create",
      "Permissions.Products.Products.Update",
      "Permissions.Products.Products.Delete",
      "Permissions.Products.Issues.View",
      "Permissions.Products.Issues.Search",
      "Permissions.Products.Issues.Create",
      "Permissions.Products.Issues.Update",
      "Permissions.Products.Issues.Delete"
    ]
  }'
```

### Permission Verification

```bash
# Verify permissions are registered
curl -X GET https://your-api-url/api/v1/identity/permissions \
  -H "Authorization: Bearer {your-token}" \
  | jq '.[] | select(.resource | contains("Products"))'

# Expected: 11 permission objects with Resources: "Products.Categories", "Products.Products", "Products.Issues"
```

## Post-Deployment Verification

### Smoke Tests

Execute these API calls to verify functionality:

#### 1. Categories Endpoint

```bash
# Create a test category
curl -X POST https://your-api-url/api/v1/products/categories \
  -H "Authorization: Bearer {admin-token}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test Category",
    "description": "Deployment verification test"
  }'
# Expected: HTTP 201 Created with Location header

# Search categories
curl -X GET "https://your-api-url/api/v1/products/categories?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer {admin-token}"
# Expected: HTTP 200 with PagedResponse containing the test category

# Get category by ID
curl -X GET https://your-api-url/api/v1/products/categories/{categoryId} \
  -H "Authorization: Bearer {admin-token}"
# Expected: HTTP 200 with CategoryResponse object
```

#### 2. Products Endpoint

```bash
# Create a test product
curl -X POST https://your-api-url/api/v1/products/products \
  -H "Authorization: Bearer {admin-token}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Test Product",
    "description": "Deployment verification test",
    "categoryId": "{categoryId-from-above}",
    "sku": "TEST-001",
    "price": 99.99,
    "stock": 100,
    "status": "Active"
  }'
# Expected: HTTP 201 Created with Location header

# Search products with filters
curl -X GET "https://your-api-url/api/v1/products/products?status=Active&minPrice=50&maxPrice=150" \
  -H "Authorization: Bearer {admin-token}"
# Expected: HTTP 200 with PagedResponse containing the test product

# Get product by ID (verify Category is included)
curl -X GET https://your-api-url/api/v1/products/products/{productId} \
  -H "Authorization: Bearer {admin-token}"
# Expected: HTTP 200 with ProductResponse including nested category object
```

#### 3. Issues Endpoint

```bash
# Create a test issue
curl -X POST https://your-api-url/api/v1/products/issues \
  -H "Authorization: Bearer {admin-token}" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Test Issue",
    "description": "Deployment verification test",
    "productId": "{productId-from-above}",
    "severity": "Medium",
    "status": "Open"
  }'
# Expected: HTTP 201 Created with Location header

# Search issues with filters
curl -X GET "https://your-api-url/api/v1/products/issues?severity=Medium&status=Open" \
  -H "Authorization: Bearer {admin-token}"
# Expected: HTTP 200 with PagedResponse containing the test issue

# Update issue status
curl -X PUT https://your-api-url/api/v1/products/issues/{issueId} \
  -H "Authorization: Bearer {admin-token}" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Test Issue",
    "description": "Deployment verification test - updated",
    "productId": "{productId}",
    "severity": "Medium",
    "status": "Resolved"
  }'
# Expected: HTTP 200 with success message
```

### Multi-Tenancy Verification

```bash
# Verify tenant isolation - create data for Tenant A
curl -X POST https://your-api-url/api/v1/products/categories \
  -H "Authorization: Bearer {tenant-a-token}" \
  -H "X-Tenant: tenant-a-id" \
  -H "Content-Type: application/json" \
  -d '{"name": "Tenant A Category"}'

# Verify Tenant B cannot see Tenant A's data
curl -X GET "https://your-api-url/api/v1/products/categories" \
  -H "Authorization: Bearer {tenant-b-token}" \
  -H "X-Tenant: tenant-b-id"
# Expected: HTTP 200 with empty results (no Tenant A data visible)
```

### Permission Verification

```bash
# Verify user without permission gets 403 Forbidden
curl -X POST https://your-api-url/api/v1/products/categories \
  -H "Authorization: Bearer {read-only-user-token}" \
  -H "Content-Type: application/json" \
  -d '{"name": "Unauthorized Test"}'
# Expected: HTTP 403 Forbidden

# Verify user with permission succeeds
curl -X POST https://your-api-url/api/v1/products/categories \
  -H "Authorization: Bearer {admin-token}" \
  -H "Content-Type: application/json" \
  -d '{"name": "Authorized Test"}'
# Expected: HTTP 201 Created
```

### Performance Verification

```bash
# Verify pagination performance with large dataset
curl -X GET "https://your-api-url/api/v1/products/products?pageNumber=1&pageSize=100" \
  -H "Authorization: Bearer {admin-token}" \
  -w "\nResponse Time: %{time_total}s\n"
# Expected: Response time < 1 second (indexes should optimize queries)

# Verify search performance
curl -X GET "https://your-api-url/api/v1/products/products?search=test&status=Active" \
  -H "Authorization: Bearer {admin-token}" \
  -w "\nResponse Time: %{time_total}s\n"
# Expected: Response time < 2 seconds
```

### Database Integrity Checks

```sql
-- Verify audit fields are populated
SELECT TOP 5
    Id,
    Name,
    CreatedBy,
    CreatedOnUtc,
    LastModifiedBy,
    LastModifiedOnUtc,
    TenantId
FROM Products.Products
ORDER BY CreatedOnUtc DESC;
-- Expected: All audit fields should have values

-- Verify soft-delete works
SELECT COUNT(*) FROM Products.Products WHERE IsDeleted = 1;
-- Expected: 0 (unless you've deleted test data)

-- Verify foreign key integrity
SELECT p.Id, p.Name, c.Name AS CategoryName
FROM Products.Products p
LEFT JOIN Products.Categories c ON p.CategoryId = c.Id
WHERE c.Id IS NULL AND p.CategoryId IS NOT NULL;
-- Expected: 0 rows (no orphaned products)
```

### Cleanup Test Data

```bash
# Delete test issue
curl -X DELETE https://your-api-url/api/v1/products/issues/{issueId} \
  -H "Authorization: Bearer {admin-token}"

# Delete test product
curl -X DELETE https://your-api-url/api/v1/products/products/{productId} \
  -H "Authorization: Bearer {admin-token}"

# Delete test category
curl -X DELETE https://your-api-url/api/v1/products/categories/{categoryId} \
  -H "Authorization: Bearer {admin-token}"
```

## Rollback Procedure

If critical issues are discovered post-deployment, follow this rollback process:

### Step 1: Assess the Issue

- [ ] Document the error/issue encountered
- [ ] Determine if issue is code-related or data-related
- [ ] Check application logs for error details
- [ ] Verify database connectivity and state

### Step 2: Immediate Mitigation

**Option A: Rollback Application Only** (if database is stable)

```bash
# Stop current application
sudo systemctl stop fsh-playground

# Restore previous application version
cp -r /var/www/fsh-playground-backup/* /var/www/fsh-playground/

# Start application
sudo systemctl start fsh-playground

# Verify rollback
curl -k https://localhost:5001/health
```

**Option B: Full Rollback** (if database migration failed or corrupted)

```bash
# Stop application
sudo systemctl stop fsh-playground

# Rollback database migration
dotnet ef database update {PreviousMigrationName} \
  --project src/Modules/Products/Products.Infrastructure \
  --startup-project src/Playground/FSH.Playground.AppHost \
  --context ProductsDbContext

# Alternative: Restore database from backup
# SQL Server example:
RESTORE DATABASE [YourDatabaseName]
FROM DISK = 'C:\Backups\YourDatabase_PreProductsDeployment_20260207.bak'
WITH REPLACE, RECOVERY;

# Restore previous application version
cp -r /var/www/fsh-playground-backup/* /var/www/fsh-playground/

# Start application
sudo systemctl start fsh-playground
```

### Step 3: Verify Rollback Success

```bash
# Check application is running
sudo systemctl status fsh-playground

# Verify health endpoint
curl -k https://localhost:5001/health

# Verify database state
dotnet ef migrations list --project src/Modules/Products/Products.Infrastructure
```

### Step 4: Post-Rollback Actions

- [ ] Notify stakeholders of rollback
- [ ] Document root cause of failure
- [ ] Create incident report with timeline
- [ ] Schedule deployment retry after fixes
- [ ] Update deployment checklist with lessons learned

## Troubleshooting

### Common Issues

#### Issue: Migration fails with "Table already exists"

**Symptom:**
```
Microsoft.Data.SqlClient.SqlException: There is already an object named 'Products' in the database.
```

**Resolution:**
```bash
# Check if tables were partially created
SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'Products';

# If tables exist, check migration history
SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory WHERE MigrationId LIKE '%Products%';

# Manually mark migration as applied (if tables are correct)
INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion)
VALUES ('{MigrationId}', '9.0.0');

# Or drop tables and rerun migration
DROP TABLE IF EXISTS Products.Issues;
DROP TABLE IF EXISTS Products.Products;
DROP TABLE IF EXISTS Products.Categories;
```

#### Issue: Permission endpoints return 403 Forbidden

**Symptom:**
All Products API calls return HTTP 403, even for admin users.

**Resolution:**
```bash
# Verify permissions are seeded in database
SELECT * FROM Permissions WHERE Resource LIKE '%Products%';

# If missing, reseed permissions
dotnet run --project src/Playground/FSH.Playground.AppHost -- --seed

# Verify role-permission assignments
SELECT r.Name AS RoleName, p.Action, p.Resource
FROM RolePermissions rp
INNER JOIN Roles r ON rp.RoleId = r.Id
INNER JOIN Permissions p ON rp.PermissionId = p.Id
WHERE p.Resource LIKE '%Products%';
```

#### Issue: Queries are slow (> 5 seconds)

**Symptom:**
Search endpoints timeout or take excessive time.

**Resolution:**
```sql
-- Verify indexes were created
EXEC sp_helpindex 'Products.Products';
EXEC sp_helpindex 'Products.Issues';

-- Check query execution plan
SET SHOWPLAN_ALL ON;
SELECT * FROM Products.Products WHERE TenantId = 'test-tenant' AND Status = 'Active';
SET SHOWPLAN_ALL OFF;

-- Rebuild indexes if fragmented
ALTER INDEX ALL ON Products.Products REBUILD;
ALTER INDEX ALL ON Products.Issues REBUILD;

-- Update statistics
UPDATE STATISTICS Products.Products;
UPDATE STATISTICS Products.Issues;
```

#### Issue: Multi-tenancy isolation broken

**Symptom:**
Tenant A can see Tenant B's data.

**Resolution:**
```bash
# Verify tenant context middleware is registered
grep -r "UseMultiTenancy" src/Playground/

# Check if TenantId is populated
SELECT TOP 10 Id, Name, TenantId FROM Products.Products;

# Verify query filters in ProductsDbContext
grep -A 10 "HasQueryFilter" src/Modules/Products/Products.Infrastructure/Persistence/ProductsDbContext.cs
```

## Success Criteria

Deployment is considered successful when:

- ✅ All migration scripts executed without errors
- ✅ Database schema matches expected state (3 tables, 9 indexes, 2 foreign keys)
- ✅ Application starts without errors
- ✅ Health check endpoint returns HTTP 200
- ✅ All 15 API endpoints respond correctly
- ✅ Permission system enforces access control (403 for unauthorized users)
- ✅ Multi-tenancy isolation verified (tenants cannot see each other's data)
- ✅ Search queries return results in < 2 seconds
- ✅ Audit fields (CreatedBy, CreatedOnUtc, etc.) are populated
- ✅ Soft-delete works correctly (deleted items are not visible in searches)
- ✅ Test data cleanup completes successfully
- ✅ No errors in application logs (check for 30 minutes post-deployment)

## Post-Deployment Tasks

After successful deployment:

- [ ] Update deployment documentation with any lessons learned
- [ ] Monitor application logs for 24 hours
- [ ] Monitor database performance metrics (query times, index usage)
- [ ] Collect user feedback on new functionality
- [ ] Schedule follow-up review meeting with team
- [ ] Update runbook with any new procedures discovered
- [ ] Archive deployment artifacts (backups, logs, scripts)

## Additional Resources

- [[Products-Module]] - Module architecture overview
- [[Database-Migrations]] - EF Core migration guide
- [[Permissions-Setup]] - Permission system documentation
- [[Multi-Tenancy]] - Multi-tenancy architecture
- [[FSH-Patterns]] - Framework patterns and conventions

## Revision History

| Date | Version | Changes | Author |
|------|---------|---------|--------|
| 2026-02-07 | 1.0 | Initial deployment guide for Products CRUD | Architetto |

## Support

For deployment issues or questions:
- Check application logs: `sudo journalctl -u fsh-playground -f`
- Review database logs for errors
- Contact DevOps team for infrastructure issues
- Reference [[Troubleshooting-Guide]] for common problems
