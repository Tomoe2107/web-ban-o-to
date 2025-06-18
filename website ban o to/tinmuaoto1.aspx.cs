using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using website_ban_o_to.Models;

namespace website_ban_o_to
{
    public partial class tinmuaoto1 : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadNews();
            }
        }

        private void LoadNews()
        {
            try
            {
                List<News> newsList = GetNewsFromDatabase();
                rptNews.DataSource = newsList;
                rptNews.DataBind();
            }
            catch (Exception ex)
            {
                // Log error và hiển thị thông báo lỗi
                Response.Write("<script>alert('Có lỗi xảy ra khi tải tin tức: " + ex.Message + "');</script>");
            }
        }

        private List<News> GetNewsFromDatabase()
        {
            List<News> newsList = new List<News>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = @"
                    SELECT n.NewsID, n.Title, n.Slug, n.Summary, n.Content, 
                           n.ImagePath, n.IsPublished, n.PublishedDate, 
                           n.CreatedBy, n.CreatedDate, n.UpdatedDate,
                           u.Username as CreatorName
                    FROM News n
                    LEFT JOIN Users u ON n.CreatedBy = u.UserID
                    WHERE n.IsPublished = 1
                    ORDER BY n.PublishedDate DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            News news = new News
                            {
                                NewsID = Convert.ToInt32(reader["NewsID"]),
                                Title = reader["Title"].ToString(),
                                Slug = reader["Slug"].ToString(),
                                Summary = reader["Summary"]?.ToString(),
                                Content = reader["Content"]?.ToString(),
                                ImagePath = reader["ImagePath"]?.ToString(),
                                IsPublished = Convert.ToBoolean(reader["IsPublished"]),
                                PublishedDate = Convert.ToDateTime(reader["PublishedDate"]),
                                CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                UpdatedDate = reader["UpdatedDate"] as DateTime?
                            };


                            newsList.Add(news);
                        }
                    }
                }
            }

            return newsList;
        }

        // Method để thêm tin tức mới (với người tạo mặc định là admin id = 1)
        public bool AddNews(string title, string slug, string summary, string content, string imagePath)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        INSERT INTO News (Title, Slug, Summary, Content, ImagePath, 
                                        IsPublished, PublishedDate, CreatedBy, CreatedDate)
                        VALUES (@Title, @Slug, @Summary, @Content, @ImagePath, 
                               @IsPublished, @PublishedDate, @CreatedBy, @CreatedDate)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Title", title);
                        cmd.Parameters.AddWithValue("@Slug", slug);
                        cmd.Parameters.AddWithValue("@Summary", summary ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Content", content ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ImagePath", imagePath ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsPublished", true);
                        cmd.Parameters.AddWithValue("@PublishedDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@CreatedBy", 1); // Admin ID mặc định
                        cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                        conn.Open();
                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error
                System.Diagnostics.Debug.WriteLine("Error adding news: " + ex.Message);
                return false;
            }
        }

        // Method để cập nhật tin tức
        public bool UpdateNews(int newsId, string title, string slug, string summary, string content, string imagePath)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        UPDATE News 
                        SET Title = @Title, Slug = @Slug, Summary = @Summary, 
                            Content = @Content, ImagePath = @ImagePath, UpdatedDate = @UpdatedDate
                        WHERE NewsID = @NewsID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NewsID", newsId);
                        cmd.Parameters.AddWithValue("@Title", title);
                        cmd.Parameters.AddWithValue("@Slug", slug);
                        cmd.Parameters.AddWithValue("@Summary", summary ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Content", content ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ImagePath", imagePath ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);

                        conn.Open();
                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error updating news: " + ex.Message);
                return false;
            }
        }

        // Method để xóa tin tức (soft delete - chỉ ẩn đi)
        public bool DeleteNews(int newsId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "UPDATE News SET IsPublished = 0, UpdatedDate = @UpdatedDate WHERE NewsID = @NewsID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NewsID", newsId);
                        cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);

                        conn.Open();
                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error deleting news: " + ex.Message);
                return false;
            }
        }

        // Method để lấy tin tức theo ID
        public News GetNewsById(int newsId)
        {
            News news = null;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT n.NewsID, n.Title, n.Slug, n.Summary, n.Content, 
                               n.ImagePath, n.IsPublished, n.PublishedDate, 
                               n.CreatedBy, n.CreatedDate, n.UpdatedDate,
                               u.Username as CreatorName
                        FROM News n
                        LEFT JOIN Users u ON n.CreatedBy = u.UserID
                        WHERE n.NewsID = @NewsID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NewsID", newsId);

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                news = new News
                                {
                                    NewsID = Convert.ToInt32(reader["NewsID"]),
                                    Title = reader["Title"].ToString(),
                                    Slug = reader["Slug"].ToString(),
                                    Summary = reader["Summary"]?.ToString(),
                                    Content = reader["Content"]?.ToString(),
                                    ImagePath = reader["ImagePath"]?.ToString(),
                                    IsPublished = Convert.ToBoolean(reader["IsPublished"]),
                                    PublishedDate = Convert.ToDateTime(reader["PublishedDate"]),
                                    CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    UpdatedDate = reader["UpdatedDate"] as DateTime?
                                };

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error getting news by ID: " + ex.Message);
            }

            return news;
        }

        // Method để tìm kiếm tin tức
        public List<News> SearchNews(string keyword)
        {
            List<News> newsList = new List<News>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT n.NewsID, n.Title, n.Slug, n.Summary, n.Content, 
                               n.ImagePath, n.IsPublished, n.PublishedDate, 
                               n.CreatedBy, n.CreatedDate, n.UpdatedDate,
                               u.Username as CreatorName
                        FROM News n
                        LEFT JOIN Users u ON n.CreatedBy = u.UserID
                        WHERE n.IsPublished = 1 
                        AND (n.Title LIKE @Keyword OR n.Summary LIKE @Keyword OR n.Content LIKE @Keyword)
                        ORDER BY n.PublishedDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");

                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                News news = new News
                                {
                                    NewsID = Convert.ToInt32(reader["NewsID"]),
                                    Title = reader["Title"].ToString(),
                                    Slug = reader["Slug"].ToString(),
                                    Summary = reader["Summary"]?.ToString(),
                                    Content = reader["Content"]?.ToString(),
                                    ImagePath = reader["ImagePath"]?.ToString(),
                                    IsPublished = Convert.ToBoolean(reader["IsPublished"]),
                                    PublishedDate = Convert.ToDateTime(reader["PublishedDate"]),
                                    CreatedBy = Convert.ToInt32(reader["CreatedBy"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    UpdatedDate = reader["UpdatedDate"] as DateTime?
                                };

                               

                                newsList.Add(news);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error searching news: " + ex.Message);
            }

            return newsList;
        }
    }
}