using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class V7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UngTuyen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FK_iMaTK_GiaSu = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FK_iMaBaiDang = table.Column<int>(type: "int", nullable: false),
                    FK_iMaHS = table.Column<int>(type: "int", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayUngTuyen = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UngTuyen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UngTuyen_BaiDangs_FK_iMaBaiDang",
                        column: x => x.FK_iMaBaiDang,
                        principalTable: "BaiDangs",
                        principalColumn: "PK_iMaBaiDang",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UngTuyen_HoSos_FK_iMaHS",
                        column: x => x.FK_iMaHS,
                        principalTable: "HoSos",
                        principalColumn: "iMaHS",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UngTuyen_Users_FK_iMaTK_GiaSu",
                        column: x => x.FK_iMaTK_GiaSu,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UngTuyen_FK_iMaBaiDang",
                table: "UngTuyen",
                column: "FK_iMaBaiDang");

            migrationBuilder.CreateIndex(
                name: "IX_UngTuyen_FK_iMaHS",
                table: "UngTuyen",
                column: "FK_iMaHS");

            migrationBuilder.CreateIndex(
                name: "IX_UngTuyen_FK_iMaTK_GiaSu",
                table: "UngTuyen",
                column: "FK_iMaTK_GiaSu");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UngTuyen");
        }
    }
}
