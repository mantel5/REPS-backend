using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REPS_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddVideoUrlToEjercicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Biografia",
                table: "Usuarios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "EsPerfilPublico",
                table: "Usuarios",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MostrarEstadisticas",
                table: "Usuarios",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RankingVisible",
                table: "Usuarios",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "VideoUrl",
                table: "Ejercicios",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Biografia",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "EsPerfilPublico",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "MostrarEstadisticas",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "RankingVisible",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "VideoUrl",
                table: "Ejercicios");
        }
    }
}
