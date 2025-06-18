<%@ Page Title="Quản lý đơn hàng" Language="C#" MasterPageFile="~/admin/quantri.Master" AutoEventWireup="true" CodeBehind="quanlydonhang.aspx.cs" Inherits="website_ban_o_to.admin.quanlydonhang" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/admin/quanly.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <h2>Quản lý đơn hàng</h2>

    <asp:GridView ID="gvDonHang" runat="server" AutoGenerateColumns="False" CssClass="admin-grid"
    OnRowCommand="gvDonHang_RowCommand">
    <Columns>
        <asp:BoundField DataField="MaDon" HeaderText="Mã đơn" />
        <asp:BoundField DataField="NgayDat" HeaderText="Ngày đặt" DataFormatString="{0:dd/MM/yyyy}" />
        <asp:BoundField DataField="KhachHang" HeaderText="Khách hàng" />
        <asp:BoundField DataField="DiaChi" HeaderText="Địa chỉ" />
        <asp:BoundField DataField="TongTien" HeaderText="Tổng tiền (triệu)" DataFormatString="{0:N0}" />
        <asp:BoundField DataField="TrangThai" HeaderText="Trạng thái" />
        <asp:TemplateField HeaderText="Thao tác">
            <ItemTemplate>
                <asp:Button ID="btnXacNhan" runat="server" Text="Xác nhận" CommandName="XacNhan" CommandArgument='<%# Eval("MaDon") %>' CssClass="btn-confirm" />
                <asp:Button ID="btnHuy" runat="server" Text="Huỷ" CommandName="Huy" CommandArgument='<%# Eval("MaDon") %>' CssClass="btn-cancel" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

</asp:Content>
