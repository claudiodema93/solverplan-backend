namespace FSH.Modules.Products.Contracts;

public static class ProductsPermissions
{
    public const string Create = "Permissions.Products.Create";
    public const string View = "Permissions.Products.View";
    public const string Update = "Permissions.Products.Update";
    public const string Delete = "Permissions.Products.Delete";

    public static class Categories
    {
        public const string CreateCategory = "Permissions.Products.Categories.Create";
        public const string ViewCategory = "Permissions.Products.Categories.View";
        public const string UpdateCategory = "Permissions.Products.Categories.Update";
        public const string DeleteCategory = "Permissions.Products.Categories.Delete";
        public const string List = "Permissions.Products.Categories.List";
    }

    public static class Issues
    {
        public const string CreateIssue = "Permissions.Products.Issues.Create";
        public const string ViewIssue = "Permissions.Products.Issues.View";
        public const string UpdateIssue = "Permissions.Products.Issues.Update";
        public const string DeleteIssue = "Permissions.Products.Issues.Delete";
        public const string List = "Permissions.Products.Issues.List";
    }
}
