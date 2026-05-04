using System;
namespace Banvemaybay.Models
{
    public class ChuyenBay
    {
        public int Id { get; set; }
        public string MaChuyenBay { get; set; } = string.Empty;
        public string HangHangKhong { get; set; } = string.Empty;
        public int DiemDiId { get; set; }
        public int DiemDenId { get; set; }
        public DateTime ThoiGianDi { get; set; }
        public DateTime ThoiGianDen { get; set; }
        public decimal GiaVe { get; set; }
        public decimal GiaKhuyenMai { get; set; }
    }
}