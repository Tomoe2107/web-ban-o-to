<%@ Page Title="" Language="C#" MasterPageFile="~/home.Master" AutoEventWireup="true" CodeBehind="banoto1.aspx.cs" Inherits="website_ban_o_to.banoto1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/trangchu.css" rel="stylesheet" />
    <link href="~/banoto.css" rel="stylesheet" />
    
    <!-- SweetAlert2 -->
    <script src="https://cdn.jsdelivr.net/npm/sweetalert2@11"></script>
    
    <style>
        .container {
            max-width: 800px;
            margin: 0 auto;
            padding: 20px;
        }
        
        .sell-form {
            background: #fff;
            padding: 30px;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            margin-top: 20px;
        }
        
        .form-group {
            margin-bottom: 20px;
        }
        
        .form-group label {
            display: block;
            margin-bottom: 5px;
            font-weight: 600;
            color: #333;
        }
        
        .form-control {
            width: 100%;
            padding: 12px;
            border: 2px solid #e1e5e9;
            border-radius: 6px;
            font-size: 14px;
            transition: border-color 0.3s;
        }
        
        .form-control:focus {
            outline: none;
            border-color: #007bff;
            box-shadow: 0 0 0 0.2rem rgba(0,123,255,0.25);
        }
        
        .btn-submit {
            background: linear-gradient(135deg, #007bff, #0056b3);
            color: white;
            padding: 14px 30px;
            border: none;
            border-radius: 6px;
            font-size: 16px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.3s;
            width: 100%;
        }
        
        .btn-submit:hover {
            background: linear-gradient(135deg, #0056b3, #004085);
            transform: translateY(-1px);
        }
        
        .message-label {
            display: block;
            margin-top: 15px;
            padding: 12px;
            border-radius: 6px;
            font-weight: 600;
        }
        
        .success {
            background-color: #d4edda;
            color: #155724;
            border: 1px solid #c3e6cb;
        }
        
        .error {
            background-color: #f8d7da;
            color: #721c24;
            border: 1px solid #f5c6cb;
        }
        
        .form-header {
            text-align: center;
            margin-bottom: 30px;
        }
        
        .form-header h2 {
            color: #333;
            margin-bottom: 10px;
        }
        
        .form-header p {
            color: #666;
            font-size: 14px;
        }
        
        .required {
            color: #dc3545;
        }
        
        .form-row {
            display: flex;
            gap: 15px;
        }
        
        .form-col {
            flex: 1;
        }
        
        @media (max-width: 768px) {
            .form-row {
                flex-direction: column;
            }
            
            .container {
                padding: 10px;
            }
            
            .sell-form {
                padding: 20px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <main class="content">
            <div class="form-header">
                <h2>Đăng tin bán xe ô tô</h2>
                <p>Điền thông tin chi tiết để có nhiều khách hàng quan tâm hơn</p>
            </div>
            
            <asp:Panel ID="pnlBanXe" runat="server" CssClass="sell-form">
                <div class="form-group">
                    <label>Tên xe <span class="required">*</span>:</label>
                    <asp:TextBox ID="txtTenXe" runat="server" CssClass="form-control" 
                                placeholder="VD: Honda City 2020, Toyota Vios 2019..." />
                    <asp:RequiredFieldValidator ID="rfvTenXe" runat="server" 
                                              ControlToValidate="txtTenXe"
                                              ErrorMessage="Vui lòng nhập tên xe"
                                              CssClass="error" Display="Dynamic" />
                </div>
                
                <div class="form-group">
                    <label>Giá mong muốn (triệu VNĐ) <span class="required">*</span>:</label>
                    <asp:TextBox ID="txtGia" runat="server" CssClass="form-control" 
                                placeholder="VD: 500, 800, 1200..." TextMode="Number" />
                    <asp:RequiredFieldValidator ID="rfvGia" runat="server" 
                                              ControlToValidate="txtGia"
                                              ErrorMessage="Vui lòng nhập giá mong muốn"
                                              CssClass="error" Display="Dynamic" />
                    <asp:RangeValidator ID="rvGia" runat="server" 
                                       ControlToValidate="txtGia"
                                       MinimumValue="1" MaximumValue="50000"
                                       Type="Double"
                                       ErrorMessage="Giá phải từ 1 đến 50,000 triệu"
                                       CssClass="error" Display="Dynamic" />
                </div>
                
                <div class="form-group">
                    <label>Mô tả chi tiết:</label>
                    <asp:TextBox ID="txtMoTa" runat="server" TextMode="MultiLine" Rows="5" 
                                CssClass="form-control" 
                                placeholder="Mô tả tình trạng xe, năm sản xuất, số km đã đi, lý do bán..." />
                </div>
                
                <div class="form-group">
                    <label>Hình ảnh xe:</label>
                    <asp:FileUpload ID="fuHinhAnh" runat="server" CssClass="form-control" 
                                   accept="image/*" />
                    <small style="color: #666;">Chấp nhận file: JPG, PNG, GIF (tối đa 5MB)</small>
                </div>
                
                <div class="form-row">
                    <div class="form-col">
                        <div class="form-group">
                            <label>Tên người liên hệ <span class="required">*</span>:</label>
                            <asp:TextBox ID="txtTenLienHe" runat="server" CssClass="form-control" 
                                        placeholder="Họ và tên của bạn" />
                            <asp:RequiredFieldValidator ID="rfvTenLienHe" runat="server" 
                                                      ControlToValidate="txtTenLienHe"
                                                      ErrorMessage="Vui lòng nhập tên liên hệ"
                                                      CssClass="error" Display="Dynamic" />
                        </div>
                    </div>
                    
                    <div class="form-col">
                        <div class="form-group">
                            <label>Số điện thoại <span class="required">*</span>:</label>
                            <asp:TextBox ID="txtSoDienThoai" runat="server" CssClass="form-control" 
                                        placeholder="0987654321" />
                            <asp:RequiredFieldValidator ID="rfvSoDienThoai" runat="server" 
                                                      ControlToValidate="txtSoDienThoai"
                                                      ErrorMessage="Vui lòng nhập số điện thoại"
                                                      CssClass="error" Display="Dynamic" />
                            <asp:RegularExpressionValidator ID="revSoDienThoai" runat="server"
                                                           ControlToValidate="txtSoDienThoai"
                                                           ValidationExpression="^[0-9]{10,11}$"
                                                           ErrorMessage="Số điện thoại phải có 10-11 chữ số"
                                                           CssClass="error" Display="Dynamic" />
                        </div>
                    </div>
                </div>
                
                <div class="form-group">
                    <label>Email liên hệ:</label>
                    <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" 
                                placeholder="email@example.com" TextMode="Email" />
                    <asp:RegularExpressionValidator ID="revEmail" runat="server"
                                                   ControlToValidate="txtEmail"
                                                   ValidationExpression="^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"
                                                   ErrorMessage="Email không đúng định dạng"
                                                   CssClass="error" Display="Dynamic" />
                </div>
                
                <asp:Button ID="btnDangTin" runat="server" Text="Đăng tin bán xe" 
                           CssClass="btn-submit" OnClick="btnDangTin_Click" />
            </asp:Panel>
            
            <asp:Label ID="lblThongBao" runat="server" CssClass="message-label" />
        </main>
    </div>
</asp:Content>