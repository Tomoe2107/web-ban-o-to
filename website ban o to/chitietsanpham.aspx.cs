using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace website_ban_o_to
{
    public partial class chitietsanpham : System.Web.UI.Page
    {
        // ✅ Kết nối cơ sở dữ liệu
        private string connectionString = "Data Source=DESKTOP-NMTRDC3\\MSSQLSERVER01;Initial Catalog=BanOto;Integrated Security=True;Connect Timeout=30;Encrypt=False";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Request.QueryString["id"] != null)
            {
                int id;
                if (int.TryParse(Request.QueryString["id"], out id))
                {
                    LoadChiTiet(id);
                }
            }
        }

        private void LoadChiTiet(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string sql = @"SELECT c.CarName, c.Price, c.LocationID, c.Description, c.Year, ci.ImagePath
               FROM Cars c
               LEFT JOIN CarImages ci ON c.CarID = ci.CarID AND ci.IsMain = 1
               WHERE c.CarID = @Id";
                SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@Id", id);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                     //imgCar.ImageUrl = reader["ImageUrl"].ToString();
                    lblName.Text = reader["CarName"].ToString();         // sửa lại đúng tên cột
                    lblPrice.Text = Convert.ToDecimal(reader["Price"]).ToString("N0") + " triệu";
                    lblLocation.Text = reader["LocationID"].ToString();   // hoặc JOIN bảng Location để lấy tên
                    lblDescription.Text = reader["Description"].ToString();
                    lblContact.Text = reader["Year"].ToString();          // nếu có trường khác bạn dùng thay thế
                    lblPhone.Text = ""; // Nếu không có Phone, có thể bỏ dòng này
                }

                reader.Close();
            }
        }
    }
}