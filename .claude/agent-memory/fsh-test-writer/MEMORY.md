# FSH Test Writer — Persistent Memory

See `patterns.md` for full details. Key quick-reference below.

## Test Project: Products.Tests

- Path: `src/Tests/Products.Tests/`
- csproj refs: `Modules.Products`, `Modules.Products.Contracts`,
  `Playground.Api`
- Packages: xUnit, Shouldly, NSubstitute, AutoFixture,
  Microsoft.AspNetCore.Mvc.Testing
- GlobalUsings: `global using Xunit; global using Shouldly;`

## Assertion Library: Shouldly (NOT FluentAssertions)

- `.ShouldBe()`, `.ShouldBeTrue()`, `.ShouldBeFalse()`, `.ShouldBeEmpty()`
- `.ShouldContain(predicate)`, `.ShouldNotContain(predicate)`
- `Should.ThrowAsync<T>(async () => ...)`

## Mock Library: NSubstitute

- `Substitute.For<IInterface>()`
- `.Returns(value)` / `.Returns(Task.FromResult(value))`
- `await mock.Received(1).Method(arg, Arg.Any<CancellationToken>())`

## Validator Test Pattern

- Instantiate `new XxxValidator()` directly in field:
  `private readonly XxxValidator _sut = new();`
- Use `_sut.Validate(command)` (NOT `ValidateAsync`)
- Group tests with `#region {PropertyName} Validation`
- `[Trait("Category", "Products")]` on class
- `sealed` test classes

## Handler Test Pattern (see Multitenancy.Tests example)

- Inject mocked deps via constructor in `public XxxHandlerTests()`
- `await _sut.Handle(command, CancellationToken.None)` — returns `ValueTask<T>`

## Integration Test Pattern

- `IClassFixture<WebApplicationFactory<Program>>` + `[Collection("Sequential")]`
- ALL tests use `[Fact(Skip = "Requires fully configured test database...")]`
- Client header: `client.DefaultRequestHeaders.Add("X-Tenant", "test-tenant")`

## Namespaces

- Validator: `FSH.Modules.Products.Features.v1.Commands.{Feature}`
- Command/Query (Contracts):
  `FSH.Modules.Products.Contracts.Features.v1.Commands.{Feature}`
- Test namespace: `Products.Tests.Validators` / `Products.Tests.Integration`

## Key FSH Rules

- `ICommand<T>` returns `ValueTask<T>`, `ICommand` (no return) returns
  `ValueTask<Unit>`
- `IQuery<T>` returns `ValueTask<T>`
- Mediator (NOT MediatR) — `using Mediator;`
- Handlers use `ProductsDbContext` + `ICurrentUser` directly (no repository
  abstraction)

See `patterns.md` for complete examples.
