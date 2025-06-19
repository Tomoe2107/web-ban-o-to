<%@ Page Title="Quản lý đơn hàng" Language="C#" MasterPageFile="~/admin/quantri.Master" AutoEventWireup="true" CodeBehind="quanlydonhang.aspx.cs" Inherits="website_ban_o_to.admin.quanlydonhang" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/admin/quanly.css" rel="stylesheet" />
    <style>
        /* Thống kê cards */
        .stats-container {
            display: flex;
            gap: 20px;
            margin-bottom: 30px;
            flex-wrap: wrap;
        }
        
        .stat-card {
            background: #fff;
            border-radius: 8px;
            padding: 20px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            flex: 1;
            min-width: 200px;
            text-align: center;
            border-left: 4px solid #007bff;
        }
        
        .stat-card.processing { border-left-color: #ffc107; }
        .stat-card.delivered { border-left-color: #28a745; }
        .stat-card.cancelled { border-left-color: #dc3545; }
        .stat-card.revenue { border-left-color: #17a2b8; }
        
        .stat-number {
            font-size: 2em;
            font-weight: bold;
            color: #333;
            margin-bottom: 5px;
        }
        
        .stat-label {
            color: #666;
            font-size: 0.9em;
        }
        
        /* Tìm kiếm */
        .search-container {
            background: #fff;
            padding: 20px;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            margin-bottom: 20px;
        }
        
        .search-row {
            display: flex;
            gap: 15px;
            align-items: end;
            flex-wrap: wrap;
            margin-bottom: 15px;
        }
        
        .search-group {
            display: flex;
            flex-direction: column;
            min-width: 150px;
        }
        
        .search-group label {
            margin-bottom: 5px;
            font-weight: 500;
            color: #333;
        }
        
        .search-group input,
        .search-group select {
            padding: 8px 12px;
            border: 1px solid #ddd;
            border-radius: 4px;
            font-size: 14px;
        }
        
        /* Buttons */
        .btn {
            padding: 8px 16px;
            border: none;
            border-radius: 4px;
            cursor: pointer;
            font-size: 14px;
            text-decoration: none;
            display: inline-block;
            margin: 2px;
        }
        
        .btn-primary { background: #007bff; color: white; }
        .btn-success { background: #28a745; color: white; }
        .btn-warning { background: #ffc107; color: #212529; }
        .btn-danger { background: #dc3545; color: white; }
        .btn-secondary { background: #6c757d; color: white; }
        
        .btn:hover {
            opacity: 0.8;
        }
        
        /* GridView styling */
        .admin-grid {
            width: 100%;
            border-collapse: collapse;
            background: #fff;
            border-radius: 8px;
            overflow: hidden;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }
        
        .admin-grid th {
            background: #f8f9fa;
            padding: 12px;
            text-align: left;
            font-weight: 600;
            border-bottom: 2px solid #dee2e6;
        }
        
        .admin-grid td {
            padding: 12px;
            border-bottom: 1px solid #dee2e6;
            color: #333;
        }
        
        .admin-grid tr:hover {
            background: #f8f9fa;
        }
        
        /* Status colors */
        .status-processing { background-color: #fff3cd !important; }
        .status-delivered { background-color: #d4edda !important; }
        .status-cancelled { background-color: #f8d7da !important; }
        
        /* Alert messages */
        .alert {
            padding: 10px 15px;
            margin: 10px 0;
            border-radius: 4px;
            border: 1px solid transparent;
        }
        
        .alert-success {
            color: #155724;
            background-color: #d4edda;
            border-color: #c3e6cb;
        }
        
        .alert-danger {
            color: #721c24;
            background-color: #f8d7da;
            border-color: #f5c6cb;
        }
        
        .alert-info {
            color: #0c5460;
            background-color: #d1ecf1;
            border-color: #bee5eb;
        }
        
        /* Responsive */
        @media (max-width: 768px) {
            .stats-container {
                flex-direction: column;
            }
            
            .search-row {
                flex-direction: column;
            }
            
            .search-group {
                min-width: 100%;
            }
        }

        /* Đảm bảo text trong GridView có màu đen */
        .admin-grid td,
        .admin-grid th {
            color: #333 !important;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container-fluid">
        <h2 style="margin-bottom: 30px; color: #333;">📊 Quản lý đơn hàng</h2>
        
        <!-- Thông báo -->
        <asp:Label ID="lblMessage" runat="server" Text="" />
        
        <!-- Thống kê tổng quan -->
        <div class="stats-container">
            <div class="stat-card">
                <div class="stat-number">
                    <asp:Label ID="lblTongDonHang" runat="server" Text="0" />
                </div>
                <div class="stat-label">Tổng đơn hàng</div>
            </div>
            
            <div class="stat-card processing">
                <div class="stat-number">
                    <asp:Label ID="lblDangXuLy" runat="server" Text="0" />
                </div>
                <div class="stat-label">Đang xử lý</div>
            </div>
            
            <div class="stat-card delivered">
                <div class="stat-number">
                    <asp:Label ID="lblDaGiao" runat="server" Text="0" />
                </div>
                <div class="stat-label">Đã giao</div>
            </div>
            
            <div class="stat-card cancelled">
                <div class="stat-number">
                    <asp:Label ID="lblDaHuy" runat="server" Text="0" />
                </div>
                <div class="stat-label">Đã hủy</div>
            </div>
            
            <div class="stat-card revenue">
                <div class="stat-number" style="font-size: 1.5em;">
                    <asp:Label ID="lblTongDoanhThu" runat="server" Text="0 VNĐ" />
                </div>
                <div class="stat-label">Tổng doanh thu</div>
            </div>
            
            <div class="stat-card revenue">
                <div class="stat-number" style="font-size: 1.5em;">
                    <asp:Label ID="lblDoanhThuThanhCong" runat="server" Text="0 VNĐ" />
                </div>
                <div class="stat-label">Doanh thu thành công</div>
            </div>
        </div>
        
        <!-- Tìm kiếm và lọc -->
        <div class="search-container">
            <h4 style="margin-bottom: 15px;">🔍 Tìm kiếm & Lọc</h4>
            
            <div class="search-row">
                <div class="search-group">
                    <label>Tìm kiếm:</label>
                    <asp:TextBox ID="txtTimKiem" runat="server" placeholder="Mã đơn, tên KH, SĐT..." />
                </div>
                
                <div class="search-group">
                    <label>Trạng thái:</label>
                    <asp:DropDownList ID="ddlTrangThai" runat="server">
                        <asp:ListItem Value="" Text="-- Tất cả --" />
                        <asp:ListItem Value="Đang xử lý" Text="Đang xử lý" />
                        <asp:ListItem Value="Đã giao" Text="Đã giao" />
                        <asp:ListItem Value="Đã hủy" Text="Đã hủy" />
                    </asp:DropDownList>
                </div>
                
                <div class="search-group">
                    <label>Từ ngày:</label>
                    <asp:TextBox ID="txtTuNgay" runat="server" TextMode="Date" />
                </div>
                
                <div class="search-group">
                    <label>Đến ngày:</label>
                    <asp:TextBox ID="txtDenNgay" runat="server" TextMode="Date" />
                </div>
                
                <div class="search-group">
                    <label>&nbsp;</label>
                    <div>
                        <asp:Button ID="btnTimKiem" runat="server" Text="Tìm kiếm" 
                            CssClass="btn btn-primary" OnClick="btnTimKiem_Click" />
                        <asp:Button ID="btnReset" runat="server" Text="Reset" 
                            CssClass="btn btn-secondary" OnClick="btnReset_Click" />
                        <asp:Button ID="btnXuatBaoCao" runat="server" Text="Xuất báo cáo" 
                            CssClass="btn btn-success" OnClick="btnXuatBaoCao_Click" />
                    </div>
                </div>
            </div>
        </div>
        
        <!-- Danh sách đơn hàng -->
        <div style="background: #fff; border-radius: 8px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1);">
            <asp:GridView ID="gvDonHang" runat="server" AutoGenerateColumns="False" 
                CssClass="admin-grid" OnRowCommand="gvDonHang_RowCommand" 
                OnRowDataBound="gvDonHang_RowDataBound" AllowPaging="True" PageSize="20">
                <Columns>
                    <asp:BoundField DataField="OrderCode" HeaderText="Mã đơn" />
                    <asp:BoundField DataField="OrderDate" HeaderText="Ngày đặt" 
                        DataFormatString="{0:dd/MM/yyyy HH:mm}" />
                    <asp:BoundField DataField="CustomerName" HeaderText="Khách hàng" />
                    <asp:BoundField DataField="CustomerPhone" HeaderText="Số điện thoại" />
                    <asp:BoundField DataField="TotalAmount" HeaderText="Tổng tiền" 
                        DataFormatString="{0:N0} VNĐ" />
                    <asp:BoundField DataField="OrderStatus" HeaderText="Trạng thái" />
                    <asp:BoundField DataField="DeliveryDate" HeaderText="Ngày giao" 
                        DataFormatString="{0:dd/MM/yyyy}" NullDisplayText="Chưa giao" />
                    
                    <asp:TemplateField HeaderText="Thao tác">
                        <ItemTemplate>
                            <asp:Button ID="btnXacNhan" runat="server" Text="Xác nhận" 
                                CommandName="XacNhan" CommandArgument='<%# Eval("OrderID") %>' 
                                CssClass="btn btn-warning" />
                            <asp:Button ID="btnGiaoHang" runat="server" Text="Giao hàng" 
                                CommandName="GiaoHang" CommandArgument='<%# Eval("OrderID") %>' 
                                CssClass="btn btn-success" />
                            <asp:Button ID="btnHuy" runat="server" Text="Hủy đơn" 
                                CommandName="Huy" CommandArgument='<%# Eval("OrderID") %>' 
                                CssClass="btn btn-danger" 
                                OnClientClick="return confirm('Bạn có chắc muốn hủy đơn hàng này?');" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
                
                <EmptyDataTemplate>
                    <div style="text-align: center; padding: 20px; color: #666;">
                        <i>📦 Không có đơn hàng nào</i>
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
        
        <!-- Thống kê theo tháng -->
        <div style="margin-top: 30px; background: #fff; border-radius: 8px; padding: 20px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);">
            <h4 style="margin-bottom: 15px;">📈 Thống kê 6 tháng gần đây</h4>
            
            <asp:GridView ID="gvThongKeThang" runat="server" AutoGenerateColumns="False" CssClass="admin-grid">
                <Columns>
                    <asp:TemplateField HeaderText="Tháng/Năm">
                        <ItemTemplate>
                            <%# string.Format("{0:00}/{1}", Eval("Thang"), Eval("Nam")) %>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:BoundField DataField="SoDonHang" HeaderText="Số đơn hàng" />
                    <asp:BoundField DataField="DoanhThu" HeaderText="Doanh thu" 
                        DataFormatString="{0:N0} VNĐ" />
                </Columns>
                
                <EmptyDataTemplate>
                    <div style="text-align: center; padding: 20px; color: #666;">
                        <i>📊 Chưa có dữ liệu thống kê</i>
                    </div>
                </EmptyDataTemplate>
            </asp:GridView>
        </div>
    </div>
</asp:Content>