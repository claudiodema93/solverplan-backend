using FSH.Framework.Web.Modules;

namespace FSH.Modules.Products;

public sealed class ProductsModuleConstants : IModuleConstants
{
    public string ModuleId => "Products";
    public string ModuleName => "Products";
    public string ApiPrefix => "products";
    public const string SchemaName = "products";
}
