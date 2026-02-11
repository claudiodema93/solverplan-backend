using System.Net;
using System.Net.Http.Json;
using FSH.Modules.Products.Contracts.Features.v1.Commands.CreateAccountingCode;
using FSH.Modules.Products.Contracts.Features.v1.Commands.UpdateAccountingCode;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Xunit;

namespace Products.Tests.Integration;

/// <summary>
/// Integration tests for AccountingCode CRUD workflow.
/// Tests the complete flow from API endpoint through mediator to database.
///
/// NOTE: These tests are currently skipped because they require:
/// - Configured authentication/authorization infrastructure
/// - Test database setup with proper schema
/// - Tenant context configuration
/// - Parent product records pre-seeded in the database
///
/// Once the test infrastructure is mature, remove the Skip attributes to enable these tests.
/// See BomItemCrudTests.cs for a similar pattern.
/// </summary>
[Trait("Category", "Products")]
[Trait("Type", "Integration")]
[Collection("Sequential")]
public class AccountingCodeCrudTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AccountingCodeCrudTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task CreateAccountingCode_ValidRequest_ReturnsCreatedAccountingCode()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        var command = new CreateAccountingCodeCommand(
            ProductId: 1,
            Code: "MAT-100",
            Description: "Raw material accounting code");

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/products/accountingcodes", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var id = await response.Content.ReadFromJsonAsync<int>();
        id.ShouldBeGreaterThan(0);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task GetAccountingCodeById_ExistingCode_ReturnsAccountingCode()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        var createCommand = new CreateAccountingCodeCommand(ProductId: 1, Code: "MAT-100");
        var createResponse = await client.PostAsJsonAsync("/api/v1/products/accountingcodes", createCommand);
        var accountingCodeId = await createResponse.Content.ReadFromJsonAsync<int>();

        // Act
        var getResponse = await client.GetAsync($"/api/v1/products/accountingcodes/{accountingCodeId}");

        // Assert
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var accountingCode = await getResponse.Content.ReadFromJsonAsync<object>();
        accountingCode.ShouldNotBeNull();
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task GetAccountingCodeById_NonExistentCode_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Act
        var response = await client.GetAsync("/api/v1/products/accountingcodes/999999");

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task UpdateAccountingCode_ExistingCode_ReturnsNoContent()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        var createCommand = new CreateAccountingCodeCommand(ProductId: 1, Code: "MAT-100");
        var createResponse = await client.PostAsJsonAsync("/api/v1/products/accountingcodes", createCommand);
        var accountingCodeId = await createResponse.Content.ReadFromJsonAsync<int>();

        // Act
        var updateRequest = new UpdateAccountingCodeCommand(
            Id: accountingCodeId,
            Code: "MAT-200",
            Description: "Updated material code");

        var updateResponse = await client.PutAsJsonAsync($"/api/v1/products/accountingcodes/{accountingCodeId}", updateRequest);

        // Assert
        updateResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task DeleteAccountingCode_ExistingCode_ReturnsNoContent()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        var createCommand = new CreateAccountingCodeCommand(ProductId: 1, Code: "MAT-100");
        var createResponse = await client.PostAsJsonAsync("/api/v1/products/accountingcodes", createCommand);
        var accountingCodeId = await createResponse.Content.ReadFromJsonAsync<int>();

        // Act
        var deleteResponse = await client.DeleteAsync($"/api/v1/products/accountingcodes/{accountingCodeId}");

        // Assert
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        // Verify the code is deleted
        var getResponse = await client.GetAsync($"/api/v1/products/accountingcodes/{accountingCodeId}");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task SearchAccountingCodes_WithPagination_ReturnsPaginatedResults()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        for (int i = 1; i <= 3; i++)
        {
            await client.PostAsJsonAsync("/api/v1/products/accountingcodes", new CreateAccountingCodeCommand(
                ProductId: 1,
                Code: $"MAT-{i:D3}"));
        }

        // Act
        var searchResponse = await client.GetAsync("/api/v1/products/accountingcodes?PageNumber=1&PageSize=10");

        // Assert
        searchResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var searchResult = await searchResponse.Content.ReadAsStringAsync();
        searchResult.ShouldNotBeNullOrEmpty();
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task SearchAccountingCodes_FilterByProductId_ReturnsFilteredResults()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Act
        var searchResponse = await client.GetAsync("/api/v1/products/accountingcodes?ProductId=1&PageNumber=1&PageSize=10");

        // Assert
        searchResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task CreateAccountingCode_DuplicateCodeForSameProduct_ReturnsConflict()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        var command = new CreateAccountingCodeCommand(ProductId: 1, Code: "MAT-100");
        await client.PostAsJsonAsync("/api/v1/products/accountingcodes", command);

        // Act - Try to create a duplicate
        var duplicateResponse = await client.PostAsJsonAsync("/api/v1/products/accountingcodes", command);

        // Assert
        duplicateResponse.StatusCode.ShouldBe(HttpStatusCode.Conflict);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task CreateAccountingCode_NonExistentProduct_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        var command = new CreateAccountingCodeCommand(ProductId: 999999, Code: "MAT-100");

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/products/accountingcodes", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Requires fully configured test database, authentication infrastructure, and pre-seeded product records")]
    public async Task SearchAccountingCodes_SearchByCode_ReturnsMatchingResults()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        await client.PostAsJsonAsync("/api/v1/products/accountingcodes", new CreateAccountingCodeCommand(ProductId: 1, Code: "MAT-100"));
        await client.PostAsJsonAsync("/api/v1/products/accountingcodes", new CreateAccountingCodeCommand(ProductId: 1, Code: "LABOR-001"));

        // Act
        var searchResponse = await client.GetAsync("/api/v1/products/accountingcodes?Search=MAT&PageNumber=1&PageSize=10");

        // Assert
        searchResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
