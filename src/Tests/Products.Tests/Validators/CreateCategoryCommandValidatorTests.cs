using FSH.Modules.Products.Application.Features.v1.Commands.CreateCategory;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.CreateCategory;

namespace Products.Tests.Validators;

/// <summary>
/// Tests for CreateCategoryCommandValidator - validates category creation requests.
/// </summary>
[Trait("Category", "Products")]
public sealed class CreateCategoryCommandValidatorTests
{
    private readonly CreateCategoryCommandValidator _sut = new();

    #region Name Validation

    [Fact]
    public void Name_Should_Pass_When_Valid()
    {
        // Arrange
        var command = new CreateCategoryCommand("Electronics");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Name");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Name_Should_Fail_When_Empty(string? name)
    {
        // Arrange
        var command = new CreateCategoryCommand(name!);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Name");
        result.Errors.ShouldContain(e => e.ErrorMessage == "Name is required.");
    }

    [Fact]
    public void Name_Should_Fail_When_ExceedsMaxLength()
    {
        // Arrange
        var name = new string('A', 129); // 129 characters (max is 128)
        var command = new CreateCategoryCommand(name);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Name");
        result.Errors.ShouldContain(e => e.ErrorMessage == "Name must not exceed 128 characters.");
    }

    [Fact]
    public void Name_Should_Pass_When_ExactlyMaxLength()
    {
        // Arrange
        var name = new string('A', 128); // Exactly 128 characters
        var command = new CreateCategoryCommand(name);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Name");
    }

    [Theory]
    [InlineData("A")]
    [InlineData("AB")]
    [InlineData("Category Name")]
    [InlineData("Cat")]
    public void Name_Should_Pass_When_ValidLength(string name)
    {
        // Arrange
        var command = new CreateCategoryCommand(name);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Name_Should_Pass_With_SpecialCharacters()
    {
        // Arrange
        var command = new CreateCategoryCommand("Category - Electronics & Parts (2024)");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Name");
    }

    [Fact]
    public void Name_Should_Pass_With_UnicodeCharacters()
    {
        // Arrange
        var command = new CreateCategoryCommand("Elektronik & Möbel");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Name");
    }

    #endregion

    #region Overall Validation

    [Fact]
    public void Should_Pass_When_AllFieldsAreValid()
    {
        // Arrange
        var command = new CreateCategoryCommand("Furniture & Equipment");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Should_Fail_When_NameIsMissing()
    {
        // Arrange
        var command = new CreateCategoryCommand(string.Empty);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThan(0);
    }

    #endregion
}
