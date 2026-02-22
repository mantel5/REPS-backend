using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REPSbackend.Migrations
{
    /// <inheritdoc />
    public partial class AddStreakTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE Usuarios SET RangoGeneral = '0' WHERE RangoGeneral = 'SinRango';");
            migrationBuilder.Sql("UPDATE Usuarios SET RangoGeneral = '1' WHERE RangoGeneral = 'Bronce';");
            migrationBuilder.Sql("UPDATE Usuarios SET RangoGeneral = '2' WHERE RangoGeneral = 'Plata';");
            migrationBuilder.Sql("UPDATE Usuarios SET RangoGeneral = '3' WHERE RangoGeneral = 'Oro';");
            migrationBuilder.Sql("UPDATE Usuarios SET RangoGeneral = '4' WHERE RangoGeneral = 'Diamante';");
            migrationBuilder.Sql("UPDATE Usuarios SET RangoGeneral = '5' WHERE RangoGeneral = 'Elite';");

            migrationBuilder.AlterColumn<int>(
                name: "RangoGeneral",
                table: "Usuarios",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaUltimaActividad",
                table: "Usuarios",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PuntosLogros",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PuntosRecords",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_RecordsPersonales_EjercicioId",
                table: "RecordsPersonales",
                column: "EjercicioId");

            migrationBuilder.AddForeignKey(
                name: "FK_RecordsPersonales_Ejercicios_EjercicioId",
                table: "RecordsPersonales",
                column: "EjercicioId",
                principalTable: "Ejercicios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RecordsPersonales_Ejercicios_EjercicioId",
                table: "RecordsPersonales");

            migrationBuilder.DropIndex(
                name: "IX_RecordsPersonales_EjercicioId",
                table: "RecordsPersonales");

            migrationBuilder.DropColumn(
                name: "FechaUltimaActividad",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "PuntosLogros",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "PuntosRecords",
                table: "Usuarios");

            migrationBuilder.AlterColumn<string>(
                name: "RangoGeneral",
                table: "Usuarios",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
