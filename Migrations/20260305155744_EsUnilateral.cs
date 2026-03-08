using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace REPS_backend.Migrations
{
    /// <inheritdoc />
    public partial class EsUnilateral : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EsUnilateral",
                table: "Ejercicios",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EsUnilateral",
                table: "Ejercicios");
        }
    }
}
