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
            // Thiết lập active menu
            SetActiveMenu();

            // Kiểm tra trạng thái đăng nhập và quyền
            CheckUserAuthentication();

            // Cập nhật số lượng giỏ hàng
            UpdateCartCount();
        }

        private void SetActiveMenu()
        {
            string currentPage = System.IO.Path.GetFileName(Request.RawUrl).ToLower();

            // Reset tất cả active class
            lnkTrangChu.CssClass = "";
            lnkTinMuaOto.CssClass = "";
            lnkBanOto.CssClass = "";
            lnkCanMua.CssClass = "";

            // Set active class cho menu hiện tại
            if (currentPage.Contains("trangchu1") || currentPage == "" || currentPage == "default.aspx")
            {
                lnkTrangChu.CssClass = "active";
            }
            else if (currentPage.Contains("tinmuaoto1"))
            {
                lnkTinMuaOto.CssClass = "active";
            }
            else if (currentPage.Contains("banoto1"))
            {
                lnkBanOto.CssClass = "active";
            }
            else if (currentPage.Contains("giohang"))
            {
                lnkCanMua.CssClass = "active";
            }
        }

        private void CheckUserAuthentication()
        {
            // Kiểm tra trạng thái đăng nhập - có thể dùng Session hoặc cookie
            bool isLoggedIn = false;
            bool isAdmin = false;
            string username = "";

            // Cách 1: Kiểm tra qua Session
            if (Session["UserID"] != null)
            {
                isLoggedIn = true;
                username = Session["Username"]?.ToString() ?? "User";
                isAdmin = Session["isAdmin"] != null && (bool)Session["isAdmin"];
            }

            // Cách 2: Kiểm tra qua Cookie (nếu có remember login)
            else if (Request.Cookies["UserLogin"] != null)
            {
                var userCookie = Request.Cookies["UserLogin"];
                if (!string.IsNullOrEmpty(userCookie.Value))
                {
                    isLoggedIn = true;
                    username = userCookie["Username"] ?? "User";
                    isAdmin = userCookie["IsAdmin"] == "True";
                }
            }

            if (isLoggedIn)
            {
                // ========== NGƯỜI DÙNG ĐÃ ĐĂNG NHẬP ==========

                // ẨN: Đăng nhập, Đăng ký
                lnkDangNhap.Visible = false;
                lnkDangKy.Visible = false;
                separator1.Visible = false; // Ẩn dấu | giữa đăng nhập và đăng ký

                // HIỆN: Thông tin user và nút đăng xuất
                lblUserInfo.Visible = true;
                lblUserInfo.Text = $"Xin chào, <strong>{username}</strong>";
                lnkDangXuat.Visible = true;

                // HIỆN menu Quản lý nếu là admin
                if (isAdmin)
                {
                    lnkQuanLy.Visible = true;
                    lnkQuanLy.Text = "🛠️ Quản lý";
                    // Hiện separator trước menu quản lý
                    if (Page.FindControl("adminSeparator") != null)
                    {
                        ((System.Web.UI.HtmlControls.HtmlGenericControl)Page.FindControl("adminSeparator")).Visible = true;
                    }
                }
                else
                {
                    lnkQuanLy.Visible = false;
                    if (Page.FindControl("adminSeparator") != null)
                    {
                        ((System.Web.UI.HtmlControls.HtmlGenericControl)Page.FindControl("adminSeparator")).Visible = false;
                    }
                }
            }
            else
            {
                // ========== NGƯỜI DÙNG CHƯA ĐĂNG NHẬP ==========

                // HIỆN: Đăng nhập, Đăng ký
                lnkDangNhap.Visible = true;
                lnkDangKy.Visible = true;
                separator1.Visible = true; // Hiện dấu | giữa đăng nhập và đăng ký

                // ẨN: Thông tin user, đăng xuất, quản lý
                lblUserInfo.Visible = false;
                lnkDangXuat.Visible = false;
                lnkQuanLy.Visible = false;
            }
        }

        private void UpdateCartCount()
        {
            // Lấy số lượng sản phẩm trong giỏ hàng từ Session
            int cartCount = 0;

            if (Session["CartCount"] != null)
            {
                cartCount = (int)Session["CartCount"];
            }
            else if (Session["Cart"] != null)
            {
                // Nếu có session Cart nhưng chưa có CartCount
                var cart = Session["Cart"] as Dictionary<int, int>; // Dictionary<CarID, Quantity>
                if (cart != null)
                {
                    cartCount = cart.Values.Sum();
                    Session["CartCount"] = cartCount;
                }
            }

            // Hiển thị số lượng trên menu giỏ hàng
            if (cartCount > 0)
            {
                lnkCanMua.Text = $"🛒 Giỏ hàng ({cartCount})";
                spanCartBadge.Visible = true;
                spanCartBadge.InnerText = cartCount.ToString();
            }
            else
            {
                lnkCanMua.Text = "🛒 Giỏ hàng";
                spanCartBadge.Visible = false;
            }
        }

        protected void lnkDangXuat_Click(object sender, EventArgs e)
        {
            try
            {
                // Xóa tất cả session
                Session.Clear();
                Session.Abandon();

                // Xóa cookie login nếu có
                if (Request.Cookies["UserLogin"] != null)
                {
                    HttpCookie userCookie = new HttpCookie("UserLogin");
                    userCookie.Expires = DateTime.Now.AddDays(-1); // Set expired
                    Response.Cookies.Add(userCookie);
                }

                // Redirect về trang chủ với thông báo
                Response.Redirect("~/trangchu1.aspx?logout=success");
            }
            catch (Exception ex)
            {
                // Log lỗi nếu cần
                Response.Redirect("~/trangchu1.aspx");
            }
        }

        // Method để check login từ bên ngoài
        public bool IsUserLoggedIn()
        {
            return Session["UserID"] != null ||
                   (Request.Cookies["UserLogin"] != null && !string.IsNullOrEmpty(Request.Cookies["UserLogin"].Value));
        }

        // Method để get thông tin user hiện tại
        public string GetCurrentUsername()
        {
            if (Session["Username"] != null)
            {
                return Session["Username"].ToString();
            }
            else if (Request.Cookies["UserLogin"] != null)
            {
                return Request.Cookies["UserLogin"]["Username"] ?? "";
            }
            return "";
        }

        // Method để check quyền admin
        public bool IsAdmin()
        {
            if (Session["isAdmin"] != null)
            {
                return (bool)Session["isAdmin"];
            }
            else if (Request.Cookies["UserLogin"] != null)
            {
                return Request.Cookies["UserLogin"]["IsAdmin"] == "True";
            }
            return false;
        }

        // Method public để các trang khác có thể gọi để cập nhật giỏ hàng
        public void RefreshCartCount()
        {
            UpdateCartCount();
        }

        // Method để highlight menu theo path tùy chỉnh
        public void SetActiveMenuByPath(string menuName)
        {
            // Reset tất cả
            lnkTrangChu.CssClass = "";
            lnkTinMuaOto.CssClass = "";
            lnkBanOto.CssClass = "";
            lnkCanMua.CssClass = "";

            // Set active theo tên menu
            switch (menuName.ToLower())
            {
                case "trangchu":
                    lnkTrangChu.CssClass = "active";
                    break;
                case "tinmuaoto":
                    lnkTinMuaOto.CssClass = "active";
                    break;
                case "banoto":
                    lnkBanOto.CssClass = "active";
                    break;
                case "giohang":
                    lnkCanMua.CssClass = "active";
                    break;
            }
        }
    }
}