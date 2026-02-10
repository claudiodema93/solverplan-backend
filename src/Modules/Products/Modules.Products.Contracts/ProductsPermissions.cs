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

    public static class QualityChecks
    {
        public const string CreateQualityCheck = "Permissions.Products.QualityChecks.Create";
        public const string ViewQualityCheck = "Permissions.Products.QualityChecks.View";
        public const string UpdateQualityCheck = "Permissions.Products.QualityChecks.Update";
        public const string DeleteQualityCheck = "Permissions.Products.QualityChecks.Delete";
        public const string List = "Permissions.Products.QualityChecks.List";
    }

    public static class BomItems
    {
        public const string CreateBomItem = "Permissions.Products.BomItems.Create";
        public const string ViewBomItem = "Permissions.Products.BomItems.View";
        public const string UpdateBomItem = "Permissions.Products.BomItems.Update";
        public const string DeleteBomItem = "Permissions.Products.BomItems.Delete";
        public const string List = "Permissions.Products.BomItems.List";
    }

    public static class AccountingCodes
    {
        public const string CreateAccountingCode = "Permissions.Products.AccountingCodes.Create";
        public const string ViewAccountingCode = "Permissions.Products.AccountingCodes.View";
        public const string UpdateAccountingCode = "Permissions.Products.AccountingCodes.Update";
        public const string DeleteAccountingCode = "Permissions.Products.AccountingCodes.Delete";
        public const string List = "Permissions.Products.AccountingCodes.List";
    }
}
