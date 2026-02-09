using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REPSbackend.Migrations
{
    /// <inheritdoc />
    public partial class AddRutinaIdToEntrenamiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RutinaId",
                table: "Entrenamientos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Entrenamientos_RutinaId",
                table: "Entrenamientos",
                column: "RutinaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Entrenamientos_Rutinas_RutinaId",
                table: "Entrenamientos",
                column: "RutinaId",
                principalTable: "Rutinas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entrenamientos_Rutinas_RutinaId",
                table: "Entrenamientos");

            migrationBuilder.DropIndex(
                name: "IX_Entrenamientos_RutinaId",
                table: "Entrenamientos");

            migrationBuilder.DropColumn(
                name: "RutinaId",
                table: "Entrenamientos");
        }
    }
}
