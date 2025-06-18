using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace website_ban_o_to.Models
{
    public class UsedCarPost
    {
        public int PostID { get; set; }

        public int? UserID { get; set; }

        [Required(ErrorMessage = "Tên xe là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên xe không được vượt quá 100 ký tự")]
        public string CarName { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Giá mong muốn phải lớn hơn 0")]
        public decimal? ExpectedPrice { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessage = "Tên người liên hệ là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên người liên hệ không được vượt quá 100 ký tự")]
        public string ContactName { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
        public string ContactEmail { get; set; }

        [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
        public string ContactPhone { get; set; }

        [StringLength(255, ErrorMessage = "Đường dẫn ảnh không được vượt quá 255 ký tự")]
        public string ImagePath { get; set; }

        public bool IsApproved { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ApprovedDate { get; set; }

        // Navigation property
        public User User { get; set; }
    }
}