using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace website_ban_o_to.admin
{
    public partial class quanlydonhang : System.Web.UI.Page
    {
        string connectionString = "Data Source=DESKTOP-NMTRDC3\\MSSQLSERVER01;Initial Catalog=BanOTo;User ID=sa;Password=Hien_2004";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadDonHang();
            }
        }

        private void LoadDonHang()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"SELECT MaDon, NgayDat, KhachHang, DiaChi, TongTien, TrangThai 
                                 FROM DonHang ORDER BY NgayDat DESC";

                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                //da.Fill(dt);

                gvDonHang.DataSource = dt;
                gvDonHang.DataBind(); //khi có data thì kích hoạt 2 dòng này 
            }
        }

        protected void gvDonHang_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            string maDon = e.CommandArgument.ToString();
            string trangThaiMoi = "";

            if (e.CommandName == "XacNhan")
            {
                trangThaiMoi = "Đã xác nhận";
            }
            else if (e.CommandName == "Huy")
            {
                trangThaiMoi = "Đã huỷ";
            }

            if (!string.IsNullOrEmpty(trangThaiMoi))
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "UPDATE DonHang SET TrangThai = @TrangThai WHERE MaDon = @MaDon";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@TrangThai", trangThaiMoi);
                    cmd.Parameters.AddWithValue("@MaDon", maDon);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                LoadDonHang(); // Refresh danh sách sau khi cập nhật
            }
        }
    }
}