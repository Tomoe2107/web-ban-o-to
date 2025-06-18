<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="uc_menu_admin.ascx.cs" Inherits="website_ban_o_to.admin.uc_menu_admin" %>
<aside class="admin-sidebar">
    <h3>Menu quản lý</h3>
    <ul class="admin-menu">
        <li>
            <asp:HyperLink ID="lnkQuanLyXe" runat="server" NavigateUrl="~/admin/quanly1.aspx" CssClass="active">
                Quản lý xe
            </asp:HyperLink>
        </li>
        <li>
            <asp:HyperLink ID="lnkQuanLyUser" runat="server" NavigateUrl="~/admin/quanlyuser1.aspx">
                Quản lý người dùng
            </asp:HyperLink>
        </li>
        <li>
            <asp:HyperLink ID="lnkQuanLyDonHang" runat="server" NavigateUrl="~/admin/quanlydonhang.aspx">
                Quản lý đơn hàng
            </asp:HyperLink>
        </li>
        <li>
            <asp:HyperLink ID="lnkThongKe" runat="server" NavigateUrl="~/admin/thongke1.aspx">
                Thống kê
            </asp:HyperLink>
        </li>
    </ul>
    <script>
    document.addEventListener("DOMContentLoaded", function () {
        const menuItems = document.querySelectorAll(".admin-menu li");

        menuItems.forEach((item, index) => {
            item.style.opacity = 0;
            item.style.transform = "translateX(-20px)";
            item.style.transition = "all 0.4s ease";
        });

        // Animate khi trang load
        setTimeout(() => {
            menuItems.forEach((item, index) => {
                setTimeout(() => {
                    item.style.opacity = 1;
                    item.style.transform = "translateX(0)";
                }, index * 100);
            });
        }, 200);
    });
    </script>

</aside>