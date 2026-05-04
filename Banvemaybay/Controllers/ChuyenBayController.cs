using Banvemaybay.Data;
using Banvemaybay.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Banvemaybay.Controllers
{
    public class ChuyenBayController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChuyenBayController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =======================================================
        // TÌM KIẾM CHUYẾN BAY (TỪ TRANG CHỦ)
        // =======================================================
        public IActionResult TimKiem(int diemDi, int diemDen, DateTime? ngayDi, string loaiVe, DateTime? ngayVe, int soNguoiLon = 1, int soTreEm = 0)
        {
            var query = _context.ChuyenBays.AsQueryable();

            // Lọc theo Điểm Đi và Điểm Đến
            query = query.Where(c => c.DiemDiId == diemDi && c.DiemDenId == diemDen);

            // Lọc theo Ngày Đi
            if (ngayDi.HasValue)
            {
                query = query.Where(c => c.ThoiGianDi.Date == ngayDi.Value.Date);
            }

            var danhSachChuyenBay = query.ToList();

            // VÁ LỖI LOGIC: Lọc bỏ các chuyến bay không đủ ghế trống cho số người yêu cầu
            int tongGheYeuCau = soNguoiLon + soTreEm;
            var danhSachHopLe = new List<ChuyenBay>();

            foreach (var cb in danhSachChuyenBay)
            {
                var cacVeDaBan = _context.Ves.Where(v => v.ChuyenBayId == cb.Id).ToList();
                int soGheDaBan = 0;
                foreach (var ve in cacVeDaBan)
                {
                    if (!string.IsNullOrEmpty(ve.GheDaChon))
                    {
                        soGheDaBan += ve.GheDaChon.Split(',').Length;
                    }
                }

                // Giả định mỗi chuyến bay có 30 ghế (theo sơ đồ hiển thị của bạn)
                if ((30 - soGheDaBan) >= tongGheYeuCau)
                {
                    danhSachHopLe.Add(cb);
                }
            }

            // Lấy tên thành phố
            ViewBag.TenDiemDi = _context.SanBays.FirstOrDefault(s => s.Id == diemDi)?.TenThanhPho ?? "N/A";
            ViewBag.TenDiemDen = _context.SanBays.FirstOrDefault(s => s.Id == diemDen)?.TenThanhPho ?? "N/A";

            // Truyền thông tin vé và hành khách ra View để hiển thị
            ViewBag.NgayDi = ngayDi?.ToString("dd/MM/yyyy");
            ViewBag.NgayVe = ngayVe?.ToString("dd/MM/yyyy");
            ViewBag.LoaiVe = loaiVe == "khuhoi" ? "Khứ hồi" : "Một chiều";
            ViewBag.SoNguoiLon = soNguoiLon;
            ViewBag.SoTreEm = soTreEm;

            return View(danhSachHopLe); // Trả về danh sách đã lọc
        }

        // =======================================================
        // GỢI Ý CHUYẾN BAY (MỤC "DÀNH CHO BẠN")
        // =======================================================
        public IActionResult DanhSach()
        {
            ViewBag.DanhSachSanBay = _context.SanBays.ToList();

            var danhSachChuyenBay = _context.ChuyenBays
                .Where(c => c.ThoiGianDi >= DateTime.Now)
                .OrderBy(c => c.ThoiGianDi)
                .ToList();

            return View(danhSachChuyenBay);
        }
    }
}