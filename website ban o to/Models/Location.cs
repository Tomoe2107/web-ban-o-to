using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace website_ban_o_to.Models
{
    public class Location
    {
        public int LocationID { get; set; }

        [Required(ErrorMessage = "Tên khu vực là bắt buộc")]
        [StringLength(100, ErrorMessage = "Tên khu vực không được vượt quá 100 ký tự")]
        public string LocationName { get; set; }

        public bool IsActive { get; set; } = true;
    }

}