<%@ Page Title="Giỏ hàng của bạn" Language="C#" MasterPageFile="~/home.Master" AutoEventWireup="true" CodeBehind="giohang.aspx.cs" Inherits="website_ban_o_to.giohang" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/giohang.css" rel="stylesheet" />
    <title>Giỏ hàng của bạn - Website Bán Ô Tô</title>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="my-posts-container">
        <div class="page-header">
            <h2>Giỏ hàng của bạn</h2>
            <asp:Button ID="btnAddNew" runat="server" Text="+ Đăng xe mới" 
                       CssClass="btn-add-new" OnClick="btnAddNew_Click" />
        </div>

        <!-- Thống kê -->
        <div class="statistics-section">
            <div class="stat-card">
                <h3>Tổng bài đăng</h3>
                <p class="stat-number"><asp:Label ID="lblTotalPosts" runat="server" Text="0" /></p>
            </div>
            <div class="stat-card">
                <h3>Đã duyệt</h3>
                <p class="stat-number approved"><asp:Label ID="lblApprovedPosts" runat="server" Text="0" /></p>
            </div>
            <div class="stat-card">
                <h3>Chờ duyệt</h3>
                <p class="stat-number pending"><asp:Label ID="lblPendingPosts" runat="server" Text="0" /></p>
            </div>
        </div>

        <!-- Thông báo -->
        <asp:Label ID="lblMessage" runat="server" CssClass="message" Visible="false" />

        <!-- Danh sách bài đăng -->
        <div class="posts-list">
            <asp:Repeater ID="rptUserPosts" runat="server">
                <ItemTemplate>
                    <div class="post-item">
                        <div class="post-image">
                            <img src='<%# Eval("ImagePath") != DBNull.Value && !string.IsNullOrEmpty(Eval("ImagePath").ToString()) ? 
                                       Eval("ImagePath") : "~/images/no-image.jpg" %>' 
                                 alt='<%# Eval("CarName") %>' />
                        </div>
                        
                        <div class="post-content">
                            <div class="post-header">
                                <h3 class="car-name"><%# Eval("CarName") %></h3>
                                <span class='status <%# GetStatusClass(Eval("IsApproved")) %>'>
                                    <%# GetStatusText(Eval("IsApproved")) %>
                                </span>
                            </div>
                            
                            <div class="post-details">
                                <p class="price">
                                    <strong>Giá: <%# FormatPrice(Eval("ExpectedPrice")) %></strong>
                                </p>
                                
                                <p class="description">
                                    <%# Eval("Description").ToString().Length > 100 ? 
                                        Eval("Description").ToString().Substring(0, 100) + "..." : 
                                        Eval("Description") %>
                                </p>
                                
                                <div class="contact-info">
                                    <p><strong>Liên hệ:</strong> <%# Eval("ContactName") %></p>
                                    <p><strong>SĐT:</strong> <%# Eval("ContactPhone") %></p>
                                    <p><strong>Email:</strong> <%# Eval("ContactEmail") %></p>
                                </div>
                                
                                <div class="post-meta">
                                    <p><strong>Ngày đăng:</strong> <%# FormatDate(Eval("CreatedDate")) %></p>
                                    <%# Convert.ToBoolean(Eval("IsApproved")) && Eval("ApprovedDate") != DBNull.Value ? 
                                        "<p><strong>Ngày duyệt:</strong> " + FormatDate(Eval("ApprovedDate")) + "</p>" : "" %>
                                </div>
                            </div>
                            
                            <div class="post-actions">
                           
                                
                                <asp:Button ID="btnDelete" runat="server" Text="Xóa" 
                                           CommandArgument='<%# Eval("PostID") %>' 
                                           CssClass="btn-delete" OnCommand="btnDelete_Command"
                                           OnClientClick="return confirm('Bạn có chắc chắn muốn xóa bài đăng này?');" />
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </div>

      
    </div>
</asp:Content>