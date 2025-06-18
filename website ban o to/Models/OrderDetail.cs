using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace website_ban_o_to.Models
{
    public class OrderDetail
    {
        public int OrderDetailID { get; set; }

        [Required(ErrorMessage = "ID đơn hàng là bắt buộc")]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "ID xe là bắt buộc")]
        public int CarID { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; } = 1;

        [Required(ErrorMessage = "Đơn giá là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Đơn giá phải lớn hơn 0")]
        public decimal UnitPrice { get; set; }

        [Required(ErrorMessage = "Thành tiền là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Thành tiền phải lớn hơn 0")]
        public decimal TotalPrice { get; set; }

        // Navigation properties
        public Order Order { get; set; }
        public Car Car { get; set; }
    }

}