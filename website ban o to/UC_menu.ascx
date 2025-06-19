<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UC_menu.ascx.cs" Inherits="website_ban_o_to.UC_menu" %>

<style>
    /* Header styling */
    .header {
        background: linear-gradient(135deg, #c0392b, #e74c3c);
        padding: 0;
        box-shadow: 0 2px 6px rgba(0,0,0,0.1);
        position: sticky;
        top: 0;
        z-index: 1000;
        transition: all 0.3s ease;
    }

    /* Header Right - User Info */
    .header-right {
        background: rgba(0,0,0,0.1);
        padding: 8px 20px;
        text-align: right;
        font-size: 13px;
        border-bottom: 1px solid rgba(255,255,255,0.1);
        min-height: 30px;
    }

    .header-right a {
        color: white;
        text-decoration: none;
        margin: 0 8px;
        padding: 4px 8px;
        border-radius: 3px;
        transition: background-color 0.3s ease;
    }

    .header-right a:hover {
        background-color: rgba(255,255,255,0.1);
        text-decoration: none;
    }

    .user-info {
        color: black;
        margin-right: 15px;
        display: inline-block;
        font-weight: 500;
        background-color:white
    }

    .logout-btn {
        background: #e74c3c;
        border: 1px solid rgba(255,255,255,0.3);
        color: white;
        padding: 4px 12px;
        border-radius: 4px;
        cursor: pointer;
        font-size: 12px;
        transition: all 0.3s ease;
        margin-left: 10px;
    }

    .logout-btn:hover {
        background: #c0392b;
        transform: translateY(-1px);
    }

    /* Separator styling */
    .separator {
        color: rgba(255,255,255,0.6);
        margin: 0 5px;
    }

    /* Navbar */
    .navbar {
        padding: 0 20px;
    }

    .nav-menu {
        list-style: none;
        margin: 0;
        padding: 0;
        display: flex;
        align-items: center;
    }

    .nav-menu li {
        margin: 0;
    }

    .nav-menu li a {
        display: block;
        padding: 15px 20px;
        color: white;
        text-decoration: none;
        font-weight: 500;
        font-size: 16px;
        transition: all 0.3s ease;
        border-bottom: 3px solid transparent;
        position: relative;
    }

    .nav-menu li a:hover {
        background-color: rgba(255,255,255,0.1);
        border-bottom-color: rgba(255,255,255,0.5);
        text-decoration: none;
        transform: translateY(-2px);
    }

    .nav-menu li a.active {
        background-color: rgba(255,255,255,0.2);
        border-bottom-color: #fff;
        font-weight: 600;
    }

    /* Cart badge */
    .cart-container {
        position: relative;
    }

    .cart-badge {
        position: absolute;
        top: 8px;
        right: 8px;
        background: #ff6b6b;
        color: white;
        border-radius: 50%;
        padding: 2px 6px;
        font-size: 11px;
        font-weight: bold;
        min-width: 18px;
        text-align: center;
        box-shadow: 0 2px 4px rgba(0,0,0,0.3);
        animation: pulse 2s infinite;
    }

    @keyframes pulse {
        0% { transform: scale(1); }
        50% { transform: scale(1.1); }
        100% { transform: scale(1); }
    }

    /* Responsive design */
    @media (max-width: 768px) {
        .header-right {
            font-size: 11px;
            padding: 5px 10px;
        }

        .header-right a {
            margin: 0 4px;
            padding: 2px 6px;
        }

        .navbar {
            padding: 0 10px;
        }

        .nav-menu {
            flex-wrap: wrap;
        }

        .nav-menu li a {
            padding: 12px 15px;
            font-size: 14px;
        }

        .user-info {
            display: block;
            margin-bottom: 5px;
        }
    }

    /* Scroll effect */
    .header.scrolled {
        background: linear-gradient(135deg, #a93226, #c0392b) !important;
        box-shadow: 0 2px 8px rgba(0,0,0,0.3) !important;
    }

    /* Icons */
    .nav-menu li a::before {
        margin-right: 5px;
    }
</style>

<!-- Header Right -->
<div class="header-right">
    <!-- ========== HIỂN THỊ KHI ĐÃ ĐĂNG NHẬP ========== -->
    <asp:Label ID="lblUserInfo" runat="server" CssClass="user-info"  Visible="false" />
    <asp:Button ID="lnkDangXuat" runat="server" Text="🚪 Đăng xuất" CssClass="logout-btn" 
                OnClick="lnkDangXuat_Click" Visible="false" 
                OnClientClick="return confirm('Bạn có chắc muốn đăng xuất?');" />
    
    <!-- ========== HIỂN THỊ KHI CHƯA ĐĂNG NHẬP ========== -->
    <asp:HyperLink ID="lnkDangNhap" runat="server" NavigateUrl="~/dangnhap1.aspx">🔑 Đăng nhập</asp:HyperLink>
    <span runat="server" id="separator1" class="separator" visible="true">|</span>
    <asp:HyperLink ID="lnkDangKy" runat="server" NavigateUrl="~/dangky1.aspx">📝 Đăng ký</asp:HyperLink>
    
    <!-- ========== LUÔN HIỂN THỊ ========== -->
    <span class="separator">|</span>
    <asp:HyperLink ID="lnkLienHe" runat="server" NavigateUrl="~/lienhe1.aspx">📞 Liên hệ</asp:HyperLink>
    
    <!-- ========== CHỈ HIỂN THỊ CHO ADMIN ========== -->
    <span runat="server" id="adminSeparator" class="separator" visible="false">|</span>
    <asp:HyperLink ID="lnkQuanLy" runat="server" NavigateUrl="~/admin/quanly1.aspx" Visible="false">🛠️ Quản lý</asp:HyperLink>
</div>

<nav class="navbar">
    <ul class="nav-menu">
        <li>
            <asp:HyperLink ID="lnkTrangChu" NavigateUrl="~/trangchu1.aspx" Text="🏠 Trang chủ" runat="server" />
        </li>
        <li>
            <asp:HyperLink ID="lnkTinMuaOto" NavigateUrl="~/tinmuaoto1.aspx" Text="📰 Tin mua ô tô" runat="server" />
        </li>
        <li>
            <asp:HyperLink ID="lnkBanOto" NavigateUrl="~/banoto1.aspx" Text="🚗 Bán ô tô" runat="server" />
        </li>
        <li class="cart-container">
            <asp:HyperLink ID="lnkCanMua" NavigateUrl="~/giohang.aspx" Text="🛒 Giỏ hàng" runat="server" />
            <span runat="server" id="spanCartBadge" class="cart-badge" visible="false">0</span>
        </li>
    </ul>
</nav>

<script>
    // Scroll effect cho header
    window.addEventListener('scroll', function () {
        var header = document.querySelector('.header');
        if (window.scrollY > 10) {
            header.classList.add('scrolled');
        } else {
            header.classList.remove('scrolled');
        }
    });

    // Smooth scroll cho internal links
    document.addEventListener('DOMContentLoaded', function () {
        const links = document.querySelectorAll('a[href^="#"]');
        links.forEach(link => {
            link.addEventListener('click', function (e) {
                e.preventDefault();
                const target = document.querySelector(this.getAttribute('href'));
                if (target) {
                    target.scrollIntoView({
                        behavior: 'smooth',
                        block: 'start'
                    });
                }
            });
        });
    });

    // Active menu highlight animation
    document.addEventListener('DOMContentLoaded', function () {
        const activeLink = document.querySelector('.nav-menu a.active');
        if (activeLink) {
            activeLink.style.animation = 'fadeInDown 0.5s ease-out';
        }
    });

    @keyframes fadeInDown {
        from {
            opacity: 0;
            transform: translateY(-10px);
        }
        to {
            opacity: 1;
            transform: translateY(0);
        }
    }

    // Mobile menu toggle (nếu cần)
    function toggleMobileMenu() {
        const navMenu = document.querySelector('.nav-menu');
        navMenu.classList.toggle('mobile-open');
    }
</script>