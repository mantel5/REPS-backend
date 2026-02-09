using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REPSbackend.Migrations
{
    /// <inheritdoc />
    public partial class AddEjerciciosToSeries : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_SerieLogs_EjercicioId",
                table: "SerieLogs",
                column: "EjercicioId");

            migrationBuilder.AddForeignKey(
                name: "FK_SerieLogs_Ejercicios_EjercicioId",
                table: "SerieLogs",
                column: "EjercicioId",
                principalTable: "Ejercicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SerieLogs_Ejercicios_EjercicioId",
                table: "SerieLogs");

            migrationBuilder.DropIndex(
                name: "IX_SerieLogs_EjercicioId",
                table: "SerieLogs");
        }
    }
}
