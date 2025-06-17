<%@ Page Title="" Language="C#" MasterPageFile="~/home.Master" AutoEventWireup="true" CodeBehind="banoto1.aspx.cs" Inherits="website_ban_o_to.banoto1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/trangchu.css" rel="stylesheet" />
    <link href="~/banoto.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <main class="content">
            <h2>Thu mua ô tô cũ - Đăng tin bán xe</h2>

            <!-- Mẫu thông tin đăng xe -->
            <asp:Panel ID="pnlBanXe" runat="server" CssClass="sell-form">
                <div class="form-group">
                    <label>Tên xe:</label>
                    <asp:TextBox ID="txtTenXe" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group">
                    <label>Giá mong muốn (triệu):</label>
                    <asp:TextBox ID="txtGia" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group">
                    <label>Mô tả chi tiết:</label>
                    <asp:TextBox ID="txtMoTa" runat="server" TextMode="MultiLine" Rows="4" CssClass="form-control" />
                </div>
                <div class="form-group">
                    <label>Hình ảnh xe:</label>
                    <asp:FileUpload ID="fuHinhAnh" runat="server" CssClass="form-control" />
                </div>
                <div class="form-group">
                    <label>Thông tin liên hệ:</label>
                    <asp:TextBox ID="txtLienHe" runat="server" CssClass="form-control" />
                </div>
                <asp:Button ID="btnDangTin" runat="server" Text="Đăng tin bán xe" CssClass="btn-submit" OnClick="btnDangTin_Click" />
            </asp:Panel>

            <asp:Label ID="lblThongBao" runat="server" CssClass="message-label" />
        </main>
    </div>
</asp:Content>