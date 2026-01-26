using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REPS_backend.Migrations
{
    /// <inheritdoc />
    public partial class AgregarEjercicios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GrupoMuscular",
                table: "Ejercicios",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GrupoMuscular",
                table: "Ejercicios");
        }
    }
}
