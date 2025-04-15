using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class V3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "sBangCap",
                table: "BaiDangs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "sGioiTinh",
                table: "BaiDangs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "sKinhNghiem",
                table: "BaiDangs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "sMonday",
                table: "BaiDangs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "sTuoi",
                table: "BaiDangs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "sYCau",
                table: "BaiDangs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "sBangCap",
                table: "BaiDangs");

            migrationBuilder.DropColumn(
                name: "sGioiTinh",
                table: "BaiDangs");

            migrationBuilder.DropColumn(
                name: "sKinhNghiem",
                table: "BaiDangs");

            migrationBuilder.DropColumn(
                name: "sMonday",
                table: "BaiDangs");

            migrationBuilder.DropColumn(
                name: "sTuoi",
                table: "BaiDangs");

            migrationBuilder.DropColumn(
                name: "sYCau",
                table: "BaiDangs");
        }
    }
}
