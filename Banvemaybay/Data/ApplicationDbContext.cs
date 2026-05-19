/**
 * 1. File: ApplicationDbContext.cs
 * Mục đích / Chức năng: Cấu hình kết nối cơ sở dữ liệu và quản lý các thực thể (Entities) cho hệ thống Bán vé máy bay.
 * Người viết: pHạm Anh Tú
 * Thời gian sửa đổi: 06/05/2026
 * Phiên bản: 1.0
 */

using Banvemaybay.Models;
using Microsoft.EntityFrameworkCore;

namespace Banvemaybay.Data
{
    /**
     * 3. Class: ApplicationDbContext
     * Mục đích / Chức năng: Lớp trung gian giữa mã nguồn C# và SQL Server, chịu trách nhiệm truy vấn và lưu trữ dữ liệu.
     * Cấu trúc: Bao gồm các DbSet đại diện cho các bảng trong Database.
     * Người viết / tg sửa đổi: pHạm Anh Tú - Cập nhật 06/05/2026
     */
    public class ApplicationDbContext : DbContext
    {
        // 4. Function: Khởi tạo ApplicationDbContext
        // Tham số: options - Các cấu hình như chuỗi kết nối (Connection String).
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        // --- Danh sách các bảng (DbSet) ---

        /** 
         * Ý nghĩa các biến (DbSet):
         * ChuyenBays: Quản lý thông tin các chuyến bay (mã bay, giờ khởi hành, giá vé).
         * SanBays: Quản lý danh mục các sân bay trong hệ thống.
         * Ves: Lưu trữ thông tin vé đã đặt và trạng thái thanh toán.
         * TaiKhoans: Quản lý thông tin người dùng và phân quyền (Admin/User).
         */
        public DbSet<ChuyenBay> ChuyenBays { get; set; }
        public DbSet<SanBay> SanBays { get; set; }
        public DbSet<Ve> Ves { get; set; }
        public DbSet<TaiKhoan> TaiKhoans { get; set; }
    }
}