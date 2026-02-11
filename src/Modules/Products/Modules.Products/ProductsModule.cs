using Asp.Versioning;
using FSH.Framework.Persistence;
using FSH.Framework.Web.Modules;
using FSH.Modules.Products.Features.v1.Commands.CreateAccountingCode;
using FSH.Modules.Products.Features.v1.Commands.CreateBomItem;
using FSH.Modules.Products.Features.v1.Commands.CreateCategory;
using FSH.Modules.Products.Features.v1.Commands.CreateIssue;
using FSH.Modules.Products.Features.v1.Commands.CreateProduct;
using FSH.Modules.Products.Features.v1.Commands.CreateQualityCheck;
using FSH.Modules.Products.Features.v1.Commands.DeleteAccountingCode;
using FSH.Modules.Products.Features.v1.Commands.DeleteBomItem;
using FSH.Modules.Products.Features.v1.Commands.DeleteCategory;
using FSH.Modules.Products.Features.v1.Commands.DeleteIssue;
using FSH.Modules.Products.Features.v1.Commands.DeleteProduct;
using FSH.Modules.Products.Features.v1.Commands.DeleteQualityCheck;
using FSH.Modules.Products.Features.v1.Commands.UpdateAccountingCode;
using FSH.Modules.Products.Features.v1.Commands.UpdateBomItem;
using FSH.Modules.Products.Features.v1.Commands.UpdateCategory;
using FSH.Modules.Products.Features.v1.Commands.UpdateIssue;
using FSH.Modules.Products.Features.v1.Commands.UpdateProduct;
using FSH.Modules.Products.Features.v1.Commands.UpdateQualityCheck;
using FSH.Modules.Products.Features.v1.Queries.GetAccountingCodeById;
using FSH.Modules.Products.Features.v1.Queries.GetBomItemById;
using FSH.Modules.Products.Features.v1.Queries.GetCategoryById;
using FSH.Modules.Products.Features.v1.Queries.GetIssueById;
using FSH.Modules.Products.Features.v1.Queries.GetProductById;
using FSH.Modules.Products.Features.v1.Queries.GetQualityCheckById;
using FSH.Modules.Products.Features.v1.Queries.SearchAccountingCodes;
using FSH.Modules.Products.Features.v1.Queries.SearchBomItems;
using FSH.Modules.Products.Features.v1.Queries.SearchCategories;
using FSH.Modules.Products.Features.v1.Queries.SearchIssues;
using FSH.Modules.Products.Features.v1.Queries.SearchProducts;
using FSH.Modules.Products.Features.v1.Queries.SearchQualityChecks;
using FSH.Modules.Products.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace FSH.Modules.Products;

public sealed class ProductsModule : IModule
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.AddHeroDbContext<ProductsDbContext>();

        builder.Services.AddHealthChecks()
            .AddDbContextCheck<ProductsDbContext>(
                name: "db:products",
                failureStatus: HealthStatus.Unhealthy);
    }

    public void MapEndpoints(IEndpointRouteBuilder endpoints)
    {
        var versionSet = endpoints.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(1))
            .ReportApiVersions()
            .Build();

        var group = endpoints.MapGroup("api/v{version:apiVersion}/products")
            .WithTags("Products")
            .WithApiVersionSet(versionSet);

        CreateProductEndpoint.Map(group);
        GetProductByIdEndpoint.Map(group);
        SearchProductsEndpoint.Map(group);
        UpdateProductEndpoint.Map(group);
        DeleteProductEndpoint.Map(group);

        var categoriesGroup = endpoints.MapGroup("api/v{version:apiVersion}/products/categories")
            .WithTags("Categories")
            .WithApiVersionSet(versionSet);

        CreateCategoryEndpoint.Map(categoriesGroup);
        GetCategoryByIdEndpoint.Map(categoriesGroup);
        SearchCategoriesEndpoint.Map(categoriesGroup);
        UpdateCategoryEndpoint.Map(categoriesGroup);
        DeleteCategoryEndpoint.Map(categoriesGroup);

        var issuesGroup = endpoints.MapGroup("api/v{version:apiVersion}/products/issues")
            .WithTags("Issues")
            .WithApiVersionSet(versionSet);

        CreateIssueEndpoint.Map(issuesGroup);
        GetIssueByIdEndpoint.Map(issuesGroup);
        SearchIssuesEndpoint.Map(issuesGroup);
        UpdateIssueEndpoint.Map(issuesGroup);
        DeleteIssueEndpoint.Map(issuesGroup);

        var qualityChecksGroup = endpoints.MapGroup("api/v{version:apiVersion}/products/qualitychecks")
            .WithTags("QualityChecks")
            .WithApiVersionSet(versionSet);

        CreateQualityCheckEndpoint.Map(qualityChecksGroup);
        GetQualityCheckByIdEndpoint.Map(qualityChecksGroup);
        SearchQualityChecksEndpoint.Map(qualityChecksGroup);
        UpdateQualityCheckEndpoint.Map(qualityChecksGroup);
        DeleteQualityCheckEndpoint.Map(qualityChecksGroup);

        var bomItemsGroup = endpoints.MapGroup("api/v{version:apiVersion}/products/bomitems")
            .WithTags("BomItems")
            .WithApiVersionSet(versionSet);

        CreateBomItemEndpoint.Map(bomItemsGroup);
        GetBomItemByIdEndpoint.Map(bomItemsGroup);
        SearchBomItemsEndpoint.Map(bomItemsGroup);
        UpdateBomItemEndpoint.Map(bomItemsGroup);
        DeleteBomItemEndpoint.Map(bomItemsGroup);

        var accountingCodesGroup = endpoints.MapGroup("api/v{version:apiVersion}/products/accountingcodes")
            .WithTags("AccountingCodes")
            .WithApiVersionSet(versionSet);

        CreateAccountingCodeEndpoint.Map(accountingCodesGroup);
        GetAccountingCodeByIdEndpoint.Map(accountingCodesGroup);
        SearchAccountingCodesEndpoint.Map(accountingCodesGroup);
        UpdateAccountingCodeEndpoint.Map(accountingCodesGroup);
        DeleteAccountingCodeEndpoint.Map(accountingCodesGroup);
    }
}
