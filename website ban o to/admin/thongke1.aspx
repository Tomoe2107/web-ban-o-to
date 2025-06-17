<%@ Page Title="" Language="C#" MasterPageFile="~/admin/quantri.Master" AutoEventWireup="true" CodeBehind="thongke1.aspx.cs" Inherits="website_ban_o_to.admin.thongke1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/admin/quanly.css" rel="stylesheet" type="text/css" />
    <link href="~/trangchu.css" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="admin-content">
        <h2>Thống kê doanh thu</h2>

        <!-- Bộ lọc thống kê -->
        <div class="statistics-filter">
            <div class="filter-row">
                <label>Loại thống kê:</label>
                <asp:DropDownList ID="ddlLoaiThongKe" runat="server" CssClass="filter-dropdown" AutoPostBack="true" OnSelectedIndexChanged="ddlLoaiThongKe_SelectedIndexChanged">
                    <asp:ListItem Value="0" Selected="True">Theo tháng</asp:ListItem>
                    <asp:ListItem Value="1">Theo quý</asp:ListItem>
                    <asp:ListItem Value="2">Theo năm</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="filter-row" id="divThang" runat="server">
                <label>Tháng:</label>
                <asp:DropDownList ID="ddlThang" runat="server" CssClass="filter-dropdown">
                    <asp:ListItem Value="1">Tháng 1</asp:ListItem>
                    <asp:ListItem Value="2">Tháng 2</asp:ListItem>
                    <asp:ListItem Value="3">Tháng 3</asp:ListItem>
                    <asp:ListItem Value="4">Tháng 4</asp:ListItem>
                    <asp:ListItem Value="5">Tháng 5</asp:ListItem>
                    <asp:ListItem Value="6">Tháng 6</asp:ListItem>
                    <asp:ListItem Value="7">Tháng 7</asp:ListItem>
                    <asp:ListItem Value="8">Tháng 8</asp:ListItem>
                    <asp:ListItem Value="9">Tháng 9</asp:ListItem>
                    <asp:ListItem Value="10">Tháng 10</asp:ListItem>
                    <asp:ListItem Value="11">Tháng 11</asp:ListItem>
                    <asp:ListItem Value="12">Tháng 12</asp:ListItem>
                </asp:DropDownList>
                <label>Năm:</label>
                <asp:DropDownList ID="ddlNam" runat="server" CssClass="filter-dropdown">
                    <asp:ListItem Value="2023">2023</asp:ListItem>
                    <asp:ListItem Value="2024">2024</asp:ListItem>
                    <asp:ListItem Value="2025" Selected="True">2025</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="filter-row" id="divQuy" runat="server" style="display:none;">
                <label>Quý:</label>
                <asp:DropDownList ID="ddlQuy" runat="server" CssClass="filter-dropdown">
                    <asp:ListItem Value="1">Quý 1</asp:ListItem>
                    <asp:ListItem Value="2">Quý 2</asp:ListItem>
                    <asp:ListItem Value="3">Quý 3</asp:ListItem>
                    <asp:ListItem Value="4">Quý 4</asp:ListItem>
                </asp:DropDownList>
                <label>Năm:</label>
                <asp:DropDownList ID="ddlNamQuy" runat="server" CssClass="filter-dropdown">
                    <asp:ListItem Value="2023">2023</asp:ListItem>
                    <asp:ListItem Value="2024">2024</asp:ListItem>
                    <asp:ListItem Value="2025" Selected="True">2025</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="filter-row" id="divNam" runat="server" style="display:none;">
                <label>Năm:</label>
                <asp:DropDownList ID="ddlNamThongKe" runat="server" CssClass="filter-dropdown">
                    <asp:ListItem Value="2023">2023</asp:ListItem>
                    <asp:ListItem Value="2024">2024</asp:ListItem>
                    <asp:ListItem Value="2025" Selected="True">2025</asp:ListItem>
                </asp:DropDownList>
            </div>
            <div class="filter-row">
                <asp:Button ID="btnXemThongKe" runat="server" Text="Xem thống kê" CssClass="view-btn" OnClick="btnXemThongKe_Click" />
            </div>
        </div>
      
        <!-- Biểu đồ và bảng thống kê -->
        <div class="statistics-container">
            <div class="statistics-chart">
                <h3>Biểu đồ doanh thu</h3>
                <asp:Chart ID="chartDoanhThu"
    <Series>
        <asp:Series Name="DoanhThu" ChartType="Column"></asp:Series>
    </Series>
    <ChartAreas>
        <asp:ChartArea Name="ChartArea1"></asp:ChartArea>
    </ChartAreas>
</asp:Chart>
            </div>
            <div class="statistics-summary">
                <h3>Tổng hợp doanh thu</h3>
                <div class="summary-item">
                    <label>Tổng doanh thu:</label>
                    <asp:Label ID="lblTongDoanhThu" runat="server" CssClass="summary-value" />
                </div>
                <div class="summary-item">
                    <label>Tổng số đơn hàng:</label>
                    <asp:Label ID="lblTongDonHang" runat="server" CssClass="summary-value" />
                </div>
                <div class="summary-item">
                    <label>Đơn hàng cao nhất:</label>
                    <asp:Label ID="lblDonHangCaoNhat" runat="server" CssClass="summary-value" />
                </div>
                <div class="summary-item">
                    <label>Đơn hàng thấp nhất:</label>
                    <asp:Label ID="lblDonHangThapNhat" runat="server" CssClass="summary-value" />
                </div>
            </div>
        </div>
        
        <!-- Bảng chi tiết đơn hàng -->
        <div class="statistics-table">
            <h3>Chi tiết đơn hàng</h3>
            <asp:GridView ID="gvChiTietDonHang" runat="server" AutoGenerateColumns="False" CssClass="admin-grid" AllowPaging="True" PageSize="10" OnPageIndexChanging="gvChiTietDonHang_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="MaDon" HeaderText="Mã đơn" />
                    <asp:BoundField DataField="NgayDat" HeaderText="Ngày đặt" DataFormatString="{0:dd/MM/yyyy}" />
                    <asp:BoundField DataField="KhachHang" HeaderText="Khách hàng" />
                    <asp:BoundField DataField="TongTien" HeaderText="Tổng tiền (triệu)" DataFormatString="{0:N0}" />
                    <asp:BoundField DataField="TrangThai" HeaderText="Trạng thái" />
                </Columns>
                <PagerStyle CssClass="grid-pager" />
            </asp:GridView>
        </div>

        <!-- Bán chạy -->
        <div class="statistics-table">
            <h3>Top sản phẩm bán chạy</h3>
            <asp:GridView ID="gvSanPhamBanChay" runat="server" AutoGenerateColumns="False" CssClass="admin-grid">
                <Columns>
                    <asp:BoundField DataField="TenXe" HeaderText="Tên xe" />
                    <asp:BoundField DataField="SoLuong" HeaderText="Số lượng bán" />
                    <asp:BoundField DataField="DoanhThu" HeaderText="Doanh thu (triệu)" DataFormatString="{0:N0}" />
                </Columns>
            </asp:GridView>
        </div>
    </main>
</asp:Content>

