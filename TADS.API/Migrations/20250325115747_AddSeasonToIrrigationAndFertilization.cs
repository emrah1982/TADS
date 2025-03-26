using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TADS.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSeasonToIrrigationAndFertilization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Season",
                table: "Irrigations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Season",
                table: "Fertilizations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Season",
                table: "Irrigations");

            migrationBuilder.DropColumn(
                name: "Season",
                table: "Fertilizations");
        }
    }
}
