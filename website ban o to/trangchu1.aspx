<%@ Page Title="" Language="C#" MasterPageFile="~/home.Master" AutoEventWireup="true" CodeBehind="trangchu1.aspx.cs" Inherits="website_ban_o_to.trangchu1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/trangchu.css" rel="stylesheet" />
    <style>
        /* Thêm CSS cho thông báo */
        .alert {
            padding: 10px 15px;
            margin: 10px 0;
            border-radius: 4px;
            display: none;
        }
        .alert-success {
            background-color: #d4edda;
            border-color: #c3e6cb;
            color: #155724;
        }
        .alert-warning {
            background-color: #fff3cd;
            border-color: #ffeaa7;
            color: #856404;
        }
        .alert-error {
            background-color: #f8d7da;
            border-color: #f5c6cb;
            color: #721c24;
        }
        .user-info {
            background: #f8f9fa;
            padding: 10px;
            border-radius: 5px;
            margin-bottom: 15px;
            border-left: 4px solid #007bff;
        }
        .logout-btn {
            background: #dc3545;
            color: white;
            padding: 5px 10px;
            border: none;
            border-radius: 3px;
            cursor: pointer;
            margin-left: 10px;
        }
        .logout-btn:hover {
            background: #c82333;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Thông báo trạng thái -->
    <asp:Panel ID="pnlMessage" runat="server" Visible="false">
        <div id="divMessage" runat="server" class="alert">
            <asp:Label ID="lblMessage" runat="server"></asp:Label>
        </div>
    </asp:Panel>

    <!-- Main Content -->
    <div class="container">
        <!-- Sidebar -->
        <aside class="sidebar">
            <!-- Thông tin người dùng (nếu đã đăng nhập) -->
            <asp:Panel ID="pnlUserInfo" runat="server" Visible="false" CssClass="user-info">
                <strong>Xin chào:</strong> 
                <asp:Label ID="lblUserName" runat="server"></asp:Label>
                <asp:Button ID="btnLogout" runat="server" Text="Đăng xuất" 
                    CssClass="logout-btn" OnClick="btnLogout_Click" />
            </asp:Panel>

            <!-- Tìm theo hãng xe (Load từ Database) -->
            <h2>Tìm theo hãng xe</h2>
            <asp:CheckBoxList ID="cblBrands" runat="server" CssClass="brand-list" 
                AutoPostBack="true" OnSelectedIndexChanged="cblBrands_SelectedIndexChanged">
            </asp:CheckBoxList>

            <!-- Lọc khu vực (Load từ Database) -->
            <div class="filter-section">
                <h3>Lọc khu vực</h3>
                <asp:DropDownList ID="ddlLocation" runat="server" CssClass="form-control" 
                    AutoPostBack="true" OnSelectedIndexChanged="ddlLocation_SelectedIndexChanged">
                    <asp:ListItem Text="Toàn quốc" Value="0" Selected="True" />
                </asp:DropDownList>
            </div>

            <!-- Tìm kiếm nâng cao -->
            <div class="quick-links">
                <h3>Tìm kiếm nâng cao</h3>
                <asp:Panel ID="pnlAdvancedSearch" runat="server">
                    <!-- Lọc theo năm sản xuất -->
                    <asp:Label ID="lblYear" runat="server" Text="Năm sản xuất:" AssociatedControlID="ddlYear" />
                    <asp:DropDownList ID="ddlYear" runat="server" CssClass="form-control">
                        <asp:ListItem Text="Tất cả năm" Value="0" />
                    </asp:DropDownList>
                    
                    <!-- Lọc theo khoảng giá -->
                    <asp:Label ID="lblPriceRange" runat="server" Text="Khoảng giá:" AssociatedControlID="ddlPriceRange" />
                    <asp:DropDownList ID="ddlPriceRange" runat="server" CssClass="form-control">
                        <asp:ListItem Text="Tất cả mức giá" Value="0" />
                        <asp:ListItem Text="Dưới 300 triệu" Value="1" />
                        <asp:ListItem Text="300-500 triệu" Value="2" />
                        <asp:ListItem Text="500-800 triệu" Value="3" />
                        <asp:ListItem Text="800-1200 triệu" Value="4" />
                        <asp:ListItem Text="Trên 1200 triệu" Value="5" />
                    </asp:DropDownList>
                    
                    <asp:Button ID="btnApplyFilter" runat="server" Text="Áp dụng" 
                        CssClass="btn-filter" OnClick="btnApplyFilter_Click" />
                    <asp:Button ID="btnClearFilter" runat="server" Text="Xóa bộ lọc" 
                        CssClass="btn-filter" OnClick="btnClearFilter_Click" />
                </asp:Panel>
            </div>
        </aside>

        <!-- Main Content Area -->
        <main class="content">
            <!-- Search Bar -->
            <div class="search-bar">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" 
                    Placeholder="Nhập từ khóa tìm kiếm (tên xe, hãng, mô tả)..." />
                <asp:Button ID="btnSearch" runat="server" Text="Tìm kiếm" 
                    CssClass="search-btn" OnClick="btnSearch_Click" />
            </div>

            <!-- Car Listings -->
            <div class="car-listings">
                <h2>MUA BÁN Ô TÔ</h2>
                <div class="results-info">
                    <asp:Label ID="lblTotalResults" runat="server" CssClass="total-listings" Text="Tổng: 0 tin" />
                    <asp:Label ID="lblCurrentFilter" runat="server" CssClass="current-filter" />
                </div>

                <!-- Loading indicator -->
                <asp:Panel ID="pnlLoading" runat="server" Visible="false" CssClass="loading">
                    <p>Đang tải dữ liệu...</p>
                </asp:Panel>

                <!-- Car Repeater - CORRECTED VERSION -->
                <asp:Repeater ID="rptCars" runat="server" OnItemDataBound="rptCars_ItemDataBound">
                    <ItemTemplate>
                        <div class="car-item">
                            <asp:Image ID="imgCar" runat="server" CssClass="car-image" 
                                AlternateText="Car Image" />
                            <div class="car-info">
                                <h3 class="car-title">
                                    <asp:HyperLink ID="lnkCarName" runat="server" />
                                </h3>
                                <div class="car-details">
                                    <div class="car-price">
                                        <asp:Literal ID="litPrice" runat="server" />
                                    </div>
                                    <div class="car-year">
                                        Năm: <asp:Literal ID="litYear" runat="server" />
                                    </div>
                                    <div class="car-brand">
                                        Hãng: <asp:Literal ID="litBrand" runat="server" />
                                    </div>
                                    <div class="car-location">
                                        📍 <asp:Literal ID="litLocation" runat="server" />
                                    </div>
                                </div>
                                <p class="car-description">
                                    <asp:Literal ID="litDescription" runat="server" />
                                </p>
                                <div class="car-meta">
                                    <span class="post-date">
                                        Đăng: <asp:Literal ID="litCreatedDate" runat="server" />
                                    </span>
                                    <span class="availability">
                                        <asp:Literal ID="litAvailability" runat="server" />
                                    </span>
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <!-- No results message -->
                <asp:Panel ID="pnlNoResults" runat="server" Visible="false" CssClass="no-results">
                    <h3>Không tìm thấy xe phù hợp</h3>
                    <p>Vui lòng thử:</p>
                    <ul>
                        <li>Thay đổi từ khóa tìm kiếm</li>
                        <li>Bỏ bớt bộ lọc</li>
                        <li>Chọn khu vực rộng hơn</li>
                    </ul>
                </asp:Panel>

                <!-- Pagination -->
                <div class="pagination">
                    <asp:Repeater ID="rptPagination" runat="server" OnItemCommand="rptPagination_ItemCommand">
                        <HeaderTemplate>
                            <div class="pagination-container">
                        </HeaderTemplate>
                        <ItemTemplate>
                            <asp:LinkButton ID="lnkPage" runat="server" 
                                CommandArgument='<%# Eval("PageNumber") %>'
                                CommandName="ChangePage"
                                CssClass='<%# Convert.ToBoolean(Eval("IsCurrent")) ? "page-link current" : "page-link" %>'
                                Text='<%# Eval("DisplayText") %>' />
                        </ItemTemplate>
                        <FooterTemplate>
                            </div>
                        </FooterTemplate>
                    </asp:Repeater>
                </div>
            </div>
        </main>
    </div>

    <script type="text/javascript">
        // Auto-hide alerts after 5 seconds
        setTimeout(function () {
            var alerts = document.querySelectorAll('.alert');
            alerts.forEach(function (alert) {
                if (alert.style.display !== 'none') {
                    alert.style.display = 'none';
                }
            });
        }, 5000);

        // Confirm before logout
        function confirmLogout() {
            return confirm('Bạn có chắc chắn muốn đăng xuất?');
        }
    </script>
</asp:Content>