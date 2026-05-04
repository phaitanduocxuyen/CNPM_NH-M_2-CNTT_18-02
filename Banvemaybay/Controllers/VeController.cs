using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Banvemaybay.Data;
using Banvemaybay.Models;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Banvemaybay.Controllers
{
    [Authorize]
    public class VeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- CHỌN GHẾ ---
        public IActionResult ChonGhe(int chuyenBayId)
        {
            ViewBag.ChuyenBayId = chuyenBayId;
            var cacVeDaBan = _context.Ves.Where(v => v.ChuyenBayId == chuyenBayId).ToList();

            var danhSachGheDaMua = new List<string>();
            foreach (var ve in cacVeDaBan)
            {
                if (!string.IsNullOrEmpty(ve.GheDaChon))
                {
                    danhSachGheDaMua.AddRange(ve.GheDaChon.Split(','));
                }
            }

            ViewBag.GheDaMua = danhSachGheDaMua;
            return View();
        }

        [HttpPost]
        public IActionResult XacNhanGhe(int chuyenBayId, string danhSachGhe)
        {
            TempData["ChuyenBayId"] = chuyenBayId;
            TempData["GheDaChon"] = danhSachGhe;
            return RedirectToAction("NhapThongTin");
        }

        // --- NHẬP THÔNG TIN ---
        public IActionResult NhapThongTin()
        {
            if (TempData["ChuyenBayId"] == null || TempData["GheDaChon"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            int chuyenBayId = Convert.ToInt32(TempData["ChuyenBayId"]);
            string gheDaChon = TempData["GheDaChon"].ToString() ?? "";

            ViewBag.ChuyenBayId = chuyenBayId;
            ViewBag.GheDaChon = gheDaChon;
            TempData.Keep("ChuyenBayId");
            TempData.Keep("GheDaChon");

            ViewBag.TenKhachHang = User.Identity?.Name;
            ViewBag.SoDienThoaiKhach = User.FindFirst("SoDienThoai")?.Value;

            var chuyenBay = _context.ChuyenBays.Find(chuyenBayId);
            if (chuyenBay != null)
            {
                ViewBag.ChuyenBay = chuyenBay;
                ViewBag.SanBayDi = _context.SanBays.Find(chuyenBay.DiemDiId)?.TenThanhPho ?? "N/A";
                ViewBag.SanBayDen = _context.SanBays.Find(chuyenBay.DiemDenId)?.TenThanhPho ?? "N/A";

                // LOGIC MỚI: TÍNH TIỀN GHẾ VIP VÀ GHẾ THƯỜNG
                var danhSachGhe = gheDaChon.Split(',');
                int soGheVip = 0;
                int soGheThuong = 0;

                foreach (var ghe in danhSachGhe)
                {
                    if (int.TryParse(ghe, out int soGheInt) && soGheInt <= 12)
                        soGheVip++; // Ghế 1->12 là VIP
                    else
                        soGheThuong++;
                }

                decimal giaVeGoc = chuyenBay.GiaKhuyenMai > 0 ? chuyenBay.GiaKhuyenMai : chuyenBay.GiaVe;
                decimal giaVeVip = giaVeGoc * 1.5m; // VIP đắt gấp rưỡi

                ViewBag.SoGheThuong = soGheThuong;
                ViewBag.SoGheVip = soGheVip;
                ViewBag.GiaVeThuong = giaVeGoc;
                ViewBag.GiaVeVip = giaVeVip;
                ViewBag.TongTien = (soGheThuong * giaVeGoc) + (soGheVip * giaVeVip);
            }

            return View();
        }

        // --- HOÀN TẤT ĐẶT VÉ ---
        [HttpPost]
        public IActionResult HoanTatDatVe(string hoTen, string soDienThoai, string gheDaChon, int chuyenBayId)
        {
            // VÁ LỖI: Chặn việc gửi ghế trống
            if (string.IsNullOrEmpty(gheDaChon))
            {
                return Content($"<script>alert('Bạn chưa chọn ghế ngồi!'); window.location.href='/Ve/ChonGhe?chuyenBayId={chuyenBayId}';</script>", "text/html; charset=utf-8");
            }

            // VÁ LỖI LOGIC: Kiểm tra chống giành giật ghế
            var cacVeDaBan = _context.Ves.Where(v => v.ChuyenBayId == chuyenBayId).ToList();
            var gheDangChon = gheDaChon.Split(',');

            foreach (var ve in cacVeDaBan)
            {
                var gheDaMua = ve.GheDaChon?.Split(',') ?? new string[0];
                if (gheDaMua.Intersect(gheDangChon).Any()) // Nếu phát hiện ghế bị trùng
                {
                    return Content($"<script>alert('Rất tiếc, ghế bạn chọn vừa có người khác nhanh tay đặt mất. Vui lòng chọn ghế khác nhé!'); window.location.href='/Ve/ChonGhe?chuyenBayId={chuyenBayId}';</script>", "text/html; charset=utf-8");
                }
            }

            // An toàn -> Tiến hành lưu vé
            var veMoi = new Ve
            {
                ChuyenBayId = chuyenBayId,
                HoTen = hoTen ?? "",
                SoDienThoai = soDienThoai ?? "",
                GheDaChon = gheDaChon,
                NgayDat = DateTime.Now
            };

            _context.Ves.Add(veMoi);
            _context.SaveChanges();

            TempData["ThongBao"] = $"Chúc mừng {hoTen} đã đặt vé thành công! Ghế của bạn: {gheDaChon}.";
            return RedirectToAction("ThanhCong");
        }

        public IActionResult ThanhCong() => View();
    }
}