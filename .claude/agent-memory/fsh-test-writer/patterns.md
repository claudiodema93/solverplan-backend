# FSH Test Patterns — Detailed Reference

## File Placement
```
src/Tests/Products.Tests/
├── GlobalUsings.cs                         # global using Xunit; global using Shouldly;
├── Validators/
│   ├── CreateXxxCommandValidatorTests.cs
│   ├── UpdateXxxCommandValidatorTests.cs
│   └── SearchXxxQueryValidatorTests.cs
└── Integration/
    └── XxxCrudTests.cs                     # All tests Skip="..."
```

## GlobalUsings Pattern
```csharp
global using Xunit;
global using Shouldly;
```
NSubstitute is added per-file with `using NSubstitute;` when needed for handler tests.

## Validator Test Template
```csharp
using FSH.Modules.Products.Application.Features.v1.Commands.CreateXxx;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.CreateXxx;

namespace Products.Tests.Validators;

[Trait("Category", "Products")]
public sealed class CreateXxxCommandValidatorTests
{
    private readonly CreateXxxCommandValidator _sut = new();

    #region {PropertyName} Validation

    [Fact]
    public void {PropertyName}_Should_Pass_When_Valid()
    {
        var command = new CreateXxxCommand(...);
        var result = _sut.Validate(command);
        result.Errors.ShouldNotContain(e => e.PropertyName == "{PropertyName}");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void {PropertyName}_Should_Fail_When_ZeroOrNegative(int value)
    {
        var command = new CreateXxxCommand(...);
        var result = _sut.Validate(command);
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "{PropertyName}");
    }

    [Fact]
    public void {PropertyName}_Should_Have_CorrectErrorMessage_When_Invalid()
    {
        var command = new CreateXxxCommand(...);
        var result = _sut.Validate(command);
        result.Errors
            .Where(e => e.PropertyName == "{PropertyName}")
            .ShouldContain(e => e.ErrorMessage == "Expected message.");
    }

    #endregion

    #region Combined Validation

    [Fact]
    public void Validate_Should_Pass_When_AllFieldsValid() { ... }

    [Fact]
    public void Validate_Should_Pass_When_OnlyRequiredFieldsProvided() { ... }

    [Fact]
    public void Validate_Should_Fail_When_MultipleFieldsInvalid() { ... }

    #endregion
}
```

## Handler Test Template (using NSubstitute, from Multitenancy pattern)
```csharp
using NSubstitute;
namespace Products.Tests.Handlers;

public sealed class CreateXxxCommandHandlerTests
{
    private readonly IXxxRepository _repo;
    private readonly CreateXxxCommandHandler _sut;

    public CreateXxxCommandHandlerTests()
    {
        _repo = Substitute.For<IXxxRepository>();
        _sut = new CreateXxxCommandHandler(_repo);
    }

    [Fact]
    public async Task Handle_Should_ReturnId_When_ValidCommand()
    {
        // Arrange
        var command = new CreateXxxCommand(...);
        _repo.AddAsync(Arg.Any<Xxx>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldBeGreaterThan(0);
        await _repo.Received(1).AddAsync(Arg.Any<Xxx>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Should_ThrowArgumentNullException_When_CommandIsNull()
    {
        await Should.ThrowAsync<ArgumentNullException>(async () =>
            await _sut.Handle(null!, CancellationToken.None));
    }
}
```

NOTE: BomItem handlers use ProductsDbContext directly (EF Core), not a repository interface.
Unit testing them requires an in-memory DbContext setup — currently only integration tests (skipped) cover these.

## Integration Test Template
```csharp
[Trait("Category", "Products")]
[Trait("Type", "Integration")]
[Collection("Sequential")]
public class XxxCrudTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public XxxCrudTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact(Skip = "Requires fully configured test database and authentication infrastructure")]
    public async Task CreateXxx_ValidRequest_ReturnsCreated()
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant");
        // ...
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
    }
}
```

## BomItem-Specific Notes
- `CreateBomItemCommand`: ProductId, ChildProductId (must differ!), Quantity (>0), Unit, IsManual, Notes
- `UpdateBomItemCommand`: Id (>0), ChildProductId (>0), Quantity (>0), Unit, IsManual, Notes
- `DeleteBomItemCommand`: Id (>0) — no validator class exists
- `SearchBomItemsQuery`: PageNumber, PageSize, Sort, Search, ProductId, ChildProductId, Unit, IsManual
- Handlers verify tenant isolation via `ICurrentUser.GetTenant()`
- `NotFoundException` thrown when entity not found

## Products Test csproj Project References
```xml
<ProjectReference Include="..\..\Modules\Products\Modules.Products\Modules.Products.csproj" />
<ProjectReference Include="..\..\Modules\Products\Modules.Products.Contracts\Modules.Products.Contracts.csproj" />
<ProjectReference Include="..\..\Playground\Playground.Api\Playground.Api.csproj" />
```

## BomItemDto namespace
`FSH.Modules.Products.Contracts.DTOs` (NOT `.Domain.DTOs`)
Note: GetBomItemByIdQuery uses `FSH.Modules.Products.Contracts.DTOs.BomItemDto?` as return type.
