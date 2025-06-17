<%@ Page Title="" Language="C#" MasterPageFile="~/home.Master" AutoEventWireup="true" CodeBehind="lienhe1.aspx.cs" Inherits="website_ban_o_to.lienhe1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/trangchu.css" rel="stylesheet" type="text/css" />
<link href="file:///c:\users\lenovo\onedrive\máy%20tính\website%20ban%20o%20to\website%20ban%20o%20to\lienhe.css" rel="stylesheet" /></asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
      <!-- Contact Form -->
    <div class="contact-container">
        <h1 class="contact-title">LIÊN HỆ VỚI CHÚNG TÔI</h1>
        
        <div class="form-group">
            <asp:Label ID="lblHoTen" runat="server" Text="Họ và tên"></asp:Label>
            <asp:TextBox ID="txtHoTen" runat="server" CssClass="form-control" placeholder="Nhập họ tên của bạn"></asp:TextBox>
        </div>
        
        <div class="form-group">
            <asp:Label ID="lblEmail" runat="server" Text="Email"></asp:Label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" placeholder="Nhập email của bạn"></asp:TextBox>
        </div>
        
        <div class="form-group">
            <asp:Label ID="lblDienThoai" runat="server" Text="Điện thoại"></asp:Label>
            <asp:TextBox ID="txtDienThoai" runat="server" CssClass="form-control" placeholder="Nhập số điện thoại"></asp:TextBox>
        </div>
        
        <div class="form-group">
            <asp:Label ID="lblNoiDung" runat="server" Text="Nội dung liên hệ"></asp:Label>
            <asp:TextBox ID="txtNoiDung" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="5" placeholder="Nhập nội dung bạn muốn liên hệ"></asp:TextBox>
        </div>
        
        <asp:Button ID="btnGuiLienHe" runat="server" Text="GỬI LIÊN HỆ" CssClass="btn-submit" OnClick="btnGuiLienHe_Click" />
        
        <asp:Label ID="lblThongBao" runat="server" CssClass="error-message" Visible="false"></asp:Label>
    </div>
</asp:Content>
