using FSH.Modules.Products.Contracts.Features.v1.Queries.SearchAccountingCodes;
using FSH.Modules.Products.Features.v1.Queries.SearchAccountingCodes;

namespace Products.Tests.Validators;

/// <summary>
/// Tests for SearchAccountingCodesQueryValidator - validates accounting code search requests.
/// </summary>
[Trait("Category", "Products")]
public sealed class SearchAccountingCodesQueryValidatorTests
{
    private readonly SearchAccountingCodesQueryValidator _sut = new();

    #region PageNumber Validation

    [Theory]
    [InlineData(1)]
    [InlineData(5)]
    [InlineData(100)]
    public void PageNumber_Should_Pass_When_PositiveValue(int pageNumber)
    {
        // Arrange
        var query = new SearchAccountingCodesQuery { PageNumber = pageNumber };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "PageNumber");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void PageNumber_Should_Fail_When_ZeroOrNegative(int pageNumber)
    {
        // Arrange
        var query = new SearchAccountingCodesQuery { PageNumber = pageNumber };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "PageNumber");
    }

    [Fact]
    public void PageNumber_Should_Pass_When_Null()
    {
        // Arrange
        var query = new SearchAccountingCodesQuery { PageNumber = null };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "PageNumber");
    }

    #endregion

    #region PageSize Validation

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(200)]
    public void PageSize_Should_Pass_When_ValidValue(int pageSize)
    {
        // Arrange
        var query = new SearchAccountingCodesQuery { PageSize = pageSize };

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
        var query = new SearchAccountingCodesQuery { PageSize = pageSize };

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
        var query = new SearchAccountingCodesQuery { PageSize = 201 };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "PageSize");
    }

    [Fact]
    public void PageSize_Should_Pass_When_Null()
    {
        // Arrange
        var query = new SearchAccountingCodesQuery { PageSize = null };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "PageSize");
    }

    #endregion

    #region ProductId Validation

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void ProductId_Should_Pass_When_PositiveValue(int productId)
    {
        // Arrange
        var query = new SearchAccountingCodesQuery { ProductId = productId };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "ProductId");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ProductId_Should_Fail_When_ZeroOrNegative(int productId)
    {
        // Arrange
        var query = new SearchAccountingCodesQuery { ProductId = productId };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "ProductId");
    }

    [Fact]
    public void ProductId_Should_Pass_When_Null()
    {
        // Arrange
        var query = new SearchAccountingCodesQuery { ProductId = null };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "ProductId");
    }

    #endregion

    #region Combined Validation

    [Fact]
    public void Validate_Should_Pass_When_AllFieldsValid()
    {
        // Arrange
        var query = new SearchAccountingCodesQuery
        {
            PageNumber = 1,
            PageSize = 10,
            ProductId = 5,
            Search = "MAT",
            Sort = "-CreatedOnUtc"
        };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    [Fact]
    public void Validate_Should_Pass_When_AllNullableFieldsAreNull()
    {
        // Arrange
        var query = new SearchAccountingCodesQuery();

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.IsValid.ShouldBeTrue();
        result.Errors.ShouldBeEmpty();
    }

    #endregion
}
