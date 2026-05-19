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

        public IActionResult TimKiem(int diemDi, int diemDen, DateTime? ngayDi, string loaiVe, DateTime? ngayVe, int soNguoiLon = 1, int soTreEm = 0)
        {
            int tongGheYeuCau = soNguoiLon + soTreEm;

            // 1. TÌM CHUYẾN ĐI
            var queryDi = _context.ChuyenBays.Where(c => c.DiemDiId == diemDi && c.DiemDenId == diemDen);
            if (ngayDi.HasValue) queryDi = queryDi.Where(c => c.ThoiGianDi.Date == ngayDi.Value.Date);

            var danhSachChuyenDi = queryDi.ToList();
            var chuyenDiHopLe = new List<ChuyenBay>();

            foreach (var cb in danhSachChuyenDi)
            {
                var cacVeDaBan = _context.Ves.Where(v => v.ChuyenBayId == cb.Id).ToList();
                int soGheDaBan = cacVeDaBan.Sum(ve => !string.IsNullOrEmpty(ve.GheDaChon) ? ve.GheDaChon.Split(',').Length : 0);
                if ((30 - soGheDaBan) >= tongGheYeuCau) chuyenDiHopLe.Add(cb);
            }

            // 2. TÌM CHUYẾN VỀ (Chỉ tìm nếu loại vé là khuhoi)
            var chuyenVeHopLe = new List<ChuyenBay>();
            if (loaiVe == "khuhoi" && ngayVe.HasValue)
            {
                // Đảo vị trí: Điểm xuất phát của chuyến về là điểm đến của chuyến đi
                var queryVe = _context.ChuyenBays.Where(c => c.DiemDiId == diemDen && c.DiemDenId == diemDi);
                queryVe = queryVe.Where(c => c.ThoiGianDi.Date == ngayVe.Value.Date);

                var danhSachChuyenVe = queryVe.ToList();
                foreach (var cb in danhSachChuyenVe)
                {
                    var cacVeDaBan = _context.Ves.Where(v => v.ChuyenBayId == cb.Id).ToList();
                    int soGheDaBan = cacVeDaBan.Sum(ve => !string.IsNullOrEmpty(ve.GheDaChon) ? ve.GheDaChon.Split(',').Length : 0);
                    if ((30 - soGheDaBan) >= tongGheYeuCau) chuyenVeHopLe.Add(cb);
                }
            }

            ViewBag.ChuyenDi = chuyenDiHopLe;
            ViewBag.ChuyenVe = chuyenVeHopLe;
            ViewBag.TenDiemDi = _context.SanBays.FirstOrDefault(s => s.Id == diemDi)?.TenThanhPho ?? "N/A";
            ViewBag.TenDiemDen = _context.SanBays.FirstOrDefault(s => s.Id == diemDen)?.TenThanhPho ?? "N/A";
            ViewBag.NgayDi = ngayDi?.ToString("dd/MM/yyyy");
            ViewBag.NgayVe = ngayVe?.ToString("dd/MM/yyyy");
            ViewBag.LoaiVe = loaiVe == "khuhoi" ? "Khứ hồi" : "Một chiều";
            ViewBag.SoNguoiLon = soNguoiLon;
            ViewBag.SoTreEm = soTreEm;

            return View();
        }

        public IActionResult DanhSach()
        {
            ViewBag.DanhSachSanBay = _context.SanBays.ToList();
            var danhSachChuyenBay = _context.ChuyenBays.Where(c => c.ThoiGianDi >= DateTime.Now).OrderBy(c => c.ThoiGianDi).ToList();
            return View(danhSachChuyenBay);
        }
    }
}