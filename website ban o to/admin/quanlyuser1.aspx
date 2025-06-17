<%@ Page Title="" Language="C#" MasterPageFile="~/admin/quantri.Master" AutoEventWireup="true" CodeBehind="quanlyuser1.aspx.cs" Inherits="website_ban_o_to.admin.quanlyuser1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
 <title>Quản lý người dùng - Website bán ô tô</title>
 <link href="~/quanly.css" rel="stylesheet" type="text/css" runat="server" />
 <link href="~/trangchu.css" rel="stylesheet" type="text/css" runat="server" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
        <!-- Phần quản lý chính -->
        <div class="admin-container">
            <!-- Nội dung quản lý -->
            <main class="admin-content">
                <h2>Quản lý người dùng</h2>
                
                <!-- Thanh tìm kiếm và thêm mới -->
                <div class="admin-toolbar">
                    <div class="search-box">
                        <asp:TextBox ID="txtSearch" runat="server" placeholder="Tìm kiếm người dùng..." CssClass="search-input"></asp:TextBox>
                        <asp:Button ID="btnSearch" runat="server" Text="Tìm" CssClass="search-btn" OnClick="btnSearch_Click" />
                    </div>
                    <asp:Button ID="btnThemUser" runat="server" Text="Thêm người dùng" CssClass="add-btn" OnClick="btnThemUser_Click" />
                </div>

                <!-- GridView hiển thị danh sách người dùng -->
                <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="False" DataKeyNames="ID"
                    CssClass="admin-grid" AllowPaging="True" PageSize="10" OnPageIndexChanging="gvUsers_PageIndexChanging"
                    OnRowEditing="gvUsers_RowEditing" OnRowUpdating="gvUsers_RowUpdating" OnRowCancelingEdit="gvUsers_RowCancelingEdit"
                    OnRowDeleting="gvUsers_RowDeleting">
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
                        <asp:TemplateField HeaderText="Tên đăng nhập">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtUsername" runat="server" Text='<%# Bind("Username") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblUsername" runat="server" Text='<%# Bind("Username") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Họ tên">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtHoTen" runat="server" Text='<%# Bind("HoTen") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblHoTen" runat="server" Text='<%# Bind("HoTen") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Email">
                            <EditItemTemplate>
                                <asp:TextBox ID="txtEmail" runat="server" Text='<%# Bind("Email") %>'></asp:TextBox>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblEmail" runat="server" Text='<%# Bind("Email") %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Vai trò">
                            <EditItemTemplate>
                                <asp:DropDownList ID="ddlRole" runat="server" SelectedValue='<%# Bind("Role") %>'>
                                    <asp:ListItem Value="Admin">Quản trị viên</asp:ListItem>
                                    <asp:ListItem Value="User">Người dùng</asp:ListItem>
                                    <asp:ListItem Value="Staff">Nhân viên</asp:ListItem>
                                </asp:DropDownList>
                            </EditItemTemplate>
                            <ItemTemplate>
                                <asp:Label ID="lblRole" runat="server" Text='<%# (Eval("Role").ToString()) %>'></asp:Label>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:CheckBoxField DataField="IsActive" HeaderText="Kích hoạt" />
                        <asp:CommandField ShowEditButton="True" ButtonType="Button" />
                        <asp:CommandField ShowDeleteButton="True" ButtonType="Button" />
                    </Columns>
                    <PagerStyle CssClass="grid-pager" />
                </asp:GridView>

                <!-- Form thêm/xem chi tiết người dùng -->
                <asp:Panel ID="pnlChiTietUser" runat="server" Visible="false" CssClass="detail-panel">
                    <h3><asp:Label ID="lblTitle" runat="server" Text="Thêm người dùng mới"></asp:Label></h3>
                    <div class="form-group">
                        <label>Tên đăng nhập:</label>
                        <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Mật khẩu:</label>
                        <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Họ tên:</label>
                        <asp:TextBox ID="txtHoTen" runat="server" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Email:</label>
                        <asp:TextBox ID="txtEmail" runat="server" TextMode="Email" CssClass="form-control"></asp:TextBox>
                    </div>
                    <div class="form-group">
                        <label>Vai trò:</label>
                        <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-control">
                            <asp:ListItem Value="Admin">Quản trị viên</asp:ListItem>
                            <asp:ListItem Value="User">Người dùng</asp:ListItem>
                            <asp:ListItem Value="Staff">Nhân viên</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="form-group">
                        <asp:CheckBox ID="chkIsActive" runat="server" Text=" Kích hoạt tài khoản" Checked="true" />
                    </div>
                    <div class="form-actions">
                        <asp:Button ID="btnLuu" runat="server" Text="Lưu" CssClass="save-btn" OnClick="btnLuu_Click" />
                        <asp:Button ID="btnHuy" runat="server" Text="Hủy" CssClass="cancel-btn" OnClick="btnHuy_Click" CausesValidation="false" />
                    </div>
                </asp:Panel>
            </main>
        </div>


</asp:Content>
