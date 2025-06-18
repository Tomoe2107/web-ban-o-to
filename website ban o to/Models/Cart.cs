using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace website_ban_o_to.Models
{
    public class Cart
    {
        public int CartID { get; set; }

        [Required(ErrorMessage = "ID người dùng là bắt buộc")]
        public int UserID { get; set; }

        [Required(ErrorMessage = "ID xe là bắt buộc")]
        public int CarID { get; set; }

        [Required(ErrorMessage = "Số lượng là bắt buộc")]
        [Range(1, int.MaxValue, ErrorMessage = "Số lượng phải lớn hơn 0")]
        public int Quantity { get; set; } = 1;

        public DateTime AddedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public User User { get; set; }
        public Car Car { get; set; }
    }

}