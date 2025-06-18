using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace website_ban_o_to.Models
{
    public class News
    {
        public int NewsID { get; set; }

        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Slug là bắt buộc")]
        [StringLength(200, ErrorMessage = "Slug không được vượt quá 200 ký tự")]
        public string Slug { get; set; }

        [StringLength(500, ErrorMessage = "Tóm tắt không được vượt quá 500 ký tự")]
        public string Summary { get; set; }

        public string Content { get; set; }

        [StringLength(255, ErrorMessage = "Đường dẫn ảnh không được vượt quá 255 ký tự")]
        public string ImagePath { get; set; }

        public bool IsPublished { get; set; } = true;
        public DateTime PublishedDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Người tạo là bắt buộc")]
        public int CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }

        // Navigation property
        public User Creator { get; set; }
    }
}