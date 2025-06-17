using System;

namespace website_ban_o_to.admin
{
    public partial class quanly : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Kiểm tra đăng nhập và vai trò
                if (Session["TaiKhoan"] == null || Session["VaiTro"]?.ToString() != "Admin")
                {
                    Response.Redirect("~/dangnhap.aspx");
                }
                else
                {
                    lblWelcome.Text = "Xin chào quản trị viên: " + Session["TaiKhoan"];
                }
            }
        }
    }
}
