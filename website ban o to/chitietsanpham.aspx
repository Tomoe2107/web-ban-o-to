<%@ Page Title="" Language="C#" MasterPageFile="~/home.Master" AutoEventWireup="true" CodeBehind="chitietsanpham.aspx.cs" Inherits="website_ban_o_to.chitietsanpham" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/trangchu.css" rel="stylesheet" />
    <style>
        .product-detail-container {
            max-width: 1200px;
            margin: 20px auto;
            padding: 20px;
            background: #fff;
            border-radius: 8px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
        }
        
        .detail-image {
            width: 100%;
            max-width: 500px;
            height: auto;
            border-radius: 8px;
            margin-bottom: 20px;
        }
        
        .detail-info {
            padding: 20px 0;
        }
        
        .detail-info h2 {
            color: #333;
            margin-bottom: 20px;
            font-size: 28px;
        }
        
        .detail-info p {
            margin: 10px 0;
            font-size: 16px;
            line-height: 1.6;
        }
        
        .detail-info strong {
            color: #444;
            font-weight: 600;
        }
        
        .price-highlight {
            font-size: 24px;
            color: #e74c3c;
            font-weight: bold;
        }
        
        .contact-info {
            background: #f8f9fa;
            padding: 15px;
            border-radius: 5px;
            margin-top: 20px;
        }
        
        .back-button {
            display: inline-block;
            padding: 10px 20px;
            background: #007bff;
            color: white;
            text-decoration: none;
            border-radius: 5px;
            margin-bottom: 20px;
            height:50px
        }
        
        .back-button:hover {
            background: #0056b3;
        }
        
        @media (min-width: 768px) {
            .product-detail-container {
                display: grid;
                grid-template-columns: 1fr 1fr;
                gap: 30px;
                align-items: start;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <a href="javascript:history.back()" class="back-button">← Quay lại</a>
        
        <div class="product-detail-container">
            <div class="image-section">
                 <asp:Image ID="imgCar" runat="server" 
            ImageUrl='<%# !string.IsNullOrEmpty(Eval("ImagePath").ToString()) ? Eval("ImagePath") : "~/images/default-news.jpg" %>' 
 CssClass="detail-image"            AlternateText='<%# Eval("Title") %>' />
            </div>
            
            <div class="detail-info">
                <h2><asp:Label ID="lblName" runat="server" /></h2>
                
                <p><strong>Giá:</strong> 
                   <span class="price-highlight"><asp:Label ID="lblPrice" runat="server" /></span>
                </p>
                
                <p><strong>Khu vực:</strong> 
                   <asp:Label ID="lblLocation" runat="server" />
                </p>
                
                <p><strong>Mô tả:</strong><br />
                   <asp:Label ID="lblDescription" runat="server" />
                </p>
                
                <div class="contact-info">
                    <h3>Thông tin liên hệ</h3>
                    <p><strong>Người liên hệ:</strong> 
                       <asp:Label ID="lblContact" runat="server" />
                    </p>
                    <p><strong>Điện thoại:</strong> 
                       <asp:Label ID="lblPhone" runat="server" />
                    </p>
                </div>
            </div>
        </div>
    </div>
</asp:Content>