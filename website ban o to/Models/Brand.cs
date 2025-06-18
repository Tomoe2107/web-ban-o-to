using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace website_ban_o_to.Models
{
    public class Brand
    {
        public int BrandID { get; set; }

        [Required(ErrorMessage = "Tên hãng xe là bắt buộc")]
        [StringLength(50, ErrorMessage = "Tên hãng xe không được vượt quá 50 ký tự")]
        public string BrandName { get; set; }

        public bool IsActive { get; set; } = true;
    }
}