using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class V8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TinNhans",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiGuiId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NguoiNhanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ThoiGianGui = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BaiDangId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TinNhans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TinNhans_BaiDangs_BaiDangId",
                        column: x => x.BaiDangId,
                        principalTable: "BaiDangs",
                        principalColumn: "PK_iMaBaiDang",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_TinNhans_Users_NguoiGuiId",
                        column: x => x.NguoiGuiId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TinNhans_Users_NguoiNhanId",
                        column: x => x.NguoiNhanId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TinNhans_BaiDangId",
                table: "TinNhans",
                column: "BaiDangId");

            migrationBuilder.CreateIndex(
                name: "IX_TinNhans_NguoiGuiId",
                table: "TinNhans",
                column: "NguoiGuiId");

            migrationBuilder.CreateIndex(
                name: "IX_TinNhans_NguoiNhanId",
                table: "TinNhans",
                column: "NguoiNhanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TinNhans");
        }
    }
}
