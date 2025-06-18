<%@ Page Title="" Language="C#" MasterPageFile="~/home.Master" AutoEventWireup="true" CodeBehind="chitietsanpham.aspx.cs" Inherits="website_ban_o_to.chitietsanpham" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/trangchu.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="product-detail-container">
        <asp:Image ID="imgCar" runat="server" CssClass="detail-image" />
        <div class="detail-info">
            <h2><asp:Label ID="lblName" runat="server" /></h2>
            <p><strong>Giá:</strong> <asp:Label ID="lblPrice" runat="server" /></p>
            <p><strong>Khu vực:</strong> <asp:Label ID="lblLocation" runat="server" /></p>
            <p><strong>Mô tả:</strong> <asp:Label ID="lblDescription" runat="server" /></p>
            <p><strong>Liên hệ:</strong> <asp:Label ID="lblContact" runat="server" /></p>
            <p><strong>Điện thoại:</strong> <asp:Label ID="lblPhone" runat="server" /></p>
        </div>
    </div>
</asp:Content>