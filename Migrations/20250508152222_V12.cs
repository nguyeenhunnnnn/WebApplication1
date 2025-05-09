using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class V12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "GoiCuoc",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "dUuTienDen",
                table: "BaiDangs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sTrangThaiGD",
                table: "BaiDangs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "tbl_GoiDichVu",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenGoi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Gia = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SoNgayHieuLuc = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_GoiDichVu", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "tbl_ThanhToan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TaiKhoanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    GoiDichVuId = table.Column<int>(type: "int", nullable: false),
                    NgayThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BaiDangId = table.Column<int>(type: "int", nullable: true),
                    IsDuyet = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tbl_ThanhToan", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tbl_ThanhToan_BaiDangs_BaiDangId",
                        column: x => x.BaiDangId,
                        principalTable: "BaiDangs",
                        principalColumn: "PK_iMaBaiDang",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_ThanhToan_Users_TaiKhoanId",
                        column: x => x.TaiKhoanId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_tbl_ThanhToan_tbl_GoiDichVu_GoiDichVuId",
                        column: x => x.GoiDichVuId,
                        principalTable: "tbl_GoiDichVu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_tbl_ThanhToan_BaiDangId",
                table: "tbl_ThanhToan",
                column: "BaiDangId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_ThanhToan_GoiDichVuId",
                table: "tbl_ThanhToan",
                column: "GoiDichVuId");

            migrationBuilder.CreateIndex(
                name: "IX_tbl_ThanhToan_TaiKhoanId",
                table: "tbl_ThanhToan",
                column: "TaiKhoanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tbl_ThanhToan");

            migrationBuilder.DropTable(
                name: "tbl_GoiDichVu");

            migrationBuilder.DropColumn(
                name: "GoiCuoc",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "dUuTienDen",
                table: "BaiDangs");

            migrationBuilder.DropColumn(
                name: "sTrangThaiGD",
                table: "BaiDangs");
        }
    }
}
