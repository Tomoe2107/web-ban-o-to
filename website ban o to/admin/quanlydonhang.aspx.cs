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
    public partial class quanlydonhang : System.Web.UI.Page
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDonHang();
                LoadThongKe();
            }
        }

        private void LoadDonHang()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            o.OrderID,
                            o.OrderCode,
                            o.CustomerName,
                            o.CustomerEmail,
                            o.CustomerPhone,
                            o.TotalAmount,
                            o.OrderStatus,
                            o.OrderDate,
                            o.DeliveryDate,
                            o.Notes,
                            u.Username
                        FROM Orders o
                        INNER JOIN Users u ON o.UserID = u.UserID
                        ORDER BY o.OrderDate DESC";

                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvDonHang.DataSource = dt;
                    gvDonHang.DataBind();

                    // Hiển thị tổng số đơn hàng
                    lblTongDonHang.Text = dt.Rows.Count.ToString();
                }
            }
            catch (Exception ex)
            {
                // Log lỗi và hiển thị thông báo
                lblMessage.Text = "Lỗi khi tải dữ liệu: " + ex.Message;
                lblMessage.CssClass = "alert alert-danger";
            }
        }

        private void LoadThongKe()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Thống kê tổng quan
                    string queryTongQuan = @"
                        SELECT 
                            COUNT(*) as TongDonHang,
                            SUM(CASE WHEN OrderStatus = @Processing THEN 1 ELSE 0 END) as DangXuLy,
                            SUM(CASE WHEN OrderStatus = @Delivered THEN 1 ELSE 0 END) as DaGiao,
                            SUM(CASE WHEN OrderStatus = @Cancelled THEN 1 ELSE 0 END) as DaHuy,
                            ISNULL(SUM(TotalAmount), 0) as TongDoanhThu,
                            ISNULL(SUM(CASE WHEN OrderStatus = @Delivered THEN TotalAmount ELSE 0 END), 0) as DoanhThuThanhCong
                        FROM Orders";

                    SqlCommand cmdTongQuan = new SqlCommand(queryTongQuan, conn);
                    cmdTongQuan.Parameters.AddWithValue("@Processing", OrderStatus.Processing);
                    cmdTongQuan.Parameters.AddWithValue("@Delivered", OrderStatus.Delivered);
                    cmdTongQuan.Parameters.AddWithValue("@Cancelled", OrderStatus.Cancelled);

                    SqlDataReader reader = cmdTongQuan.ExecuteReader();
                    if (reader.Read())
                    {
                        lblTongDonHang.Text = reader["TongDonHang"].ToString();
                        lblDangXuLy.Text = reader["DangXuLy"].ToString();
                        lblDaGiao.Text = reader["DaGiao"].ToString();
                        lblDaHuy.Text = reader["DaHuy"].ToString();
                        lblTongDoanhThu.Text = string.Format("{0:N0} VNĐ", reader["TongDoanhThu"]);
                        lblDoanhThuThanhCong.Text = string.Format("{0:N0} VNĐ", reader["DoanhThuThanhCong"]);
                    }
                    reader.Close();

                    // Thống kê theo tháng
                    LoadThongKeTheoThang(conn);
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Lỗi khi tải thống kê: " + ex.Message;
                lblMessage.CssClass = "alert alert-danger";
            }
        }

        private void LoadThongKeTheoThang(SqlConnection conn)
        {
            string queryThang = @"
                SELECT 
                    YEAR(OrderDate) as Nam,
                    MONTH(OrderDate) as Thang,
                    COUNT(*) as SoDonHang,
                    SUM(TotalAmount) as DoanhThu
                FROM Orders 
                WHERE OrderDate >= DATEADD(MONTH, -6, GETDATE())
                GROUP BY YEAR(OrderDate), MONTH(OrderDate)
                ORDER BY YEAR(OrderDate) DESC, MONTH(OrderDate) DESC";

            SqlDataAdapter daThang = new SqlDataAdapter(queryThang, conn);
            DataTable dtThang = new DataTable();
            daThang.Fill(dtThang);

            gvThongKeThang.DataSource = dtThang;
            gvThongKeThang.DataBind();
        }

        protected void gvDonHang_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            try
            {
                int orderID = Convert.ToInt32(e.CommandArgument);
                string trangThaiMoi = "";

                switch (e.CommandName)
                {
                    case "XacNhan":
                        trangThaiMoi = OrderStatus.Processing;
                        break;
                    case "GiaoHang":
                        trangThaiMoi = OrderStatus.Delivered;
                        break;
                    case "Huy":
                        trangThaiMoi = OrderStatus.Cancelled;
                        break;
                }

                if (!string.IsNullOrEmpty(trangThaiMoi))
                {
                    CapNhatTrangThaiDonHang(orderID, trangThaiMoi);
                    LoadDonHang();
                    LoadThongKe();

                    lblMessage.Text = "Cập nhật trạng thái đơn hàng thành công!";
                    lblMessage.CssClass = "alert alert-success";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Lỗi khi cập nhật: " + ex.Message;
                lblMessage.CssClass = "alert alert-danger";
            }
        }

        private void CapNhatTrangThaiDonHang(int orderID, string trangThaiMoi)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    UPDATE Orders 
                    SET OrderStatus = @TrangThai,
                        DeliveryDate = CASE WHEN @TrangThai = @Delivered THEN GETDATE() ELSE DeliveryDate END
                    WHERE OrderID = @OrderID";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@TrangThai", trangThaiMoi);
                cmd.Parameters.AddWithValue("@OrderID", orderID);
                cmd.Parameters.AddWithValue("@Delivered", OrderStatus.Delivered);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        protected void btnTimKiem_Click(object sender, EventArgs e)
        {
            TimKiemDonHang();
        }

        private void TimKiemDonHang()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string whereClause = "WHERE 1=1";
                    List<SqlParameter> parameters = new List<SqlParameter>();

                    // Tìm kiếm theo mã đơn hàng
                    if (!string.IsNullOrEmpty(txtTimKiem.Text.Trim()))
                    {
                        whereClause += " AND (o.OrderCode LIKE @SearchText OR o.CustomerName LIKE @SearchText OR o.CustomerPhone LIKE @SearchText)";
                        parameters.Add(new SqlParameter("@SearchText", "%" + txtTimKiem.Text.Trim() + "%"));
                    }

                    // Lọc theo trạng thái
                    if (ddlTrangThai.SelectedValue != "")
                    {
                        whereClause += " AND o.OrderStatus = @TrangThai";
                        parameters.Add(new SqlParameter("@TrangThai", ddlTrangThai.SelectedValue));
                    }

                    // Lọc theo ngày
                    if (!string.IsNullOrEmpty(txtTuNgay.Text))
                    {
                        whereClause += " AND o.OrderDate >= @TuNgay";
                        parameters.Add(new SqlParameter("@TuNgay", DateTime.Parse(txtTuNgay.Text)));
                    }

                    if (!string.IsNullOrEmpty(txtDenNgay.Text))
                    {
                        whereClause += " AND o.OrderDate <= @DenNgay";
                        parameters.Add(new SqlParameter("@DenNgay", DateTime.Parse(txtDenNgay.Text).AddDays(1)));
                    }

                    string query = $@"
                        SELECT 
                            o.OrderID,
                            o.OrderCode,
                            o.CustomerName,
                            o.CustomerEmail,
                            o.CustomerPhone,
                            o.TotalAmount,
                            o.OrderStatus,
                            o.OrderDate,
                            o.DeliveryDate,
                            o.Notes,
                            u.Username
                        FROM Orders o
                        INNER JOIN Users u ON o.UserID = u.UserID
                        {whereClause}
                        ORDER BY o.OrderDate DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddRange(parameters.ToArray());

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvDonHang.DataSource = dt;
                    gvDonHang.DataBind();

                    lblMessage.Text = $"Tìm thấy {dt.Rows.Count} đơn hàng";
                    lblMessage.CssClass = "alert alert-info";
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Lỗi khi tìm kiếm: " + ex.Message;
                lblMessage.CssClass = "alert alert-danger";
            }
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            txtTimKiem.Text = "";
            ddlTrangThai.SelectedIndex = 0;
            txtTuNgay.Text = "";
            txtDenNgay.Text = "";
            LoadDonHang();
            lblMessage.Text = "";
        }

        protected void gvDonHang_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // Tô màu theo trạng thái
                string trangThai = DataBinder.Eval(e.Row.DataItem, "OrderStatus").ToString();
                switch (trangThai)
                {
                    case "Đang xử lý":
                        e.Row.CssClass += " status-processing";
                        break;
                    case "Đã giao":
                        e.Row.CssClass += " status-delivered";
                        break;
                    case "Đã hủy":
                        e.Row.CssClass += " status-cancelled";
                        break;
                }

                // Ẩn/hiện nút theo trạng thái
                Button btnXacNhan = (Button)e.Row.FindControl("btnXacNhan");
                Button btnGiaoHang = (Button)e.Row.FindControl("btnGiaoHang");
                Button btnHuy = (Button)e.Row.FindControl("btnHuy");

                if (trangThai == OrderStatus.Delivered || trangThai == OrderStatus.Cancelled)
                {
                    btnXacNhan.Visible = false;
                    btnGiaoHang.Visible = false;
                    btnHuy.Visible = false;
                }
                else if (trangThai == OrderStatus.Processing)
                {
                    btnXacNhan.Visible = false;
                }
            }
        }

        protected void btnXuatBaoCao_Click(object sender, EventArgs e)
        {
            // Xuất báo cáo Excel (có thể implement sau)
            lblMessage.Text = "Chức năng xuất báo cáo đang được phát triển";
            lblMessage.CssClass = "alert alert-info";
        }
    }
}