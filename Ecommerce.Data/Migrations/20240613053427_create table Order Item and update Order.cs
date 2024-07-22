using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class createtableOrderItemandupdateOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
           name: "Orders",
           columns: table => new
           {
               OrderId = table.Column<int>(type: "int", nullable: false)
                   .Annotation("SqlServer:Identity", "1, 1"),
               PurchaseDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
               OrderNumber = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
               IsPaid = table.Column<bool>(type: "bit", nullable: false),
               UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
               OrderStatusId = table.Column<int>(type: "int", nullable: false),
               ProductsProduct_Id = table.Column<int>(type: "int", nullable: false)
           },
           constraints: table =>
           {
               table.PrimaryKey("PK_Orders", x => x.OrderId);
               table.ForeignKey(
                   name: "FK_Orders_AspNetUsers_UserId",
                   column: x => x.UserId,
                   principalTable: "AspNetUsers",
                   principalColumn: "Id",
                   onDelete: ReferentialAction.Cascade);
               table.ForeignKey(
                   name: "FK_Orders_OrderStatus_OrderStatusId",
                   column: x => x.OrderStatusId,
                   principalTable: "OrderStatus",
                   principalColumn: "OrderStatus_Id",
                   onDelete: ReferentialAction.Cascade);
               table.ForeignKey(
                   name: "FK_Orders_Products_ProductsProduct_Id",
                   column: x => x.ProductsProduct_Id,
                   principalTable: "Products",
                   principalColumn: "Product_Id",
                   onDelete: ReferentialAction.Cascade);
           });

            migrationBuilder.CreateTable(
                name: "OrderItem",
                columns: table => new
                {
                    OrderItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderId = table.Column<int>(type: "int", nullable: false),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    Size = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "money", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItem", x => x.OrderItemId);
                    table.ForeignKey(
                        name: "FK_OrderItem_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "OrderId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropTable(
                name: "OrderItem");

            migrationBuilder.DropTable(
                name: "Orders");

        }
    }
}
