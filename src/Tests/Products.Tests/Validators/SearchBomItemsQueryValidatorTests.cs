using FSH.Modules.Products.Application.Features.v1.Queries.SearchBomItems;
using FSH.Modules.Products.Contracts.Application.Features.v1.Queries.SearchBomItems;
using FSH.Modules.Products.Contracts.Domain.Enums;

namespace Products.Tests.Validators;

/// <summary>
/// Tests for SearchBomItemsQueryValidator - validates BOM item search/filter requests.
/// </summary>
[Trait("Category", "Products")]
public sealed class SearchBomItemsQueryValidatorTests
{
    private readonly SearchBomItemsQueryValidator _sut = new();

    #region PageNumber Validation

    [Fact]
    public void PageNumber_Should_Pass_When_Null()
    {
        // Arrange
        var query = new SearchBomItemsQuery { PageNumber = null };

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
        var query = new SearchBomItemsQuery { PageNumber = pageNumber };

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
        var query = new SearchBomItemsQuery { PageNumber = pageNumber };

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
        var query = new SearchBomItemsQuery { PageNumber = 0 };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "PageNumber")
            .ShouldContain(e => e.ErrorMessage == "PageNumber must be greater than or equal to 1.");
    }

    #endregion

    #region PageSize Validation

    [Fact]
    public void PageSize_Should_Pass_When_Null()
    {
        // Arrange
        var query = new SearchBomItemsQuery { PageSize = null };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "PageSize");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(200)]
    public void PageSize_Should_Pass_When_Between1And200(int pageSize)
    {
        // Arrange
        var query = new SearchBomItemsQuery { PageSize = pageSize };

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
        var query = new SearchBomItemsQuery { PageSize = pageSize };

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
        var query = new SearchBomItemsQuery { PageSize = 201 };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "PageSize");
    }

    [Fact]
    public void PageSize_Should_Have_CorrectErrorMessage_When_ZeroOrNegative()
    {
        // Arrange
        var query = new SearchBomItemsQuery { PageSize = 0 };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "PageSize")
            .ShouldContain(e => e.ErrorMessage == "PageSize must be greater than 0.");
    }

    [Fact]
    public void PageSize_Should_Have_CorrectErrorMessage_When_ExceedsMaximum()
    {
        // Arrange
        var query = new SearchBomItemsQuery { PageSize = 201 };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "PageSize")
            .ShouldContain(e => e.ErrorMessage == "PageSize must not exceed 200.");
    }

    #endregion

    #region ProductId Validation

    [Fact]
    public void ProductId_Should_Pass_When_Null()
    {
        // Arrange
        var query = new SearchBomItemsQuery { ProductId = null };

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
        var query = new SearchBomItemsQuery { ProductId = productId };

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
        var query = new SearchBomItemsQuery { ProductId = productId };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "ProductId");
    }

    [Fact]
    public void ProductId_Should_Have_CorrectErrorMessage_When_Invalid()
    {
        // Arrange
        var query = new SearchBomItemsQuery { ProductId = 0 };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "ProductId")
            .ShouldContain(e => e.ErrorMessage == "ProductId must be greater than 0.");
    }

    #endregion

    #region ChildProductId Validation

    [Fact]
    public void ChildProductId_Should_Pass_When_Null()
    {
        // Arrange
        var query = new SearchBomItemsQuery { ChildProductId = null };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "ChildProductId");
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(9999)]
    public void ChildProductId_Should_Pass_When_PositiveValue(int childProductId)
    {
        // Arrange
        var query = new SearchBomItemsQuery { ChildProductId = childProductId };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "ChildProductId");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void ChildProductId_Should_Fail_When_ZeroOrNegative(int childProductId)
    {
        // Arrange
        var query = new SearchBomItemsQuery { ChildProductId = childProductId };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.ShouldContain(e => e.PropertyName == "ChildProductId");
    }

    [Fact]
    public void ChildProductId_Should_Have_CorrectErrorMessage_When_Invalid()
    {
        // Arrange
        var query = new SearchBomItemsQuery { ChildProductId = 0 };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors
            .Where(e => e.PropertyName == "ChildProductId")
            .ShouldContain(e => e.ErrorMessage == "ChildProductId must be greater than 0.");
    }

    #endregion

    #region Optional Filters (Search, Sort, Unit, IsManual)

    [Fact]
    public void Search_Should_Pass_When_Null()
    {
        // Arrange
        var query = new SearchBomItemsQuery { Search = null };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Search");
    }

    [Fact]
    public void Search_Should_Pass_When_Provided()
    {
        // Arrange
        var query = new SearchBomItemsQuery { Search = "assembly" };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Search");
    }

    [Fact]
    public void Sort_Should_Pass_When_Null()
    {
        // Arrange
        var query = new SearchBomItemsQuery { Sort = null };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Sort");
    }

    [Theory]
    [InlineData("quantity")]
    [InlineData("-quantity")]
    [InlineData("createdonutc")]
    [InlineData("-createdonutc")]
    public void Sort_Should_Pass_When_ValidSortExpression(string sort)
    {
        // Arrange
        var query = new SearchBomItemsQuery { Sort = sort };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Sort");
    }

    [Theory]
    [InlineData(UnitOfMeasurement.Pcs)]
    [InlineData(UnitOfMeasurement.Inches)]
    [InlineData(UnitOfMeasurement.Millimeters)]
    [InlineData(UnitOfMeasurement.Liters)]
    public void Unit_Should_Pass_When_ValidEnumValue(UnitOfMeasurement unit)
    {
        // Arrange
        var query = new SearchBomItemsQuery { Unit = unit };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Unit");
    }

    [Fact]
    public void Unit_Should_Pass_When_Null()
    {
        // Arrange
        var query = new SearchBomItemsQuery { Unit = null };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "Unit");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void IsManual_Should_Pass_For_BothValues(bool isManual)
    {
        // Arrange
        var query = new SearchBomItemsQuery { IsManual = isManual };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "IsManual");
    }

    [Fact]
    public void IsManual_Should_Pass_When_Null()
    {
        // Arrange
        var query = new SearchBomItemsQuery { IsManual = null };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.Errors.ShouldNotContain(e => e.PropertyName == "IsManual");
    }

    #endregion

    #region Combined Validation

    [Fact]
    public void Validate_Should_Pass_When_AllFieldsValid()
    {
        // Arrange
        var query = new SearchBomItemsQuery
        {
            PageNumber = 1,
            PageSize = 25,
            Sort = "-createdonutc",
            Search = "assembly",
            ProductId = 10,
            ChildProductId = 20,
            Unit = UnitOfMeasurement.Pcs,
            IsManual = true
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
        var query = new SearchBomItemsQuery();

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
        var query = new SearchBomItemsQuery
        {
            PageNumber = 0,
            PageSize = 0,
            ProductId = -1,
            ChildProductId = -1
        };

        // Act
        var result = _sut.Validate(query);

        // Assert
        result.IsValid.ShouldBeFalse();
        result.Errors.Count.ShouldBeGreaterThanOrEqualTo(4);
    }

    #endregion
}
