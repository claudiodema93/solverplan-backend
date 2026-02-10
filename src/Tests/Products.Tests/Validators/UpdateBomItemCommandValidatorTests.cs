using FSH.Modules.Products.Application.Features.v1.Commands.UpdateBomItem;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateBomItem;
using FSH.Modules.Products.Contracts.Domain.Enums;

namespace Products.Tests.Validators;

/// <summary>
/// Tests for UpdateBomItemCommandValidator - validates BOM item update requests.
/// </summary>
[Trait("Category", "Products")]
public sealed class UpdateBomItemCommandValidatorTests
{
    private readonly UpdateBomItemCommandValidator _sut = new();

    #region Id Validation

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(9999)]
    public void Id_Should_Pass_When_PositiveValue(int id)
    {
        // Arrange
        var command = new UpdateBomItemCommand(Id: id, ChildProductId: 2, Quantity: 1.0, Unit: UnitOfMeasurement.Pcs, IsManual: false, Notes: null);

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
        var command = new UpdateBomItemCommand(Id: id, ChildProductId: 2, Quantity: 1.0, Unit: UnitOfMeasurement.Pcs, IsManual: false, Notes: null);

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
        var command = new UpdateBomItemCommand(Id: 0, ChildProductId: 2, Quantity: 1.0, Unit: UnitOfMeasurement.Pcs, IsManual: false, Notes: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Id")
            .ShouldContain(e => e.ErrorMessage == "Id must be greater than 0.");
    }

    #endregion

    #region ChildProductId Validation

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(9999)]
    public void ChildProductId_Should_Pass_When_PositiveValue(int childProductId)
    {
        // Arrange
        var command = new UpdateBomItemCommand(Id: 1, ChildProductId: childProductId, Quantity: 1.0, Unit: UnitOfMeasurement.Pcs, IsManual: false, Notes: null);

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
        var command = new UpdateBomItemCommand(Id: 1, ChildProductId: childProductId, Quantity: 1.0, Unit: UnitOfMeasurement.Pcs, IsManual: false, Notes: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "ChildProductId");
    }

    [Fact]
    public void ChildProductId_Should_Have_CorrectErrorMessage_When_Invalid()
    {
        // Arrange
        var command = new UpdateBomItemCommand(Id: 1, ChildProductId: 0, Quantity: 1.0, Unit: UnitOfMeasurement.Pcs, IsManual: false, Notes: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "ChildProductId")
            .ShouldContain(e => e.ErrorMessage == "ChildProductId must be greater than 0.");
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
        var command = new UpdateBomItemCommand(Id: 1, ChildProductId: 2, Quantity: quantity, Unit: UnitOfMeasurement.Pcs, IsManual: false, Notes: null);

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
        var command = new UpdateBomItemCommand(Id: 1, ChildProductId: 2, Quantity: quantity, Unit: UnitOfMeasurement.Pcs, IsManual: false, Notes: null);

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
        var command = new UpdateBomItemCommand(Id: 1, ChildProductId: 2, Quantity: 0.0, Unit: UnitOfMeasurement.Pcs, IsManual: false, Notes: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Quantity")
            .ShouldContain(e => e.ErrorMessage == "Quantity must be greater than 0.");
    }

    #endregion

    #region Notes Validation

    [Fact]
    public void Notes_Should_Pass_When_Null()
    {
        // Arrange
        var command = new UpdateBomItemCommand(Id: 1, ChildProductId: 2, Quantity: 1.0, Unit: UnitOfMeasurement.Pcs, IsManual: false, Notes: null);

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
        var command = new UpdateBomItemCommand(Id: 1, ChildProductId: 2, Quantity: 1.0, Unit: UnitOfMeasurement.Pcs, IsManual: false, Notes: "");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Notes");
    }

    [Fact]
    public void Notes_Should_Pass_When_Valid()
    {
        // Arrange
        var command = new UpdateBomItemCommand(Id: 1, ChildProductId: 2, Quantity: 1.0, Unit: UnitOfMeasurement.Pcs, IsManual: false, Notes: "Updated assembly note");

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
        var command = new UpdateBomItemCommand(Id: 1, ChildProductId: 2, Quantity: 1.0, Unit: UnitOfMeasurement.Pcs, IsManual: false, Notes: maxLengthNotes);

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
        var command = new UpdateBomItemCommand(Id: 1, ChildProductId: 2, Quantity: 1.0, Unit: UnitOfMeasurement.Pcs, IsManual: false, Notes: longNotes);

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
        var command = new UpdateBomItemCommand(Id: 1, ChildProductId: 2, Quantity: 1.0, Unit: UnitOfMeasurement.Pcs, IsManual: false, Notes: longNotes);

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
        var command = new UpdateBomItemCommand(Id: 1, ChildProductId: 2, Quantity: 1.0, Unit: unit, IsManual: false, Notes: null);

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
        var command = new UpdateBomItemCommand(Id: 1, ChildProductId: 2, Quantity: 1.0, Unit: UnitOfMeasurement.Pcs, IsManual: isManual, Notes: null);

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
        var command = new UpdateBomItemCommand(
            Id: 1,
            ChildProductId: 2,
            Quantity: 5.0,
            Unit: UnitOfMeasurement.Millimeters,
            IsManual: true,
            Notes: "Updated assembly note for component");

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
        var command = new UpdateBomItemCommand(
            Id: 1,
            ChildProductId: 2,
            Quantity: 1.0,
            Unit: UnitOfMeasurement.Pcs,
            IsManual: false,
            Notes: null);

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
        var command = new UpdateBomItemCommand(
            Id: 0,
            ChildProductId: 0,
            Quantity: -1.0,
            Unit: UnitOfMeasurement.Pcs,
            IsManual: false,
            Notes: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(3);
    }

    #endregion
}
