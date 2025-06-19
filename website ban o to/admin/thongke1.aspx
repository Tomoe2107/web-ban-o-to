<%@ Page Title="Thống kê doanh thu" Language="C#" MasterPageFile="~/admin/quantri.Master" AutoEventWireup="true" CodeBehind="thongke1.aspx.cs" Inherits="website_ban_o_to.admin.thongke1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/admin/quanly.css" rel="stylesheet" type="text/css" />
    <style>
        /* Container chính */
        .admin-content {
            padding: 20px;
            background: #f8f9fa;
        }

        .admin-content h2 {
            color: #333;
            margin-bottom: 30px;
            font-size: 28px;
            border-bottom: 3px solid #007bff;
            padding-bottom: 10px;
        }

        /* Bộ lọc thống kê */
        .statistics-filter {
            background: #fff;
            padding: 25px;
            border-radius: 10px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
            margin-bottom: 30px;
        }

        .filter-row {
            display: flex;
            align-items: center;
            gap: 15px;
            margin-bottom: 15px;
            flex-wrap: wrap;
        }

        .filter-row label {
            font-weight: 600;
            color: #333;
            min-width: 100px;
        }

        .filter-dropdown {
            padding: 10px 15px;
            border: 2px solid #e0e0e0;
            border-radius: 8px;
            font-size: 14px;
            background: #fff;
            min-width: 150px;
            transition: border-color 0.3s ease;
        }

        .filter-dropdown:focus {
            border-color: #007bff;
            outline: none;
            box-shadow: 0 0 0 3px rgba(0, 123, 255, 0.1);
        }

        .view-btn {
            background: linear-gradient(135deg, #007bff, #0056b3);
            color: white;
            border: none;
            padding: 12px 30px;
            border-radius: 8px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s ease;
            box-shadow: 0 4px 15px rgba(0, 123, 255, 0.3);
        }

        .view-btn:hover {
            background: linear-gradient(135deg, #0056b3, #004085);
            transform: translateY(-2px);
            box-shadow: 0 6px 20px rgba(0, 123, 255, 0.4);
        }

        /* Container thống kê */
        .statistics-container {
            display: grid;
            grid-template-columns: 1fr;
            gap: 30px;
            margin-bottom: 30px;
        }

        .statistics-chart {
            background: #fff;
            padding: 25px;
            border-radius: 10px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
        }

        .statistics-chart h3 {
            color: #333;
            margin-bottom: 20px;
            font-size: 20px;
            border-left: 4px solid #28a745;
            padding-left: 15px;
        }

        .statistics-summary {
            background: #fff;
            padding: 25px;
            border-radius: 10px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
        }

        .statistics-summary h3 {
            color: #333;
            margin-bottom: 20px;
            font-size: 20px;
            border-left: 4px solid #ffc107;
            padding-left: 15px;
        }

        .summary-item {
            display: flex;
            justify-content: space-between;
            align-items: center;
            padding: 15px 0;
            border-bottom: 1px solid #eee;
        }

        .summary-item:last-child {
            border-bottom: none;
        }

        .summary-item label {
            font-weight: 600;
            color: #555;
        }

        .summary-value {
            font-weight: 700;
            font-size: 18px;
            color: #007bff;
        }

        /* Bảng thống kê */
        .statistics-table {
            background: #fff;
            padding: 25px;
            border-radius: 10px;
            box-shadow: 0 4px 15px rgba(0,0,0,0.1);
            margin-bottom: 30px;
        }

        .statistics-table h3 {
            color: #333;
            margin-bottom: 20px;
            font-size: 20px;
            border-left: 4px solid #dc3545;
            padding-left: 15px;
        }

        /* GridView styling */
        .admin-grid {
            width: 100%;
            border-collapse: collapse;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }

        .admin-grid th {
            background: linear-gradient(135deg, #343a40, #495057);
            color: white;
            padding: 15px 12px;
            text-align: left;
            font-weight: 600;
            font-size: 14px;
            text-transform: uppercase;
            letter-spacing: 0.5px;
        }

        .admin-grid td {
            padding: 12px;
            border-bottom: 1px solid #dee2e6;
            color: #333;
            background: #fff;
        }

        .admin-grid tr:nth-child(even) td {
            background: #f8f9fa;
        }

        .admin-grid tr:hover td {
            background: #e3f2fd;
        }

        /* Paging */
        .grid-pager {
            background: #f8f9fa;
            padding: 15px;
            text-align: center;
        }

        .grid-pager table {
            margin: 0 auto;
        }

        .grid-pager td {
            padding: 8px 12px;
        }

        .grid-pager a {
            background: #007bff;
            color: white;
            padding: 8px 12px;
            text-decoration: none;
            border-radius: 4px;
            margin: 0 2px;
        }

        .grid-pager a:hover {
            background: #0056b3;
        }

        .grid-pager span {
            background: #6c757d;
            color: white;
            padding: 8px 12px;
            border-radius: 4px;
            margin: 0 2px;
        }

        /* Chart styling */
        .chart-container {
            height: 400px;
            width: 100%;
        }

        /* Responsive */
        @media (max-width: 1200px) {
            .statistics-container {
                grid-template-columns: 1fr;
            }
        }

        @media (max-width: 768px) {
            .filter-row {
                flex-direction: column;
                align-items: flex-start;
            }

            .filter-row label {
                min-width: auto;
            }

            .filter-dropdown {
                width: 100%;
            }

            .admin-content {
                padding: 10px;
            }

            .statistics-filter,
            .statistics-chart,
            .statistics-summary,
            .statistics-table {
                padding: 15px;
            }
        }

        /* Icons cho các tiêu đề */
        .icon-filter::before { content: "🔍 "; }
        .icon-summary::before { content: "💰 "; }
        .icon-table::before { content: "📋 "; }
        .icon-bestseller::before { content: "🏆 "; }

        /* Animation cho cards */
        .statistics-filter,
        .statistics-summary,
        .statistics-table {
            animation: fadeInUp 0.6s ease-out;
        }

        @keyframes fadeInUp {
            from {
                opacity: 0;
                transform: translateY(30px);
            }
            to {
                opacity: 1;
                transform: translateY(0);
            }
        }

        /* Highlight cho số liệu quan trọng */
        .highlight-number {
            background: linear-gradient(135deg, #28a745, #20c997);
            color: white;
            padding: 5px 10px;
            border-radius: 15px;
            font-weight: bold;
        }

        .highlight-currency {
            background: linear-gradient(135deg, #007bff, #17a2b8);
            color: white;
            padding: 5px 10px;
            border-radius: 15px;
            font-weight: bold;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <main class="admin-content">
        <h2>📈 Thống kê doanh thu & báo cáo</h2>

        <!-- Bộ lọc thống kê -->
        <div class="statistics-filter">
            <h3 class="icon-filter">Bộ lọc thống kê</h3>
            
            <div class="filter-row">
                <label>Loại thống kê:</label>
                <asp:DropDownList ID="ddlLoaiThongKe" runat="server" CssClass="filter-dropdown" AutoPostBack="true" OnSelectedIndexChanged="ddlLoaiThongKe_SelectedIndexChanged">
                    <asp:ListItem Value="0" Selected="True">📅 Theo tháng</asp:ListItem>
                    <asp:ListItem Value="1">📆 Theo quý</asp:ListItem>
                    <asp:ListItem Value="2">🗓️ Theo năm</asp:ListItem>
                </asp:DropDownList>
            </div>
            
            <!-- Filter theo tháng -->
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
                    <asp:ListItem Value="2022">2022</asp:ListItem>
                    <asp:ListItem Value="2023">2023</asp:ListItem>
                    <asp:ListItem Value="2024">2024</asp:ListItem>
                    <asp:ListItem Value="2025" Selected="True">2025</asp:ListItem>
                    <asp:ListItem Value="2026">2026</asp:ListItem>
                </asp:DropDownList>
            </div>
            
            <!-- Filter theo quý -->
            <div class="filter-row" id="divQuy" runat="server" style="display:none;">
                <label>Quý:</label>
                <asp:DropDownList ID="ddlQuy" runat="server" CssClass="filter-dropdown">
                    <asp:ListItem Value="1">Quý 1 (T1-T3)</asp:ListItem>
                    <asp:ListItem Value="2">Quý 2 (T4-T6)</asp:ListItem>
                    <asp:ListItem Value="3">Quý 3 (T7-T9)</asp:ListItem>
                    <asp:ListItem Value="4">Quý 4 (T10-T12)</asp:ListItem>
                </asp:DropDownList>
                
                <label>Năm:</label>
                <asp:DropDownList ID="ddlNamQuy" runat="server" CssClass="filter-dropdown">
                    <asp:ListItem Value="2022">2022</asp:ListItem>
                    <asp:ListItem Value="2023">2023</asp:ListItem>
                    <asp:ListItem Value="2024">2024</asp:ListItem>
                    <asp:ListItem Value="2025" Selected="True">2025</asp:ListItem>
                    <asp:ListItem Value="2026">2026</asp:ListItem>
                </asp:DropDownList>
            </div>
            
            <!-- Filter theo năm -->
            <div class="filter-row" id="divNam" runat="server" style="display:none;">
                <label>Năm thống kê:</label>
                <asp:DropDownList ID="ddlNamThongKe" runat="server" CssClass="filter-dropdown">
                    <asp:ListItem Value="2022">2022</asp:ListItem>
                    <asp:ListItem Value="2023">2023</asp:ListItem>
                    <asp:ListItem Value="2024">2024</asp:ListItem>
                    <asp:ListItem Value="2025" Selected="True">2025</asp:ListItem>
                    <asp:ListItem Value="2026">2026</asp:ListItem>
                </asp:DropDownList>
            </div>
            
            <div class="filter-row">
                <asp:Button ID="btnXemThongKe" runat="server" Text="📊 Xem thống kê" CssClass="view-btn" OnClick="btnXemThongKe_Click" />
            </div>
        </div>
      
        <!-- Tổng hợp doanh thu -->
        <div class="statistics-container">
            <div class="statistics-summary">
                <h3 class="icon-summary">Tổng hợp doanh thu</h3>
                
                <div class="summary-item">
                    <label>💰 Tổng doanh thu:</label>
                    <asp:Label ID="lblTongDoanhThu" runat="server" CssClass="summary-value highlight-currency" Text="0 triệu" />
                </div>
                
                <div class="summary-item">
                    <label>📦 Tổng số đơn hàng:</label>
                    <asp:Label ID="lblTongDonHang" runat="server" CssClass="summary-value highlight-number" Text="0" />
                </div>
                
                <div class="summary-item">
                    <label>📈 Đơn hàng cao nhất:</label>
                    <asp:Label ID="lblDonHangCaoNhat" runat="server" CssClass="summary-value" Text="0 triệu" />
                </div>
                
                <div class="summary-item">
                    <label>📉 Đơn hàng thấp nhất:</label>
                    <asp:Label ID="lblDonHangThapNhat" runat="server" CssClass="summary-value" Text="0 triệu" />
                </div>
            </div>
        </div>
        
        <!-- Bảng chi tiết đơn hàng -->
        <div class="statistics-table">
            <h3 class="icon-table">Chi tiết đơn hàng</h3>
            <asp:GridView ID="gvChiTietDonHang" runat="server" AutoGenerateColumns="False" CssClass="admin-grid" 
                AllowPaging="True" PageSize="15" OnPageIndexChanging="gvChiTietDonHang_PageIndexChanging">
                <Columns>
                    <asp:BoundField DataField="MaDon" HeaderText="Mã đơn hàng" />
                    <asp:BoundField DataField="NgayDat" HeaderText="Ngày đặt" DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    <asp:BoundField DataField="KhachHang" HeaderText="Khách hàng" />
                    <asp:BoundField DataField="TongTien" HeaderText="Tổng tiền" DataFormatString="{0:N0} triệu" />
                    <asp:BoundField DataField="TrangThai" HeaderText="Trạng thái" />
                </Columns>
                <PagerStyle CssClass="grid-pager" />
                <EmptyDataTemplate>
                    <div style="text-align: center; padding: 30px; color: #666; font-style: italic;">
                        📭 Không có đơn hàng nào trong khoảng thời gian này
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>

        <!-- Top sản phẩm bán chạy -->
        <div class="statistics-table">
            <h3 class="icon-bestseller">Top sản phẩm bán chạy</h3>
            <asp:GridView ID="gvSanPhamBanChay" runat="server" AutoGenerateColumns="False" CssClass="admin-grid">
                <Columns>
                    <asp:TemplateField HeaderText="Thứ hạng">
                        <ItemTemplate>
                            <div style="text-align: center; font-weight: bold; color: #007bff;">
                                #<%# Container.DataItemIndex + 1 %>
                            </div>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="TenXe" HeaderText="Tên xe" />
                    <asp:BoundField DataField="SoLuong" HeaderText="Số lượng bán" DataFormatString="{0:N0}" />
                    <asp:BoundField DataField="DoanhThu" HeaderText="Doanh thu" DataFormatString="{0:N0} triệu" />
                </Columns>
                <EmptyDataTemplate>
                    <div style="text-align: center; padding: 30px; color: #666; font-style: italic;">
                        🚗 Chưa có sản phẩm bán chạy trong khoảng thời gian này
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </main>

    <script type="text/javascript">
        // Animation cho các card khi load trang
        window.addEventListener('load', function () {
            const cards = document.querySelectorAll('.statistics-filter, .statistics-summary, .statistics-table');
            cards.forEach((card, index) => {
                setTimeout(() => {
                    card.style.opacity = '1';
                    card.style.transform = 'translateY(0)';
                }, index * 100);
            });
        });

        // Hiệu ứng hover cho summary items
        document.addEventListener('DOMContentLoaded', function () {
            const summaryItems = document.querySelectorAll('.summary-item');
            summaryItems.forEach(item => {
                item.addEventListener('mouseenter', function () {
                    this.style.backgroundColor = '#f8f9fa';
                    this.style.transform = 'translateX(5px)';
                    this.style.transition = 'all 0.3s ease';
                });

                item.addEventListener('mouseleave', function () {
                    this.style.backgroundColor = 'transparent';
                    this.style.transform = 'translateX(0)';
                });
            });
        });
    </script>
</asp:Content>