using System.Net;
using System.Net.Http.Json;
using FSH.Modules.Products.Contracts.Features.v1.Commands.UpdateProduct;
using FSH.Modules.Products.Contracts.Domain.Enums;
using FSH.Modules.Products.Contracts.Features.v1.Commands.CreateProduct;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Xunit;

namespace Products.Tests.Integration;

/// <summary>
/// Integration tests for Product CRUD workflow.
/// Tests the complete flow from API endpoint through mediator to database.
///
/// NOTE: These tests are currently skipped because they require:
/// - Configured authentication/authorization infrastructure
/// - Test database setup with proper schema
/// - Tenant context configuration
/// - Properly seeded test data
///
/// Once the test infrastructure is mature, remove the Skip attributes to enable these tests.
/// See TenantLifecycleTests.cs in Multitenancy.Tests for a similar pattern.
/// </summary>
[Trait("Category", "Products")]
[Trait("Type", "Integration")]
[Collection("Sequential")] // Run sequentially to avoid database conflicts
public class ProductCrudTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public ProductCrudTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task CreateProduct_ValidRequest_ReturnsCreatedProduct()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        var command = new CreateProductCommand(
            Title: "Test Product",
            Revision: 1,
            Description: "Test Description",
            Status: ProductState.Draft,
            Version: 1);

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/products", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CreateProductResponse>();
        result.ShouldNotBeNull();
        result.Id.ShouldBeGreaterThan(0);
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task GetProductById_ExistingProduct_ReturnsProduct()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create a product first
        var createCommand = new CreateProductCommand(
            Title: "Get Test Product",
            Revision: 1,
            Description: "For retrieval testing",
            Status: ProductState.Draft);

        var createResponse = await client.PostAsJsonAsync("/api/v1/products", createCommand);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateProductResponse>();
        var productId = createResult!.Id;

        // Act
        var getResponse = await client.GetAsync($"/api/v1/products/{productId}");

        // Assert
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var product = await getResponse.Content.ReadFromJsonAsync<object>();
        product.ShouldNotBeNull();
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task UpdateProduct_ExistingProduct_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create a product first
        var createCommand = new CreateProductCommand(
            Title: "Update Test Product",
            Revision: 1,
            Description: "Original description",
            Status: ProductState.Draft);

        var createResponse = await client.PostAsJsonAsync("/api/v1/products", createCommand);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateProductResponse>();
        var productId = createResult!.Id;

        // Act - Update the product
        var updateCommand = new UpdateProductCommand(
            Id: productId,
            Title: "Updated Product Title",
            Revision: 2,
            Description: "Updated description",
            Status: ProductState.Active);

        var updateResponse = await client.PutAsJsonAsync($"/api/v1/products/{productId}", updateCommand);

        // Assert
        updateResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task SearchProducts_WithPagination_ReturnsPaginatedResults()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create test products
        for (int i = 1; i <= 3; i++)
        {
            await client.PostAsJsonAsync("/api/v1/products", new CreateProductCommand(
                Title: $"Search Test Product {i}",
                Revision: 1,
                Status: ProductState.Active));
        }

        // Act - Search with pagination
        var searchResponse = await client.GetAsync("/api/v1/products?PageNumber=1&PageSize=10");

        // Assert
        searchResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var searchResult = await searchResponse.Content.ReadAsStringAsync();
        searchResult.ShouldNotBeNullOrEmpty();
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task DeleteProduct_ExistingProduct_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create a product first
        var createCommand = new CreateProductCommand(
            Title: "Delete Test Product",
            Revision: 1,
            Description: "To be deleted",
            Status: ProductState.Draft);

        var createResponse = await client.PostAsJsonAsync("/api/v1/products", createCommand);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateProductResponse>();
        var productId = createResult!.Id;

        // Act - Delete the product
        var deleteResponse = await client.DeleteAsync($"/api/v1/products/{productId}");

        // Assert
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Verify the product is deleted (or soft-deleted)
        var getResponse = await client.GetAsync($"/api/v1/products/{productId}");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task CreateProduct_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Invalid command - empty title
        var invalidCommand = new CreateProductCommand(
            Title: "", // Invalid: empty title
            Revision: 1,
            Description: "Test",
            Status: ProductState.Draft);

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/products", invalidCommand);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
