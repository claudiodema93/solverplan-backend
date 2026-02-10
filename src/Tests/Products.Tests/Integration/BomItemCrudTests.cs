using System.Net;
using System.Net.Http.Json;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.CreateBomItem;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateBomItem;
using FSH.Modules.Products.Contracts.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Xunit;

namespace Products.Tests.Integration;

/// <summary>
/// Integration tests for BomItem CRUD workflow.
/// Tests the complete flow from API endpoint through mediator to database.
///
/// NOTE: These tests are currently skipped because they require:
/// - Configured authentication/authorization infrastructure
/// - Test database setup with proper schema
/// - Tenant context configuration
/// - Parent and child product records pre-seeded in the database
///
/// Once the test infrastructure is mature, remove the Skip attributes to enable these tests.
/// See TenantLifecycleTests.cs in Multitenancy.Tests for a similar pattern.
/// </summary>
[Trait("Category", "Products")]
[Trait("Type", "Integration")]
[Collection("Sequential")] // Run sequentially to avoid database conflicts
public class BomItemCrudTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public BomItemCrudTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task CreateBomItem_ValidRequest_ReturnsCreatedBomItem()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        var command = new CreateBomItemCommand(
            ProductId: 1,
            ChildProductId: 2,
            Quantity: 5.0,
            Unit: UnitOfMeasurement.Pcs,
            IsManual: false,
            Notes: "Required for assembly");

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/bomitems", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var id = await response.Content.ReadFromJsonAsync<int>();
        id.ShouldBeGreaterThan(0);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task GetBomItemById_ExistingBomItem_ReturnsBomItem()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create a BOM item first
        var createCommand = new CreateBomItemCommand(
            ProductId: 1,
            ChildProductId: 2,
            Quantity: 1.0,
            Unit: UnitOfMeasurement.Pcs);

        var createResponse = await client.PostAsJsonAsync("/api/v1/bomitems", createCommand);
        var bomItemId = await createResponse.Content.ReadFromJsonAsync<int>();

        // Act
        var getResponse = await client.GetAsync($"/api/v1/bomitems/{bomItemId}");

        // Assert
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var bomItem = await getResponse.Content.ReadFromJsonAsync<object>();
        bomItem.ShouldNotBeNull();
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task GetBomItemById_NonExistentBomItem_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        var nonExistentId = 999999;

        // Act
        var response = await client.GetAsync($"/api/v1/bomitems/{nonExistentId}");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task UpdateBomItem_ExistingBomItem_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create a BOM item first
        var createCommand = new CreateBomItemCommand(
            ProductId: 1,
            ChildProductId: 2,
            Quantity: 1.0,
            Unit: UnitOfMeasurement.Pcs);

        var createResponse = await client.PostAsJsonAsync("/api/v1/bomitems", createCommand);
        var bomItemId = await createResponse.Content.ReadFromJsonAsync<int>();

        // Act - Update the BOM item
        var updateCommand = new UpdateBomItemCommand(
            Id: bomItemId,
            ChildProductId: 2,
            Quantity: 10.0,
            Unit: UnitOfMeasurement.Millimeters,
            IsManual: true,
            Notes: "Updated assembly requirement");

        var updateResponse = await client.PutAsJsonAsync($"/api/v1/bomitems/{bomItemId}", updateCommand);

        // Assert
        updateResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task DeleteBomItem_ExistingBomItem_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create a BOM item first
        var createCommand = new CreateBomItemCommand(
            ProductId: 1,
            ChildProductId: 2,
            Quantity: 1.0,
            Unit: UnitOfMeasurement.Pcs);

        var createResponse = await client.PostAsJsonAsync("/api/v1/bomitems", createCommand);
        var bomItemId = await createResponse.Content.ReadFromJsonAsync<int>();

        // Act - Delete the BOM item
        var deleteResponse = await client.DeleteAsync($"/api/v1/bomitems/{bomItemId}");

        // Assert
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Verify the BOM item is deleted
        var getResponse = await client.GetAsync($"/api/v1/bomitems/{bomItemId}");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task SearchBomItems_WithPagination_ReturnsPaginatedResults()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create test BOM items
        for (int i = 1; i <= 3; i++)
        {
            await client.PostAsJsonAsync("/api/v1/bomitems", new CreateBomItemCommand(
                ProductId: 1,
                ChildProductId: i + 1,
                Quantity: i,
                Unit: UnitOfMeasurement.Pcs));
        }

        // Act - Search with pagination
        var searchResponse = await client.GetAsync("/api/v1/bomitems?PageNumber=1&PageSize=10");

        // Assert
        searchResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var searchResult = await searchResponse.Content.ReadAsStringAsync();
        searchResult.ShouldNotBeNullOrEmpty();
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task SearchBomItems_FilterByProductId_ReturnsFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");
        var targetProductId = 1;

        // Act - Search filtered by ProductId
        var searchResponse = await client.GetAsync($"/api/v1/bomitems?ProductId={targetProductId}&PageNumber=1&PageSize=10");

        // Assert
        searchResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task CreateBomItem_WithSameProductIdAndChildProductId_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Invalid: parent and child product IDs are the same
        var invalidCommand = new CreateBomItemCommand(
            ProductId: 1,
            ChildProductId: 1, // Same as ProductId - invalid
            Quantity: 1.0,
            Unit: UnitOfMeasurement.Pcs);

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/bomitems", invalidCommand);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task CreateBomItem_NonExistentParentProduct_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        var command = new CreateBomItemCommand(
            ProductId: 999999, // Non-existent product
            ChildProductId: 2,
            Quantity: 1.0,
            Unit: UnitOfMeasurement.Pcs);

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/bomitems", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
