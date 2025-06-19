using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using website_ban_o_to.Models;

namespace website_ban_o_to.admin
{
    public partial class quanly1 : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGridView();
                AddImageSizeCSS();
            }
        }

        private void AddImageSizeCSS()
        {
            string css = @"<style type='text/css'>.admin-grid img { width: 100px !important; height: 60px !important; object-fit: cover !important; }</style>";
            Page.Header.Controls.Add(new Literal { Text = css });
        }

        private void BindGridView()
        {
            var cars = GetDanhSachCars();
            gvXe.DataSource = cars;
            gvXe.DataBind();
        }

        private List<object> GetDanhSachCars()
        {
            List<object> cars = new List<object>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"
                SELECT 
                    c.CarID, 
                    c.CarName, 
                    c.Price, 
                    c.IsDisplay, 
                    c.CreatedDate,
                    COALESCE(ci.ImagePath, '~/images/no-image.png') as ImagePath
                FROM Cars c 
                LEFT JOIN CarImages ci ON c.CarID = ci.CarID AND ci.IsMain = 1
                WHERE c.IsDisplay = 1 
                ORDER BY c.CreatedDate DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cars.Add(new
                            {
                                CarID = Convert.ToInt32(reader["CarID"]),
                                CarName = reader["CarName"].ToString(),
                                Price = Convert.ToDecimal(reader["Price"]),
                                IsDisplay = Convert.ToBoolean(reader["IsDisplay"]),
                                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                ImagePath = reader["ImagePath"].ToString()
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Lỗi khi lấy danh sách xe: " + ex.Message);
            }
            return cars;
        }

        private Car GetCarById(int carId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT * FROM Cars WHERE CarID = @CarID";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CarID", carId);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Car
                                {
                                    CarID = Convert.ToInt32(reader["CarID"]),
                                    BrandID = Convert.ToInt32(reader["BrandID"]),
                                    LocationID = reader["LocationID"] != DBNull.Value ? (int?)Convert.ToInt32(reader["LocationID"]) : null,
                                    CarName = reader["CarName"].ToString(),
                                    Price = Convert.ToDecimal(reader["Price"]),
                                    Description = reader["Description"]?.ToString(),
                                    Year = reader["Year"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Year"]) : null,
                                    IsAvailable = reader["IsAvailable"] != DBNull.Value ? Convert.ToBoolean(reader["IsAvailable"]) : true,
                                    IsDisplay = Convert.ToBoolean(reader["IsDisplay"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    UpdatedDate = reader["UpdatedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["UpdatedDate"]) : null
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Lỗi khi lấy thông tin xe: " + ex.Message);
            }
            return null;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            HideAllPanels();
            string searchKeyword = txtSearch.Text.Trim();
            var filteredCars = SearchCars(searchKeyword);
            gvXe.DataSource = filteredCars;
            gvXe.DataBind();
        }

        private List<object> SearchCars(string keyword)
        {
            List<object> cars = new List<object>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"
                SELECT 
                    c.CarID, 
                    c.CarName, 
                    c.Price, 
                    c.IsDisplay, 
                    c.CreatedDate,
                    COALESCE(ci.ImagePath, '~/images/no-image.png') as ImagePath
                FROM Cars c 
                LEFT JOIN CarImages ci ON c.CarID = ci.CarID AND ci.IsMain = 1
                WHERE c.IsDisplay = 1 AND (c.CarName LIKE @Keyword OR c.Description LIKE @Keyword)
                ORDER BY c.CreatedDate DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cars.Add(new
                                {
                                    CarID = Convert.ToInt32(reader["CarID"]),
                                    CarName = reader["CarName"].ToString(),
                                    Price = Convert.ToDecimal(reader["Price"]),
                                    IsDisplay = Convert.ToBoolean(reader["IsDisplay"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    ImagePath = reader["ImagePath"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Lỗi khi tìm kiếm: " + ex.Message);
            }
            return cars;
        }

        protected void btnThemXe_Click(object sender, EventArgs e)
        {
            HideAllPanels();
            pnlChiTietXe.Visible = true;
            lblTitle.Text = "Thêm xe mới";
            ClearCarForm();
        }

        private void ClearCarForm()
        {
            txtTenXe.Text = "";
            txtGia.Text = "";
            txtMoTa.Text = "";
            chkTrangThai.Checked = true;
        }

        protected void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                Car car = new Car
                {
                    BrandID = 1, // Default brand - có thể thay đổi thành dropdown
                    LocationID = null, // Có thể thêm dropdown chọn location
                    CarName = txtTenXe.Text.Trim(),
                    Price = Convert.ToDecimal(txtGia.Text.Trim()),
                    Description = txtMoTa.Text.Trim(),
                    Year = null, // Có thể thêm field năm sản xuất
                    IsAvailable = true,
                    IsDisplay = chkTrangThai.Checked,
                    CreatedDate = DateTime.Now
                };

                int newCarId = InsertCar(car);

                // Xử lý upload ảnh sau khi thêm xe thành công
                if (fuHinhAnh.HasFile && newCarId > 0)
                {
                    string imagePath = SaveCarImage(fuHinhAnh);
                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        InsertCarImage(newCarId, imagePath, car.CarName, true);
                    }
                }

                HideAllPanels();
                BindGridView();
                ShowMessage("Thêm xe mới thành công!");
            }
            catch (Exception ex)
            {
                ShowMessage("Có lỗi xảy ra: " + ex.Message);
            }
        }

        private int InsertCar(Car car)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO Cars (BrandID, LocationID, CarName, Price, Description, Year, IsAvailable, IsDisplay, CreatedDate)
                              VALUES (@BrandID, @LocationID, @CarName, @Price, @Description, @Year, @IsAvailable, @IsDisplay, @CreatedDate);
                              SELECT SCOPE_IDENTITY();";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@BrandID", car.BrandID);
                    cmd.Parameters.AddWithValue("@LocationID", (object)car.LocationID ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@CarName", car.CarName);
                    cmd.Parameters.AddWithValue("@Price", car.Price);
                    cmd.Parameters.AddWithValue("@Description", car.Description ?? "");
                    cmd.Parameters.AddWithValue("@Year", (object)car.Year ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsAvailable", car.IsAvailable);
                    cmd.Parameters.AddWithValue("@IsDisplay", car.IsDisplay);
                    cmd.Parameters.AddWithValue("@CreatedDate", car.CreatedDate);

                    object result = cmd.ExecuteScalar();
                    return Convert.ToInt32(result);
                }
            }
        }

        private void UpdateCar(Car car)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"UPDATE Cars SET 
                              CarName = @CarName, 
                              Price = @Price, 
                              Description = @Description, 
                              Year = @Year,
                              IsAvailable = @IsAvailable,
                              IsDisplay = @IsDisplay, 
                              UpdatedDate = @UpdatedDate 
                              WHERE CarID = @CarID";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CarID", car.CarID);
                    cmd.Parameters.AddWithValue("@CarName", car.CarName);
                    cmd.Parameters.AddWithValue("@Price", car.Price);
                    cmd.Parameters.AddWithValue("@Description", car.Description ?? "");
                    cmd.Parameters.AddWithValue("@Year", (object)car.Year ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsAvailable", car.IsAvailable);
                    cmd.Parameters.AddWithValue("@IsDisplay", car.IsDisplay);
                    cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Phương thức lưu ảnh xe
        private string SaveCarImage(FileUpload fileUpload)
        {
            if (fileUpload.HasFile)
            {
                string fileName = Path.GetFileName(fileUpload.FileName);
                string fileExtension = Path.GetExtension(fileName).ToLower();

                // Kiểm tra định dạng file
                if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif")
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                    string uploadPath = Server.MapPath("~/images/cars/");

                    // Tạo thư mục nếu chưa có
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    string fullPath = Path.Combine(uploadPath, uniqueFileName);
                    fileUpload.SaveAs(fullPath);
                    return "~/images/cars/" + uniqueFileName;
                }
                else
                {
                    ShowMessage("Chỉ chấp nhận file ảnh có định dạng: jpg, jpeg, png, gif");
                }
            }
            return "";
        }

        // Phương thức thêm ảnh xe vào database
        private void InsertCarImage(int carID, string imagePath, string imageName = null, bool isMain = false)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                var insertQuery = @"INSERT INTO CarImages (CarID, ImagePath, ImageName, IsMain, CreatedDate) 
                           VALUES (@CarID, @ImagePath, @ImageName, @IsMain, @CreatedDate)";

                using (SqlCommand insertCmd = new SqlCommand(insertQuery, conn))
                {
                    insertCmd.Parameters.AddWithValue("@CarID", carID);
                    insertCmd.Parameters.AddWithValue("@ImagePath", imagePath);
                    insertCmd.Parameters.AddWithValue("@ImageName", imageName ?? "");
                    insertCmd.Parameters.AddWithValue("@IsMain", isMain);
                    insertCmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                    insertCmd.ExecuteNonQuery();
                }
            }
        }

        // Phương thức cập nhật ảnh xe
        private void UpdateCarImage(int carID, string imagePath, string imageName = null, bool isMain = false)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Kiểm tra xem carID đã tồn tại trong CarImages chưa
                var checkQuery = @"SELECT COUNT(*) FROM CarImages WHERE CarID = @CarID AND IsMain = 1";

                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@CarID", carID);
                    int count = (int)checkCmd.ExecuteScalar();

                    if (count == 0)
                    {
                        // Chưa có ảnh chính -> Thêm mới
                        InsertCarImage(carID, imagePath, imageName, isMain);
                    }
                    else
                    {
                        // Đã có ảnh chính -> Cập nhật
                        var updateQuery = @"UPDATE CarImages 
                                   SET ImagePath = @ImagePath, 
                                       ImageName = @ImageName, 
                                       IsMain = @IsMain
                                   WHERE CarID = @CarID AND IsMain = 1";

                        using (SqlCommand updateCmd = new SqlCommand(updateQuery, conn))
                        {
                            updateCmd.Parameters.AddWithValue("@CarID", carID);
                            updateCmd.Parameters.AddWithValue("@ImagePath", imagePath);
                            updateCmd.Parameters.AddWithValue("@ImageName", imageName ?? "");
                            updateCmd.Parameters.AddWithValue("@IsMain", isMain);

                            updateCmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        protected void btnHuy_Click(object sender, EventArgs e)
        {
            HideAllPanels();
        }

        protected void gvXe_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvXe.PageIndex = e.NewPageIndex;
            BindGridView();
        }

        protected void gvXe_RowEditing(object sender, GridViewEditEventArgs e)
        {
            HideAllPanels();
            gvXe.EditIndex = e.NewEditIndex;
            BindGridView();
        }

        protected void gvXe_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                int carId = Convert.ToInt32(gvXe.DataKeys[e.RowIndex].Value);

                TextBox txtTenXeEdit = (TextBox)gvXe.Rows[e.RowIndex].FindControl("txtTenXe");
                TextBox txtGiaEdit = (TextBox)gvXe.Rows[e.RowIndex].FindControl("txtGia");
                FileUpload fuHinhAnhEdit = (FileUpload)gvXe.Rows[e.RowIndex].FindControl("fuHinhAnh");

                if (txtTenXeEdit != null && txtGiaEdit != null)
                {
                    Car car = GetCarById(carId);
                    if (car != null)
                    {
                        car.CarName = txtTenXeEdit.Text.Trim();
                        car.Price = Convert.ToDecimal(txtGiaEdit.Text.Trim());
                        car.UpdatedDate = DateTime.Now;
                        UpdateCar(car);

                        // Xử lý upload ảnh nếu có
                        if (fuHinhAnhEdit != null && fuHinhAnhEdit.HasFile)
                        {
                            string imagePath = SaveCarImage(fuHinhAnhEdit);
                            if (!string.IsNullOrEmpty(imagePath))
                            {
                                UpdateCarImage(carId, imagePath, car.CarName, true);
                            }
                        }

                        ShowMessage("Cập nhật thành công!");
                    }
                }

                gvXe.EditIndex = -1;
                BindGridView();
            }
            catch (Exception ex)
            {
                ShowMessage("Có lỗi xảy ra khi cập nhật: " + ex.Message);
            }
        }

        protected void gvXe_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvXe.EditIndex = -1;
            BindGridView();
        }

        protected void gvXe_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int carId = Convert.ToInt32(gvXe.DataKeys[e.RowIndex].Value);
                DeleteCar(carId);
                BindGridView();
                ShowMessage("Xóa thành công!");
            }
            catch (Exception ex)
            {
                ShowMessage("Có lỗi xảy ra khi xóa: " + ex.Message);
            }
        }

        private void DeleteCar(int carId)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = "UPDATE Cars SET IsDisplay = 0, UpdatedDate = @UpdatedDate WHERE CarID = @CarID";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@CarID", carId);
                    cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void gvXe_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Empty implementation
        }

        // News section
        protected void btnThemTinTuc_Click(object sender, EventArgs e)
        {
            HideAllPanels();
            pnlThemTinTuc.Visible = true;
            lblTieuDeTinTuc.Text = "Thêm tin tức mới";
            ClearNewsForm();
        }

        private void ClearNewsForm()
        {
            txtTieuDe.Text = "";
            txtNoiDung.Text = "";
            chkTinTucHienThi.Checked = true;
        }

        protected void btnLuuTinTuc_Click(object sender, EventArgs e)
        {
            try
            {
                string imagePath = "";
                if (fuHinhAnhTinTuc.HasFile)
                {
                    imagePath = SaveNewsImage();
                }

                News news = new News
                {
                    Title = txtTieuDe.Text.Trim(),
                    Slug = CreateSlug(txtTieuDe.Text.Trim()),
                    Summary = "", // Có thể thêm field tóm tắt
                    Content = txtNoiDung.Text.Trim(),
                    ImagePath = imagePath,
                    IsPublished = chkTinTucHienThi.Checked,
                    PublishedDate = DateTime.Now,
                    CreatedBy = 1, // Default admin user
                    CreatedDate = DateTime.Now,
                    UpdatedDate = null
                };

                InsertNews(news);
                HideAllPanels();
                ShowMessage("Thêm tin tức thành công!");
            }
            catch (Exception ex)
            {
                ShowMessage("Có lỗi xảy ra: " + ex.Message);
            }
        }

        private string SaveNewsImage()
        {
            if (fuHinhAnhTinTuc.HasFile)
            {
                string fileName = Path.GetFileName(fuHinhAnhTinTuc.FileName);
                string fileExtension = Path.GetExtension(fileName).ToLower();

                if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif")
                {
                    string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                    string uploadPath = Server.MapPath("~/images/news/");

                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    string fullPath = Path.Combine(uploadPath, uniqueFileName);
                    fuHinhAnhTinTuc.SaveAs(fullPath);
                    return "~/images/news/" + uniqueFileName;
                }
                else
                {
                    ShowMessage("Chỉ chấp nhận file ảnh có định dạng: jpg, jpeg, png, gif");
                }
            }
            return "";
        }

        private string CreateSlug(string title)
        {
            if (string.IsNullOrEmpty(title))
                return "";

            // Tạo slug đơn giản - có thể cải thiện thêm
            return title.ToLower()
                       .Replace(" ", "-")
                       .Replace("--", "-")
                       .Trim('-');
        }

        private void InsertNews(News news)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"INSERT INTO News (Title, Slug, Summary, Content, ImagePath, IsPublished, PublishedDate, CreatedBy, CreatedDate, UpdatedDate)
                              VALUES (@Title, @Slug, @Summary, @Content, @ImagePath, @IsPublished, @PublishedDate, @CreatedBy, @CreatedDate, @UpdatedDate)";

                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", news.Title);
                    cmd.Parameters.AddWithValue("@Slug", news.Slug);
                    cmd.Parameters.AddWithValue("@Summary", news.Summary ?? "");
                    cmd.Parameters.AddWithValue("@Content", news.Content);
                    cmd.Parameters.AddWithValue("@ImagePath", news.ImagePath ?? "");
                    cmd.Parameters.AddWithValue("@IsPublished", news.IsPublished);
                    cmd.Parameters.AddWithValue("@PublishedDate", news.PublishedDate);
                    cmd.Parameters.AddWithValue("@CreatedBy", news.CreatedBy);
                    cmd.Parameters.AddWithValue("@CreatedDate", news.CreatedDate);
                    cmd.Parameters.AddWithValue("@UpdatedDate", (object)news.UpdatedDate ?? DBNull.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void btnHuyTinTuc_Click(object sender, EventArgs e)
        {
            HideAllPanels();
        }

        private void HideAllPanels()
        {
            pnlChiTietXe.Visible = false;
            pnlThemTinTuc.Visible = false;
            gvXe.EditIndex = -1;
        }

        private void ShowMessage(string message)
        {
            string script = "alert('" + message.Replace("'", "\\'") + "');";
            ClientScript.RegisterStartupScript(this.GetType(), "alert", script, true);
        }
    }
}