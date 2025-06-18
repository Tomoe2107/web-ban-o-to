using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.UI;
using website_ban_o_to.Models;

namespace website_ban_o_to
{
    public partial class lienhe1 : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                lblThongBao.Visible = false;
            }
        }

        protected void btnGuiLienHe_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate form data
                string validationError = ValidateForm();
                if (!string.IsNullOrEmpty(validationError))
                {
                    ShowMessage(validationError, "error");
                    return;
                }

                // Create contact object
                var contact = new Contact
                {
                    FullName = txtHoTen.Text.Trim(),
                    Email = txtEmail.Text.Trim().ToLower(),
                    Phone = string.IsNullOrWhiteSpace(txtDienThoai.Text) ? null : txtDienThoai.Text.Trim(),
                    Message = txtNoiDung.Text.Trim(),
                    IsRead = false,
                    CreatedDate = DateTime.Now
                };

                // Save to database
                if (SaveContact(contact))
                {
                    ShowMessage("Gửi liên hệ thành công!","success");
                    ClearForm();
                }
                else
                {
                    ShowMessage("Có lỗi xảy ra. Vui lòng thử lại.", "error");
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Có lỗi xảy ra. Vui lòng thử lại.", "error");
                // Log error if needed
                System.Diagnostics.Debug.WriteLine($"Contact form error: {ex.Message}");
            }
        }

        private string ValidateForm()
        {
            // Validate họ tên
            if (string.IsNullOrWhiteSpace(txtHoTen.Text))
                return "Vui lòng nhập họ tên!";

            if (txtHoTen.Text.Trim().Length < 2)
                return "Họ tên phải có ít nhất 2 ký tự!";

            if (txtHoTen.Text.Trim().Length > 100)
                return "Họ tên không được vượt quá 100 ký tự!";

            // Check họ tên chỉ chứa chữ cái và khoảng trắng
            if (!Regex.IsMatch(txtHoTen.Text.Trim(), @"^[a-zA-ZàáạảãâầấậẩẫăằắặẳẵèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữỳýỵỷỹĐđ\s]+$"))
                return "Họ tên chỉ được chứa chữ cái và khoảng trắng!";

            // Validate email
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
                return "Vui lòng nhập email!";

            if (!IsValidEmail(txtEmail.Text.Trim()))
                return "Email không đúng định dạng!";

            if (txtEmail.Text.Trim().Length > 100)
                return "Email không được vượt quá 100 ký tự!";

            // Validate số điện thoại (optional)
            if (!string.IsNullOrWhiteSpace(txtDienThoai.Text))
            {
                if (txtDienThoai.Text.Trim().Length > 20)
                    return "Số điện thoại không được vượt quá 20 ký tự!";

                if (!IsValidPhone(txtDienThoai.Text.Trim()))
                    return "Số điện thoại không đúng định dạng! (VD: 0987654321)";
            }

            // Validate nội dung
            if (string.IsNullOrWhiteSpace(txtNoiDung.Text))
                return "Vui lòng nhập nội dung liên hệ!";

            if (txtNoiDung.Text.Trim().Length < 10)
                return "Nội dung liên hệ phải có ít nhất 10 ký tự!";

            if (txtNoiDung.Text.Trim().Length > 2000)
                return "Nội dung liên hệ không được vượt quá 2000 ký tự!";

            return ""; // No validation errors
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

        private bool IsValidPhone(string phone)
        {
            // Chỉ cho phép số và một số ký tự đặc biệt
            string cleanPhone = Regex.Replace(phone, @"[\s\-\(\)]", "");

            // Kiểm tra định dạng số điện thoại Việt Nam
            return Regex.IsMatch(cleanPhone, @"^(0|\+84)[0-9]{9,10}$");
        }

        private bool SaveContact(Contact contact)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        INSERT INTO Contacts 
                        (FullName, Email, Phone, Message, IsRead, CreatedDate) 
                        VALUES 
                        (@FullName, @Email, @Phone, @Message, @IsRead, @CreatedDate)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FullName", contact.FullName);
                        cmd.Parameters.AddWithValue("@Email", contact.Email);
                        cmd.Parameters.AddWithValue("@Phone", (object)contact.Phone ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@Message", contact.Message);
                        cmd.Parameters.AddWithValue("@IsRead", contact.IsRead);
                        cmd.Parameters.AddWithValue("@CreatedDate", contact.CreatedDate);

                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error if needed
                System.Diagnostics.Debug.WriteLine($"Database error: {ex.Message}");
                return false;
            }
        }

        private void ShowMessage(string message, string type)
        {
            if (lblThongBao != null)
            {
                lblThongBao.Text = message;
                lblThongBao.Visible = true;

                // Set CSS class based on message type
                switch (type.ToLower())
                {
                    case "success":
                        lblThongBao.CssClass = "alert alert-success";
                        break;
                    case "error":
                        lblThongBao.CssClass = "alert alert-danger";
                        break;
                    case "warning":
                        lblThongBao.CssClass = "alert alert-warning";
                        break;
                    default:
                        lblThongBao.CssClass = "alert alert-info";
                        break;
                }
            }
        }

        private void ClearForm()
        {
            txtHoTen.Text = "";
            txtEmail.Text = "";
            txtDienThoai.Text = "";
            txtNoiDung.Text = "";
        }
    }
}