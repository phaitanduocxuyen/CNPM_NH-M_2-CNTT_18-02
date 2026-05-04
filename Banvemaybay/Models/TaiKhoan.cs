using System.ComponentModel.DataAnnotations;
namespace Banvemaybay.Models
{
    public class TaiKhoan
    {
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