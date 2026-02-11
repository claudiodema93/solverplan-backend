using System.Net;
using System.Net.Http.Json;
using FSH.Modules.Products.Contracts.Features.v1.Commands.UpdateQualityCheck;
using FSH.Modules.Products.Contracts.Features.v1.Commands.CreateQualityCheck;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Xunit;

namespace Products.Tests.Integration;

/// <summary>
/// Integration tests for QualityCheck CRUD workflow.
/// Tests the complete flow from API endpoint through mediator to database.
///
/// NOTE: These tests are currently skipped because they require:
/// - Configured authentication/authorization infrastructure
/// - Test database setup with proper schema
/// - Tenant context configuration
/// - Parent product records pre-seeded in the database
///
/// Once the test infrastructure is mature, remove the Skip attributes to enable these tests.
/// See BomItemCrudTests.cs in Products.Tests for a similar pattern.
/// </summary>
[Trait("Category", "Products")]
[Trait("Type", "Integration")]
[Collection("Sequential")] // Run sequentially to avoid database conflicts
public class QualityCheckCrudTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public QualityCheckCrudTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task CreateQualityCheck_ValidRequest_ReturnsCreatedQualityCheck()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        var command = new CreateQualityCheckCommand(
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Inspect product surface for visible defects before packaging",
            Department: "Quality Assurance",
            IsRequired: true,
            DisplayOrder: 1);

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/products/qualitychecks", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var result = await response.Content.ReadFromJsonAsync<CreateQualityCheckResponse>();
        result.ShouldNotBeNull();
        result!.Id.ShouldBeGreaterThan(0);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task CreateQualityCheck_NonExistentProduct_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        var command = new CreateQualityCheckCommand(
            ProductId: 999999, // Non-existent product
            Title: "Visual Inspection",
            Description: "Inspect product surface for visible defects",
            Department: "QA");

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/products/qualitychecks", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task CreateQualityCheck_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        var invalidCommand = new CreateQualityCheckCommand(
            ProductId: 0, // Invalid: must be > 0
            Title: "", // Invalid: required
            Description: "", // Invalid: required
            Department: ""); // Invalid: required

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/products/qualitychecks", invalidCommand);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task GetQualityCheckById_ExistingQualityCheck_ReturnsQualityCheck()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create a quality check first
        var createCommand = new CreateQualityCheckCommand(
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Inspect product surface for visible defects",
            Department: "QA",
            IsRequired: false,
            DisplayOrder: 0);

        var createResponse = await client.PostAsJsonAsync("/api/v1/products/qualitychecks", createCommand);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateQualityCheckResponse>();

        // Act
        var getResponse = await client.GetAsync($"/api/v1/products/qualitychecks/{createResult!.Id}");

        // Assert
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var qualityCheck = await getResponse.Content.ReadFromJsonAsync<object>();
        qualityCheck.ShouldNotBeNull();
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task GetQualityCheckById_NonExistentQualityCheck_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        var nonExistentId = 999999;

        // Act
        var response = await client.GetAsync($"/api/v1/products/qualitychecks/{nonExistentId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task UpdateQualityCheck_ExistingQualityCheck_ReturnsNoContent()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create a quality check first
        var createCommand = new CreateQualityCheckCommand(
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Inspect product surface for visible defects",
            Department: "QA");

        var createResponse = await client.PostAsJsonAsync("/api/v1/products/qualitychecks", createCommand);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateQualityCheckResponse>();

        // Act - Update the quality check
        var updateCommand = new UpdateQualityCheckCommand(
            Id: createResult!.Id,
            ProductId: 1,
            Title: "Updated Visual Inspection",
            Description: "Updated: Inspect product surface, edges, and corners for visible defects",
            Department: "Quality Assurance",
            IsRequired: true,
            DisplayOrder: 2);

        var updateResponse = await client.PutAsJsonAsync(
            $"/api/v1/products/qualitychecks/{createResult.Id}", updateCommand);

        // Assert
        updateResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task UpdateQualityCheck_MismatchedRouteAndBodyId_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        var routeId = 1;
        var commandWithDifferentId = new UpdateQualityCheckCommand(
            Id: 999, // Mismatched ID
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Inspect product surface",
            Department: "QA");

        // Act
        var response = await client.PutAsJsonAsync(
            $"/api/v1/products/qualitychecks/{routeId}", commandWithDifferentId);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task UpdateQualityCheck_NonExistentQualityCheck_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        var nonExistentId = 999999;
        var updateCommand = new UpdateQualityCheckCommand(
            Id: nonExistentId,
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Inspect product surface",
            Department: "QA");

        // Act
        var response = await client.PutAsJsonAsync(
            $"/api/v1/products/qualitychecks/{nonExistentId}", updateCommand);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task DeleteQualityCheck_ExistingQualityCheck_ReturnsNoContent()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create a quality check first
        var createCommand = new CreateQualityCheckCommand(
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Inspect product surface for visible defects",
            Department: "QA");

        var createResponse = await client.PostAsJsonAsync("/api/v1/products/qualitychecks", createCommand);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateQualityCheckResponse>();

        // Act - Delete the quality check
        var deleteResponse = await client.DeleteAsync(
            $"/api/v1/products/qualitychecks/{createResult!.Id}");

        // Assert
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // Verify the quality check is deleted
        var getResponse = await client.GetAsync($"/api/v1/products/qualitychecks/{createResult.Id}");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task DeleteQualityCheck_NonExistentQualityCheck_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        var nonExistentId = 999999;

        // Act
        var response = await client.DeleteAsync($"/api/v1/products/qualitychecks/{nonExistentId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task SearchQualityChecks_WithPagination_ReturnsPaginatedResults()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create test quality checks
        for (int i = 1; i <= 3; i++)
        {
            await client.PostAsJsonAsync("/api/v1/products/qualitychecks", new CreateQualityCheckCommand(
                ProductId: 1,
                Title: $"Quality Check {i}",
                Description: $"Description for quality check {i}",
                Department: "QA",
                DisplayOrder: i));
        }

        // Act - Search with pagination
        var searchResponse = await client.GetAsync(
            "/api/v1/products/qualitychecks?PageNumber=1&PageSize=10");

        // Assert
        searchResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var searchResult = await searchResponse.Content.ReadAsStringAsync();
        searchResult.ShouldNotBeNullOrEmpty();
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task SearchQualityChecks_FilterByProductId_ReturnsFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");
        var targetProductId = 1;

        // Act - Search filtered by ProductId
        var searchResponse = await client.GetAsync(
            $"/api/v1/products/qualitychecks?ProductId={targetProductId}&PageNumber=1&PageSize=10");

        // Assert
        searchResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task SearchQualityChecks_FilterByDepartment_ReturnsFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Act - Search filtered by Department
        var searchResponse = await client.GetAsync(
            "/api/v1/products/qualitychecks?Department=Quality+Assurance&PageNumber=1&PageSize=10");

        // Assert
        searchResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task SearchQualityChecks_FilterByIsRequired_ReturnsFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Act - Search filtered by IsRequired
        var searchResponse = await client.GetAsync(
            "/api/v1/products/qualitychecks?IsRequired=true&PageNumber=1&PageSize=10");

        // Assert
        searchResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task SearchQualityChecks_WithSearchTerm_ReturnsMatchingResults()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Act - Search with free-text search term
        var searchResponse = await client.GetAsync(
            "/api/v1/products/qualitychecks?Search=visual&PageNumber=1&PageSize=10");

        // Assert
        searchResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task SearchQualityChecks_InvalidPageSize_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Act - Search with invalid page size (> 100)
        var searchResponse = await client.GetAsync(
            "/api/v1/products/qualitychecks?PageNumber=1&PageSize=101");

        // Assert
        searchResponse.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
