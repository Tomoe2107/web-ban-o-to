using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace website_ban_o_to.Models
{
    public class Order
    {
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Mã đơn hàng là bắt buộc")]
        [StringLength(20, ErrorMessage = "Mã đơn hàng không được vượt quá 20 ký tự")]
        public string OrderCode { get; set; }

        [Required(ErrorMessage = "ID người dùng là bắt buộc")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Tên khách hàng là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên khách hàng không được vượt quá 100 ký tự")]
        public string CustomerName { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string CustomerEmail { get; set; }

        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string CustomerPhone { get; set; }

        [Required(ErrorMessage = "Tổng tiền là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Tổng tiền phải lớn hơn 0")]
        public decimal TotalAmount { get; set; }

        [StringLength(20, ErrorMessage = "Trạng thái đơn hàng không được vượt quá 20 ký tự")]
        public string OrderStatus { get; set; } = "Đang xử lý";

        public DateTime OrderDate { get; set; } = DateTime.Now;
        public DateTime? DeliveryDate { get; set; }
        public string Notes { get; set; }

        // Navigation property
        public User User { get; set; }
    }
}