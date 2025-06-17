<%@ Page Title="" Language="C#" MasterPageFile="~/home.Master" AutoEventWireup="true" CodeBehind="trangchu1.aspx.cs" Inherits="website_ban_o_to.trangchu1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/trangchu.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- Main Content -->
    <div class="container">
        <!-- Sidebar -->
        <aside class="sidebar">
            <!-- Tìm theo hãng -->
            <h2>Tìm theo hãng xe</h2>
            <asp:CheckBoxList ID="cblBrands" runat="server" CssClass="brand-list">
                <asp:ListItem Text="Audi" Value="1" />
                <asp:ListItem Text="BMW" Value="2" />
                <asp:ListItem Text="Chevrolet" Value="3" />
                <asp:ListItem Text="Ford" Value="4" />
                <asp:ListItem Text="Honda" Value="5" />
                <asp:ListItem Text="Hyundai" Value="6" />
                <asp:ListItem Text="Kia" Value="7" />
                <asp:ListItem Text="Toyota" Value="8" />
                <asp:ListItem Text="VinFast" Value="9" />
            </asp:CheckBoxList>

            <!-- Lọc khu vực -->
            <div class="filter-section">
                <h3>Lọc khu vực</h3>
                <asp:DropDownList ID="ddlRegion" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlRegion_SelectedIndexChanged">
                    <asp:ListItem Text="Toàn quốc" Value="0" Selected="True" />
                    <asp:ListItem Text="Hà Nội" Value="1" />
                    <asp:ListItem Text="TP HCM" Value="2" />
                </asp:DropDownList>

                <asp:DropDownList ID="ddlProvince" runat="server" CssClass="form-control" Visible="false">
                    <asp:ListItem Text="Chọn tỉnh thành" Value="0" />
                </asp:DropDownList>
            </div>

            <!-- Tìm kiếm nâng cao -->
            <div class="quick-links">
                <h3>Tìm kiếm nâng cao</h3>
                <asp:Panel ID="pnlAdvancedSearch" runat="server">
                    <asp:Label ID="lblPriceRange" runat="server" Text="Khoảng giá:" AssociatedControlID="ddlPriceRange" />
                    <asp:DropDownList ID="ddlPriceRange" runat="server" CssClass="form-control">
                        <asp:ListItem Text="Tất cả mức giá" Value="0" />
                        <asp:ListItem Text="Dưới 300 triệu" Value="1" />
                        <asp:ListItem Text="300-500 triệu" Value="2" />
                        <asp:ListItem Text="500-800 triệu" Value="3" />
                        <asp:ListItem Text="Trên 800 triệu" Value="4" />
                    </asp:DropDownList>
                    <asp:Button ID="btnApplyFilter" runat="server" Text="Áp dụng" CssClass="btn-filter" OnClick="btnApplyFilter_Click" />
                </asp:Panel>
            </div>
        </aside>

        <!-- Main Content Area -->
        <main class="content">
            <!-- Search Bar -->
            <div class="search-bar">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" Placeholder="Nhập từ khóa tìm kiếm..." />
                <asp:Button ID="btnSearch" runat="server" Text="Tìm kiếm" CssClass="search-btn" OnClick="btnSearch_Click" />
            </div>

            <!-- Car Listings -->
            <div class="car-listings">
                <h2>MUA BÁN Ô TÔ</h2>
                <asp:Label ID="lblTotalResults" runat="server" CssClass="total-listings" Text="Tổng: 0 tin" />

                <!-- Repeater -->
                <asp:Repeater ID="rptCars" runat="server" OnItemDataBound="rptCars_ItemDataBound">
                    <ItemTemplate>
                        <div class="car-item">
                            <asp:Image ID="imgCar" runat="server" CssClass="car-image" 
                                ImageUrl='<%# Eval("ImageUrl") %>' AlternateText='<%# Eval("Name") %>' />
                            <div class="car-info">
                                <h3 class="car-title">
                                    <asp:HyperLink ID="lnkCarName" runat="server" 
                                        NavigateUrl='<%# "~/chitietsanpham.aspx?id=" + Eval("Id") %>' 
                                        Text='<%# Eval("Name") %>' />
                                </h3>
                                <div class="car-price"><%# Eval("Price", "{0:N0} triệu") %></div>
                                <div class="car-location"><%# Eval("Location") %></div>
                                <p class="car-description"><%# Eval("Description") %></p>
                                <div class="car-contact">
                                    <asp:Label ID="lblContact" runat="server" Text='<%# "LH: " + Eval("Contact") %>' />
                                </div>
                                <div class="car-contact">
                                    <asp:Label ID="lblPhone" runat="server" Text='<%# "DT: " + Eval("Phone") %>' />
                                </div>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <!-- Pagination -->
                <div class="pagination">
                    <div class="rpt-grid">
                        <asp:Repeater ID="rptPagination" runat="server" OnItemCommand="rptPagination_ItemCommand">
                            <ItemTemplate>
                                <asp:LinkButton ID="lnkPage" runat="server" CommandArgument='<%# Eval("PageNumber") %>'
                                    CommandName="ChangePage"
                                    CssClass='<%# Convert.ToBoolean(Eval("IsCurrent")) ? "current-page" : "" %>'>
                                    <%# Eval("PageNumber") %>
                                </asp:LinkButton>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
            </div>
        </main>
    </div>
</asp:Content>
