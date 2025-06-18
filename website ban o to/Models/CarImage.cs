using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace website_ban_o_to.Models
{
    public class CarImage
    {
        public int ImageID { get; set; }

        [Required(ErrorMessage = "ID xe là bắt buộc")]
        public int CarID { get; set; }

        [Required(ErrorMessage = "Đường dẫn ảnh là bắt buộc")]
        [StringLength(255, ErrorMessage = "Đường dẫn ảnh không được vượt quá 255 ký tự")]
        public string ImagePath { get; set; }

        [StringLength(100, ErrorMessage = "Tên ảnh không được vượt quá 100 ký tự")]
        public string ImageName { get; set; }

        public bool IsMain { get; set; } = false;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation property
        public Car Car { get; set; }
    }
}