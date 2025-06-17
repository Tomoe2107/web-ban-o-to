using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace website_ban_o_to
{
    public partial class dangky1 : System.Web.UI.Page
    {
        // Sửa tên database và connection string
        private string connectionString = "Data Source=LAPTOP-VUB71TLF\\SQLEXPRESS;Initial Catalog=BanOto;Integrated Security=True;Connect Timeout=30;Encrypt=False";

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
                // Validate dữ liệu đầu vào
                if (!ValidateInput())
                    return;

                // Kiểm tra mật khẩu xác nhận
                if (txtMatKhau.Text.Trim() != txtXacNhanMatKhau.Text.Trim())
                {
                    ShowError("Mật khẩu xác nhận không khớp!");
                    return;
                }

                // Log để debug
                System.Diagnostics.Debug.WriteLine($"Attempting to register user: {txtTenDangNhap.Text.Trim()}");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    System.Diagnostics.Debug.WriteLine("Database connection opened successfully");

                    // Kiểm tra username đã tồn tại chưa
                    if (IsUsernameExists(conn, txtTenDangNhap.Text.Trim()))
                    {
                        ShowError("Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác!");
                        return;
                    }

                    // Kiểm tra email đã tồn tại chưa
                    if (IsEmailExists(conn, txtEmail.Text.Trim()))
                    {
                        ShowError("Email đã được sử dụng. Vui lòng sử dụng email khác!");
                        return;
                    }

                    // Insert user mới
                    string query = @"INSERT INTO Users (Username, Password, FullName, Email, Phone, Role, IsAdmin, IsActive, CreatedDate) 
                                   VALUES (@Username, @Password, @FullName, @Email, @Phone, @Role, @IsAdmin, @IsActive, @CreatedDate)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", txtTenDangNhap.Text.Trim());
                        cmd.Parameters.AddWithValue("@Password", txtMatKhau.Text.Trim()); // Trong thực tế nên hash
                        cmd.Parameters.AddWithValue("@FullName", txtHoTen.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
                        cmd.Parameters.AddWithValue("@Phone", string.IsNullOrEmpty(txtDienThoai.Text.Trim()) ? (object)DBNull.Value : txtDienThoai.Text.Trim());
                        cmd.Parameters.AddWithValue("@Role", "User"); // Mặc định là User
                        cmd.Parameters.AddWithValue("@IsAdmin", false); // Mặc định không phải admin
                        cmd.Parameters.AddWithValue("@IsActive", true); // Mặc định active
                        cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            System.Diagnostics.Debug.WriteLine($"User {txtTenDangNhap.Text.Trim()} registered successfully");
                            ShowSuccess("Đăng ký thành công! Bạn có thể đăng nhập ngay bây giờ.");
                            ClearForm();

                            // Tự động chuyển về trang đăng nhập sau 2 giây
                            Response.AddHeader("REFRESH", "2;URL=dangnhap1.aspx");
                        }
                        else
                        {
                            ShowError("Có lỗi xảy ra khi đăng ký. Vui lòng thử lại!");
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error: {sqlEx.Message}");

                // Xử lý các lỗi SQL cụ thể
                if (sqlEx.Number == 2627 || sqlEx.Number == 2601) // Duplicate key
                {
                    ShowError("Tên đăng nhập hoặc email đã tồn tại!");
                }
                else
                {
                    ShowError("Lỗi kết nối cơ sở dữ liệu. Vui lòng thử lại!");
                }

                System.Diagnostics.Debug.WriteLine($"SQL Error Details: {sqlEx.ToString()}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"General Error: {ex.Message}");
                ShowError("Có lỗi xảy ra. Vui lòng thử lại!");
                System.Diagnostics.Debug.WriteLine($"Error Details: {ex.ToString()}");
            }
        }

        private bool ValidateInput()
        {
            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                ShowError("Vui lòng nhập họ tên!");
                txtHoTen.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                ShowError("Vui lòng nhập email!");
                txtEmail.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtTenDangNhap.Text))
            {
                ShowError("Vui lòng nhập tên đăng nhập!");
                txtTenDangNhap.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtMatKhau.Text))
            {
                ShowError("Vui lòng nhập mật khẩu!");
                txtMatKhau.Focus();
                return false;
            }

            // Validate email format
            if (!IsValidEmail(txtEmail.Text.Trim()))
            {
                ShowError("Email không đúng định dạng!");
                txtEmail.Focus();
                return false;
            }

            // Validate username (chỉ cho phép chữ, số và gạch dưới, 3-20 ký tự)
            if (!IsValidUsername(txtTenDangNhap.Text.Trim()))
            {
                ShowError("Tên đăng nhập chỉ được chứa chữ cái, số và dấu gạch dưới, từ 3-20 ký tự!");
                txtTenDangNhap.Focus();
                return false;
            }

            // Validate password (ít nhất 6 ký tự)
            if (txtMatKhau.Text.Trim().Length < 6)
            {
                ShowError("Mật khẩu phải có ít nhất 6 ký tự!");
                txtMatKhau.Focus();
                return false;
            }

            // Validate phone number (nếu có nhập)
            if (!string.IsNullOrWhiteSpace(txtDienThoai.Text) && !IsValidPhoneNumber(txtDienThoai.Text.Trim()))
            {
                ShowError("Số điện thoại không đúng định dạng!");
                txtDienThoai.Focus();
                return false;
            }

            return true;
        }

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
    }
}