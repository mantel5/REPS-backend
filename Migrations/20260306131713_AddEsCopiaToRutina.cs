using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REPS_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddEsCopiaToRutina : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreadorOriginalNombre",
                table: "Rutinas",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "EsCopia",
                table: "Rutinas",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreadorOriginalNombre",
                table: "Rutinas");

            migrationBuilder.DropColumn(
                name: "EsCopia",
                table: "Rutinas");
        }
    }
}
