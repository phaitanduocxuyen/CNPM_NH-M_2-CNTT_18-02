using System;
namespace Banvemaybay.Models
{
    public class Ve
    {
        public int Id { get; set; }
        public int ChuyenBayId { get; set; }
        public string HoTen { get; set; } = string.Empty;
        public string SoDienThoai { get; set; } = string.Empty;
        public string GheDaChon { get; set; } = string.Empty;
        public DateTime NgayDat { get; set; } = DateTime.Now;
    }
}