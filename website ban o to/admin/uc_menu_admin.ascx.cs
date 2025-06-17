using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace website_ban_o_to.admin
{
    public partial class uc_menu_admin : System.Web.UI.UserControl
    {
       protected void Page_Load(object sender, EventArgs e)
{
    string currentPage = System.IO.Path.GetFileName(Request.RawUrl).ToLower();

    lnkQuanLyXe.CssClass = (currentPage == "quanly1.aspx") ? "active" : "";
    lnkQuanLyUser.CssClass = (currentPage == "quanlyuser1.aspx") ? "active" : "";
    lnkQuanLyDonHang.CssClass = (currentPage == "quanlydonhang.aspx") ? "active" : "";
    lnkThongKe.CssClass = (currentPage == "thongke1.aspx") ? "active" : "";
}

    }
}