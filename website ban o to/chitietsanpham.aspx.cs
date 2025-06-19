using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using website_ban_o_to.Models;

namespace website_ban_o_to
{
    public partial class chitietsanpham : System.Web.UI.Page
    {
        // ✅ Kết nối cơ sở dữ liệu
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Request.QueryString["id"] != null)
            {
                int id;
                if (int.TryParse(Request.QueryString["id"], out id))
                {
                    LoadCarDetail(id);
                }
                else
                {
                    ShowMessage("ID sản phẩm không hợp lệ.", "error");
                    Response.Redirect("~/dangnhap1.aspx");
                }
            }
        }

        private void LoadCarDetail(int carId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    // Thêm ContactPhone vào query nếu có trong database
                    string query = @"
                SELECT 
                     c.CarName, c.Price, c.Description, c.CreatedDate,
                    
                    b.BrandName, l.LocationName,
                    ci.ImagePath
                FROM Cars c
                LEFT JOIN Brands b ON c.BrandID = b.BrandID
                LEFT JOIN Locations l ON c.LocationID = l.LocationID
                LEFT JOIN CarImages ci ON c.CarID = ci.CarID AND ci.IsMain = 1
                WHERE c.CarID = @CarID AND c.IsDisplay = 1 AND c.IsAvailable = 1";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@CarID", carId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            DisplayCarDetail(reader);
                        }
                        else
                        {
                            ShowMessage("Không tìm thấy sản phẩm hoặc sản phẩm không còn khả dụng.", "error");
                            Response.Redirect("~/dangnhap1.aspx");
                        }
                    }
                }
            }
            catch (SqlException sqlEx)
            {
                // Log lỗi SQL cụ thể
                LogError("LoadCarDetail - SQL Error", sqlEx);
                ShowMessage($"Lỗi truy vấn cơ sở dữ liệu: {sqlEx.Message}", "error");
            }
            catch (Exception ex)
            {
                LogError("LoadCarDetail", ex);
                ShowMessage("Có lỗi xảy ra khi tải thông tin sản phẩm.", "error");
            }
        }
        private void DisplayCarDetail(SqlDataReader reader)
        {
            try
            {
                // Hiển thị thông tin xe
                lblName.Text = GetSafeString(reader, "CarName");
                lblPrice.Text = GetSafeDecimal(reader, "Price").ToString("N0") + " VND";
                lblLocation.Text = GetSafeString(reader, "LocationName", "Chưa xác định");
                lblDescription.Text = GetSafeString(reader, "Description", "Chưa có mô tả");

                // Thông tin liên hệ - Sửa lỗi ở đây
                lblContact.Text = GetSafeString(reader, "ContactName", "Chưa cập nhật");
                // Bỏ dòng này vì không có ContactPhone trong query
                // lblPhone.Text = GetSafeString(reader, "ContactPhone", "Chưa cập nhật");
                lblPhone.Text = "Chưa cập nhật"; // Hoặc ẩn control này đi

                // Hiển thị hình ảnh
                string imagePath = GetSafeString(reader, "ImagePath");
                if (!string.IsNullOrEmpty(imagePath))
                {
                    // Kiểm tra imagePath có bắt đầu bằng ~/ không
                    if (imagePath.StartsWith("~/"))
                    {
                        imgCar.ImageUrl = imagePath;
                    }
                    else if (imagePath.StartsWith("/"))
                    {
                        imgCar.ImageUrl = "~" + imagePath;
                    }
                    else
                    {
                        imgCar.ImageUrl = "~/" + imagePath;
                    }
                    imgCar.AlternateText = GetSafeString(reader, "CarName");
                }
                else
                {
                    imgCar.ImageUrl = "~/images/no-image.jpg"; // Hình ảnh mặc định
                    imgCar.AlternateText = "Không có hình ảnh";
                }

                // Thêm năm sản xuất
                int year = GetSafeInt(reader, "Year");
                if (year > 0)
                {
                    lblName.Text += $" ({year})";
                }

                // Thêm tên thương hiệu
                string brandName = GetSafeString(reader, "BrandName");
                if (!string.IsNullOrEmpty(brandName))
                {
                    lblName.Text = $"{brandName} - {lblName.Text}";
                }

                // Bỏ dòng debug này khi production
                // ShowMessage("L:L", imagePath);
            }
            catch (Exception ex)
            {
                LogError("DisplayCarDetail", ex);
                ShowMessage("Có lỗi xảy ra khi hiển thị thông tin sản phẩm.", "error");
            }
        }


        #region Data Reading Helper Methods
        private string GetSafeString(SqlDataReader reader, string columnName, string defaultValue = "")
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? defaultValue : reader.GetString(ordinal);
            }
            catch
            {
                return defaultValue;
            }
        }

        private int GetSafeInt(SqlDataReader reader, string columnName, int defaultValue = 0)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? defaultValue : reader.GetInt32(ordinal);
            }
            catch
            {
                return defaultValue;
            }
        }

       

        private decimal GetSafeDecimal(SqlDataReader reader, string columnName, decimal defaultValue = 0)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? defaultValue : reader.GetDecimal(ordinal);
            }
            catch
            {
                return defaultValue;
            }
        }

       
        private void LogError(string methodName, Exception ex)
        {
            // Ghi log lỗi
            System.Diagnostics.Debug.WriteLine($"[ERROR] {DateTime.Now} in {methodName}: {ex.Message}");
            if (ex.InnerException != null)
            {
                System.Diagnostics.Debug.WriteLine($"[INNER ERROR]: {ex.InnerException.Message}");
            }
        }

        private void ShowMessage(string message, string type)
        {
            // Hiển thị thông báo cho người dùng
            // Có thể sử dụng JavaScript alert hoặc div thông báo
            string script = $"alert('{message.Replace("'", "\\'")}');";
            ClientScript.RegisterStartupScript(this.GetType(), "ShowMessage", script, true);
        }
        #endregion
    }
}