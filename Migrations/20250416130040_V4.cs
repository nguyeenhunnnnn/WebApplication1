using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class V4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "iKinhNghiem",
                table: "HoSos");

            migrationBuilder.AddColumn<string>(
                name: "sKinhNghiem",
                table: "HoSos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sKinhNghiem",
                table: "HoSos");

            migrationBuilder.AddColumn<int>(
                name: "iKinhNghiem",
                table: "HoSos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
