using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatetableorderandorderitem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Products_ProductsProduct_Id",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ProductsProduct_Id",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ProductsProduct_Id",
                table: "Orders");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PurchaseDate",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldDefaultValueSql: "GETDATE()");

            migrationBuilder.CreateTable(
                name: "ApplicationUserProduct",
                columns: table => new
                {
                    ProductsProduct_Id = table.Column<int>(type: "int", nullable: false),
                    UsersId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUserProduct", x => new { x.ProductsProduct_Id, x.UsersId });
                    table.ForeignKey(
                        name: "FK_ApplicationUserProduct_AspNetUsers_UsersId",
                        column: x => x.UsersId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUserProduct_Products_ProductsProduct_Id",
                        column: x => x.ProductsProduct_Id,
                        principalTable: "Products",
                        principalColumn: "Product_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUserProduct_UsersId",
                table: "ApplicationUserProduct",
                column: "UsersId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApplicationUserProduct");

            migrationBuilder.AlterColumn<DateTime>(
                name: "PurchaseDate",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETDATE()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "ProductsProduct_Id",
                table: "Orders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ProductsProduct_Id",
                table: "Orders",
                column: "ProductsProduct_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Products_ProductsProduct_Id",
                table: "Orders",
                column: "ProductsProduct_Id",
                principalTable: "Products",
                principalColumn: "Product_Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
