<%@ Page Title="" Language="C#" MasterPageFile="~/home.Master" AutoEventWireup="true" CodeBehind="chitiettintuc.aspx.cs" Inherits="website_ban_o_to.chitiettintuc" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/chitiettintuc.css" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="news-detail-container">
        <asp:Label ID="lblTieuDe" runat="server" CssClass="news-title" />
        <asp:Label ID="lblNgayDang" runat="server" CssClass="news-date" />
        <asp:Image ID="imgTinTuc" runat="server" CssClass="news-image" />
        <asp:Label ID="lblNoiDung" runat="server" CssClass="news-content" />
    </div>
</asp:Content>