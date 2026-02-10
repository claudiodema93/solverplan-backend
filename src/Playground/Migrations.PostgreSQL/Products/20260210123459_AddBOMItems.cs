using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FSH.Playground.Migrations.PostgreSQL.Products
{
    /// <inheritdoc />
    public partial class AddBOMItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BomItems",
                schema: "products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    ChildProductId = table.Column<int>(type: "integer", nullable: false),
                    Quantity = table.Column<double>(type: "double precision", nullable: false),
                    Unit = table.Column<int>(type: "integer", nullable: false),
                    IsManual = table.Column<bool>(type: "boolean", nullable: false),
                    Notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    TenantId = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    CreatedOnUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    LastModifiedOnUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BomItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BomItems_Products_ChildProductId",
                        column: x => x.ChildProductId,
                        principalSchema: "products",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BomItems_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "products",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QualityChecks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    Department = table.Column<string>(type: "text", nullable: false),
                    IsRequired = table.Column<bool>(type: "boolean", nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    TenantId = table.Column<string>(type: "text", nullable: false),
                    CreatedOnUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    LastModifiedOnUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    LastModifiedBy = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    DeletedOnUtc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    DeletedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QualityChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QualityChecks_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "products",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_Status",
                schema: "products",
                table: "Products",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId_CategoryId",
                schema: "products",
                table: "Products",
                columns: new[] { "TenantId", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_Products_TenantId_Status",
                schema: "products",
                table: "Products",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_Issues_Severity",
                schema: "products",
                table: "Issues",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_Status",
                schema: "products",
                table: "Issues",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_TenantId_ProductId",
                schema: "products",
                table: "Issues",
                columns: new[] { "TenantId", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_Issues_TenantId_Status",
                schema: "products",
                table: "Issues",
                columns: new[] { "TenantId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_BomItems_ChildProductId",
                schema: "products",
                table: "BomItems",
                column: "ChildProductId");

            migrationBuilder.CreateIndex(
                name: "IX_BomItems_ProductId",
                schema: "products",
                table: "BomItems",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_BomItems_TenantId",
                schema: "products",
                table: "BomItems",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_BomItems_TenantId_ChildProductId",
                schema: "products",
                table: "BomItems",
                columns: new[] { "TenantId", "ChildProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_BomItems_TenantId_ProductId",
                schema: "products",
                table: "BomItems",
                columns: new[] { "TenantId", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_QualityChecks_ProductId",
                table: "QualityChecks",
                column: "ProductId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BomItems",
                schema: "products");

            migrationBuilder.DropTable(
                name: "QualityChecks");

            migrationBuilder.DropIndex(
                name: "IX_Products_Status",
                schema: "products",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_TenantId_CategoryId",
                schema: "products",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_TenantId_Status",
                schema: "products",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Issues_Severity",
                schema: "products",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_Status",
                schema: "products",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_TenantId_ProductId",
                schema: "products",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_TenantId_Status",
                schema: "products",
                table: "Issues");
        }
    }
}
