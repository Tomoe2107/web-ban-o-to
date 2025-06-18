using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static website_ban_o_to.trangchu1;

namespace website_ban_o_to.Models
{
    public class Car
    {
        public int CarID { get; set; }

        [Required(ErrorMessage = "Hãng xe là bắt buộc")]
        public int BrandID { get; set; }

        public int? LocationID { get; set; }

        [Required(ErrorMessage = "Tên xe là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên xe không được vượt quá 100 ký tự")]
        public string CarName { get; set; }

        [Required(ErrorMessage = "Giá xe là bắt buộc")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá xe phải lớn hơn 0")]
        public decimal Price { get; set; }

        public string Description { get; set; }

        [Range(1900, 2100, ErrorMessage = "Năm sản xuất không hợp lệ")]
        public int? Year { get; set; }

        public bool IsAvailable { get; set; } = true;
        public bool IsDisplay { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        // Navigation properties
        public Brand Brand { get; set; }
        public Location Location { get; set; }
    }

}