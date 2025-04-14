using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class V2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HoSos",
                columns: table => new
                {
                    iMaHS = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FK_iMaTK = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    sTieuDe = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    iKinhNghiem = table.Column<int>(type: "int", nullable: false),
                    sBangCap = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    sKyNang = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    sDuongDanTep = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HoSos", x => x.iMaHS);
                    table.ForeignKey(
                        name: "FK_HoSos_Users_FK_iMaTK",
                        column: x => x.FK_iMaTK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BaiDangs",
                columns: table => new
                {
                    PK_iMaBaiDang = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FK_iMaTK = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    sTieuDe = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    sMoTa = table.Column<string>(type: "NVARCHAR(MAX)", nullable: true),
                    sDiaDiem = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    fMucLuong = table.Column<decimal>(type: "DECIMAL(10,2)", nullable: true),
                    sTrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    dNgayTao = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()"),
                    dThoiGianHetHan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FK_iMaHS = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BaiDangs", x => x.PK_iMaBaiDang);
                    table.ForeignKey(
                        name: "FK_BaiDangs_HoSos_FK_iMaHS",
                        column: x => x.FK_iMaHS,
                        principalTable: "HoSos",
                        principalColumn: "iMaHS",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BaiDangs_Users_FK_iMaTK",
                        column: x => x.FK_iMaTK,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BaiDangs_FK_iMaHS",
                table: "BaiDangs",
                column: "FK_iMaHS");

            migrationBuilder.CreateIndex(
                name: "IX_BaiDangs_FK_iMaTK",
                table: "BaiDangs",
                column: "FK_iMaTK");

            migrationBuilder.CreateIndex(
                name: "IX_HoSos_FK_iMaTK",
                table: "HoSos",
                column: "FK_iMaTK");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BaiDangs");

            migrationBuilder.DropTable(
                name: "HoSos");
        }
    }
}
