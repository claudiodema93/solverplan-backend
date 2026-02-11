using FSH.Modules.Products.Contracts.Features.v1.Commands.CreateBomItem;
using FSH.Modules.Products.Contracts.Domain.Enums;
using FSH.Modules.Products.Features.v1.Commands.CreateBomItem;

namespace Products.Tests.Validators;

/// <summary>
/// Tests for CreateBomItemCommandValidator - validates BOM item creation requests.
/// </summary>
[Trait("Category", "Products")]
public sealed class CreateBomItemCommandValidatorTests
{
    private readonly CreateBomItemCommandValidator _sut = new();

    #region ProductId Validation

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(9999)]
    public void ProductId_Should_Pass_When_PositiveValue(int productId)
    {
        // Arrange
        var command = new CreateBomItemCommand(ProductId: productId, ChildProductId: productId + 1);

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
        var command = new CreateBomItemCommand(ProductId: productId, ChildProductId: 1);

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
        var command = new CreateBomItemCommand(ProductId: 0, ChildProductId: 1);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "ProductId")
            .ShouldContain(e => e.ErrorMessage == "ProductId must be greater than 0.");
    }

    #endregion

    #region ChildProductId Validation

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(9999)]
    public void ChildProductId_Should_Pass_When_PositiveAndDifferentFromProductId(int childProductId)
    {
        // Arrange
        var command = new CreateBomItemCommand(ProductId: childProductId + 1, ChildProductId: childProductId);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "ChildProductId");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void ChildProductId_Should_Fail_When_ZeroOrNegative(int childProductId)
    {
        // Arrange
        var command = new CreateBomItemCommand(ProductId: 1, ChildProductId: childProductId);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "ChildProductId");
    }

    [Fact]
    public void ChildProductId_Should_Have_CorrectErrorMessage_When_ZeroOrNegative()
    {
        // Arrange
        var command = new CreateBomItemCommand(ProductId: 1, ChildProductId: 0);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "ChildProductId")
            .ShouldContain(e => e.ErrorMessage == "ChildProductId must be greater than 0.");
    }

    [Fact]
    public void ChildProductId_Should_Fail_When_EqualToProductId()
    {
        // Arrange
        var command = new CreateBomItemCommand(ProductId: 5, ChildProductId: 5);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "ChildProductId");
    }

    [Fact]
    public void ChildProductId_Should_Have_CorrectErrorMessage_When_EqualToProductId()
    {
        // Arrange
        var command = new CreateBomItemCommand(ProductId: 5, ChildProductId: 5);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "ChildProductId")
            .ShouldContain(e => e.ErrorMessage == "ChildProductId must be different from ProductId. A product cannot reference itself in a BOM.");
    }

    #endregion

    #region Quantity Validation

    [Theory]
    [InlineData(0.001)]
    [InlineData(1.0)]
    [InlineData(100.5)]
    [InlineData(9999.99)]
    public void Quantity_Should_Pass_When_PositiveValue(double quantity)
    {
        // Arrange
        var command = new CreateBomItemCommand(ProductId: 1, ChildProductId: 2, Quantity: quantity);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Quantity");
    }

    [Theory]
    [InlineData(0.0)]
    [InlineData(-1.0)]
    [InlineData(-100.5)]
    public void Quantity_Should_Fail_When_ZeroOrNegative(double quantity)
    {
        // Arrange
        var command = new CreateBomItemCommand(ProductId: 1, ChildProductId: 2, Quantity: quantity);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Quantity");
    }

    [Fact]
    public void Quantity_Should_Have_CorrectErrorMessage_When_ZeroOrNegative()
    {
        // Arrange
        var command = new CreateBomItemCommand(ProductId: 1, ChildProductId: 2, Quantity: 0.0);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Quantity")
            .ShouldContain(e => e.ErrorMessage == "Quantity must be greater than 0.");
    }

    [Fact]
    public void Quantity_Should_Pass_When_DefaultValue()
    {
        // Arrange
        var command = new CreateBomItemCommand(ProductId: 1, ChildProductId: 2); // Default quantity is 1.0

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Quantity");
    }

    #endregion

    #region Notes Validation

    [Fact]
    public void Notes_Should_Pass_When_Null()
    {
        // Arrange
        var command = new CreateBomItemCommand(ProductId: 1, ChildProductId: 2, Notes: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Notes");
    }

    [Fact]
    public void Notes_Should_Pass_When_Empty()
    {
        // Arrange
        // Note: Empty string bypasses the When() condition
        var command = new CreateBomItemCommand(ProductId: 1, ChildProductId: 2, Notes: "");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Notes");
    }

    [Fact]
    public void Notes_Should_Pass_When_Valid()
    {
        // Arrange
        var command = new CreateBomItemCommand(ProductId: 1, ChildProductId: 2, Notes: "Assembly note for this component");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Notes");
    }

    [Fact]
    public void Notes_Should_Pass_When_ExactlyMaxLength()
    {
        // Arrange
        var maxLengthNotes = new string('a', 2000);
        var command = new CreateBomItemCommand(ProductId: 1, ChildProductId: 2, Notes: maxLengthNotes);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Notes");
    }

    [Fact]
    public void Notes_Should_Fail_When_ExceedsMaxLength()
    {
        // Arrange
        var longNotes = new string('a', 2001);
        var command = new CreateBomItemCommand(ProductId: 1, ChildProductId: 2, Notes: longNotes);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Notes");
    }

    [Fact]
    public void Notes_Should_Have_CorrectErrorMessage_When_TooLong()
    {
        // Arrange
        var longNotes = new string('a', 2001);
        var command = new CreateBomItemCommand(ProductId: 1, ChildProductId: 2, Notes: longNotes);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Notes")
            .ShouldContain(e => e.ErrorMessage == "Notes must not exceed 2000 characters.");
    }

    #endregion

    #region Unit Validation

    [Theory]
    [InlineData(UnitOfMeasurement.Pcs)]
    [InlineData(UnitOfMeasurement.Inches)]
    [InlineData(UnitOfMeasurement.Millimeters)]
    [InlineData(UnitOfMeasurement.Liters)]
    public void Unit_Should_Pass_For_AllValidEnumValues(UnitOfMeasurement unit)
    {
        // Arrange
        var command = new CreateBomItemCommand(ProductId: 1, ChildProductId: 2, Unit: unit);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Unit");
    }

    #endregion

    #region IsManual Validation

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsManual_Should_Pass_For_BothValues(bool isManual)
    {
        // Arrange
        var command = new CreateBomItemCommand(ProductId: 1, ChildProductId: 2, IsManual: isManual);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "IsManual");
    }

    #endregion

    #region Combined Validation

    [Fact]
    public void Validate_Should_Pass_When_AllFieldsValid()
    {
        // Arrange
        var command = new CreateBomItemCommand(
            ProductId: 1,
            ChildProductId: 2,
            Quantity: 5.0,
            Unit: UnitOfMeasurement.Pcs,
            IsManual: true,
            Notes: "Required for assembly step 3");

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
        var command = new CreateBomItemCommand(ProductId: 1, ChildProductId: 2);

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
        var command = new CreateBomItemCommand(
            ProductId: 0,
            ChildProductId: 0,
            Quantity: -1.0);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public void Validate_Should_Fail_When_ProductIdAndChildProductIdAreTheSame()
    {
        // Arrange
        var command = new CreateBomItemCommand(ProductId: 10, ChildProductId: 10);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e =>
            e.PropertyName == "ChildProductId" &&
            e.ErrorMessage == "ChildProductId must be different from ProductId. A product cannot reference itself in a BOM.");
    }

    #endregion
}
