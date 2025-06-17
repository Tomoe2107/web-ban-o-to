<%@ Page Title="" Language="C#" MasterPageFile="~/home.Master" AutoEventWireup="true" CodeBehind="dangnhap1.aspx.cs" Inherits="website_ban_o_to.dangnhap1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <title>Đăng nhập hệ thống</title>
    <link href="~/trangchu.css" rel="stylesheet" type="text/css" />
    <link href="~/dangnhap.css" rel="stylesheet" type="text/css" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
       <!-- Login Form -->
    <div class="login-wrapper">
        <div class="login-container">
            <div class="login-header">
                <h2><i class="fas fa-sign-in-alt"></i> ĐĂNG NHẬP</h2>
                <p>Vui lòng nhập thông tin tài khoản</p>
            </div>

            <div class="form-group">
                <label for="txtUsername"><i class="fas fa-user"></i> Tên đăng nhập</label>
                <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control" placeholder="Nhập username"></asp:TextBox>
            </div>

            <div class="form-group">
                <label for="txtPassword"><i class="fas fa-lock"></i> Mật khẩu</label>
                <asp:TextBox ID="txtPassword" runat="server" TextMode="Password" CssClass="form-control" placeholder="Nhập mật khẩu"></asp:TextBox>
                <div class="password-toggle">
                    <i class="far fa-eye" id="togglePassword"></i>
                </div>
            </div>

            <div class="form-options">
                <label class="remember-me">
                    <asp:CheckBox ID="cbRemember" runat="server" />
                    Ghi nhớ đăng nhập
                </label>
                <a href="#" class="forgot-password">Quên mật khẩu?</a>
            </div>

            <asp:Button ID="btnLogin" runat="server" Text="ĐĂNG NHẬP" CssClass="btn-login" OnClick="btnLogin_Click" />

            <div class="login-footer">
                Chưa có tài khoản? <a href="dangky.aspx">Đăng ký ngay</a>
            </div>

            <asp:Label ID="lblError" runat="server" CssClass="error-message" Visible="false"></asp:Label>
        </div>
    </div>

    <script>
        // Hiển thị/ẩn mật khẩu
        document.addEventListener("DOMContentLoaded", function () {
            document.getElementById('togglePassword').addEventListener('click', function () {
                const passwordField = document.getElementById('<%= txtPassword.ClientID %>');
                const icon = this;
                if (passwordField.type === 'password') {
                    passwordField.type = 'text';
                    icon.classList.replace('fa-eye', 'fa-eye-slash');
                } else {
                    passwordField.type = 'password';
                    icon.classList.replace('fa-eye-slash', 'fa-eye');
                }
            });
        });
    </script>
</asp:Content>
