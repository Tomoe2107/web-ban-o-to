using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace website_ban_o_to
{
    public partial class UC_menu : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string currentPage = System.IO.Path.GetFileName(Request.RawUrl).ToLower();

            if (currentPage.Contains("trangchu1")) lnkTrangChu.CssClass = "active";
            else if (currentPage.Contains("tinmuaoto1")) lnkTinMuaOto.CssClass = "active";
            else if (currentPage.Contains("banoto1")) lnkBanOto.CssClass = "active";
            else if (currentPage.Contains("giohang")) lnkCanMua.CssClass = "active";
            // Có thể kiểm tra session để bật/tắt link quản lý
            if (Session["isAdmin"] != null && (bool)Session["isAdmin"])
            {
                lnkQuanLy.Visible = true;
            }
        }
    }
}