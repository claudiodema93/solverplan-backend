using FSH.Modules.Products.Contracts.Features.v1.Commands.UpdateAccountingCode;
using FSH.Modules.Products.Features.v1.Commands.UpdateAccountingCode;

namespace Products.Tests.Validators;

/// <summary>
/// Tests for UpdateAccountingCodeCommandValidator - validates accounting code update requests.
/// </summary>
[Trait("Category", "Products")]
public sealed class UpdateAccountingCodeCommandValidatorTests
{
    private readonly UpdateAccountingCodeCommandValidator _sut = new();

    #region Id Validation

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(9999)]
    public void Id_Should_Pass_When_PositiveValue(int id)
    {
        // Arrange
        var command = new UpdateAccountingCodeCommand(Id: id, Code: "MAT-100");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Id");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Id_Should_Fail_When_ZeroOrNegative(int id)
    {
        // Arrange
        var command = new UpdateAccountingCodeCommand(Id: id, Code: "MAT-100");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Id");
    }

    [Fact]
    public void Id_Should_Have_CorrectErrorMessage_When_Invalid()
    {
        // Arrange
        var command = new UpdateAccountingCodeCommand(Id: 0, Code: "MAT-100");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Id")
            .ShouldContain(e => e.ErrorMessage == "Id must be greater than 0.");
    }

    #endregion

    #region Code Validation

    [Theory]
    [InlineData("MAT-100")]
    [InlineData("LABOR-001")]
    [InlineData("A")]
    public void Code_Should_Pass_When_Valid(string code)
    {
        // Arrange
        var command = new UpdateAccountingCodeCommand(Id: 1, Code: code);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Code");
    }

    [Fact]
    public void Code_Should_Fail_When_Empty()
    {
        // Arrange
        var command = new UpdateAccountingCodeCommand(Id: 1, Code: "");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Code");
    }

    [Fact]
    public void Code_Should_Pass_When_ExactlyMaxLength()
    {
        // Arrange
        var maxLengthCode = new string('A', 64);
        var command = new UpdateAccountingCodeCommand(Id: 1, Code: maxLengthCode);

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
        var command = new UpdateAccountingCodeCommand(Id: 1, Code: longCode);

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
        var command = new UpdateAccountingCodeCommand(Id: 1, Code: longCode);

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
        var command = new UpdateAccountingCodeCommand(Id: 1, Code: "MAT-100", Description: null);

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
        var command = new UpdateAccountingCodeCommand(Id: 1, Code: "MAT-100", Description: longDescription);

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
        var command = new UpdateAccountingCodeCommand(Id: 1, Code: "MAT-100", Description: longDescription);

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
        var command = new UpdateAccountingCodeCommand(
            Id: 1,
            Code: "MAT-200",
            Description: "Updated material code");

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
        var command = new UpdateAccountingCodeCommand(Id: 0, Code: "");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    #endregion
}
