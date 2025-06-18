using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace website_ban_o_to.Models
{
   
        public class User
        {
            public int UserID { get; set; }

            [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
            [StringLength(50, ErrorMessage = "Tên đăng nhập không được vượt quá 50 ký tự")]
            public string Username { get; set; }

            [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
            [StringLength(255, ErrorMessage = "Mật khẩu không được vượt quá 255 ký tự")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Họ tên là bắt buộc")]
            [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự")]
            public string FullName { get; set; }

            [Required(ErrorMessage = "Email là bắt buộc")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            [StringLength(100, ErrorMessage = "Email không được vượt quá 100 ký tự")]
            public string Email { get; set; }

            [StringLength(20, ErrorMessage = "Số điện thoại không được vượt quá 20 ký tự")]
            public string Phone { get; set; }

            [StringLength(20, ErrorMessage = "Vai trò không được vượt quá 20 ký tự")]
            public string Role { get; set; } = "User";

            public bool IsAdmin { get; set; } = false;
            public bool IsActive { get; set; } = true;
            public DateTime CreatedDate { get; set; } = DateTime.Now;
            public DateTime? UpdatedDate { get; set; }
        }
    
}