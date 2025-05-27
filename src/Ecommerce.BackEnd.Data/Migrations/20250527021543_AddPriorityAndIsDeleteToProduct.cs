using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.BackEnd.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPriorityAndIsDeleteToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Products",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Products",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Products");
        }
    }
}
