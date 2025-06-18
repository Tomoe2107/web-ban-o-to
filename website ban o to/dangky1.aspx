<%@ Page Title="" Language="C#" MasterPageFile="~/home.Master" AutoEventWireup="true" CodeBehind="dangky1.aspx.cs" Inherits="website_ban_o_to.dangky1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Đăng ký thành viên</title>
    <link href="~/trangchu.css" rel="stylesheet" type="text/css" />
<link href="~\dangky.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    
    <!-- Registration Form -->
    <div class="register-container">
        <h1 class="register-title">ĐĂNG KÝ THÀNH VIÊN</h1>

        <div class="form-group">
            <asp:Label ID="lblHoTen" runat="server" Text="Họ và tên:" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="txtHoTen" runat="server" CssClass="form-control" placeholder="Nhập họ tên đầy đủ"></asp:TextBox>
        </div>

        <div class="form-group">
            <asp:Label ID="lblEmail" runat="server" Text="Email:" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" placeholder="example@email.com"></asp:TextBox>
        </div>

        <div class="form-group">
            <asp:Label ID="lblDienThoai" runat="server" Text="Điện thoại:" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="txtDienThoai" runat="server" CssClass="form-control" placeholder="0987 654 321"></asp:TextBox>
        </div>

        <div class="form-group">
            <asp:Label ID="lblTenDangNhap" runat="server" Text="Tên đăng nhập:" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="txtTenDangNhap" runat="server" CssClass="form-control" placeholder="Từ 6-20 ký tự"></asp:TextBox>
        </div>

        <div class="form-group">
            <asp:Label ID="lblMatKhau" runat="server" Text="Mật khẩu:" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="txtMatKhau" runat="server" TextMode="Password" CssClass="form-control" placeholder="Ít nhất 8 ký tự"></asp:TextBox>
        </div>

        <div class="form-group">
            <asp:Label ID="lblXacNhanMatKhau" runat="server" Text="Xác nhận mật khẩu:" CssClass="form-label"></asp:Label>
            <asp:TextBox ID="txtXacNhanMatKhau" runat="server" TextMode="Password" CssClass="form-control" placeholder="Nhập lại mật khẩu"></asp:TextBox>
        </div>

        <asp:Button ID="btnDangKy" runat="server" Text="ĐĂNG KÝ" CssClass="btn-register" OnClick="btnDangKy_Click" />

        <div class="login-link">
            Đã có tài khoản? <a href="dangnhap1.aspx">Đăng nhập ngay</a>
        </div>

        <asp:Label ID="lblThongBao" runat="server" CssClass="error-message" Visible="false"></asp:Label>
    </div>


</asp:Content>
