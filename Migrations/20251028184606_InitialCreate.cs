using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FigurasGeometricasApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Figuras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Tipo = table.Column<string>(type: "TEXT", maxLength: 13, nullable: false),
                    Radio = table.Column<double>(type: "REAL", nullable: true),
                    Base = table.Column<double>(type: "REAL", nullable: true),
                    Altura = table.Column<double>(type: "REAL", nullable: true),
                    LadoA = table.Column<double>(type: "REAL", nullable: true),
                    LadoB = table.Column<double>(type: "REAL", nullable: true),
                    LadoC = table.Column<double>(type: "REAL", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Figuras", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Figuras");
        }
    }
}
