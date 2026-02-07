using System.Net;
using System.Net.Http.Json;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.CreateIssue;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.CreateProduct;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.DeleteIssue;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.DeleteProduct;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateIssue;
using FSH.Modules.Products.Contracts.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using Shouldly;
using Xunit;

namespace Products.Tests.Integration;

/// <summary>
/// Integration tests for Issue CRUD workflow and cascade delete behavior.
/// Tests the complete flow from API endpoint through mediator to database,
/// including relationship with Products and cascade delete scenarios.
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
public class IssueCrudTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public IssueCrudTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task CreateIssue_ValidRequest_ReturnsCreatedIssue()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create a product first
        var createProductCommand = new CreateProductCommand(
            Title: "Product with Issue",
            Revision: 1,
            Description: "Product for issue testing",
            Status: ProductState.Active);

        var productResponse = await client.PostAsJsonAsync("/api/v1/products", createProductCommand);
        var productResult = await productResponse.Content.ReadFromJsonAsync<CreateProductResponse>();
        var productId = productResult!.Id;

        // Create an issue for this product
        var command = new CreateIssueCommand(
            ProductId: productId,
            Title: "Critical Bug",
            Description: "This is a critical bug that needs immediate attention",
            Severity: IssueSeverity.Critical,
            Status: IssueState.Open);

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/issues", command);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<CreateIssueResponse>();
        result.ShouldNotBeNull();
        result.Id.ShouldBeGreaterThan(0);
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task GetIssueById_ExistingIssue_ReturnsIssue()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create a product and issue first
        var createProductCommand = new CreateProductCommand(
            Title: "Product for Get Test",
            Revision: 1,
            Status: ProductState.Active);

        var productResponse = await client.PostAsJsonAsync("/api/v1/products", createProductCommand);
        var productResult = await productResponse.Content.ReadFromJsonAsync<CreateProductResponse>();
        var productId = productResult!.Id;

        var createIssueCommand = new CreateIssueCommand(
            ProductId: productId,
            Title: "Get Test Issue",
            Description: "Issue for retrieval testing",
            Severity: IssueSeverity.High,
            Status: IssueState.Open);

        var issueResponse = await client.PostAsJsonAsync("/api/v1/issues", createIssueCommand);
        var issueResult = await issueResponse.Content.ReadFromJsonAsync<CreateIssueResponse>();
        var issueId = issueResult!.Id;

        // Act
        var getResponse = await client.GetAsync($"/api/v1/issues/{issueId}");

        // Assert
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var issue = await getResponse.Content.ReadFromJsonAsync<object>();
        issue.ShouldNotBeNull();
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task UpdateIssue_StatusAndResolution_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create a product and issue first
        var createProductCommand = new CreateProductCommand(
            Title: "Product for Update Test",
            Revision: 1,
            Status: ProductState.Active);

        var productResponse = await client.PostAsJsonAsync("/api/v1/products", createProductCommand);
        var productResult = await productResponse.Content.ReadFromJsonAsync<CreateProductResponse>();
        var productId = productResult!.Id;

        var createIssueCommand = new CreateIssueCommand(
            ProductId: productId,
            Title: "Update Test Issue",
            Description: "Original description",
            Severity: IssueSeverity.Medium,
            Status: IssueState.Open);

        var issueResponse = await client.PostAsJsonAsync("/api/v1/issues", createIssueCommand);
        var issueResult = await issueResponse.Content.ReadFromJsonAsync<CreateIssueResponse>();
        var issueId = issueResult!.Id;

        // Act - Update the issue with resolution
        var updateCommand = new UpdateIssueCommand(
            Id: issueId,
            Title: "Update Test Issue",
            Description: "Updated description with more details",
            Severity: IssueSeverity.High,
            Status: IssueState.Resolved,
            ResolutionNotes: "Fixed by updating the configuration file");

        var updateResponse = await client.PutAsJsonAsync($"/api/v1/issues/{issueId}", updateCommand);

        // Assert
        updateResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task SearchIssues_WithPagination_ReturnsPaginatedResults()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create a product
        var createProductCommand = new CreateProductCommand(
            Title: "Product with Multiple Issues",
            Revision: 1,
            Status: ProductState.Active);

        var productResponse = await client.PostAsJsonAsync("/api/v1/products", createProductCommand);
        var productResult = await productResponse.Content.ReadFromJsonAsync<CreateProductResponse>();
        var productId = productResult!.Id;

        // Create test issues
        var severities = new[] { IssueSeverity.Low, IssueSeverity.Medium, IssueSeverity.High };
        for (int i = 1; i <= 3; i++)
        {
            await client.PostAsJsonAsync("/api/v1/issues", new CreateIssueCommand(
                ProductId: productId,
                Title: $"Search Test Issue {i}",
                Description: $"Description for issue {i}",
                Severity: severities[i - 1],
                Status: IssueState.Open));
        }

        // Act - Search with pagination
        var searchResponse = await client.GetAsync("/api/v1/issues?PageNumber=1&PageSize=10");

        // Assert
        searchResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var searchResult = await searchResponse.Content.ReadAsStringAsync();
        searchResult.ShouldNotBeNullOrEmpty();
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task DeleteIssue_ExistingIssue_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create a product and issue first
        var createProductCommand = new CreateProductCommand(
            Title: "Product for Delete Test",
            Revision: 1,
            Status: ProductState.Active);

        var productResponse = await client.PostAsJsonAsync("/api/v1/products", createProductCommand);
        var productResult = await productResponse.Content.ReadFromJsonAsync<CreateProductResponse>();
        var productId = productResult!.Id;

        var createIssueCommand = new CreateIssueCommand(
            ProductId: productId,
            Title: "Delete Test Issue",
            Description: "To be deleted",
            Severity: IssueSeverity.Low,
            Status: IssueState.Open);

        var issueResponse = await client.PostAsJsonAsync("/api/v1/issues", createIssueCommand);
        var issueResult = await issueResponse.Content.ReadFromJsonAsync<CreateIssueResponse>();
        var issueId = issueResult!.Id;

        // Act - Delete the issue
        var deleteResponse = await client.DeleteAsync($"/api/v1/issues/{issueId}");

        // Assert
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Verify the issue is deleted (or soft-deleted)
        var getResponse = await client.GetAsync($"/api/v1/issues/{issueId}");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task DeleteProduct_WithIssues_CascadeDeletesIssues()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create a product
        var createProductCommand = new CreateProductCommand(
            Title: "Product for Cascade Delete Test",
            Revision: 1,
            Status: ProductState.Active);

        var productResponse = await client.PostAsJsonAsync("/api/v1/products", createProductCommand);
        var productResult = await productResponse.Content.ReadFromJsonAsync<CreateProductResponse>();
        var productId = productResult!.Id;

        // Create multiple issues for this product
        var issueIds = new List<int>();
        for (int i = 1; i <= 3; i++)
        {
            var createIssueCommand = new CreateIssueCommand(
                ProductId: productId,
                Title: $"Cascade Test Issue {i}",
                Description: $"Issue {i} to be cascade deleted",
                Severity: IssueSeverity.Medium,
                Status: IssueState.Open);

            var issueResponse = await client.PostAsJsonAsync("/api/v1/issues", createIssueCommand);
            var issueResult = await issueResponse.Content.ReadFromJsonAsync<CreateIssueResponse>();
            issueIds.Add(issueResult!.Id);
        }

        // Act - Delete the product (should cascade delete all issues)
        var deleteProductResponse = await client.DeleteAsync($"/api/v1/products/{productId}");

        // Assert
        deleteProductResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Verify the product is deleted
        var getProductResponse = await client.GetAsync($"/api/v1/products/{productId}");
        getProductResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);

        // Verify all issues are cascade deleted
        foreach (var issueId in issueIds)
        {
            var getIssueResponse = await client.GetAsync($"/api/v1/issues/{issueId}");
            getIssueResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task CreateIssue_InvalidData_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Invalid command - empty title
        var invalidCommand = new CreateIssueCommand(
            ProductId: 1,
            Title: "", // Invalid: empty title
            Description: "Valid description",
            Severity: IssueSeverity.Low,
            Status: IssueState.Open);

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/issues", invalidCommand);

        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task CreateIssue_NonExistentProduct_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Invalid command - non-existent product ID
        var invalidCommand = new CreateIssueCommand(
            ProductId: 999999, // Non-existent product ID
            Title: "Test Issue",
            Description: "This should fail",
            Severity: IssueSeverity.Medium,
            Status: IssueState.Open);

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/issues", invalidCommand);

        // Assert
        response.StatusCode.ShouldBeOneOf(
            HttpStatusCode.BadRequest,
            HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task UpdateIssue_ChangeStatus_FromOpenToResolved_ReturnsSuccess()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");

        // Create a product and issue
        var createProductCommand = new CreateProductCommand(
            Title: "Product for Status Change Test",
            Revision: 1,
            Status: ProductState.Active);

        var productResponse = await client.PostAsJsonAsync("/api/v1/products", createProductCommand);
        var productResult = await productResponse.Content.ReadFromJsonAsync<CreateProductResponse>();
        var productId = productResult!.Id;

        var createIssueCommand = new CreateIssueCommand(
            ProductId: productId,
            Title: "Status Change Issue",
            Description: "Testing status changes",
            Severity: IssueSeverity.Critical,
            Status: IssueState.Open);

        var issueResponse = await client.PostAsJsonAsync("/api/v1/issues", createIssueCommand);
        var issueResult = await issueResponse.Content.ReadFromJsonAsync<CreateIssueResponse>();
        var issueId = issueResult!.Id;

        // Act - Change status from Open to Resolved
        var updateCommand = new UpdateIssueCommand(
            Id: issueId,
            Title: "Status Change Issue",
            Description: "Testing status changes",
            Severity: IssueSeverity.Critical,
            Status: IssueState.Resolved,
            ResolutionNotes: "Issue resolved by deploying hotfix v1.2.3");

        var updateResponse = await client.PutAsJsonAsync($"/api/v1/issues/{issueId}", updateCommand);

        // Assert
        updateResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

        // Verify the update by getting the issue
        var getResponse = await client.GetAsync($"/api/v1/issues/{issueId}");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
    }
}
