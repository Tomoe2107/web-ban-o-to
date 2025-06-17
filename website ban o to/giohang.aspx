<%@ Page Title="" Language="C#" MasterPageFile="~/home.Master" AutoEventWireup="true" CodeBehind="giohang.aspx.cs" Inherits="website_ban_o_to.giohang" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/giohang.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="cart-container">
        <h2>Giỏ hàng của bạn</h2>

        <asp:Repeater ID="rptCartItems" runat="server">
            <ItemTemplate>
                <div class="cart-item">
                    <img src='<%# Eval("ImageUrl") %>' class="cart-image" />
                    <div class="cart-info">
                        <h3><%# Eval("Name") %></h3>
                        <p>Giá: <strong><%# Eval("Price", "{0:N0} triệu") %></strong></p>
                        <asp:Button ID="btnRemove" runat="server" Text="Xóa" CommandArgument='<%# Eval("Id") %>' CssClass="btn-remove" OnCommand="btnRemove_Command" />
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>

        <div class="cart-summary">
            <p>Tổng cộng: <asp:Label ID="lblTongTien" runat="server" CssClass="total-price" /></p>
            <asp:Button ID="btnThanhToan" runat="server" Text="Thanh toán" CssClass="btn-checkout" OnClick="btnThanhToan_Click" />
        </div>
    </div>
</asp:Content>
