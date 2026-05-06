/**
 * 1. File: ChuyenBay.cs
 * Mục đích / Chức năng: Định nghĩa cấu trúc dữ liệu cho đối tượng Chuyến bay trong hệ thống.
 * Người viết: pHạm Anh Tú
 * Thời gian sửa đổi: 06/05/2026
 * Phiên bản: 1.0
 */

using System;

namespace Banvemaybay.Models
{
    /**
     * 3. Class: ChuyenBay
     * Mục đích / Chức năng: Đại diện cho một chuyến bay cụ thể, chứa các thông tin về hành trình và giá cả.
     * Cấu trúc: Bao gồm các thuộc tính định danh, thời gian và thông tin tài chính.
     * Người viết / tg sửa đổi: pHạm Anh Tú - Cập nhật 06/05/2026
     */
    public class ChuyenBay
    {
        /** 
         * Ý nghĩa các biến (Properties):
         * Id: Mã định danh duy nhất (Primary Key) trong cơ sở dữ liệu.
         * MaChuyenBay: Mã hiệu chuyến bay (ví dụ: VJ123, VN456).
         * HangHangKhong: Tên hãng hàng không cung cấp chuyến bay.
         * DiemDiId / DiemDenId: Khóa ngoại liên kết tới bảng SanBay để xác định địa điểm.
         * ThoiGianDi / ThoiGianDen: Lịch trình thời gian dự kiến của chuyến bay.
         * GiaVe: Giá niêm yết chưa qua chiết khấu.
         * GiaKhuyenMai: Giá thực tế sau khi áp dụng các chương trình ưu đãi.
         */

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