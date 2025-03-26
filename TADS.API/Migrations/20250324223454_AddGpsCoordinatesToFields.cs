using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TADS.API.Migrations
{
    /// <inheritdoc />
    public partial class AddGpsCoordinatesToFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Fields",
                type: "decimal(10,6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Fields",
                type: "decimal(10,6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Fields");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Fields");
        }
    }
}
