using FSH.Modules.Products.Application.Features.v1.Queries.SearchQualityChecks;
using FSH.Modules.Products.Contracts.Application.Features.v1.Queries.SearchQualityChecks;

namespace Products.Tests.Validators;

/// <summary>
/// Tests for SearchQualityChecksQueryValidator - validates quality check search/filter requests.
/// </summary>
[Trait("Category", "Products")]
public sealed class SearchQualityChecksQueryValidatorTests
{
    private readonly SearchQualityChecksQueryValidator _sut = new();

    #region PageNumber Validation

    [Fact]
    public void PageNumber_Should_Pass_When_Null()
    {
        // Arrange
        var query = new SearchQualityChecksQuery { PageNumber = null };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "PageNumber");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(100)]
    public void PageNumber_Should_Pass_When_GreaterThanOrEqualTo1(int pageNumber)
    {
        // Arrange
        var query = new SearchQualityChecksQuery { PageNumber = pageNumber };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "PageNumber");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void PageNumber_Should_Fail_When_LessThan1(int pageNumber)
    {
        // Arrange
        var query = new SearchQualityChecksQuery { PageNumber = pageNumber };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "PageNumber");
    }

    [Fact]
    public void PageNumber_Should_Have_CorrectErrorMessage_When_Invalid()
    {
        // Arrange
        var query = new SearchQualityChecksQuery { PageNumber = 0 };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "PageNumber")
            .ShouldContain(e => e.ErrorMessage == "Page number must be at least 1.");
    }

    #endregion

    #region PageSize Validation

    [Fact]
    public void PageSize_Should_Pass_When_Null()
    {
        // Arrange
        var query = new SearchQualityChecksQuery { PageSize = null };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "PageSize");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    public void PageSize_Should_Pass_When_Between1And100(int pageSize)
    {
        // Arrange
        var query = new SearchQualityChecksQuery { PageSize = pageSize };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "PageSize");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void PageSize_Should_Fail_When_ZeroOrNegative(int pageSize)
    {
        // Arrange
        var query = new SearchQualityChecksQuery { PageSize = pageSize };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "PageSize");
    }

    [Fact]
    public void PageSize_Should_Fail_When_ExceedsMaximum()
    {
        // Arrange
        var query = new SearchQualityChecksQuery { PageSize = 101 };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "PageSize");
    }

    [Fact]
    public void PageSize_Should_Have_CorrectErrorMessage_When_Invalid()
    {
        // Arrange
        var query = new SearchQualityChecksQuery { PageSize = 0 };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "PageSize");
    }

    #endregion

    #region Optional Filters (Search, Sort, ProductId, Department, IsRequired)

    [Fact]
    public void Search_Should_Pass_When_Null()
    {
        // Arrange
        var query = new SearchQualityChecksQuery { Search = null };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Search");
    }

    [Fact]
    public void Search_Should_Pass_When_Provided()
    {
        // Arrange
        var query = new SearchQualityChecksQuery { Search = "visual inspection" };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Search");
    }

    [Fact]
    public void Sort_Should_Pass_When_Null()
    {
        // Arrange
        var query = new SearchQualityChecksQuery { Sort = null };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Sort");
    }

    [Theory]
    [InlineData("title")]
    [InlineData("-title")]
    [InlineData("department")]
    [InlineData("-department")]
    [InlineData("displayorder")]
    [InlineData("-displayorder")]
    [InlineData("createdonutc")]
    [InlineData("-createdonutc")]
    public void Sort_Should_Pass_When_ValidSortExpression(string sort)
    {
        // Arrange
        var query = new SearchQualityChecksQuery { Sort = sort };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Sort");
    }

    [Fact]
    public void ProductId_Should_Pass_When_Null()
    {
        // Arrange
        var query = new SearchQualityChecksQuery { ProductId = null };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "ProductId");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(9999)]
    public void ProductId_Should_Pass_When_PositiveValue(int productId)
    {
        // Arrange
        var query = new SearchQualityChecksQuery { ProductId = productId };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "ProductId");
    }

    [Fact]
    public void Department_Should_Pass_When_Null()
    {
        // Arrange
        var query = new SearchQualityChecksQuery { Department = null };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Department");
    }

    [Fact]
    public void Department_Should_Pass_When_Provided()
    {
        // Arrange
        var query = new SearchQualityChecksQuery { Department = "Quality Assurance" };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Department");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsRequired_Should_Pass_For_BothValues(bool isRequired)
    {
        // Arrange
        var query = new SearchQualityChecksQuery { IsRequired = isRequired };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "IsRequired");
    }

    [Fact]
    public void IsRequired_Should_Pass_When_Null()
    {
        // Arrange
        var query = new SearchQualityChecksQuery { IsRequired = null };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "IsRequired");
    }

    #endregion

    #region Combined Validation

    [Fact]
    public void Validate_Should_Pass_When_AllFieldsValid()
    {
        // Arrange
        var query = new SearchQualityChecksQuery
        {
            PageNumber = 1,
            PageSize = 25,
            Sort = "-createdonutc",
            Search = "inspection",
            ProductId = 10,
            Department = "Quality Assurance",
            IsRequired = true
        };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Validate_Should_Pass_When_NoFiltersProvided()
    {
        // Arrange
        var query = new SearchQualityChecksQuery();

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Validate_Should_Fail_When_MultipleFieldsInvalid()
    {
        // Arrange
        var query = new SearchQualityChecksQuery
        {
            PageNumber = 0,
            PageSize = 0
        };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(2);
    }

    #endregion
}
