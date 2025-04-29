using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class V10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DanhGiaGiaSus",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NguoiDanhGiaId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GiaSuId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SoSao = table.Column<int>(type: "int", nullable: false),
                    NoiDung = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DanhGiaGiaSus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DanhGiaGiaSus_Users_GiaSuId",
                        column: x => x.GiaSuId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DanhGiaGiaSus_Users_NguoiDanhGiaId",
                        column: x => x.NguoiDanhGiaId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DanhGiaGiaSus_GiaSuId",
                table: "DanhGiaGiaSus",
                column: "GiaSuId");

            migrationBuilder.CreateIndex(
                name: "IX_DanhGiaGiaSus_NguoiDanhGiaId",
                table: "DanhGiaGiaSus",
                column: "NguoiDanhGiaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DanhGiaGiaSus");
        }
    }
}
