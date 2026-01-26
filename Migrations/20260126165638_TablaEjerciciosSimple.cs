using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REPS_backend.Migrations
{
    /// <inheritdoc />
    public partial class TablaEjerciciosSimple : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DetallesMusculares_Ejercicios_EjercicioId",
                table: "DetallesMusculares");

            migrationBuilder.DropIndex(
                name: "IX_DetallesMusculares_EjercicioId",
                table: "DetallesMusculares");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_DetallesMusculares_EjercicioId",
                table: "DetallesMusculares",
                column: "EjercicioId");

            migrationBuilder.AddForeignKey(
                name: "FK_DetallesMusculares_Ejercicios_EjercicioId",
                table: "DetallesMusculares",
                column: "EjercicioId",
                principalTable: "Ejercicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
