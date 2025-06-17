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
    public partial class dangnhap1 : System.Web.UI.Page
    {
        private string connectionString = "Data Source=DESKTOP-NMTRDC3\\MSSQLSERVER01;Initial Catalog=BanOto;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Ẩn thông báo lỗi khi trang load lần đầu
                if (lblError != null)
                    lblError.Visible = false;
            }
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text.Trim();

                // Validate input
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    ShowError("Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!");
                    return;
                }

                // Log để debug
                System.Diagnostics.Debug.WriteLine($"Attempting login for user: {username}");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Query để lấy thêm thông tin user
                    string query = @"SELECT UserID, Username, FullName, Email, Role, IsAdmin, IsActive 
                                   FROM Users 
                                   WHERE Username = @Username AND Password = @Password AND IsActive = 1";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", username);
                    cmd.Parameters.AddWithValue("@Password", password); // Trong thực tế nên hash password

                    conn.Open();
                    System.Diagnostics.Debug.WriteLine("Database connection opened successfully");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            // Lưu thông tin user vào Session
                            Session["UserID"] = reader["UserID"].ToString();
                            Session["Username"] = reader["Username"].ToString();
                            Session["FullName"] = reader["FullName"].ToString();
                            Session["Email"] = reader["Email"].ToString();
                            Session["Role"] = reader["Role"].ToString();
                            Session["IsAdmin"] = Convert.ToBoolean(reader["IsAdmin"]);

                            System.Diagnostics.Debug.WriteLine($"Login successful for user: {username}");

                            // Chuyển hướng dựa trên role
                            string role = reader["Role"].ToString();
                            bool isAdmin = Convert.ToBoolean(reader["IsAdmin"]);

                            if (isAdmin || role == "Admin")
                            {
                                Response.Redirect("~/admin/quanlyuser1.aspx", false);
                            }
                            else
                            {
                                Response.Redirect("~/trangchu1.aspx", false);
                            }
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Login failed for user: {username}");
                            ShowError("Tên đăng nhập hoặc mật khẩu không đúng!");
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error: {sqlEx.Message}");
                ShowError("Lỗi kết nối cơ sở dữ liệu. Vui lòng thử lại!");

                // Log chi tiết lỗi
                System.Diagnostics.Debug.WriteLine($"SQL Error Details: {sqlEx.ToString()}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"General Error: {ex.Message}");
                ShowError("Có lỗi xảy ra. Vui lòng thử lại!");

                // Log chi tiết lỗi
                System.Diagnostics.Debug.WriteLine($"Error Details: {ex.ToString()}");
            }
        }

        private void ShowError(string message)
        {
            if (lblError != null)
            {
                lblError.Text = message;
                lblError.Visible = true;
            }
        }

        // Method để kiểm tra user đã đăng nhập chưa
        protected bool IsUserLoggedIn()
        {
            return Session["UserID"] != null;
        }

        // Method để logout (có thể dùng ở nơi khác)
        protected void Logout()
        {
            Session.Clear();
            Session.Abandon();
            Response.Redirect("~/dangnhap.aspx");
        }
    }
}