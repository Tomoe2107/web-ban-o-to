using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text.RegularExpressions;

namespace website_ban_o_to
{
    public partial class lienhe1 : System.Web.UI.Page
    {
        // Connection string
        private string connectionString = "Data Source=DESKTOP-NMTRDC3\\MSSQLSERVER01;Initial Catalog=BanOto;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (lblThongBao != null)
                    lblThongBao.Visible = false;

                // Test connection và table structure
                TestContactTable();
            }
        }

        protected void btnGuiLienHe_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate input đầy đủ
                if (!ValidateInput())
                    return;

                System.Diagnostics.Debug.WriteLine("Attempting to save contact form");

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    System.Diagnostics.Debug.WriteLine("Database connection opened successfully");

                    // Query đơn giản - chỉ các field cần thiết
                    string query = @"INSERT INTO Contacts 
                                    (FullName, Email, Phone, Message, IsRead, CreatedDate) 
                                    VALUES 
                                    (@FullName, @Email, @Phone, @Message, @IsRead, @CreatedDate)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        // Chỉ map các field có trong form
                        cmd.Parameters.AddWithValue("@FullName", txtHoTen.Text.Trim());
                        cmd.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());

                        // Phone - có thể null
                        if (string.IsNullOrWhiteSpace(txtDienThoai.Text))
                        {
                            cmd.Parameters.AddWithValue("@Phone", DBNull.Value);
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue("@Phone", txtDienThoai.Text.Trim());
                        }

                        cmd.Parameters.AddWithValue("@Message", txtNoiDung.Text.Trim());
                        cmd.Parameters.AddWithValue("@IsRead", false); // Chưa đọc
                        cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                        System.Diagnostics.Debug.WriteLine($"Executing query: {query}");
                        System.Diagnostics.Debug.WriteLine($"Parameters: FullName={txtHoTen.Text}, Email={txtEmail.Text}");

                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            System.Diagnostics.Debug.WriteLine("Contact saved successfully");
                            ShowMessage("Gửi liên hệ thành công! Chúng tôi sẽ phản hồi sớm nhất.", true);
                            ClearForm();

                            // Thông báo cho admin
                            System.Diagnostics.Debug.WriteLine($"New contact from: {txtEmail.Text.Trim()}");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("No rows affected");
                            ShowMessage("Không thể gửi liên hệ. Vui lòng thử lại.");
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                System.Diagnostics.Debug.WriteLine($"SQL Error: {sqlEx.Number} - {sqlEx.Message}");

                switch (sqlEx.Number)
                {
                    case 2: // Cannot open database
                        ShowMessage("Không thể kết nối cơ sở dữ liệu!");
                        System.Diagnostics.Debug.WriteLine("Database CarDealershipDB không tồn tại");
                        break;
                    case 208: // Invalid object name
                        ShowMessage("Bảng Contacts chưa được tạo!");
                        System.Diagnostics.Debug.WriteLine("Cần chạy script tạo bảng Contacts");
                        break;
                    case 207: // Invalid column name
                        ShowMessage("Cấu trúc bảng không đúng!");
                        System.Diagnostics.Debug.WriteLine($"Column error: {sqlEx.Message}");
                        break;
                    case 515: // Cannot insert NULL
                        ShowMessage("Thiếu dữ liệu bắt buộc!");
                        break;
                    default:
                        ShowMessage($"Lỗi SQL #{sqlEx.Number}: {sqlEx.Message}");
                        break;
                }

                System.Diagnostics.Debug.WriteLine($"Full SQL Error: {sqlEx.ToString()}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"General Error: {ex.Message}");
                ShowMessage($"Có lỗi xảy ra: {ex.Message}");
            }
        }

        private bool ValidateInput()
        {
            // Kiểm tra họ tên
            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
            {
                ShowMessage("Vui lòng nhập họ tên!");
                txtHoTen.Focus();
                return false;
            }

            if (txtHoTen.Text.Trim().Length < 2)
            {
                ShowMessage("Họ tên phải có ít nhất 2 ký tự!");
                txtHoTen.Focus();
                return false;
            }

            // Kiểm tra email
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                ShowMessage("Vui lòng nhập email!");
                txtEmail.Focus();
                return false;
            }

            if (!IsValidEmail(txtEmail.Text.Trim()))
            {
                ShowMessage("Email không đúng định dạng!");
                txtEmail.Focus();
                return false;
            }

            // Kiểm tra nội dung
            if (string.IsNullOrWhiteSpace(txtNoiDung.Text))
            {
                ShowMessage("Vui lòng nhập nội dung liên hệ!");
                txtNoiDung.Focus();
                return false;
            }

            if (txtNoiDung.Text.Trim().Length < 10)
            {
                ShowMessage("Nội dung liên hệ phải có ít nhất 10 ký tự!");
                txtNoiDung.Focus();
                return false;
            }

            if (txtNoiDung.Text.Trim().Length > 2000)
            {
                ShowMessage("Nội dung liên hệ không được vượt quá 2000 ký tự!");
                txtNoiDung.Focus();
                return false;
            }

            // Kiểm tra số điện thoại (nếu có nhập)
            if (!string.IsNullOrWhiteSpace(txtDienThoai.Text))
            {
                if (!IsValidPhoneNumber(txtDienThoai.Text.Trim()))
                {
                    ShowMessage("Số điện thoại không đúng định dạng! (VD: 0987654321)");
                    txtDienThoai.Focus();
                    return false;
                }
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

        private bool IsValidPhoneNumber(string phone)
        {
            // Phone number Vietnam: bắt đầu bằng 0, có 10-11 số
            return Regex.IsMatch(phone, @"^0[0-9]{9,10}$");
        }

        private void TestContactTable()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    System.Diagnostics.Debug.WriteLine("✅ Database connection successful");

                    // Kiểm tra bảng Contacts
                    string checkTableQuery = "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'Contacts'";
                    using (SqlCommand cmd = new SqlCommand(checkTableQuery, conn))
                    {
                        int tableExists = (int)cmd.ExecuteScalar();

                        if (tableExists == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("❌ Table 'Contacts' DOES NOT EXIST");
                            System.Diagnostics.Debug.WriteLine("Execute this SQL to create the table:");
                            System.Diagnostics.Debug.WriteLine(@"
CREATE TABLE Contacts (
    ContactID INT IDENTITY(1,1) PRIMARY KEY,
    FullName NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Phone NVARCHAR(20) NULL,
    Subject NVARCHAR(200) NULL,
    Message NTEXT NOT NULL,
    IsRead BIT NOT NULL DEFAULT 0,
    CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
    ReplyDate DATETIME NULL,
    ReplyMessage NTEXT NULL
);");
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("✅ Table 'Contacts' exists");

                            // Đếm số record hiện tại
                            string countQuery = "SELECT COUNT(*) FROM Contacts";
                            using (SqlCommand countCmd = new SqlCommand(countQuery, conn))
                            {
                                int recordCount = (int)countCmd.ExecuteScalar();
                                System.Diagnostics.Debug.WriteLine($"Current contacts count: {recordCount}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"❌ Database test failed: {ex.Message}");
            }
        }

        private void ShowMessage(string message, bool isSuccess = false)
        {
            if (lblThongBao != null)
            {
                lblThongBao.Text = message;
                lblThongBao.ForeColor = isSuccess ? System.Drawing.Color.Green : System.Drawing.Color.Red;
                lblThongBao.Visible = true;

                System.Diagnostics.Debug.WriteLine($"Message shown: {message} (Success: {isSuccess})");
            }
        }

        private void ClearForm()
        {
            txtHoTen.Text = "";
            txtEmail.Text = "";
            txtDienThoai.Text = "";
            txtNoiDung.Text = "";

            System.Diagnostics.Debug.WriteLine("Form cleared");
        }
    }
}