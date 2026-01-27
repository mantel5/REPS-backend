using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REPSbackend.Migrations
{
    /// <inheritdoc />
    public partial class FixFinalRutinaEjercicio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Rutinas_UsuarioId",
                table: "Rutinas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_RutinaEjercicios_EjercicioId",
                table: "RutinaEjercicios",
                column: "EjercicioId");

            migrationBuilder.AddForeignKey(
                name: "FK_RutinaEjercicios_Ejercicios_EjercicioId",
                table: "RutinaEjercicios",
                column: "EjercicioId",
                principalTable: "Ejercicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rutinas_Usuarios_UsuarioId",
                table: "Rutinas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RutinaEjercicios_Ejercicios_EjercicioId",
                table: "RutinaEjercicios");

            migrationBuilder.DropForeignKey(
                name: "FK_Rutinas_Usuarios_UsuarioId",
                table: "Rutinas");

            migrationBuilder.DropIndex(
                name: "IX_Rutinas_UsuarioId",
                table: "Rutinas");

            migrationBuilder.DropIndex(
                name: "IX_RutinaEjercicios_EjercicioId",
                table: "RutinaEjercicios");
        }
    }
}
