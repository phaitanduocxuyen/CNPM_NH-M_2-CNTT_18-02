using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Banvemaybay.Migrations
{
    /// <inheritdoc />
    public partial class Update_TaiKhoan_SDT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TenDangNhap",
                table: "TaiKhoans");

            migrationBuilder.AddColumn<string>(
                name: "SoDienThoai",
                table: "TaiKhoans",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SoDienThoai",
                table: "TaiKhoans");

            migrationBuilder.AddColumn<string>(
                name: "TenDangNhap",
                table: "TaiKhoans",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
