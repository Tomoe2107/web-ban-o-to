<%@ Page Title="Thanh toán" Language="C#" MasterPageFile="~/home.Master" AutoEventWireup="true" CodeBehind="thanhtoan.aspx.cs" Inherits="website_ban_o_to.thanhtoan" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/trangchu.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="checkout-container">
        <h2>THÔNG TIN THANH TOÁN</h2>

        <!-- Hiển thị giỏ hàng -->
        <asp:GridView ID="gvGioHang" runat="server" AutoGenerateColumns="False" CssClass="admin-grid">
            <Columns>
                <asp:BoundField DataField="TenXe" HeaderText="Tên xe" />
                <asp:BoundField DataField="SoLuong" HeaderText="Số lượng" />
                <asp:BoundField DataField="Gia" HeaderText="Đơn giá (triệu)" DataFormatString="{0:N0}" />
                <asp:TemplateField HeaderText="Thành tiền (triệu)">
                    <ItemTemplate>
                        <%# (Convert.ToDecimal(Eval("SoLuong")) * Convert.ToDecimal(Eval("Gia"))).ToString("N0") %>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>

        <div class="form-group">
            <label>Họ tên:</label>
            <asp:TextBox ID="txtHoTen" runat="server" CssClass="form-control" />
        </div>
        <div class="form-group">
            <label>Địa chỉ:</label>
            <asp:TextBox ID="txtDiaChi" runat="server" CssClass="form-control" />
        </div>
        <div class="form-group">
            <label>Điện thoại:</label>
            <asp:TextBox ID="txtDienThoai" runat="server" CssClass="form-control" />
        </div>
        <div class="form-group">
            <label>Email:</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" />
        </div>

        <asp:Button ID="btnDatHang" runat="server" Text="Đặt hàng" CssClass="btn-checkout" OnClick="btnDatHang_Click" />
        <asp:Label ID="lblThongBao" runat="server" CssClass="error-message" Visible="false" />
    </div>
</asp:Content>
