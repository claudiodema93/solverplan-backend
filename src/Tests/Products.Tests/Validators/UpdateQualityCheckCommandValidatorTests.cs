using FSH.Modules.Products.Contracts.Features.v1.Commands.UpdateQualityCheck;
using FSH.Modules.Products.Features.v1.Commands.UpdateQualityCheck;

namespace Products.Tests.Validators;

/// <summary>
/// Tests for UpdateQualityCheckCommandValidator - validates quality check update requests.
/// </summary>
[Trait("Category", "Products")]
public sealed class UpdateQualityCheckCommandValidatorTests
{
    private readonly UpdateQualityCheckCommandValidator _sut = new();

    #region Id Validation

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(9999)]
    public void Id_Should_Pass_When_PositiveValue(int id)
    {
        // Arrange
        var command = new UpdateQualityCheckCommand(
            Id: id,
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Check for visible defects",
            Department: "QA");

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
        var command = new UpdateQualityCheckCommand(
            Id: id,
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Check for visible defects",
            Department: "QA");

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
        var command = new UpdateQualityCheckCommand(
            Id: 0,
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Check for visible defects",
            Department: "QA");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Id")
            .ShouldContain(e => e.ErrorMessage == "Quality check ID must be greater than 0.");
    }

    #endregion

    #region ProductId Validation

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(9999)]
    public void ProductId_Should_Pass_When_PositiveValue(int productId)
    {
        // Arrange
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: productId,
            Title: "Visual Inspection",
            Description: "Check for visible defects",
            Department: "QA");

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
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: productId,
            Title: "Visual Inspection",
            Description: "Check for visible defects",
            Department: "QA");

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
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 0,
            Title: "Visual Inspection",
            Description: "Check for visible defects",
            Department: "QA");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "ProductId")
            .ShouldContain(e => e.ErrorMessage == "Product ID must be greater than 0.");
    }

    #endregion

    #region Title Validation

    [Fact]
    public void Title_Should_Pass_When_Valid()
    {
        // Arrange
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 1,
            Title: "Updated Visual Inspection",
            Description: "Check for visible defects",
            Department: "QA");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Title");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Title_Should_Fail_When_Empty(string? title)
    {
        // Arrange
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 1,
            Title: title!,
            Description: "Check for visible defects",
            Department: "QA");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Title_Should_Fail_When_ExceedsMaxLength()
    {
        // Arrange
        var longTitle = new string('a', 201);
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 1,
            Title: longTitle,
            Description: "Check for visible defects",
            Department: "QA");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Title_Should_Pass_When_ExactlyMaxLength()
    {
        // Arrange
        var maxLengthTitle = new string('a', 200);
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 1,
            Title: maxLengthTitle,
            Description: "Check for visible defects",
            Department: "QA");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Title_Should_Have_CorrectErrorMessage_When_Empty()
    {
        // Arrange
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 1,
            Title: "",
            Description: "Check for visible defects",
            Department: "QA");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Title")
            .ShouldContain(e => e.ErrorMessage == "Title is required.");
    }

    [Fact]
    public void Title_Should_Have_CorrectErrorMessage_When_TooLong()
    {
        // Arrange
        var longTitle = new string('a', 201);
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 1,
            Title: longTitle,
            Description: "Check for visible defects",
            Department: "QA");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Title")
            .ShouldContain(e => e.ErrorMessage == "Title must not exceed 200 characters.");
    }

    #endregion

    #region Description Validation

    [Fact]
    public void Description_Should_Pass_When_Valid()
    {
        // Arrange
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Updated description for quality check",
            Department: "QA");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Description");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Description_Should_Fail_When_Empty(string? description)
    {
        // Arrange
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 1,
            Title: "Visual Inspection",
            Description: description!,
            Department: "QA");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Description_Should_Fail_When_ExceedsMaxLength()
    {
        // Arrange
        var longDescription = new string('a', 2001);
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 1,
            Title: "Visual Inspection",
            Description: longDescription,
            Department: "QA");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Description_Should_Pass_When_ExactlyMaxLength()
    {
        // Arrange
        var maxLengthDescription = new string('a', 2000);
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 1,
            Title: "Visual Inspection",
            Description: maxLengthDescription,
            Department: "QA");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Description_Should_Have_CorrectErrorMessage_When_Empty()
    {
        // Arrange
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "",
            Department: "QA");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Description")
            .ShouldContain(e => e.ErrorMessage == "Description is required.");
    }

    [Fact]
    public void Description_Should_Have_CorrectErrorMessage_When_TooLong()
    {
        // Arrange
        var longDescription = new string('a', 2001);
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 1,
            Title: "Visual Inspection",
            Description: longDescription,
            Department: "QA");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Description")
            .ShouldContain(e => e.ErrorMessage == "Description must not exceed 2000 characters.");
    }

    #endregion

    #region Department Validation

    [Fact]
    public void Department_Should_Pass_When_Valid()
    {
        // Arrange
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Check for visible defects",
            Department: "Manufacturing");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Department");
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void Department_Should_Fail_When_Empty(string? department)
    {
        // Arrange
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Check for visible defects",
            Department: department!);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Department");
    }

    [Fact]
    public void Department_Should_Fail_When_ExceedsMaxLength()
    {
        // Arrange
        var longDepartment = new string('a', 101);
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Check for visible defects",
            Department: longDepartment);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Department");
    }

    [Fact]
    public void Department_Should_Pass_When_ExactlyMaxLength()
    {
        // Arrange
        var maxLengthDepartment = new string('a', 100);
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Check for visible defects",
            Department: maxLengthDepartment);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Department");
    }

    [Fact]
    public void Department_Should_Have_CorrectErrorMessage_When_Empty()
    {
        // Arrange
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Check for visible defects",
            Department: "");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Department")
            .ShouldContain(e => e.ErrorMessage == "Department is required.");
    }

    [Fact]
    public void Department_Should_Have_CorrectErrorMessage_When_TooLong()
    {
        // Arrange
        var longDepartment = new string('a', 101);
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Check for visible defects",
            Department: longDepartment);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Department")
            .ShouldContain(e => e.ErrorMessage == "Department must not exceed 100 characters.");
    }

    #endregion

    #region DisplayOrder Validation

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(100)]
    public void DisplayOrder_Should_Pass_When_ZeroOrPositive(int displayOrder)
    {
        // Arrange
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Check for visible defects",
            Department: "QA",
            DisplayOrder: displayOrder);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "DisplayOrder");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void DisplayOrder_Should_Fail_When_Negative(int displayOrder)
    {
        // Arrange
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Check for visible defects",
            Department: "QA",
            DisplayOrder: displayOrder);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "DisplayOrder");
    }

    [Fact]
    public void DisplayOrder_Should_Have_CorrectErrorMessage_When_Negative()
    {
        // Arrange
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Check for visible defects",
            Department: "QA",
            DisplayOrder: -1);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "DisplayOrder")
            .ShouldContain(e => e.ErrorMessage == "Display order must be 0 or greater.");
    }

    #endregion

    #region Combined Validation

    [Fact]
    public void Validate_Should_Pass_When_AllFieldsValid()
    {
        // Arrange
        var command = new UpdateQualityCheckCommand(
            Id: 1,
            ProductId: 1,
            Title: "Updated Visual Inspection",
            Description: "Updated: Inspect product surface for defects before packaging",
            Department: "Quality Assurance",
            IsRequired: true,
            DisplayOrder: 2);

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
        var command = new UpdateQualityCheckCommand(
            Id: 0,
            ProductId: 0,
            Title: "",
            Description: "",
            Department: "",
            DisplayOrder: -1);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(5);
    }

    #endregion
}
