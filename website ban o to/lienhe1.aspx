<%@ Page Title="Liên Hệ" Language="C#" MasterPageFile="~/home.Master" AutoEventWireup="true" CodeBehind="lienhe1.aspx.cs" Inherits="website_ban_o_to.lienhe1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <title>Liên Hệ - Showroom Ô Tô</title>
    <meta name="description" content="Liên hệ với chúng tôi để được tư vấn và hỗ trợ mua bán xe ô tô tốt nhất." />
    <meta name="keywords" content="liên hệ, tư vấn xe, showroom ô tô, hỗ trợ khách hàng" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0" />
    
    <!-- Font Awesome Icons -->
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.0.0/css/all.min.css" />
    
    <style>
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }
        
        body {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
            min-height: 100vh;
        }

        /* Page Container */
        .contact-page {
            padding: 40px 0;
            min-height: 100vh;
        }

        .container {
            max-width: 1400px;
            margin: 0 auto;
            padding: 0 20px;
        }

        /* Page Header */
        .page-header {
            text-align: center;
            margin-bottom: 40px;
        }

        .page-title {
            font-size: 3rem;
            font-weight: 700;
            color: #2c3e50;
            margin-bottom: 15px;
            text-transform: uppercase;
            letter-spacing: 3px;
            position: relative;
        }

        .page-title::after {
            content: '';
            display: block;
            width: 80px;
            height: 4px;
            background: linear-gradient(135deg, #3498db, #2980b9);
            margin: 20px auto;
            border-radius: 2px;
        }

        .page-subtitle {
            font-size: 1.2rem;
            color: #7f8c8d;
            max-width: 600px;
            margin: 0 auto;
        }

        /* Main Content Layout - Horizontal Layout */
        .contact-content {
            display: flex;
            gap: 30px;
            align-items: flex-start;
            justify-content: center;
        }

        /* Contact Form Section */
        .contact-form-section {
            background: white;
            border-radius: 20px;
            padding: 40px;
            box-shadow: 0 15px 35px rgba(0, 0, 0, 0.1);
            position: relative;
            overflow: hidden;
            flex: 1;
            max-width: 600px;
        }

        .contact-form-section::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            height: 5px;
            background: linear-gradient(135deg, #3498db, #2980b9);
        }

        .form-title {
            font-size: 1.8rem;
            color: #2c3e50;
            margin-bottom: 30px;
            text-align: center;
            font-weight: 600;
        }

        /* Form Layout - Labels and inputs on same row */
        .form-group {
            margin-bottom: 25px;
            display: flex;
            align-items: center;
            gap: 15px;
            position: relative;
        }

        .form-label {
            color: #34495e;
            font-weight: 600;
            font-size: 0.95rem;
            min-width: 120px;
            text-align: right;
            flex-shrink: 0;
        }

        .form-label .required {
            color: #e74c3c;
            margin-left: 3px;
        }

        .form-input-wrapper {
            flex: 1;
            position: relative;
        }

        .form-control {
            width: 100%;
            min-width: 200px;
            padding: 16px 20px;
            border: 2px solid #e8ecf0;
            border-radius: 12px;
            font-size: 16px;
            color: #2c3e50;
            background-color: #fafbfc;
            font-family: inherit;
        }

        .form-control:focus {
            outline: none;
            border-color: #3498db;
            background-color: #ffffff;
            box-shadow: 0 0 0 4px rgba(52, 152, 219, 0.1);
        }

        .form-control.error {
            border-color: #e74c3c;
            background-color: #fdf2f2;
        }

        .form-control.success {
            border-color: #27ae60;
            background-color: #f2fdf5;
        }

        /* Textarea specific styling */
        .form-control[rows] {
            resize: vertical;
            min-height: 120px;
            font-family: inherit;
        }

        /* Message field - special layout for textarea */
        .form-group.message-group {
            align-items: flex-start;
        }

        .form-group.message-group .form-label {
            padding-top: 16px;
        }

        /* Button group - centered */
        .form-group.button-group {
            justify-content: center;
            margin-top: 30px;
        }

        /* Button Styling */
        .btn-submit {
            padding: 18px 50px;
            background: linear-gradient(135deg, #3498db, #2980b9);
            color: white;
            border: none;
            border-radius: 12px;
            font-size: 16px;
            font-weight: 600;
            text-transform: uppercase;
            cursor: pointer;
            position: relative;
            overflow: hidden;
            letter-spacing: 1px;
            min-width: 200px;
        }

        .btn-submit:hover {
            background: linear-gradient(135deg, #2980b9, #1f6391);
            box-shadow: 0 10px 25px rgba(52, 152, 219, 0.3);
        }

        .btn-submit:active {
            background: linear-gradient(135deg, #1f6391, #16527a);
        }

        .btn-submit:disabled {
            opacity: 0.7;
            cursor: not-allowed;
        }

        /* Message Display */
        .alert {
            border: none;
            border-radius: 12px;
            padding: 16px 20px;
            margin: 15px 0;
            font-weight: 500;
            position: relative;
            width: 100%;
        }

        .alert-success {
            background: linear-gradient(135deg, #d4edda, #c3e6cb);
            color: #155724;
            border-left: 4px solid #28a745;
        }

        .alert-danger {
            background: linear-gradient(135deg, #f8d7da, #f1aeb5);
            color: #721c24;
            border-left: 4px solid #dc3545;
        }

        .alert-warning {
            background: linear-gradient(135deg, #fff3cd, #ffeaa7);
            color: #856404;
            border-left: 4px solid #ffc107;
        }

        .alert-info {
            background: linear-gradient(135deg, #d1ecf1, #bee5eb);
            color: #0c5460;
            border-left: 4px solid #17a2b8;
        }

        /* Validation Messages */
        .validator-message,
        .field-error-message {
            color: #e74c3c;
            font-size: 0.85rem;
            margin-top: 5px;
            font-weight: 500;
            display: block;
            position: absolute;
            top: 100%;
            left: 0;
            width: 100%;
        }

        .character-counter {
            text-align: right;
            font-size: 0.8rem;
            color: #95a5a6;
            margin-top: 5px;
            font-weight: 500;
            position: absolute;
            top: 100%;
            right: 0;
        }

        /* Contact Info Section */
        .contact-info-section {
            background: white;
            border-radius: 20px;
            padding: 40px;
            box-shadow: 0 15px 35px rgba(0, 0, 0, 0.1);
            position: relative;
            overflow: hidden;
            flex: 0 0 400px;
            max-width: 400px;
        }

        .contact-info-section::before {
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            height: 5px;
            background: linear-gradient(135deg, #e74c3c, #c0392b);
        }

        .info-title {
            font-size: 1.8rem;
            color: #2c3e50;
            margin-bottom: 30px;
            text-align: center;
            font-weight: 600;
        }

        .info-grid {
            display: grid;
            gap: 25px;
        }

        .info-item {
            display: flex;
            align-items: center;
            padding: 20px;
            background: #f8f9fa;
            border-radius: 15px;
            border-left: 4px solid transparent;
        }

        .info-item:hover {
            background: #e8f4fd;
            border-left-color: #3498db;
        }

        .info-item i {
            font-size: 1.5rem;
            color: #3498db;
            margin-right: 20px;
            width: 30px;
            text-align: center;
        }

        .info-content h4 {
            color: #2c3e50;
            font-size: 1.1rem;
            margin-bottom: 5px;
            font-weight: 600;
        }

        .info-content p {
            color: #7f8c8d;
            margin: 0;
            font-size: 0.95rem;
        }

        .info-content a {
            color: #3498db;
            text-decoration: none;
        }

        .info-content a:hover {
            color: #2980b9;
            text-decoration: underline;
        }

        /* Responsive Design */
        @media (max-width: 1024px) {
            .contact-content {
                flex-direction: column;
                align-items: center;
            }
            
            .contact-form-section,
            .contact-info-section {
                max-width: 800px;
                width: 100%;
            }
            
            .contact-info-section {
                flex: none;
            }
        }

        @media (max-width: 768px) {
            .container {
                padding: 0 15px;
            }

            .page-title {
                font-size: 2rem;
                letter-spacing: 1px;
            }

            .contact-form-section,
            .contact-info-section {
                padding: 25px;
            }

            /* Stack form elements vertically on mobile */
            .form-group {
                flex-direction: column;
                align-items: flex-start;
                gap: 8px;
            }

            .form-label {
                min-width: auto;
                text-align: left;
            }

            .form-group.message-group .form-label {
                padding-top: 0;
            }

            .form-control {
                min-width: 200px;
                padding: 14px 16px;
                font-size: 15px;
            }

            .btn-submit {
                padding: 16px 25px;
                font-size: 15px;
                min-width: 150px;
            }
        }

        @media (max-width: 480px) {
            .page-header {
                margin-bottom: 20px;
            }

            .page-title {
                font-size: 1.8rem;
            }

            .contact-form-section,
            .contact-info-section {
                padding: 20px;
            }

            .form-group {
                margin-bottom: 20px;
            }

            .form-control {
                min-width: 180px;
            }
        }

        /* Focus indicators for accessibility */
        .form-control:focus,
        .btn-submit:focus {
            outline: 2px solid #3498db;
            outline-offset: 2px;
        }

        /* High contrast mode support */
        @media (prefers-contrast: high) {
            .form-control {
                border-width: 3px;
            }
            
            .btn-submit {
                border: 3px solid #2980b9;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="contact-page">
         <!-- Page Header -->
 <div class="page-header">
     <h1 class="page-title">Liên Hệ Với Chúng Tôi</h1>
     <p class="page-subtitle">
         Chúng tôi luôn sẵn sàng lắng nghe và hỗ trợ bạn. Hãy để lại thông tin và chúng tôi sẽ liên hệ trong thời gian sớm nhất.
     </p>
 </div>
        <div class="">
           

            <!-- Main Content - Horizontal Layout -->
            <div class="contact-content">
                <!-- Contact Form Section -->
                <div class="contact-form-section">
                    <h2 class="form-title">
                        <i class="fas fa-envelope"></i>
                        Gửi Tin Nhắn
                    </h2>
                    
                    <!-- Message Display Area -->
                    <asp:Label ID="lblThongBao" runat="server" Visible="false" CssClass="alert"></asp:Label>
                    
                    <div class="contact-form">
                        <!-- Full Name Field -->
                        <div class="form-group">
                            <asp:Label ID="lblHoTen" runat="server" Text="Họ và tên" AssociatedControlID="txtHoTen" CssClass="form-label">
                                Họ và tên<span class="required">*</span>
                            </asp:Label>
                            <div class="form-input-wrapper">
                                <asp:TextBox ID="txtHoTen" runat="server" 
                                    CssClass="form-control" 
                                    placeholder="Nhập họ tên của bạn"
                                    MaxLength="100"
                                    autocomplete="name"
                                    aria-required="true"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvHoTen" runat="server"
                                    ControlToValidate="txtHoTen"
                                    ErrorMessage="Vui lòng nhập họ tên!"
                                    CssClass="validator-message"
                                    Display="Dynamic"
                                    EnableClientScript="true"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        
                        <!-- Email Field -->
                        <div class="form-group">
                            <asp:Label ID="lblEmail" runat="server" Text="Email" AssociatedControlID="txtEmail" CssClass="form-label">
                                Email<span class="required">*</span>
                            </asp:Label>
                            <div class="form-input-wrapper">
                                <asp:TextBox ID="txtEmail" runat="server" 
                                    CssClass="form-control" 
                                    TextMode="Email" 
                                    placeholder="Nhập email của bạn"
                                    MaxLength="100"
                                    autocomplete="email"
                                    aria-required="true"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvEmail" runat="server"
                                    ControlToValidate="txtEmail"
                                    ErrorMessage="Vui lòng nhập email!"
                                    CssClass="validator-message"
                                    Display="Dynamic"
                                    EnableClientScript="true"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="revEmail" runat="server"
                                    ControlToValidate="txtEmail"
                                    ErrorMessage="Email không đúng định dạng!"
                                    ValidationExpression="^[^\s@]+@[^\s@]+\.[^\s@]+$"
                                    CssClass="validator-message"
                                    Display="Dynamic"
                                    EnableClientScript="true"></asp:RegularExpressionValidator>
                            </div>
                        </div>
                        
                        <!-- Phone Field -->
                        <div class="form-group">
                            <asp:Label ID="lblDienThoai" runat="server" Text="Điện thoại" AssociatedControlID="txtDienThoai" CssClass="form-label">
                                Số điện thoại
                            </asp:Label>
                            <div class="form-input-wrapper">
                                <asp:TextBox ID="txtDienThoai" runat="server" 
                                    CssClass="form-control" 
                                    placeholder="Nhập số điện thoại (không bắt buộc)"
                                    MaxLength="20"
                                    autocomplete="tel"
                                    TextMode="Phone"></asp:TextBox>
                                <asp:RegularExpressionValidator ID="revDienThoai" runat="server"
                                    ControlToValidate="txtDienThoai"
                                    ErrorMessage="Số điện thoại không đúng định dạng!"
                                    ValidationExpression="^(\+?84|0)[0-9]{9,10}$"
                                    CssClass="validator-message"
                                    Display="Dynamic"
                                    EnableClientScript="true"></asp:RegularExpressionValidator>
                            </div>
                        </div>
                        
                        <!-- Message Field -->
                        <div class="form-group message-group">
                            <asp:Label ID="lblNoiDung" runat="server" Text="Nội dung liên hệ" AssociatedControlID="txtNoiDung" CssClass="form-label">
                                Nội dung<span class="required">*</span>
                            </asp:Label>
                            <div class="form-input-wrapper">
                                <asp:TextBox ID="txtNoiDung" runat="server" 
                                    CssClass="form-control" 
                                    TextMode="MultiLine" 
                                    Rows="6" 
                                    placeholder="Nhập nội dung bạn muốn liên hệ, câu hỏi hoặc yêu cầu hỗ trợ..."
                                    MaxLength="2000"
                                    aria-required="true"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="rfvNoiDung" runat="server"
                                    ControlToValidate="txtNoiDung"
                                    ErrorMessage="Vui lòng nhập nội dung liên hệ!"
                                    CssClass="validator-message"
                                    Display="Dynamic"
                                    EnableClientScript="true"></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        
                        <!-- Submit Button -->
                        <div class="form-group button-group">
                            <asp:Button ID="btnGuiLienHe" runat="server" 
                                Text="Gửi Liên Hệ" 
                                CssClass="btn-submit" 
                                OnClick="btnGuiLienHe_Click" />
                        </div>
                    </div>
                </div>


            </div>
        </div>
    </div>

    <!-- Simple JavaScript for basic functionality -->
    <script>
        // Basic form validation
        function validateForm() {
            const txtHoTen = document.getElementById('<%= txtHoTen.ClientID %>');
            const txtEmail = document.getElementById('<%= txtEmail.ClientID %>');
            const txtNoiDung = document.getElementById('<%= txtNoiDung.ClientID %>');

            let isValid = true;

            if (txtHoTen && txtHoTen.value.trim() === '') {
                isValid = false;
            }

            if (txtEmail && txtEmail.value.trim() === '') {
                isValid = false;
            }

            if (txtNoiDung && txtNoiDung.value.trim() === '') {
                isValid = false;
            }

            return isValid;
        }

        // Character counter for message field
        document.addEventListener('DOMContentLoaded', function () {
            const txtNoiDung = document.getElementById('<%= txtNoiDung.ClientID %>');

            if (txtNoiDung) {
                const counter = document.createElement('div');
                counter.className = 'character-counter';
                txtNoiDung.parentNode.appendChild(counter);

                txtNoiDung.addEventListener('input', function () {
                    const length = this.value.length;
                    const maxLength = 2000;
                    counter.textContent = length + '/' + maxLength + ' ký tự';

                    if (length > maxLength * 0.9) {
                        counter.style.color = '#e74c3c';
                    } else if (length > maxLength * 0.7) {
                        counter.style.color = '#f39c12';
                    } else {
                        counter.style.color = '#95a5a6';
                    }
                });

                // Initial update
                txtNoiDung.dispatchEvent(new Event('input'));
            }
        });
    </script>
</asp:Content>