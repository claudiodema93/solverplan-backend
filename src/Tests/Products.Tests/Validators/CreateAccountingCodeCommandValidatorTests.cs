using FSH.Modules.Products.Application.Features.v1.Commands.CreateAccountingCode;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.CreateAccountingCode;

namespace Products.Tests.Validators;

/// <summary>
/// Tests for CreateAccountingCodeCommandValidator - validates accounting code creation requests.
/// </summary>
[Trait("Category", "Products")]
public sealed class CreateAccountingCodeCommandValidatorTests
{
    private readonly CreateAccountingCodeCommandValidator _sut = new();

    #region ProductId Validation

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(9999)]
    public void ProductId_Should_Pass_When_PositiveValue(int productId)
    {
        // Arrange
        var command = new CreateAccountingCodeCommand(ProductId: productId, Code: "MAT-100");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "ProductId");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void ProductId_Should_Fail_When_ZeroOrNegative(int productId)
    {
        // Arrange
        var command = new CreateAccountingCodeCommand(ProductId: productId, Code: "MAT-100");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "ProductId");
    }

    [Fact]
    public void ProductId_Should_Have_CorrectErrorMessage_When_Invalid()
    {
        // Arrange
        var command = new CreateAccountingCodeCommand(ProductId: 0, Code: "MAT-100");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "ProductId")
            .ShouldContain(e => e.ErrorMessage == "ProductId must be greater than 0.");
    }

    #endregion

    #region Code Validation

    [Theory]
    [InlineData("MAT-100")]
    [InlineData("LABOR-001")]
    [InlineData("A")]
    [InlineData("CODE-WITH-64-CHARS-EXACTLY-PADDED-TO-THE-MAXIMUM-ALLOWED-LENGTH")]
    public void Code_Should_Pass_When_Valid(string code)
    {
        // Arrange
        var command = new CreateAccountingCodeCommand(ProductId: 1, Code: code);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Code");
    }

    [Fact]
    public void Code_Should_Fail_When_Empty()
    {
        // Arrange
        var command = new CreateAccountingCodeCommand(ProductId: 1, Code: "");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Code");
    }

    [Fact]
    public void Code_Should_Have_CorrectErrorMessage_When_Empty()
    {
        // Arrange
        var command = new CreateAccountingCodeCommand(ProductId: 1, Code: "");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Code")
            .ShouldContain(e => e.ErrorMessage == "Code is required.");
    }

    [Fact]
    public void Code_Should_Pass_When_ExactlyMaxLength()
    {
        // Arrange
        var maxLengthCode = new string('A', 64);
        var command = new CreateAccountingCodeCommand(ProductId: 1, Code: maxLengthCode);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Code");
    }

    [Fact]
    public void Code_Should_Fail_When_ExceedsMaxLength()
    {
        // Arrange
        var longCode = new string('A', 65);
        var command = new CreateAccountingCodeCommand(ProductId: 1, Code: longCode);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Code");
    }

    [Fact]
    public void Code_Should_Have_CorrectErrorMessage_When_TooLong()
    {
        // Arrange
        var longCode = new string('A', 65);
        var command = new CreateAccountingCodeCommand(ProductId: 1, Code: longCode);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Code")
            .ShouldContain(e => e.ErrorMessage == "Code must not exceed 64 characters.");
    }

    #endregion

    #region Description Validation

    [Fact]
    public void Description_Should_Pass_When_Null()
    {
        // Arrange
        var command = new CreateAccountingCodeCommand(ProductId: 1, Code: "MAT-100", Description: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Description_Should_Pass_When_Valid()
    {
        // Arrange
        var command = new CreateAccountingCodeCommand(ProductId: 1, Code: "MAT-100", Description: "Material accounting code for raw materials");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Description_Should_Pass_When_ExactlyMaxLength()
    {
        // Arrange
        var maxLengthDescription = new string('a', 512);
        var command = new CreateAccountingCodeCommand(ProductId: 1, Code: "MAT-100", Description: maxLengthDescription);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Description_Should_Fail_When_ExceedsMaxLength()
    {
        // Arrange
        var longDescription = new string('a', 513);
        var command = new CreateAccountingCodeCommand(ProductId: 1, Code: "MAT-100", Description: longDescription);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Description_Should_Have_CorrectErrorMessage_When_TooLong()
    {
        // Arrange
        var longDescription = new string('a', 513);
        var command = new CreateAccountingCodeCommand(ProductId: 1, Code: "MAT-100", Description: longDescription);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Description")
            .ShouldContain(e => e.ErrorMessage == "Description must not exceed 512 characters.");
    }

    #endregion

    #region Combined Validation

    [Fact]
    public void Validate_Should_Pass_When_AllFieldsValid()
    {
        // Arrange
        var command = new CreateAccountingCodeCommand(
            ProductId: 1,
            Code: "MAT-100",
            Description: "Raw material accounting code");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Validate_Should_Pass_When_OnlyRequiredFieldsProvided()
    {
        // Arrange
        var command = new CreateAccountingCodeCommand(ProductId: 1, Code: "MAT-100");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Validate_Should_Fail_When_MultipleFieldsInvalid()
    {
        // Arrange
        var command = new CreateAccountingCodeCommand(ProductId: 0, Code: "");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    #endregion
}
