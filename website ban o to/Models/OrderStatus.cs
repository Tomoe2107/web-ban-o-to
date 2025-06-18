using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace website_ban_o_to.Models
{
    public static class OrderStatus
    {
        public const string Processing = "Đang xử lý";
        public const string Delivered = "Đã giao";
        public const string Cancelled = "Đã hủy";
    }
}