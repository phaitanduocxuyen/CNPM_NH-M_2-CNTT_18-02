/**
 * 1. File: ErrorViewModel.cs
 * M?c ?ích / Ch?c n?ng: Ch?a thông tin v? l?i phát sinh trong h? th?ng ?? hi?n th? cho ng??i dùng ho?c ph?c v? debug.
 * Ng??i vi?t: pH?m Anh Tú
 * Th?i gian s?a ??i: 06/05/2026
 * Phiên b?n: 1.0
 */

namespace Banvemaybay.Models
{
    /**
     * 3. Class: ErrorViewModel
     * M?c ?ích / Ch?c n?ng: Model trung gian dùng ?? truy?n d? li?u l?i t? Controller sang View thông báo l?i.
     * C?u trúc: G?m mã ??nh danh yêu c?u và logic ki?m tra hi?n th?.
     * Ng??i vi?t / tg s?a ??i: pH?m Anh Tú - C?p nh?t 06/05/2026
     */
    public class ErrorViewModel
    {
        /** 
         * Ý ngh?a các bi?n (Properties):
         * RequestId: Mã ??nh danh duy nh?t cho m?i yêu c?u HTTP, giúp tra soát log l?i trong h? th?ng.
         * ShowRequestId: Thu?c tính logic (getter) dùng ?? quy?t ??nh xem có hi?n th? mã l?i lên giao di?n hay không.
         */

        public string? RequestId { get; set; }

        // 5. ?o?n logic: Ki?m tra ?i?u ki?n hi?n th?
        // Gi?i thích: Tr? v? true n?u RequestId có giá tr? (không null ho?c r?ng), ng??c l?i tr? v? false.
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}