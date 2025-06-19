using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using website_ban_o_to.Models;

namespace website_ban_o_to.admin
{
    public partial class thongke1 : System.Web.UI.Page
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Kiểm tra quyền admin (bỏ comment khi có authentication)
                //if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
                //{
                //    Response.Redirect("~/dangnhap.aspx");
                //}

                KhoiTaoDropDownList();
                HienThiTheoThang();
            }
        }

        private void KhoiTaoDropDownList()
        {
            // Thiết lập tháng hiện tại
            ddlThang.SelectedValue = DateTime.Now.Month.ToString();

            // Thiết lập năm hiện tại cho tất cả dropdown năm
            ddlNam.SelectedValue = DateTime.Now.Year.ToString();
            ddlNamQuy.SelectedValue = DateTime.Now.Year.ToString();
            ddlNamThongKe.SelectedValue = DateTime.Now.Year.ToString();

            // Thiết lập quý hiện tại
            int quyHienTai = (DateTime.Now.Month - 1) / 3 + 1;
            ddlQuy.SelectedValue = quyHienTai.ToString();
        }

        protected void ddlLoaiThongKe_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Ẩn tất cả các div filter
            divThang.Style["display"] = "none";
            divQuy.Style["display"] = "none";
            divNam.Style["display"] = "none";

            // Hiển thị div tương ứng
            switch (ddlLoaiThongKe.SelectedValue)
            {
                case "0":
                    divThang.Style["display"] = "block";
                    break;
                case "1":
                    divQuy.Style["display"] = "block";
                    break;
                case "2":
                    divNam.Style["display"] = "block";
                    break;
            }
        }

        protected void btnXemThongKe_Click(object sender, EventArgs e)
        {
            try
            {
                switch (ddlLoaiThongKe.SelectedValue)
                {
                    case "0": HienThiTheoThang(); break;
                    case "1": HienThiTheoQuy(); break;
                    case "2": HienThiTheoNam(); break;
                }
            }
            catch (Exception ex)
            {
                // Hiển thị thông báo lỗi
                Response.Write("<script>alert('Lỗi: " + ex.Message + "');</script>");
            }
        }

        private void HienThiTheoThang()
        {
            int thang = int.Parse(ddlThang.SelectedValue);
            int nam = int.Parse(ddlNam.SelectedValue);

            // Lấy dữ liệu doanh thu theo ngày trong tháng
            DataTable dtDoanhThu = LayDuLieuDoanhThuTheoNgay(thang, nam);

            // Tính toán và hiển thị tổng hợp
            TinhToanVaHienThiTongHop(dtDoanhThu);

            // Lấy và hiển thị chi tiết đơn hàng
            DataTable dtChiTiet = LayChiTietDonHang(thang, nam, 0);
            gvChiTietDonHang.DataSource = dtChiTiet;
            gvChiTietDonHang.DataBind();

            // Lấy và hiển thị sản phẩm bán chạy
            DataTable dtBanChay = LaySanPhamBanChay(thang, nam, 0);
            gvSanPhamBanChay.DataSource = dtBanChay;
            gvSanPhamBanChay.DataBind();
        }

        private void HienThiTheoQuy()
        {
            int quy = int.Parse(ddlQuy.SelectedValue);
            int nam = int.Parse(ddlNamQuy.SelectedValue);

            // Tính tháng bắt đầu và kết thúc của quý
            int thangBatDau = (quy - 1) * 3 + 1;
            int thangKetThuc = quy * 3;

            // Lấy dữ liệu doanh thu theo tháng trong quý
            DataTable dtDoanhThu = LayDuLieuDoanhThuTheoThang(thangBatDau, thangKetThuc, nam);

            // Tính toán và hiển thị tổng hợp
            TinhToanVaHienThiTongHop(dtDoanhThu);

            // Lấy và hiển thị chi tiết đơn hàng
            DataTable dtChiTiet = LayChiTietDonHangTheoQuy(quy, nam);
            gvChiTietDonHang.DataSource = dtChiTiet;
            gvChiTietDonHang.DataBind();

            // Lấy và hiển thị sản phẩm bán chạy
            DataTable dtBanChay = LaySanPhamBanChayTheoQuy(quy, nam);
            gvSanPhamBanChay.DataSource = dtBanChay;
            gvSanPhamBanChay.DataBind();
        }

        private void HienThiTheoNam()
        {
            int nam = int.Parse(ddlNamThongKe.SelectedValue);

            // Lấy dữ liệu doanh thu theo tháng trong năm
            DataTable dtDoanhThu = LayDuLieuDoanhThuTheoThang(1, 12, nam);

            // Tính toán và hiển thị tổng hợp
            TinhToanVaHienThiTongHop(dtDoanhThu);

            // Lấy và hiển thị chi tiết đơn hàng
            DataTable dtChiTiet = LayChiTietDonHangTheoNam(nam);
            gvChiTietDonHang.DataSource = dtChiTiet;
            gvChiTietDonHang.DataBind();

            // Lấy và hiển thị sản phẩm bán chạy
            DataTable dtBanChay = LaySanPhamBanChayTheoNam(nam);
            gvSanPhamBanChay.DataSource = dtBanChay;
            gvSanPhamBanChay.DataBind();
        }

        private DataTable LayDuLieuDoanhThuTheoNgay(int thang, int nam)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Ngay", typeof(int));
            dt.Columns.Add("DoanhThu", typeof(decimal));
            dt.Columns.Add("SoDonHang", typeof(int));

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        DAY(OrderDate) as Ngay,
                        ISNULL(SUM(TotalAmount), 0) as DoanhThu,
                        COUNT(*) as SoDonHang
                    FROM Orders 
                    WHERE MONTH(OrderDate) = @Thang 
                        AND YEAR(OrderDate) = @Nam
                        AND OrderStatus = @DeliveredStatus
                    GROUP BY DAY(OrderDate)
                    ORDER BY DAY(OrderDate)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Thang", thang);
                cmd.Parameters.AddWithValue("@Nam", nam);
                cmd.Parameters.AddWithValue("@DeliveredStatus", OrderStatus.Delivered);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        private DataTable LayDuLieuDoanhThuTheoThang(int thangBatDau, int thangKetThuc, int nam)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Thang", typeof(int));
            dt.Columns.Add("DoanhThu", typeof(decimal));
            dt.Columns.Add("SoDonHang", typeof(int));

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        MONTH(OrderDate) as Thang,
                        ISNULL(SUM(TotalAmount), 0) as DoanhThu,
                        COUNT(*) as SoDonHang
                    FROM Orders 
                    WHERE MONTH(OrderDate) BETWEEN @ThangBatDau AND @ThangKetThuc
                        AND YEAR(OrderDate) = @Nam
                        AND OrderStatus = @DeliveredStatus
                    GROUP BY MONTH(OrderDate)
                    ORDER BY MONTH(OrderDate)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ThangBatDau", thangBatDau);
                cmd.Parameters.AddWithValue("@ThangKetThuc", thangKetThuc);
                cmd.Parameters.AddWithValue("@Nam", nam);
                cmd.Parameters.AddWithValue("@DeliveredStatus", OrderStatus.Delivered);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }

            return dt;
        }

        private void TinhToanVaHienThiTongHop(DataTable dt)
        {
            decimal tongDoanhThu = 0;
            decimal caoNhat = 0;
            decimal thapNhat = decimal.MaxValue;
            int tongDonHang = 0;

            foreach (DataRow row in dt.Rows)
            {
                decimal doanhThu = Convert.ToDecimal(row["DoanhThu"]);
                int soDonHang = Convert.ToInt32(row["SoDonHang"]);

                tongDoanhThu += doanhThu;
                tongDonHang += soDonHang;
                caoNhat = Math.Max(caoNhat, doanhThu);

                if (doanhThu > 0)
                    thapNhat = Math.Min(thapNhat, doanhThu);
            }

            lblTongDoanhThu.Text = tongDoanhThu.ToString("N0") + " VNĐ";
            lblTongDonHang.Text = tongDonHang.ToString();
            lblDonHangCaoNhat.Text = (caoNhat == 0 ? "0" : caoNhat.ToString("N0")) + " VNĐ";
            lblDonHangThapNhat.Text = (thapNhat == decimal.MaxValue ? "0" : thapNhat.ToString("N0")) + " VNĐ";
        }

        private DataTable LayChiTietDonHang(int thang, int nam, int quy)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string whereClause = "";
                if (quy == 0) // Theo tháng
                {
                    whereClause = "WHERE MONTH(o.OrderDate) = @Thang AND YEAR(o.OrderDate) = @Nam";
                }

                string query = $@"
                    SELECT 
                        o.OrderCode as MaDon,
                        o.OrderDate as NgayDat,
                        o.CustomerName as KhachHang,
                        o.TotalAmount as TongTien,
                        o.OrderStatus as TrangThai
                    FROM Orders o
                    {whereClause}
                    ORDER BY o.OrderDate DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                if (quy == 0)
                {
                    cmd.Parameters.AddWithValue("@Thang", thang);
                    cmd.Parameters.AddWithValue("@Nam", nam);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private DataTable LayChiTietDonHangTheoQuy(int quy, int nam)
        {
            int thangBatDau = (quy - 1) * 3 + 1;
            int thangKetThuc = quy * 3;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        o.OrderCode as MaDon,
                        o.OrderDate as NgayDat,
                        o.CustomerName as KhachHang,
                        o.TotalAmount as TongTien,
                        o.OrderStatus as TrangThai
                    FROM Orders o
                    WHERE MONTH(o.OrderDate) BETWEEN @ThangBatDau AND @ThangKetThuc
                        AND YEAR(o.OrderDate) = @Nam
                    ORDER BY o.OrderDate DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ThangBatDau", thangBatDau);
                cmd.Parameters.AddWithValue("@ThangKetThuc", thangKetThuc);
                cmd.Parameters.AddWithValue("@Nam", nam);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private DataTable LayChiTietDonHangTheoNam(int nam)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT 
                        o.OrderCode as MaDon,
                        o.OrderDate as NgayDat,
                        o.CustomerName as KhachHang,
                        o.TotalAmount as TongTien,
                        o.OrderStatus as TrangThai
                    FROM Orders o
                    WHERE YEAR(o.OrderDate) = @Nam
                    ORDER BY o.OrderDate DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Nam", nam);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private DataTable LaySanPhamBanChay(int thang, int nam, int loai)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string whereClause = "";
                if (loai == 0) // Theo tháng
                {
                    whereClause = "AND MONTH(o.OrderDate) = @Thang AND YEAR(o.OrderDate) = @Nam";
                }

                string query = $@"
                    SELECT TOP 10
                        c.CarName as TenXe,
                        SUM(od.Quantity) as SoLuong,
                        SUM(od.TotalPrice) as DoanhThu
                    FROM OrderDetails od
                    INNER JOIN Orders o ON od.OrderID = o.OrderID
                    INNER JOIN Cars c ON od.CarID = c.CarID
                    WHERE o.OrderStatus = @DeliveredStatus {whereClause}
                    GROUP BY c.CarName, c.CarID
                    ORDER BY SUM(od.Quantity) DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DeliveredStatus", OrderStatus.Delivered);
                if (loai == 0)
                {
                    cmd.Parameters.AddWithValue("@Thang", thang);
                    cmd.Parameters.AddWithValue("@Nam", nam);
                }

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private DataTable LaySanPhamBanChayTheoQuy(int quy, int nam)
        {
            int thangBatDau = (quy - 1) * 3 + 1;
            int thangKetThuc = quy * 3;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT TOP 10
                        c.CarName as TenXe,
                        SUM(od.Quantity) as SoLuong,
                        SUM(od.TotalPrice) as DoanhThu
                    FROM OrderDetails od
                    INNER JOIN Orders o ON od.OrderID = o.OrderID
                    INNER JOIN Cars c ON od.CarID = c.CarID
                    WHERE o.OrderStatus = @DeliveredStatus 
                        AND MONTH(o.OrderDate) BETWEEN @ThangBatDau AND @ThangKetThuc
                        AND YEAR(o.OrderDate) = @Nam
                    GROUP BY c.CarName, c.CarID
                    ORDER BY SUM(od.Quantity) DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DeliveredStatus", OrderStatus.Delivered);
                cmd.Parameters.AddWithValue("@ThangBatDau", thangBatDau);
                cmd.Parameters.AddWithValue("@ThangKetThuc", thangKetThuc);
                cmd.Parameters.AddWithValue("@Nam", nam);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private DataTable LaySanPhamBanChayTheoNam(int nam)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT TOP 10
                        c.CarName as TenXe,
                        SUM(od.Quantity) as SoLuong,
                        SUM(od.TotalPrice) as DoanhThu
                    FROM OrderDetails od
                    INNER JOIN Orders o ON od.OrderID = o.OrderID
                    INNER JOIN Cars c ON od.CarID = c.CarID
                    WHERE o.OrderStatus = @DeliveredStatus 
                        AND YEAR(o.OrderDate) = @Nam
                    GROUP BY c.CarName, c.CarID
                    ORDER BY SUM(od.Quantity) DESC";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@DeliveredStatus", OrderStatus.Delivered);
                cmd.Parameters.AddWithValue("@Nam", nam);

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        protected void gvChiTietDonHang_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvChiTietDonHang.PageIndex = e.NewPageIndex;

            // Gọi lại phương thức hiển thị tương ứng
            switch (ddlLoaiThongKe.SelectedValue)
            {
                case "0": HienThiTheoThang(); break;
                case "1": HienThiTheoQuy(); break;
                case "2": HienThiTheoNam(); break;
            }
        }
    }
}