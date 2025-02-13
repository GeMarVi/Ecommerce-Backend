﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class updatetableShipmentInforowphone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Phone",
                table: "ShipmentInfo",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Phone",
                table: "ShipmentInfo",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
