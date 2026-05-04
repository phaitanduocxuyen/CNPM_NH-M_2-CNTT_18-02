using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Banvemaybay.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChuyenBays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaChuyenBay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    HangHangKhong = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DiemDiId = table.Column<int>(type: "int", nullable: false),
                    DiemDenId = table.Column<int>(type: "int", nullable: false),
                    ThoiGianDi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ThoiGianDen = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GiaVe = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    GiaKhuyenMai = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChuyenBays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SanBays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaSanBay = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TenThanhPho = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SanBays", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ves",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChuyenBayId = table.Column<int>(type: "int", nullable: false),
                    HoTen = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SoDienThoai = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GheDaChon = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NgayDat = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ves", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChuyenBays");

            migrationBuilder.DropTable(
                name: "SanBays");

            migrationBuilder.DropTable(
                name: "Ves");
        }
    }
}
