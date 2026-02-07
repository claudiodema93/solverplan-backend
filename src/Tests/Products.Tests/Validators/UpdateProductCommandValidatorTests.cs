using FSH.Modules.Products.Application.Features.v1.Commands.UpdateProduct;
using FSH.Modules.Products.Contracts.Application.Features.v1.Commands.UpdateProduct;
using FSH.Modules.Products.Contracts.Domain.Enums;

namespace Products.Tests.Validators;

/// <summary>
/// Tests for UpdateProductCommandValidator - validates product update requests.
/// </summary>
[Trait("Category", "Products")]
public sealed class UpdateProductCommandValidatorTests
{
    private readonly UpdateProductCommandValidator _sut = new();

    #region Id Validation

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(9999)]
    public void Id_Should_Pass_When_PositiveValue(int id)
    {
        // Arrange
        var command = new UpdateProductCommand(id, "Valid Product Title");

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
        var command = new UpdateProductCommand(id, "Valid Product Title");

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
        var command = new UpdateProductCommand(0, "Valid Product Title");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Id")
            .ShouldContain(e => e.ErrorMessage == "Product Id must be greater than 0.");
    }

    #endregion

    #region Title Validation

    [Fact]
    public void Title_Should_Pass_When_Valid()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Valid Product Title");

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
        var command = new UpdateProductCommand(1, title!);

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
        var longTitle = new string('a', 257);
        var command = new UpdateProductCommand(1, longTitle);

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
        var maxLengthTitle = new string('a', 256);
        var command = new UpdateProductCommand(1, maxLengthTitle);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Title");
    }

    [Fact]
    public void Title_Should_Have_CorrectErrorMessage_When_Empty()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "");

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
        var longTitle = new string('a', 257);
        var command = new UpdateProductCommand(1, longTitle);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Title")
            .ShouldContain(e => e.ErrorMessage == "Title must not exceed 256 characters.");
    }

    #endregion

    #region Description Validation

    [Fact]
    public void Description_Should_Pass_When_Null()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", Description: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Description_Should_Pass_When_Empty()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", Description: "");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Description_Should_Pass_When_Valid()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", Description: "This is a valid description");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Description_Should_Fail_When_ExceedsMaxLength()
    {
        // Arrange
        var longDescription = new string('a', 2001);
        var command = new UpdateProductCommand(1, "Product", Description: longDescription);

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
        var command = new UpdateProductCommand(1, "Product", Description: maxLengthDescription);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Description");
    }

    [Fact]
    public void Description_Should_Have_CorrectErrorMessage_When_TooLong()
    {
        // Arrange
        var longDescription = new string('a', 2001);
        var command = new UpdateProductCommand(1, "Product", Description: longDescription);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Description")
            .ShouldContain(e => e.ErrorMessage == "Description must not exceed 2000 characters.");
    }

    #endregion

    #region Revision Validation

    [Fact]
    public void Revision_Should_Pass_When_Zero()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", Revision: 0);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Revision");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(9999)]
    public void Revision_Should_Pass_When_PositiveValue(int revision)
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", Revision: revision);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Revision");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Revision_Should_Fail_When_NegativeValue(int revision)
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", Revision: revision);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Revision");
    }

    [Fact]
    public void Revision_Should_Have_CorrectErrorMessage_When_Negative()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", Revision: -1);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Revision")
            .ShouldContain(e => e.ErrorMessage == "Revision must be greater than or equal to 0.");
    }

    #endregion

    #region Version Validation

    [Fact]
    public void Version_Should_Pass_When_Zero()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", Version: 0);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Version");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void Version_Should_Pass_When_PositiveValue(int version)
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", Version: version);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Version");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Version_Should_Fail_When_NegativeValue(int version)
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", Version: version);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Version");
    }

    [Fact]
    public void Version_Should_Have_CorrectErrorMessage_When_Negative()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", Version: -1);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Version")
            .ShouldContain(e => e.ErrorMessage == "Version must be greater than or equal to 0.");
    }

    #endregion

    #region Variant Validation

    [Fact]
    public void Variant_Should_Pass_When_Empty()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", Variant: "");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Variant");
    }

    [Fact]
    public void Variant_Should_Pass_When_Valid()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", Variant: "Standard");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Variant");
    }

    [Fact]
    public void Variant_Should_Fail_When_ExceedsMaxLength()
    {
        // Arrange
        var longVariant = new string('a', 101);
        var command = new UpdateProductCommand(1, "Product", Variant: longVariant);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Variant");
    }

    [Fact]
    public void Variant_Should_Pass_When_ExactlyMaxLength()
    {
        // Arrange
        var maxLengthVariant = new string('a', 100);
        var command = new UpdateProductCommand(1, "Product", Variant: maxLengthVariant);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Variant");
    }

    [Fact]
    public void Variant_Should_Have_CorrectErrorMessage_When_TooLong()
    {
        // Arrange
        var longVariant = new string('a', 101);
        var command = new UpdateProductCommand(1, "Product", Variant: longVariant);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Variant")
            .ShouldContain(e => e.ErrorMessage == "Variant must not exceed 100 characters.");
    }

    #endregion

    #region CategoryId Validation

    [Fact]
    public void CategoryId_Should_Pass_When_Null()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", CategoryId: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "CategoryId");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void CategoryId_Should_Pass_When_PositiveValue(int categoryId)
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", CategoryId: categoryId);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "CategoryId");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void CategoryId_Should_Fail_When_ZeroOrNegative(int categoryId)
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", CategoryId: categoryId);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "CategoryId");
    }

    [Fact]
    public void CategoryId_Should_Have_CorrectErrorMessage_When_Invalid()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", CategoryId: 0);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "CategoryId")
            .ShouldContain(e => e.ErrorMessage == "CategoryId must be greater than 0.");
    }

    #endregion

    #region Keywords Validation

    [Fact]
    public void Keywords_Should_Pass_When_Null()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", Keywords: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Keywords");
    }

    [Fact]
    public void Keywords_Should_Pass_When_Valid()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", Keywords: "electronics, gadget, modern");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Keywords");
    }

    [Fact]
    public void Keywords_Should_Fail_When_ExceedsMaxLength()
    {
        // Arrange
        var longKeywords = new string('a', 501);
        var command = new UpdateProductCommand(1, "Product", Keywords: longKeywords);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Keywords");
    }

    [Fact]
    public void Keywords_Should_Pass_When_ExactlyMaxLength()
    {
        // Arrange
        var maxLengthKeywords = new string('a', 500);
        var command = new UpdateProductCommand(1, "Product", Keywords: maxLengthKeywords);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Keywords");
    }

    [Fact]
    public void Keywords_Should_Have_CorrectErrorMessage_When_TooLong()
    {
        // Arrange
        var longKeywords = new string('a', 501);
        var command = new UpdateProductCommand(1, "Product", Keywords: longKeywords);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Keywords")
            .ShouldContain(e => e.ErrorMessage == "Keywords must not exceed 500 characters.");
    }

    #endregion

    #region Language Validation

    [Fact]
    public void Language_Should_Pass_When_Null()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", Language: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Language");
    }

    [Fact]
    public void Language_Should_Pass_When_Valid()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", Language: "en-US");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Language");
    }

    [Fact]
    public void Language_Should_Fail_When_ExceedsMaxLength()
    {
        // Arrange
        var longLanguage = new string('a', 51);
        var command = new UpdateProductCommand(1, "Product", Language: longLanguage);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Language");
    }

    [Fact]
    public void Language_Should_Pass_When_ExactlyMaxLength()
    {
        // Arrange
        var maxLengthLanguage = new string('a', 50);
        var command = new UpdateProductCommand(1, "Product", Language: maxLengthLanguage);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Language");
    }

    [Fact]
    public void Language_Should_Have_CorrectErrorMessage_When_TooLong()
    {
        // Arrange
        var longLanguage = new string('a', 51);
        var command = new UpdateProductCommand(1, "Product", Language: longLanguage);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Language")
            .ShouldContain(e => e.ErrorMessage == "Language must not exceed 50 characters.");
    }

    #endregion

    #region Subject Validation

    [Fact]
    public void Subject_Should_Pass_When_Null()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", Subject: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Subject");
    }

    [Fact]
    public void Subject_Should_Pass_When_Valid()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", Subject: "Electronics Manufacturing");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Subject");
    }

    [Fact]
    public void Subject_Should_Fail_When_ExceedsMaxLength()
    {
        // Arrange
        var longSubject = new string('a', 201);
        var command = new UpdateProductCommand(1, "Product", Subject: longSubject);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Subject");
    }

    [Fact]
    public void Subject_Should_Pass_When_ExactlyMaxLength()
    {
        // Arrange
        var maxLengthSubject = new string('a', 200);
        var command = new UpdateProductCommand(1, "Product", Subject: maxLengthSubject);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Subject");
    }

    [Fact]
    public void Subject_Should_Have_CorrectErrorMessage_When_TooLong()
    {
        // Arrange
        var longSubject = new string('a', 201);
        var command = new UpdateProductCommand(1, "Product", Subject: longSubject);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Subject")
            .ShouldContain(e => e.ErrorMessage == "Subject must not exceed 200 characters.");
    }

    #endregion

    #region Notes Validation

    [Fact]
    public void Notes_Should_Pass_When_Null()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", Notes: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Notes");
    }

    [Fact]
    public void Notes_Should_Pass_When_Valid()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", Notes: "Important notes about the product");

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
        var command = new UpdateProductCommand(1, "Product", Notes: longNotes);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Notes");
    }

    [Fact]
    public void Notes_Should_Pass_When_ExactlyMaxLength()
    {
        // Arrange
        var maxLengthNotes = new string('a', 2000);
        var command = new UpdateProductCommand(1, "Product", Notes: maxLengthNotes);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Notes");
    }

    [Fact]
    public void Notes_Should_Have_CorrectErrorMessage_When_TooLong()
    {
        // Arrange
        var longNotes = new string('a', 2001);
        var command = new UpdateProductCommand(1, "Product", Notes: longNotes);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Notes")
            .ShouldContain(e => e.ErrorMessage == "Notes must not exceed 2000 characters.");
    }

    #endregion

    #region HashSha256 Validation

    [Fact]
    public void HashSha256_Should_Pass_When_Null()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", HashSha256: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "HashSha256");
    }

    [Fact]
    public void HashSha256_Should_Pass_When_ValidHash()
    {
        // Arrange
        var validHash = "a".PadRight(64, 'f');
        var command = new UpdateProductCommand(1, "Product", HashSha256: validHash);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "HashSha256");
    }

    [Theory]
    [InlineData("abc")]                                    // Too short
    [InlineData("abcdefghijklmnopqrstuvwxyz12345")]        // Too short
    [InlineData("g123456789012345678901234567890123456789012345678901234567890123")] // Invalid character 'g'
    [InlineData("ABCDEF123456789012345678901234567890123456789012345678901234567890123")] // Too long
    public void HashSha256_Should_Fail_When_InvalidFormat(string invalidHash)
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", HashSha256: invalidHash);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "HashSha256");
    }

    [Fact]
    public void HashSha256_Should_Have_CorrectErrorMessage_When_Invalid()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", HashSha256: "invalid");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "HashSha256")
            .ShouldContain(e => e.ErrorMessage == "HashSha256 must be a valid SHA-256 hash (64 hexadecimal characters).");
    }

    #endregion

    #region CustomerId Validation

    [Fact]
    public void CustomerId_Should_Pass_When_Null()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", CustomerId: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "CustomerId");
    }

    [Fact]
    public void CustomerId_Should_Pass_When_Valid()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", CustomerId: "CUST-12345");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "CustomerId");
    }

    [Fact]
    public void CustomerId_Should_Pass_When_Empty()
    {
        // Arrange
        // Note: Due to validator's When() condition, empty string bypasses NotEmpty validation
        var command = new UpdateProductCommand(1, "Product", CustomerId: "");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "CustomerId");
    }

    [Fact]
    public void CustomerId_Should_Fail_When_Whitespace()
    {
        // Arrange
        // Note: Whitespace triggers the When() condition since !string.IsNullOrEmpty(" ") is true
        var command = new UpdateProductCommand(1, "Product", CustomerId: " ");

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "CustomerId");
    }

    #endregion

    #region Mirroring Validation

    [Fact]
    public void Mirroring_Should_Pass_When_Null()
    {
        // Arrange
        var command = new UpdateProductCommand(1, "Product", Mirroring: null);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Mirroring");
    }

    [Fact]
    public void Mirroring_Should_Pass_When_NotMirrored()
    {
        // Arrange
        var mirroring = new MirroringInfoDto(IsMirrored: false);
        var command = new UpdateProductCommand(1, "Product", Mirroring: mirroring);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Mirroring");
    }

    [Fact]
    public void Mirroring_Should_Pass_When_MirroredWithSourceTitle()
    {
        // Arrange
        var mirroring = new MirroringInfoDto(IsMirrored: true, SourceProductTitle: "Source Product");
        var command = new UpdateProductCommand(1, "Product", Mirroring: mirroring);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Mirroring");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    public void Mirroring_Should_Fail_When_MirroredWithoutSourceTitle(string? sourceTitle)
    {
        // Arrange
        var mirroring = new MirroringInfoDto(IsMirrored: true, SourceProductTitle: sourceTitle);
        var command = new UpdateProductCommand(1, "Product", Mirroring: mirroring);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "Mirroring");
    }

    [Fact]
    public void Mirroring_Should_Have_CorrectErrorMessage_When_SourceTitleMissing()
    {
        // Arrange
        var mirroring = new MirroringInfoDto(IsMirrored: true, SourceProductTitle: null);
        var command = new UpdateProductCommand(1, "Product", Mirroring: mirroring);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "Mirroring")
            .ShouldContain(e => e.ErrorMessage == "SourceProductTitle is required when IsMirrored is true.");
    }

    #endregion

    #region Combined Validation

    [Fact]
    public void Validate_Should_Pass_When_AllFieldsValid()
    {
        // Arrange
        var command = new UpdateProductCommand(
            Id: 1,
            Title: "Test Product",
            Revision: 1,
            Description: "A test product",
            Status: ProductState.Active,
            Private: false,
            CustomerId: "CUST-001",
            Variant: "Standard",
            Version: 1,
            CategoryId: 10,
            Keywords: "test, product",
            Language: "en-US",
            Subject: "Testing",
            Notes: "Test notes",
            HashSha256: "a".PadRight(64, 'f'),
            IsLatest: true,
            Characteristics: new ProductCharacteristicsDto(IsAssembly: true, IsManufacturable: true),
            Mirroring: new MirroringInfoDto(IsMirrored: false));

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
        var command = new UpdateProductCommand(1, "Minimal Product");

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
        var command = new UpdateProductCommand(
            Id: 0,
            Title: "",
            Revision: -1,
            Version: -1,
            CategoryId: 0);

        // Act
        var result = _sut.Validate(command);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(5);
    }

    #endregion
}
