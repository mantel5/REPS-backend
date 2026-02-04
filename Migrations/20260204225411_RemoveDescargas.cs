using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REPSbackend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveDescargas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Descargas",
                table: "Rutinas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.AddColumn<int>(
                name: "Descargas",
                table: "Rutinas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
