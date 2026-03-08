using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REPS_backend.Migrations
{
    /// <inheritdoc />
    public partial class MakeSesionIdNullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeriesLogs_Sesiones_SesionId",
                table: "SeriesLogs");

            migrationBuilder.DropColumn(
                name: "EsUnilateral",
                table: "Ejercicios");

            migrationBuilder.AlterColumn<int>(
                name: "SesionId",
                table: "SeriesLogs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_SeriesLogs_Sesiones_SesionId",
                table: "SeriesLogs",
                column: "SesionId",
                principalTable: "Sesiones",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SeriesLogs_Sesiones_SesionId",
                table: "SeriesLogs");

            migrationBuilder.AlterColumn<int>(
                name: "SesionId",
                table: "SeriesLogs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "EsUnilateral",
                table: "Ejercicios",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_SeriesLogs_Sesiones_SesionId",
                table: "SeriesLogs",
                column: "SesionId",
                principalTable: "Sesiones",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
