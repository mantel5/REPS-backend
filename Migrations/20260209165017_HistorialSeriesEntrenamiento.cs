using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REPSbackend.Migrations
{
    /// <inheritdoc />
    public partial class HistorialSeriesEntrenamiento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SerieLog_Sesiones_SesionId",
                table: "SerieLog");

            migrationBuilder.DropTable(
                name: "Sesiones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SerieLog",
                table: "SerieLog");

            migrationBuilder.RenameTable(
                name: "SerieLog",
                newName: "SerieLogs");

            migrationBuilder.RenameColumn(
                name: "SesionId",
                table: "SerieLogs",
                newName: "EntrenamientoId");

            migrationBuilder.RenameIndex(
                name: "IX_SerieLog_SesionId",
                table: "SerieLogs",
                newName: "IX_SerieLogs_EntrenamientoId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SerieLogs",
                table: "SerieLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SerieLogs_Entrenamientos_EntrenamientoId",
                table: "SerieLogs",
                column: "EntrenamientoId",
                principalTable: "Entrenamientos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SerieLogs_Entrenamientos_EntrenamientoId",
                table: "SerieLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SerieLogs",
                table: "SerieLogs");

            migrationBuilder.RenameTable(
                name: "SerieLogs",
                newName: "SerieLog");

            migrationBuilder.RenameColumn(
                name: "EntrenamientoId",
                table: "SerieLog",
                newName: "SesionId");

            migrationBuilder.RenameIndex(
                name: "IX_SerieLogs_EntrenamientoId",
                table: "SerieLog",
                newName: "IX_SerieLog_SesionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SerieLog",
                table: "SerieLog",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Sesiones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    DuracionRealMinutos = table.Column<int>(type: "int", nullable: false),
                    EstaFinalizada = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    NombreRutinaSnapshot = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PuntosGanados = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sesiones", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddForeignKey(
                name: "FK_SerieLog_Sesiones_SesionId",
                table: "SerieLog",
                column: "SesionId",
                principalTable: "Sesiones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
