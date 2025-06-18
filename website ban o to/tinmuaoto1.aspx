<%@ Page Title="Tin tức ô tô" Language="C#" MasterPageFile="~/home.Master" AutoEventWireup="true" CodeBehind="tinmuaoto1.aspx.cs" Inherits="website_ban_o_to.tinmuaoto1" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/trangchu.css" rel="stylesheet" />
    <link href="~/tinmuaoto.css" rel="stylesheet" />
    <style>
        .news-container {
            max-width: 1200px;
            margin: 0 auto;
            padding: 20px;
        }
        
        .news-header {
            text-align: center;
            margin-bottom: 30px;
            color: #333;
            font-size: 28px;
            font-weight: bold;
            text-transform: uppercase;
        }
        
        .news-item {
            display: flex;
            background: #fff;
            border-radius: 10px;
            box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            margin-bottom: 20px;
            overflow: hidden;
            transition: transform 0.3s ease;
        }
        
        .news-item:hover {
            transform: translateY(-5px);
            box-shadow: 0 5px 20px rgba(0,0,0,0.15);
        }
        
        .news-thumb {
            width: 250px;
            height: 180px;
            object-fit: cover;
            flex-shrink: 0;
        }
        
        .news-content {
            padding: 20px;
            flex: 1;
            display: flex;
            flex-direction: column;
        }
        
        .news-title {
            color: #2c5aa0;
            text-decoration: none;
            font-size: 20px;
            font-weight: bold;
            margin-bottom: 10px;
            display: block;
        }
        
        .news-title:hover {
            color: #1e3d6f;
            text-decoration: underline;
        }
        
        .news-summary {
            color: #666;
            line-height: 1.6;
            margin-bottom: 15px;
            flex: 1;
        }
        
        .news-meta {
            display: flex;
            justify-content: space-between;
            align-items: center;
            color: #999;
            font-size: 14px;
        }
        
        .news-date {
            font-weight: bold;
        }
        
        .news-creator {
            font-style: italic;
        }
        
        .no-news {
            text-align: center;
            color: #666;
            font-size: 18px;
            margin: 50px 0;
        }
        
        @media (max-width: 768px) {
            .news-item {
                flex-direction: column;
            }
            
            .news-thumb {
                width: 100%;
                height: 200px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="news-container">
        <h2 class="news-header">TIN TỨC Ô TÔ MỚI NHẤT</h2>
        
        <asp:Repeater ID="rptNews" runat="server">
            <ItemTemplate>
                <div class="news-item">
                    <asp:Image ID="imgThumb" runat="server" 
                               ImageUrl='<%# !string.IsNullOrEmpty(Eval("ImagePath").ToString()) ? Eval("ImagePath") : "~/images/default-news.jpg" %>' 
                               CssClass="news-thumb" 
                               AlternateText='<%# Eval("Title") %>' />
                    
                    <div class="news-content">
                        <h3>
                            <asp:HyperLink ID="lnkTitle" runat="server" 
                                           Text='<%# Eval("Title") %>' 
                                           CssClass="news-title" />
                        </h3>
                        
                        <p class="news-summary">
                            <%# !string.IsNullOrEmpty(Eval("Summary").ToString()) ? 
                                (Eval("Summary").ToString().Length > 150 ? 
                                 Eval("Summary").ToString().Substring(0, 150) + "..." : 
                                 Eval("Summary").ToString()) : 
                                "Nội dung tin tức sẽ được cập nhật sớm..." %>
                        </p>
                        
                        <div class="news-meta">
                            <span class="news-date">
                                <i class="fa fa-calendar"></i>
                                <%# Eval("PublishedDate", "{0:dd/MM/yyyy HH:mm}") %>
                            </span>
                            
                            <span class="news-creator">
                                <i class="fa fa-user"></i>
                                Tác giả: <%# Eval("Creator.Username") != null ? Eval("Creator.Username") : "Admin" %>
                            </span>
                        </div>
                    </div>
                </div>
            </ItemTemplate>
            
         
        </asp:Repeater>
        
    
    </div>
</asp:Content>