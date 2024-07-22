using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Data.Migrations
{
    /// <inheritdoc />
    public partial class droptableshippmentinfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Eliminar la tabla antigua
            migrationBuilder.DropTable(name: "ShipmentInfo");

            // Crear la nueva tabla con los campos especificados
            migrationBuilder.CreateTable(
                name: "ShipmentInfo",
                columns: table => new
                {
                    ShipmentInfo_Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    ExteriorNumber = table.Column<int>(nullable: false),
                    InteriorNumber = table.Column<int>(nullable: true),
                    CodigoPostal = table.Column<int>(nullable: false),
                    AditionalInformation = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Municipality = table.Column<string>(nullable: true),
                    Colony = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    User_Id = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentInfo", x => x.ShipmentInfo_Id);
                    table.ForeignKey(
                        name: "FK_ShipmentInfo_User_User_Id",
                        column: x => x.User_Id,
                        principalTable: "AspNetUsers",  // Assuming your User table is named AspNetUsers
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentInfo_User_Id",
                table: "ShipmentInfo",
                column: "User_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Eliminar la tabla creada en el método Up
            migrationBuilder.DropTable(name: "ShipmentInfo");

            // Volver a crear la tabla antigua con la columna Id de tipo int
            migrationBuilder.CreateTable(
                name: "ShipmentInfo",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Street = table.Column<string>(nullable: true),
                    ExteriorNumber = table.Column<int>(nullable: false),
                    InteriorNumber = table.Column<int>(nullable: true),
                    CodigoPostal = table.Column<int>(nullable: false),
                    AditionalInformation = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Municipality = table.Column<string>(nullable: true),
                    Colony = table.Column<string>(nullable: true),
                    Phone = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    User_Id = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShipmentInfo_User_User_Id",
                        column: x => x.User_Id,
                        principalTable: "AspNetUsers",  // Assuming your User table is named AspNetUsers
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentInfo_User_Id",
                table: "ShipmentInfo",
                column: "User_Id");
        }
    }
}
