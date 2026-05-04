using Microsoft.AspNetCore.Mvc;
using Banvemaybay.Data;
using System.Linq;

namespace Banvemaybay.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.DanhSachSanBay = _context.SanBays.ToList();

            // Lấy 4 chuyến bay có giá khuyến mãi thấp nhất để hiển thị ở phần Ưu Đãi
            var uuDai = _context.ChuyenBays
                .OrderBy(c => c.GiaKhuyenMai)
                .Take(4)
                .ToList();

            ViewBag.DanhSachUuDai = uuDai;
            return View();
        }
    }
}