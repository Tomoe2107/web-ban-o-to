using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using website_ban_o_to.Models;

namespace website_ban_o_to
{
    public partial class banoto1 : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Kiểm tra xem người dùng đã đăng nhập chưa
                CheckUserLogin();
            }
        }

        private void CheckUserLogin()
        {
            if (Session["UserID"] == null)
            {
                return;
            }
        }

        protected void btnDangTin_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra đăng nhập trước khi xử lý
                if (Session["UserID"] == null)
                {
                    // Hiển thị alert và chuyển hướng đến trang đăng nhập
                    ShowAlertAndRedirect("Bạn cần đăng nhập để đăng tin bán xe!", "dangnhap1.aspx");
                    return;
                }

                // Kiểm tra validation
                if (!Page.IsValid)
                {
                    return;
                }

                // Lấy thông tin từ form
                string tenXe = txtTenXe.Text.Trim();
                decimal gia;
                if (!decimal.TryParse(txtGia.Text.Trim(), out gia))
                {
                    ShowAlert("Giá nhập vào không hợp lệ!");
                    return;
                }

                string moTa = txtMoTa.Text.Trim();
                string tenLienHe = txtTenLienHe.Text.Trim();
                string soDienThoai = txtSoDienThoai.Text.Trim();
                string email = txtEmail.Text.Trim();
                int userId = Convert.ToInt32(Session["UserID"]);

                // Xử lý upload hình ảnh
                string imagePath = null;
                if (fuHinhAnh.HasFile)
                {
                    imagePath = SaveUploadedImage();
                    if (imagePath == null)
                    {
                        ShowAlert("Có lỗi khi tải lên hình ảnh. Vui lòng thử lại!");
                        return;
                    }
                }

                // Tạo đối tượng UsedCarPost
                var carPost = new UsedCarPost
                {
                    UserID = userId,
                    CarName = tenXe,
                    ExpectedPrice = gia,
                    Description = moTa,
                    ContactName = tenLienHe,
                    ContactPhone = soDienThoai,
                    ContactEmail = email,
                    ImagePath = imagePath,
                    CreatedDate = DateTime.Now,
                    IsActive = true,
                    IsApproved = false // Cần admin duyệt
                };

                // Lưu vào database
                if (SaveCarPost(carPost))
                {
                    ShowSweetAlert("Thành công!", $"Đăng tin thành công! Tin của bạn đang chờ duyệt.\\nXe: {tenXe} - Giá: {gia:N0} triệu VNĐ", "success");
                    ClearForm();

                }
                else
                {
                    ShowAlert("Có lỗi xảy ra khi đăng tin. Vui lòng thử lại!");
                }
            }
            catch (Exception ex)
            {
                // Log lỗi
                System.Diagnostics.Debug.WriteLine($"Error in btnDangTin_Click: {ex.Message}");
                ShowAlert("Có lỗi xảy ra. Vui lòng thử lại sau!");
            }
        }

        // JAVASCRIPT ALERT METHODS
        private void ShowAlert(string message)
        {
            string script = $"alert('{message.Replace("'", "\\'")}');";
            ClientScript.RegisterStartupScript(this.GetType(), "alert", script, true);
        }

        private void ShowAlertAndRedirect(string message, string redirectUrl)
        {
            string script = $@"
                alert('{message.Replace("'", "\\'")}');
                window.location.href = '{redirectUrl}';";
            ClientScript.RegisterStartupScript(this.GetType(), "alertRedirect", script, true);
        }

        private void ShowSweetAlert(string title, string message, string type = "info")
        {
            string script = $@"
                Swal.fire({{
                    title: '{title}',
                    text: '{message}',
                    icon: '{type}',
                    confirmButtonText: 'OK'
                }});";
            ClientScript.RegisterStartupScript(this.GetType(), "sweetAlert", script, true);
        }

        private void ShowConfirm(string message, string postBackEventReference)
        {
            string script = $@"
                if(confirm('{message.Replace("'", "\\'")}')) {{
                    {postBackEventReference}
                }}";
            ClientScript.RegisterStartupScript(this.GetType(), "confirm", script, true);
        }

        private string SaveUploadedImage()
        {
            try
            {
                if (!fuHinhAnh.HasFile)
                    return null;

                // Kiểm tra loại file
                string fileExtension = Path.GetExtension(fuHinhAnh.FileName).ToLower();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

                if (!allowedExtensions.Contains(fileExtension))
                {
                    ShowAlert("Chỉ chấp nhận file hình ảnh (JPG, PNG, GIF)!");
                    return null;
                }

                // Kiểm tra kích thước file (5MB)
                if (fuHinhAnh.PostedFile.ContentLength > 5 * 1024 * 1024)
                {
                    ShowAlert("File hình ảnh không được vượt quá 5MB!");
                    return null;
                }

                // Tạo tên file unique
                string fileName = Guid.NewGuid().ToString() + fileExtension;
                string uploadPath = Server.MapPath("~/uploads/cars/");

                // Tạo thư mục nếu chưa có
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                string fullPath = Path.Combine(uploadPath, fileName);
                fuHinhAnh.SaveAs(fullPath);

                return "~/uploads/cars/" + fileName;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving image: {ex.Message}");
                return null;
            }
        }

        private bool SaveCarPost(UsedCarPost carPost)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        INSERT INTO UsedCarPosts 
                        (UserID, CarName, ExpectedPrice, Description, ContactName, ContactPhone, 
                         ContactEmail, ImagePath, CreatedDate, IsActive, IsApproved)
                        VALUES 
                        (@UserID, @CarName, @ExpectedPrice, @Description, @ContactName, @ContactPhone, 
                         @ContactEmail, @ImagePath, @CreatedDate, @IsActive, @IsApproved)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", carPost.UserID);
                        cmd.Parameters.AddWithValue("@CarName", carPost.CarName);
                        cmd.Parameters.AddWithValue("@ExpectedPrice", carPost.ExpectedPrice);
                        cmd.Parameters.AddWithValue("@Description", carPost.Description ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ContactName", carPost.ContactName);
                        cmd.Parameters.AddWithValue("@ContactPhone", carPost.ContactPhone);
                        cmd.Parameters.AddWithValue("@ContactEmail", carPost.ContactEmail ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ImagePath", carPost.ImagePath ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreatedDate", carPost.CreatedDate);
                        cmd.Parameters.AddWithValue("@IsActive", carPost.IsActive);
                        cmd.Parameters.AddWithValue("@IsApproved", carPost.IsApproved);

                        conn.Open();
                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving car post: {ex.Message}");
                return false;
            }
        }

        

        private void ClearForm()
        {
            txtTenXe.Text = "";
            txtGia.Text = "";
            txtMoTa.Text = "";
            txtTenLienHe.Text = "";
            txtSoDienThoai.Text = "";
            txtEmail.Text = "";
        }
    }
}