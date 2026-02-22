using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REPSbackend.Migrations
{
    /// <inheritdoc />
    public partial class AddCardioAndPreferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UnidadPreferida",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CaloriasQuemadasReales",
                table: "SerieLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DistanciaReal",
                table: "SerieLogs",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InclinacionReal",
                table: "SerieLogs",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TiempoSegundosReal",
                table: "SerieLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VelocidadReal",
                table: "SerieLogs",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "InclinacionSugerida",
                table: "RutinaEjercicios",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TiempoSegundosSugerido",
                table: "RutinaEjercicios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "VelocidadSugerida",
                table: "RutinaEjercicios",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsCardio",
                table: "Ejercicios",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnidadPreferida",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CaloriasQuemadasReales",
                table: "SerieLogs");

            migrationBuilder.DropColumn(
                name: "DistanciaReal",
                table: "SerieLogs");

            migrationBuilder.DropColumn(
                name: "InclinacionReal",
                table: "SerieLogs");

            migrationBuilder.DropColumn(
                name: "TiempoSegundosReal",
                table: "SerieLogs");

            migrationBuilder.DropColumn(
                name: "VelocidadReal",
                table: "SerieLogs");

            migrationBuilder.DropColumn(
                name: "InclinacionSugerida",
                table: "RutinaEjercicios");

            migrationBuilder.DropColumn(
                name: "TiempoSegundosSugerido",
                table: "RutinaEjercicios");

            migrationBuilder.DropColumn(
                name: "VelocidadSugerida",
                table: "RutinaEjercicios");

            migrationBuilder.DropColumn(
                name: "EsCardio",
                table: "Ejercicios");
        }
    }
}
