<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UC_menu.ascx.cs" Inherits="website_ban_o_to.UC_menu" %>

<!-- Header Right -->
<div class="header-right">
    <asp:HyperLink ID="lnkDangNhap" runat="server" NavigateUrl="~/dangnhap1.aspx">Đăng nhập</asp:HyperLink> |
    <asp:HyperLink ID="lnkDangKy" runat="server" NavigateUrl="~/dangky1.aspx">Đăng ký</asp:HyperLink> |
    <asp:HyperLink ID="lnkLienHe" runat="server" NavigateUrl="~/lienhe1.aspx">Liên hệ</asp:HyperLink> |
    <asp:HyperLink ID="lnkQuanLy" runat="server" NavigateUrl="~/admin/quanly1.aspx">Quản lý</asp:HyperLink>
</div>

<nav class="navbar">
    <ul class="nav-menu">
        <li><asp:HyperLink ID="lnkTrangChu" NavigateUrl="~/trangchu1.aspx" Text="Trang chủ" runat="server" /></li>
        <li><asp:HyperLink ID="lnkTinMuaOto" NavigateUrl="~/tinmuaoto1.aspx" Text="Tin mua ô tô" runat="server" /></li>
        <li><asp:HyperLink ID="lnkBanOto" NavigateUrl="~/banoto1.aspx" Text="Bán ô tô" runat="server" /></li>
        <li><asp:HyperLink ID="lnkCanMua" NavigateUrl="~/giohang.aspx" Text="Giỏ hàng" runat="server" /></li>
    </ul>
</nav>
<script>
    window.addEventListener('scroll', function () {
        var header = document.querySelector('.header');
        if (window.scrollY > 10) {
            header.style.backgroundColor = '#a93226'; // đổi màu đậm hơn khi cuộn
            header.style.boxShadow = '0 2px 8px rgba(0,0,0,0.3)';
        } else {
            header.style.backgroundColor = '#c0392b';
            header.style.boxShadow = '0 2px 6px rgba(0,0,0,0.1)';
        }
    });
</script>
