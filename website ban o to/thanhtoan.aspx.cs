using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace website_ban_o_to
{
    public partial class thanhtoan : System.Web.UI.Page
    {
        private string connectionString = "Data Source=DESKTOP-NMTRDC3\\MSSQLSERVER01;Initial Catalog=BanOto;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True";

        protected void Page_Load(object sender, EventArgs e)
        {
            lblThongBao.Visible = false;
            LoadGioHang();
        }
        private void LoadGioHang()
        {
            if (Session["Cart"] is List<CartItem> cart && cart.Count > 0)
            {
                gvGioHang.DataSource = cart;
                gvGioHang.DataBind();
            }
            else
            {
                lblThongBao.Text = "Giỏ hàng đang trống!";
                lblThongBao.Visible = true;
            }
        }

        protected void btnDatHang_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHoTen.Text) || string.IsNullOrWhiteSpace(txtDiaChi.Text) || string.IsNullOrWhiteSpace(txtDienThoai.Text))
            {
                lblThongBao.Text = "Vui lòng nhập đầy đủ thông tin.";
                lblThongBao.Visible = true;
                return;
            }

            if (Session["Cart"] is List<CartItem> cart && cart.Count > 0)
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlTransaction tran = conn.BeginTransaction();

                    try
                    {
                        string queryDon = @"INSERT INTO DonHang (HoTen, DiaChi, DienThoai, Email, NgayDat, TrangThai)
                                            OUTPUT INSERTED.ID
                                            VALUES (@HoTen, @DiaChi, @DienThoai, @Email, GETDATE(), N'Chờ xác nhận')";
                        SqlCommand cmd = new SqlCommand(queryDon, conn, tran);
                        cmd.Parameters.AddWithValue("@HoTen", txtHoTen.Text);
                        cmd.Parameters.AddWithValue("@DiaChi", txtDiaChi.Text);
                        cmd.Parameters.AddWithValue("@DienThoai", txtDienThoai.Text);
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text);
                        int maDon = (int)cmd.ExecuteScalar();

                        foreach (CartItem item in cart)
                        {
                            SqlCommand cmdCT = new SqlCommand(@"INSERT INTO ChiTietDonHang (MaDon, MaXe, SoLuong, DonGia)
                                                                VALUES (@MaDon, @MaXe, @SoLuong, @DonGia)", conn, tran);
                            cmdCT.Parameters.AddWithValue("@MaDon", maDon);
                            cmdCT.Parameters.AddWithValue("@MaXe", item.MaXe);
                            cmdCT.Parameters.AddWithValue("@SoLuong", item.SoLuong);
                            cmdCT.Parameters.AddWithValue("@DonGia", item.Gia);
                            cmdCT.ExecuteNonQuery();
                        }

                        tran.Commit();
                        Session["Cart"] = null;
                        Response.Redirect("xacnhan.aspx");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        lblThongBao.Text = "Lỗi đặt hàng: " + ex.Message;
                        lblThongBao.Visible = true;
                    }
                }
            }
            else
            {
                lblThongBao.Text = "Giỏ hàng trống!";
                lblThongBao.Visible = true;
            }
        }

        public class CartItem
        {
            public int MaXe { get; set; }
            public string TenXe { get; set; }
            public decimal Gia { get; set; }
            public int SoLuong { get; set; }
        }
    }
}