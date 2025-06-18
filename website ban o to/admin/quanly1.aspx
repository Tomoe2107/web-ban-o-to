<%@ Page Title="" Language="C#" MasterPageFile="~/admin/quantri.Master" AutoEventWireup="true" CodeBehind="quanly1.aspx.cs" Inherits="website_ban_o_to.admin.quanly1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/admin/quanly.css" rel="stylesheet" type="text/css" />
    <link href="~/trangchu.css" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <main class="admin-content">
        <h2>Quản lý danh sách xe</h2>

        <!-- Tìm kiếm và Thêm mới -->
        <div class="admin-toolbar">
            <div class="search-box">
                <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Tìm kiếm xe..." />
                <asp:Button ID="btnSearch" runat="server" Text="Tìm" CssClass="search-btn" OnClick="btnSearch_Click" />
            </div>
            
            <asp:Button ID="btnThemXe" runat="server" Text="Thêm xe mới" CssClass="add-btn" OnClick="btnThemXe_Click" />
            <br />
            <asp:Button ID="btnThemTinTuc" runat="server" Text="Thêm tin tức" CssClass="add-btn" OnClick="btnThemTinTuc_Click" Width="175px" />

        </div>

        <!-- Danh sách xe -->
        <asp:GridView ID="gvXe" runat="server" AutoGenerateColumns="False" DataKeyNames="ID"
            CssClass="admin-grid" AllowPaging="True" PageSize="10"
            OnPageIndexChanging="gvXe_PageIndexChanging"
            OnRowEditing="gvXe_RowEditing"
            OnRowUpdating="gvXe_RowUpdating"
            OnRowCancelingEdit="gvXe_RowCancelingEdit"
            OnRowDeleting="gvXe_RowDeleting" OnSelectedIndexChanged="gvXe_SelectedIndexChanged">
            <Columns>
                <asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" />
                <asp:TemplateField HeaderText="Tên xe">
                    <ItemTemplate>
                        <asp:Label ID="lblTenXe" runat="server" Text='<%# Eval("TenXe") %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtTenXe" runat="server" Text='<%# Bind("TenXe") %>' CssClass="form-control" />
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Giá (triệu)">
                    <ItemTemplate>
                        <asp:Label ID="lblGia" runat="server" Text='<%# Eval("Gia", "{0:N0}") %>' />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="txtGia" runat="server" Text='<%# Bind("Gia") %>' CssClass="form-control" />
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Hình ảnh">
                    <ItemTemplate>
                        <asp:Image ID="imgXe" runat="server" ImageUrl='<%# Eval("HinhAnh") %>' Width="100px" />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:FileUpload ID="fuHinhAnh" runat="server" CssClass="form-control" />
                    </EditItemTemplate>
                </asp:TemplateField>
                <asp:CheckBoxField DataField="TrangThai" HeaderText="Hiển thị" />
                <asp:CommandField ShowEditButton="True" ButtonType="Button" />
                <asp:CommandField ShowDeleteButton="True" ButtonType="Button" />
            </Columns>
        </asp:GridView>

        <!-- Form chi tiết thêm/sửa -->
        <asp:Panel ID="pnlChiTietXe" runat="server" Visible="false" CssClass="detail-panel">
            <h3><asp:Label ID="lblTitle" runat="server" Text="Thêm xe mới" /></h3>
            <div class="form-group">
                <label>Tên xe:</label>
                <asp:TextBox ID="txtTenXe" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <label>Giá (triệu):</label>
                <asp:TextBox ID="txtGia" runat="server" CssClass="form-control" TextMode="Number" />
            </div>
            <div class="form-group">
                <label>Mô tả:</label>
                <asp:TextBox ID="txtMoTa" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="4" />
            </div>
            <div class="form-group">
                <label>Hình ảnh:</label>
                <asp:FileUpload ID="fuHinhAnh" runat="server" CssClass="form-control" />
            </div>
            <div class="form-group">
                <asp:CheckBox ID="chkTrangThai" runat="server" Text=" Hiển thị trên website" Checked="true" />
            </div>
            <div class="form-actions">
                <asp:Button ID="btnLuu" runat="server" Text="Lưu" CssClass="save-btn" OnClick="btnLuu_Click" />
                <asp:Button ID="btnHuy" runat="server" Text="Hủy" CssClass="cancel-btn" OnClick="btnHuy_Click" CausesValidation="false" />
            </div>
        </asp:Panel>
         <!-- Form thêm tin tức -->
<asp:Panel ID="pnlThemTinTuc" runat="server" Visible="false" CssClass="detail-panel">
    <h3><asp:Label ID="lblTieuDeTinTuc" runat="server" Text="Thêm tin tức mới" /></h3>

    <div class="form-group">
        <label>Tiêu đề:</label>
        <asp:TextBox ID="txtTieuDe" runat="server" CssClass="form-control" />
    </div>

    <div class="form-group">
        <label>Nội dung:</label>
        <asp:TextBox ID="txtNoiDung" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="6" />
    </div>

    <div class="form-group">
        <label>Hình ảnh:</label>
        <asp:FileUpload ID="fuHinhAnhTinTuc" runat="server" CssClass="form-control" />
    </div>

    <div class="form-group">
        <asp:CheckBox ID="chkTinTucHienThi" runat="server" Text=" Hiển thị trên trang chủ" Checked="true" />
    </div>

    <div class="form-actions">
        <asp:Button ID="btnLuuTinTuc" runat="server" Text="Lưu tin tức" CssClass="save-btn" OnClick="btnLuuTinTuc_Click" />
        <asp:Button ID="btnHuyTinTuc" runat="server" Text="Hủy" CssClass="cancel-btn" OnClick="btnHuyTinTuc_Click" CausesValidation="false" />
    </div>
</asp:Panel>
    </main>
</asp:Content>
