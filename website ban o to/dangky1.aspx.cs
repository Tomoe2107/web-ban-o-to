using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;
using website_ban_o_to.Models;

namespace website_ban_o_to
{
    public partial class dangky1 : System.Web.UI.Page
    {
        // Connection string - nên đưa vào web.config
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Ẩn thông báo khi trang load lần đầu
                if (lblThongBao != null)
                    lblThongBao.Visible = false;
            }
        }

        protected void btnDangKy_Click(object sender, EventArgs e)
        {
            try
            {
                // Tạo model User từ form data
                User newUser = CreateUserFromForm();

                // Validate model
                var validationResults = ValidateUser(newUser);
                if (validationResults.Any())
                {
                    ShowError(string.Join("<br/>", validationResults));
                    return;
                }

                // Kiểm tra mật khẩu xác nhận
                if (txtMatKhau.Text.Trim() != txtXacNhanMatKhau.Text.Trim())
                {
                    ShowError("Mật khẩu xác nhận không khớp!");
                    return;
                }

                // Log để debug
                System.Diagnostics.Debug.WriteLine($"Attempting to register user: {newUser.Username}");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    System.Diagnostics.Debug.WriteLine("Database connection opened successfully");

                    // Kiểm tra username đã tồn tại chưa
                    if (IsUsernameExists(conn, newUser.Username))
                    {
                        ShowError("Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác!");
                        return;
                    }

                    // Kiểm tra email đã tồn tại chưa
                    if (IsEmailExists(conn, newUser.Email))
                    {
                        ShowError("Email đã được sử dụng. Vui lòng sử dụng email khác!");
                        return;
                    }


                    // Insert user mới
                    if (InsertUser(conn, newUser))
                    {
                        System.Diagnostics.Debug.WriteLine($"User {newUser.Username} registered successfully");
                        ShowSuccess("Đăng ký thành công! Bạn có thể đăng nhập ngay bây giờ.");
                        ClearForm();

                        // Tự động chuyển về trang đăng nhập sau 3 giây
                        Response.AddHeader("REFRESH", "3;URL=dangnhap1.aspx");
                    }
                    else
                    {
                        ShowError("Có lỗi xảy ra khi đăng ký. Vui lòng thử lại!");
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error: {sqlEx.Message}");
                HandleSqlException(sqlEx);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"General Error: {ex.Message}");
                ShowError("Có lỗi xảy ra. Vui lòng thử lại!");
                System.Diagnostics.Debug.WriteLine($"Error Details: {ex.ToString()}");
            }
        }

        private User CreateUserFromForm()
        {
            return new User
            {
                Username = txtTenDangNhap.Text.Trim(),
                Password = txtMatKhau.Text.Trim(), // Sẽ được hash sau
                FullName = txtHoTen.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                Phone = string.IsNullOrWhiteSpace(txtDienThoai.Text.Trim()) ? null : txtDienThoai.Text.Trim(),
                Role = "User", // Mặc định là User
                IsAdmin = false, // Mặc định không phải admin
                IsActive = true, // Mặc định active
                CreatedDate = DateTime.Now
            };
        }

        private List<string> ValidateUser(User user)
        {
            var errors = new List<string>();

            // Validate các trường bắt buộc
            if (string.IsNullOrWhiteSpace(user.FullName))
            {
                errors.Add("Vui lòng nhập họ tên!");
                txtHoTen.Focus();
            }

            if (string.IsNullOrWhiteSpace(user.Email))
            {
                errors.Add("Vui lòng nhập email!");
                if (errors.Count == 1) txtEmail.Focus();
            }

            if (string.IsNullOrWhiteSpace(user.Username))
            {
                errors.Add("Vui lòng nhập tên đăng nhập!");
                if (errors.Count == 1) txtTenDangNhap.Focus();
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                errors.Add("Vui lòng nhập mật khẩu!");
                if (errors.Count == 1) txtMatKhau.Focus();
            }

            // Validate format nếu đã có dữ liệu
            if (!string.IsNullOrWhiteSpace(user.Email) && !IsValidEmail(user.Email))
            {
                errors.Add("Email không đúng định dạng!");
                if (errors.Count == 1) txtEmail.Focus();
            }

            if (!string.IsNullOrWhiteSpace(user.Username) && !IsValidUsername(user.Username))
            {
                errors.Add("Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới, từ 3-20 ký tự!");
                if (errors.Count == 1) txtTenDangNhap.Focus();
            }

            if (!string.IsNullOrWhiteSpace(user.Password) && user.Password.Length < 6)
            {
                errors.Add("Mật khẩu phải có ít nhất 6 ký tự!");
                if (errors.Count == 1) txtMatKhau.Focus();
            }

            // Validate phone number (nếu có nhập)
            if (!string.IsNullOrWhiteSpace(user.Phone) && !IsValidPhoneNumber(user.Phone))
            {
                errors.Add("Số điện thoại không đúng định dạng!");
                if (errors.Count == 1) txtDienThoai.Focus();
            }

            // Validate length constraints theo model
            if (!string.IsNullOrWhiteSpace(user.Username) && user.Username.Length > 50)
            {
                errors.Add("Tên đăng nhập không được vượt quá 50 ký tự!");
            }

            if (!string.IsNullOrWhiteSpace(user.FullName) && user.FullName.Length > 100)
            {
                errors.Add("Họ tên không được vượt quá 100 ký tự!");
            }

            if (!string.IsNullOrWhiteSpace(user.Email) && user.Email.Length > 100)
            {
                errors.Add("Email không được vượt quá 100 ký tự!");
            }

            if (!string.IsNullOrWhiteSpace(user.Phone) && user.Phone.Length > 20)
            {
                errors.Add("Số điện thoại không được vượt quá 20 ký tự!");
            }

            return errors;
        }

        private bool InsertUser(SqlConnection conn, User user)
        {
            string query = @"INSERT INTO Users (Username, Password, FullName, Email, Phone, Role, IsAdmin, IsActive, CreatedDate) 
                           VALUES (@Username, @Password, @FullName, @Email, @Phone, @Role, @IsAdmin, @IsActive, @CreatedDate)";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@Password", user.Password);
                cmd.Parameters.AddWithValue("@FullName", user.FullName);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Phone", (object)user.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Role", user.Role);
                cmd.Parameters.AddWithValue("@IsAdmin", user.IsAdmin);
                cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
                cmd.Parameters.AddWithValue("@CreatedDate", user.CreatedDate);

                int result = cmd.ExecuteNonQuery();
                return result > 0;
            }
        }

        private string HashPassword(string password)
        {
            // Sử dụng SHA256 để hash password (trong thực tế nên dùng bcrypt hoặc Argon2)
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void HandleSqlException(SqlException sqlEx)
        {
            // Xử lý các lỗi SQL cụ thể
            switch (sqlEx.Number)
            {
                case 2627: // Duplicate key
                case 2601: // Duplicate key
                    ShowError("Tên đăng nhập hoặc email đã tồn tại!");
                    break;
                case 2: // Timeout
                    ShowError("Kết nối cơ sở dữ liệu bị timeout. Vui lòng thử lại!");
                    break;
                case 18456: // Login failed
                    ShowError("Lỗi xác thực cơ sở dữ liệu!");
                    break;
                default:
                    ShowError("Lỗi cơ sở dữ liệu. Vui lòng thử lại!");
                    break;
            }

            System.Diagnostics.Debug.WriteLine($"SQL Error Details: {sqlEx.ToString()}");
        }

        #region Validation Methods

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidUsername(string username)
        {
            // Username: 3-20 ký tự, chỉ chứa chữ, số và gạch dưới
            return Regex.IsMatch(username, @"^[a-zA-Z0-9_]{3,20}$");
        }

        private bool IsValidPhoneNumber(string phone)
        {
            // Phone number Vietnam: bắt đầu bằng 0, có 10-11 số
            return Regex.IsMatch(phone, @"^0[0-9]{9,10}$");
        }

        #endregion

        #region Database Check Methods

        private bool IsUsernameExists(SqlConnection conn, string username)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @Username";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Username", username);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        private bool IsEmailExists(SqlConnection conn, string email)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@Email", email);
                int count = (int)cmd.ExecuteScalar();
                return count > 0;
            }
        }

        #endregion

        #region UI Helper Methods

        private void ShowError(string message)
        {
            if (lblThongBao != null)
            {
                lblThongBao.Text = message;
                lblThongBao.CssClass = "error-message";
                lblThongBao.Visible = true;
            }
        }

        private void ShowSuccess(string message)
        {
            if (lblThongBao != null)
            {
                lblThongBao.Text = message;
                lblThongBao.CssClass = "success-message";
                lblThongBao.Visible = true;
            }
        }

        private void ClearForm()
        {
            txtHoTen.Text = "";
            txtEmail.Text = "";
            txtDienThoai.Text = "";
            txtTenDangNhap.Text = "";
            txtMatKhau.Text = "";
            txtXacNhanMatKhau.Text = "";
        }

        #endregion
    }
}