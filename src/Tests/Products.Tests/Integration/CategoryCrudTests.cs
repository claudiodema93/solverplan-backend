using System.Net;
using System.Net.Http.Json;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.CreateCategory;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.CreateProduct;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.DeleteCategory;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateCategory;
using FSH.Modules.Products.Contracts.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Xunit;

namespace Products.Tests.Integration;

/// <summary>
/// Integration tests for Category CRUD workflow and relationship management.
/// Tests the complete flow from API endpoint through mediator to database,
/// including referential integrity with Products.
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
public class CategoryCrudTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CategoryCrudTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task CreateCategory_ValidRequest_ReturnsCreatedCategory()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        var command = new CreateCategoryCommand(Name: "Electronics");

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/categories", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CreateCategoryResponse>();
        result.ShouldNotBeNull();
        result.Id.ShouldBeGreaterThan(0);
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task GetCategoryById_ExistingCategory_ReturnsCategory()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create a category first
        var createCommand = new CreateCategoryCommand(Name: "Furniture");
        var createResponse = await client.PostAsJsonAsync("/api/v1/categories", createCommand);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateCategoryResponse>();
        var categoryId = createResult!.Id;

        // Act
        var getResponse = await client.GetAsync($"/api/v1/categories/{categoryId}");

        // Assert
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var category = await getResponse.Content.ReadFromJsonAsync<object>();
        category.ShouldNotBeNull();
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task UpdateCategory_ExistingCategory_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create a category first
        var createCommand = new CreateCategoryCommand(Name: "Original Category");
        var createResponse = await client.PostAsJsonAsync("/api/v1/categories", createCommand);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateCategoryResponse>();
        var categoryId = createResult!.Id;

        // Act - Update the category
        var updateCommand = new UpdateCategoryCommand(Id: categoryId, Name: "Updated Category");
        var updateResponse = await client.PutAsJsonAsync($"/api/v1/categories/{categoryId}", updateCommand);

        // Assert
        updateResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task SearchCategories_WithPagination_ReturnsPaginatedResults()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create test categories
        for (int i = 1; i <= 3; i++)
        {
            await client.PostAsJsonAsync("/api/v1/categories", new CreateCategoryCommand(
                Name: $"Search Test Category {i}"));
        }

        // Act - Search with pagination
        var searchResponse = await client.GetAsync("/api/v1/categories?PageNumber=1&PageSize=10");

        // Assert
        searchResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var searchResult = await searchResponse.Content.ReadAsStringAsync();
        searchResult.ShouldNotBeNullOrEmpty();
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task DeleteCategory_WithoutProducts_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create a category without any products
        var createCommand = new CreateCategoryCommand(Name: "Delete Test Category");
        var createResponse = await client.PostAsJsonAsync("/api/v1/categories", createCommand);
        var createResult = await createResponse.Content.ReadFromJsonAsync<CreateCategoryResponse>();
        var categoryId = createResult!.Id;

        // Act - Delete the category
        var deleteResponse = await client.DeleteAsync($"/api/v1/categories/{categoryId}");

        // Assert
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Verify the category is deleted
        var getResponse = await client.GetAsync($"/api/v1/categories/{categoryId}");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task DeleteCategory_WithProducts_PreventsDelete()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create a category
        var createCategoryCommand = new CreateCategoryCommand(Name: "Category With Products");
        var createCategoryResponse = await client.PostAsJsonAsync("/api/v1/categories", createCategoryCommand);
        var categoryResult = await createCategoryResponse.Content.ReadFromJsonAsync<CreateCategoryResponse>();
        var categoryId = categoryResult!.Id;

        // Create a product in this category
        var createProductCommand = new CreateProductCommand(
            Title: "Test Product",
            Revision: 1,
            Description: "Product in category",
            Status: ProductState.Draft,
            CategoryId: categoryId);
        await client.PostAsJsonAsync("/api/v1/products", createProductCommand);

        // Act - Try to delete the category
        var deleteResponse = await client.DeleteAsync($"/api/v1/categories/{categoryId}");

        // Assert - Should fail due to referential integrity
        deleteResponse.StatusCode.ShouldBeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.Conflict);

        // Verify the category still exists
        var getResponse = await client.GetAsync($"/api/v1/categories/{categoryId}");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task CreateCategory_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Invalid command - empty name
        var invalidCommand = new CreateCategoryCommand(Name: "");

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/categories", invalidCommand);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task CreateCategory_NameTooLong_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Invalid command - name exceeds max length (128)
        var invalidCommand = new CreateCategoryCommand(Name: new string('A', 129));

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/categories", invalidCommand);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
