using FSH.Modules.Products.Contracts.Features.v1.Commands.CreateIssue;
using FSH.Modules.Products.Contracts.Domain.Enums;
using FSH.Modules.Products.Features.v1.Commands.CreateIssue;

namespace Products.Tests.Validators;

/// <summary>
/// Tests for CreateIssueCommandValidator - validates issue creation requests.
/// </summary>
[Trait("Category", "Products")]
public sealed class CreateIssueCommandValidatorTests
{
    private readonly CreateIssueCommandValidator _sut = new();

    #region ProductId Validation

    [Fact]
    public void ProductId_Should_Pass_When_Valid()
    {
        // Arrange
        var command = new CreateIssueCommand(
            ProductId: 1,
            Title: "Valid Issue",
            Description: "Valid description",
            Severity: IssueSeverity.Medium,
            Status: IssueState.Open);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "ProductId");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void ProductId_Should_Fail_When_NotGreaterThanZero(int productId)
    {
        // Arrange
        var command = new CreateIssueCommand(
            ProductId: productId,
            Title: "Valid Issue",
            Description: "Valid description",
            Severity: null,
            Status: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "ProductId");
        result.Errors.ShouldContain(e => e.ErrorMessage == "ProductId is required.");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999999)]
    public void ProductId_Should_Pass_When_GreaterThanZero(int productId)
    {
        // Arrange
        var command = new CreateIssueCommand(
            ProductId: productId,
            Title: "Valid Issue",
            Description: "Valid description",
            Severity: null,
            Status: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "ProductId");
    }

    #endregion

    #region Title Validation

    [Fact]
    public void Title_Should_Pass_When_Valid()
    {
        // Arrange
        var command = new CreateIssueCommand(
            ProductId: 1,
            Title: "Valid Issue Title",
            Description: "Valid description",
            Severity: null,
            Status: null);

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
        var command = new CreateIssueCommand(
            ProductId: 1,
            Title: title!,
            Description: "Valid description",
            Severity: null,
            Status: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Title");
        result.Errors.ShouldContain(e => e.ErrorMessage == "Title is required.");
    }

    [Fact]
    public void Title_Should_Fail_When_ExceedsMaxLength()
    {
        // Arrange
        var title = new string('A', 257); // 257 characters (max is 256)
        var command = new CreateIssueCommand(
            ProductId: 1,
            Title: title,
            Description: "Valid description",
            Severity: null,
            Status: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Title");
        result.Errors.ShouldContain(e => e.ErrorMessage == "Title must not exceed 256 characters.");
    }

    [Fact]
    public void Title_Should_Pass_When_ExactlyMaxLength()
    {
        // Arrange
        var title = new string('A', 256); // Exactly 256 characters
        var command = new CreateIssueCommand(
            ProductId: 1,
            Title: title,
            Description: "Valid description",
            Severity: null,
            Status: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Title");
    }

    [Theory]
    [InlineData("A")]
    [InlineData("Issue Title")]
    [InlineData("Short")]
    public void Title_Should_Pass_When_ValidLength(string title)
    {
        // Arrange
        var command = new CreateIssueCommand(
            ProductId: 1,
            Title: title,
            Description: "Valid description",
            Severity: null,
            Status: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Title");
    }

    #endregion

    #region Description Validation

    [Fact]
    public void Description_Should_Pass_When_Valid()
    {
        // Arrange
        var command = new CreateIssueCommand(
            ProductId: 1,
            Title: "Valid Issue",
            Description: "This is a valid description for the issue.",
            Severity: null,
            Status: null);

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
        var command = new CreateIssueCommand(
            ProductId: 1,
            Title: "Valid Issue",
            Description: description!,
            Severity: null,
            Status: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Description");
        result.Errors.ShouldContain(e => e.ErrorMessage == "Description is required.");
    }

    [Fact]
    public void Description_Should_Fail_When_ExceedsMaxLength()
    {
        // Arrange
        var description = new string('A', 4001); // 4001 characters (max is 4000)
        var command = new CreateIssueCommand(
            ProductId: 1,
            Title: "Valid Issue",
            Description: description,
            Severity: null,
            Status: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Description");
        result.Errors.ShouldContain(e => e.ErrorMessage == "Description must not exceed 4000 characters.");
    }

    [Fact]
    public void Description_Should_Pass_When_ExactlyMaxLength()
    {
        // Arrange
        var description = new string('A', 4000); // Exactly 4000 characters
        var command = new CreateIssueCommand(
            ProductId: 1,
            Title: "Valid Issue",
            Description: description,
            Severity: null,
            Status: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Description");
    }

    [Theory]
    [InlineData("Short")]
    [InlineData("This is a longer description with more details about the issue.")]
    public void Description_Should_Pass_When_ValidLength(string description)
    {
        // Arrange
        var command = new CreateIssueCommand(
            ProductId: 1,
            Title: "Valid Issue",
            Description: description,
            Severity: null,
            Status: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Description");
    }

    #endregion

    #region Severity Validation

    [Theory]
    [InlineData(IssueSeverity.Low)]
    [InlineData(IssueSeverity.Medium)]
    [InlineData(IssueSeverity.High)]
    [InlineData(IssueSeverity.Critical)]
    public void Severity_Should_Pass_When_ValidEnum(IssueSeverity severity)
    {
        // Arrange
        var command = new CreateIssueCommand(
            ProductId: 1,
            Title: "Valid Issue",
            Description: "Valid description",
            Severity: severity,
            Status: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Severity");
    }

    [Fact]
    public void Severity_Should_Pass_When_Null()
    {
        // Arrange
        var command = new CreateIssueCommand(
            ProductId: 1,
            Title: "Valid Issue",
            Description: "Valid description",
            Severity: null,
            Status: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Severity");
    }

    [Fact]
    public void Severity_Should_Fail_When_InvalidEnum()
    {
        // Arrange
        var command = new CreateIssueCommand(
            ProductId: 1,
            Title: "Valid Issue",
            Description: "Valid description",
            Severity: (IssueSeverity)999, // Invalid enum value
            Status: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Severity");
        result.Errors.ShouldContain(e => e.ErrorMessage == "Severity must be a valid enum value.");
    }

    #endregion

    #region Status Validation

    [Theory]
    [InlineData(IssueState.Open)]
    [InlineData(IssueState.Close)]
    [InlineData(IssueState.Cancelled)]
    [InlineData(IssueState.Resolved)]
    public void Status_Should_Pass_When_ValidEnum(IssueState status)
    {
        // Arrange
        var command = new CreateIssueCommand(
            ProductId: 1,
            Title: "Valid Issue",
            Description: "Valid description",
            Severity: null,
            Status: status);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Status");
    }

    [Fact]
    public void Status_Should_Pass_When_Null()
    {
        // Arrange
        var command = new CreateIssueCommand(
            ProductId: 1,
            Title: "Valid Issue",
            Description: "Valid description",
            Severity: null,
            Status: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Status");
    }

    [Fact]
    public void Status_Should_Fail_When_InvalidEnum()
    {
        // Arrange
        var command = new CreateIssueCommand(
            ProductId: 1,
            Title: "Valid Issue",
            Description: "Valid description",
            Severity: null,
            Status: (IssueState)999); // Invalid enum value

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Status");
        result.Errors.ShouldContain(e => e.ErrorMessage == "Status must be a valid enum value.");
    }

    #endregion

    #region Overall Validation

    [Fact]
    public void Should_Pass_When_AllFieldsAreValid()
    {
        // Arrange
        var command = new CreateIssueCommand(
            ProductId: 1,
            Title: "Critical bug in product rendering",
            Description: "The product is not rendering correctly on the screen when viewed from mobile devices.",
            Severity: IssueSeverity.Critical,
            Status: IssueState.Open);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Should_Pass_When_OptionalFieldsAreNull()
    {
        // Arrange
        var command = new CreateIssueCommand(
            ProductId: 1,
            Title: "Issue without severity or status",
            Description: "A valid description",
            Severity: null,
            Status: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Should_Fail_When_MultipleFieldsAreInvalid()
    {
        // Arrange
        var command = new CreateIssueCommand(
            ProductId: 0,
            Title: "",
            Description: "",
            Severity: (IssueSeverity)999,
            Status: (IssueState)888);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThan(0);
        result.Errors.ShouldContain(e => e.PropertyName == "ProductId");
        result.Errors.ShouldContain(e => e.PropertyName == "Title");
        result.Errors.ShouldContain(e => e.PropertyName == "Description");
        result.Errors.ShouldContain(e => e.PropertyName == "Severity");
        result.Errors.ShouldContain(e => e.PropertyName == "Status");
    }

    [Fact]
    public void Should_Pass_With_OnlySeverityProvided()
    {
        // Arrange
        var command = new CreateIssueCommand(
            ProductId: 1,
            Title: "Issue with severity",
            Description: "Valid description",
            Severity: IssueSeverity.High,
            Status: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    [Fact]
    public void Should_Pass_With_OnlyStatusProvided()
    {
        // Arrange
        var command = new CreateIssueCommand(
            ProductId: 1,
            Title: "Issue with status",
            Description: "Valid description",
            Severity: null,
            Status: IssueState.Resolved);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeTrue();
    }

    #endregion
}
