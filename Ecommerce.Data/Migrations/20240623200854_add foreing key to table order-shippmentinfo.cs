using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class addforeingkeytotableordershippmentinfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ShippmentDetail_Id",
                table: "Orders",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShippmentDetail_Id",
                table: "Orders",
                column: "ShippmentDetail_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_ShipmentInfo_ShippmentDetail_Id",
                table: "Orders",
                column: "ShippmentDetail_Id",
                principalTable: "ShipmentInfo",
                principalColumn: "ShipmentInfo_Id"
               );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_ShipmentInfo_ShippmentDetail_Id",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ShippmentDetail_Id",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippmentDetail_Id",
                table: "Orders");
        }
    }
}
