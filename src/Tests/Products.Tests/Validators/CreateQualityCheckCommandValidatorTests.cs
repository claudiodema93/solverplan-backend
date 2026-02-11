using FSH.Modules.Products.Contracts.Features.v1.Commands.CreateQualityCheck;
using FSH.Modules.Products.Features.v1.Commands.CreateQualityCheck;

namespace Products.Tests.Validators;

/// <summary>
/// Tests for CreateQualityCheckCommandValidator - validates quality check creation requests.
/// </summary>
[Trait("Category", "Products")]
public sealed class CreateQualityCheckCommandValidatorTests
{
    private readonly CreateQualityCheckCommandValidator _sut = new();

    #region ProductId Validation

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(9999)]
    public void ProductId_Should_Pass_When_PositiveValue(int productId)
    {
        // Arrange
        var command = new CreateQualityCheckCommand(
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
        var command = new CreateQualityCheckCommand(
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
        var command = new CreateQualityCheckCommand(
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
        var command = new CreateQualityCheckCommand(
            ProductId: 1,
            Title: "Visual Inspection",
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
        var command = new CreateQualityCheckCommand(
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
        var command = new CreateQualityCheckCommand(
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
        var command = new CreateQualityCheckCommand(
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
        var command = new CreateQualityCheckCommand(
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
        var command = new CreateQualityCheckCommand(
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
        var command = new CreateQualityCheckCommand(
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Check for visible defects on the surface",
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
        var command = new CreateQualityCheckCommand(
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
        var command = new CreateQualityCheckCommand(
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
        var command = new CreateQualityCheckCommand(
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
        var command = new CreateQualityCheckCommand(
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
        var command = new CreateQualityCheckCommand(
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
        var command = new CreateQualityCheckCommand(
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Check for visible defects",
            Department: "Quality Assurance");

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
        var command = new CreateQualityCheckCommand(
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
        var command = new CreateQualityCheckCommand(
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
        var command = new CreateQualityCheckCommand(
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
        var command = new CreateQualityCheckCommand(
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
        var command = new CreateQualityCheckCommand(
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
    [InlineData(9999)]
    public void DisplayOrder_Should_Pass_When_ZeroOrPositive(int displayOrder)
    {
        // Arrange
        var command = new CreateQualityCheckCommand(
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
        var command = new CreateQualityCheckCommand(
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
        var command = new CreateQualityCheckCommand(
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

    #region IsRequired Validation

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsRequired_Should_Pass_For_BothValues(bool isRequired)
    {
        // Arrange
        var command = new CreateQualityCheckCommand(
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Check for visible defects",
            Department: "QA",
            IsRequired: isRequired);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "IsRequired");
    }

    #endregion

    #region Combined Validation

    [Fact]
    public void Validate_Should_Pass_When_AllFieldsValid()
    {
        // Arrange
        var command = new CreateQualityCheckCommand(
            ProductId: 1,
            Title: "Visual Inspection",
            Description: "Inspect product surface for defects before packaging",
            Department: "Quality Assurance",
            IsRequired: true,
            DisplayOrder: 1);

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
        var command = new CreateQualityCheckCommand(
            ProductId: 1,
            Title: "Minimal Check",
            Description: "A minimal quality check",
            Department: "QA");

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
        var command = new CreateQualityCheckCommand(
            ProductId: 0,
            Title: "",
            Description: "",
            Department: "",
            DisplayOrder: -1);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(4);
    }

    #endregion
}
