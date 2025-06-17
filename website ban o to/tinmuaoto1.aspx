<%@ Page Title="" Language="C#" MasterPageFile="~/home.Master" AutoEventWireup="true" CodeBehind="tinmuaoto1.aspx.cs" Inherits="website_ban_o_to.tinmuaoto1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="~/trangchu.css" rel="stylesheet" />
    <link href="~\tinmuaoto.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="container">
        <main class="content">
            <h2>TIN TỨC Ô TÔ MỚI NHẤT</h2>
            <asp:Repeater ID="rptNews" runat="server">
                <ItemTemplate>
                    <div class="news-item">
                        <asp:Image ID="imgThumb" runat="server" ImageUrl='<%# Eval("ImageUrl") %>' CssClass="news-thumb" />
                        <div class="news-content">
                            <h3>
                                <asp:HyperLink ID="lnkTitle" runat="server" 
                                               NavigateUrl='<%# "~/chitiettintuc.aspx?id=" + Eval("Id") %>' 
                                               Text='<%# Eval("Title") %>' CssClass="news-title" />
                            </h3>
                            <p class="news-summary"><%# Eval("Summary") %></p>
                            <span class="news-date"><%# Eval("Date", "{0:dd/MM/yyyy}") %></span>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
        </main>
    </div>
</asp:Content>