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
    public partial class chitiettintuc : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Request.QueryString["id"] != null)
            {
                int newsId;
                if (int.TryParse(Request.QueryString["id"], out newsId))
                {
                    LoadChiTietTinTuc(newsId);
                }
            }
        }

        private void LoadChiTietTinTuc(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT Title, Content, ImagePath, PublishedDate FROM News WHERE NewsID = @id AND IsPublished = 1";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", id);

                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    lblTieuDe.Text = reader["Title"].ToString();
                    lblNgayDang.Text = Convert.ToDateTime(reader["PublishedDate"]).ToString("dd/MM/yyyy");
                    imgTinTuc.ImageUrl = reader["ImagePath"].ToString();
                    lblNoiDung.Text = reader["Content"].ToString();
                }
                else
                {
                    lblTieuDe.Text = "Không tìm thấy tin tức.";
                    lblNoiDung.Text = "";
                }
            }
        }
    }
}