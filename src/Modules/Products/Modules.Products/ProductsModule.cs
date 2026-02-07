using Asp.Versioning;
using FSH.Framework.Persistence;
using FSH.Framework.Web.Modules;
using FSH.Modules.Products.Endpoints.v1;
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

        var categoriesGroup = endpoints.MapGroup("api/v{version:apiVersion}/categories")
            .WithTags("Categories")
            .WithApiVersionSet(versionSet);

        CreateCategoryEndpoint.Map(categoriesGroup);
        GetCategoryByIdEndpoint.Map(categoriesGroup);
        SearchCategoriesEndpoint.Map(categoriesGroup);
        UpdateCategoryEndpoint.Map(categoriesGroup);
        DeleteCategoryEndpoint.Map(categoriesGroup);

        var issuesGroup = endpoints.MapGroup("api/v{version:apiVersion}/issues")
            .WithTags("Issues")
            .WithApiVersionSet(versionSet);

        CreateIssueEndpoint.Map(issuesGroup);
        GetIssueByIdEndpoint.Map(issuesGroup);
        SearchIssuesEndpoint.Map(issuesGroup);
    }
}
