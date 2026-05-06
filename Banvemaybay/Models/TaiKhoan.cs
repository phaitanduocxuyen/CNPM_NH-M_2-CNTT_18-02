/**
 * 1. File: TaiKhoan.cs
 * Mục đích / Chức năng: Quản lý thông tin định danh, mật khẩu và quyền hạn của người dùng trong hệ thống.
 * Người viết: pHạm Anh Tú
 * Thời gian sửa đổi: 06/05/2026
 * Phiên bản: 1.0
 */

using System.ComponentModel.DataAnnotations;

namespace Banvemaybay.Models
{
    /**
     * 3. Class: TaiKhoan
     * Mục đích / Chức năng: Lưu trữ thông tin tài khoản để thực hiện các chức năng Đăng ký, Đăng nhập và Phân quyền.
     * Cấu trúc: Bao gồm thông tin xác thực (SĐT, mật khẩu), thông tin cá nhân và vai trò người dùng.
     * Người viết / tg sửa đổi: pHạm Anh Tú - Cập nhật 06/05/2026
     */
    public class TaiKhoan
    {
        /** 
         * Ý nghĩa các biến (Properties):
         * Id: Khóa chính tự tăng dùng để định danh tài khoản.
         * SoDienThoai: Số điện thoại dùng làm tên đăng nhập (Username), giới hạn 15 ký tự.
         * MatKhau: Mật khẩu đã được mã hóa, giới hạn 255 ký tự để đảm bảo bảo mật.
         * HoTen: Tên đầy đủ của người dùng để hiển thị trên vé và giao diện.
         * VaiTro: Quyền hạn trong hệ thống (Mặc định là "KhachHang", ngoài ra còn có "Admin").
         */

        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [MaxLength(15)]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [MaxLength(255)]
        public string MatKhau { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        [MaxLength(100)]
        public string HoTen { get; set; } = string.Empty;

        public string VaiTro { get; set; } = "KhachHang";
    }
}