// ==============================================================================
// MỤC ĐÍCH / CHỨC NĂNG FILE: Điều hướng trang chủ và hiển thị nội dung cơ bản.
// NGƯỜI VIẾT: Nguyễn Văn Huy
// THỜI GIAN SỬA ĐỔI: 06/05/2026
// PHIÊN BẢN: 1.0
// ==============================================================================

using Microsoft.AspNetCore.Mvc;
using Banvemaybay.Data;
using System.Linq;

namespace Banvemaybay.Controllers
{
    /// <summary>
    /// Mục đích / Chức năng: Controller cung cấp dữ liệu khởi tạo cho màn hình Landing Page (Trang chủ).
    /// Cấu trúc: Hàm Index trả về giao diện gốc.
    /// Người viết: Nguyễn Văn Huy - Thời gian sửa đổi: 06/05/2026
    /// </summary>
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Mục đích: Chuẩn bị dữ liệu hiển thị khung tìm kiếm và danh sách ưu đãi.
        /// Ý nghĩa biến: 'uuDai' lấy ra 4 chuyến bay có thuộc tính Giá Khuyến Mãi thấp nhất.
        /// Người viết: Nguyễn Văn Huy - Thời gian sửa: 06/05/2026
        /// </summary>
        /// <returns>View Index</returns>
        public IActionResult Index()
        {
            ViewBag.DanhSachSanBay = _context.SanBays.ToList();

            var uuDai = _context.ChuyenBays
                .OrderBy(c => c.GiaKhuyenMai)
                .Take(4)
                .ToList();

            ViewBag.DanhSachUuDai = uuDai;
            return View();
        }
    }
}