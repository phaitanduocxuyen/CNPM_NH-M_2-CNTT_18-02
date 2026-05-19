using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Banvemaybay.Controllers
{
    public class BaiVietController : Controller
    {
        public IActionResult Index()
        {
            var danhSachBaiViet = new List<dynamic>
            {
                new {
                    Id = 1,
                    TieuDe = "Kinh nghiệm du lịch Đà Lạt mùa sương mù",
                    MoTa = "Khám phá vẻ đẹp mờ ảo của Đà Lạt vào những tháng cuối năm. Ăn gì, chơi ở đâu để có những bức ảnh triệu like?",
                    AnhDaiDien = "https://picsum.photos/400/220?random=1",
                    NgayDang = "15/05/2026",
                    TacGia = "Dũng Đi Phượt"
                },
                new {
                    Id = 2,
                    TieuDe = "5 Quán ăn ngon tuyệt cú mèo tại Hà Nội",
                    MoTa = "Điểm danh những món ăn đường phố không thể bỏ qua khi đến Thủ đô. Review chân thực từ dân bản địa.",
                    AnhDaiDien = "https://picsum.photos/400/220?random=2",
                    NgayDang = "16/05/2026",
                    TacGia = "Travelz Team"
                },
                new {
                    Id = 3,
                    TieuDe = "Sổ tay khám phá TP. Hồ Chí Minh trong 24h",
                    MoTa = "Lịch trình chi tiết cho người bận rộn muốn càn quét Sài Gòn từ sáng sớm đến đêm khuya.",
                    AnhDaiDien = "https://picsum.photos/400/220?random=3",
                    NgayDang = "18/05/2026",
                    TacGia = "Admin"
                }
            };

            return View(danhSachBaiViet);
        }

        // ĐÂY LÀ PHẦN VỪA THÊM VÀO: Hàm xử lý khi khách click "Đọc Tiếp"
        public IActionResult ChiTiet(int id)
        {
            // Cứ bấm vào bài nào thì cũng nhảy ra 1 trang nội dung mẫu này cho nhanh
            return View();
        }
    }
}