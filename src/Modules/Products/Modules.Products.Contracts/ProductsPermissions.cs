namespace FSH.Modules.Products.Contracts;

public static class ProductsPermissions
{
    public const string Create = "Permissions.Products.Create";
    public const string View = "Permissions.Products.View";
    public const string Update = "Permissions.Products.Update";
    public const string Delete = "Permissions.Products.Delete";

    public static class Categories
    {
        public const string Create = "Permissions.Products.Categories.Create";
        public const string View = "Permissions.Products.Categories.View";
        public const string Update = "Permissions.Products.Categories.Update";
        public const string Delete = "Permissions.Products.Categories.Delete";
        public const string List = "Permissions.Products.Categories.List";
    }
}
