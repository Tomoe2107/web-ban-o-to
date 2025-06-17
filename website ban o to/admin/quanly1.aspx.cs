using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace website_ban_o_to.admin
{
    // Class đại diện cho đối tượng Car theo cấu trúc database
    public class Car
    {
        public int CarID { get; set; }
        public int BrandID { get; set; }
        public int? LocationID { get; set; }
        public string CarName { get; set; }
        public decimal Price { get; set; } // Giá tính bằng triệu VND
        public string Description { get; set; }
        public int? Year { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsDisplay { get; set; } // Hiển thị trên website
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Thêm các thuộc tính bổ sung cho hiển thị
        public string BrandName { get; set; } // Tên thương hiệu
        public string LocationName { get; set; } // Tên địa điểm
        public string ImagePath { get; set; } // Đường dẫn hình ảnh

        // Properties để tương thích với GridView cũ
        public int ID => CarID;
        public string TenXe => CarName;
        public decimal Gia => Price;
        public string HinhAnh => ImagePath ?? "/images/default.jpg";
        public bool TrangThai => IsDisplay;
    }

    // Class đại diện cho đối tượng News theo cấu trúc database
    public class News
    {
        public int NewsID { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Summary { get; set; }
        public string Content { get; set; }
        public string ImagePath { get; set; }
        public bool IsPublished { get; set; }
        public DateTime PublishedDate { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }

        // Properties bổ sung cho hiển thị
        public string CreatedByName { get; set; } // Tên người tạo
    }

    public partial class quanly1 : System.Web.UI.Page
    {
        private string connectionString = "Data Source=DESKTOP-NMTRDC3\\MSSQLSERVER01;Initial Catalog=BanOto;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
                {
                    // Redirect if unauthorized
                    //Response.Redirect("~/dangnhap.aspx");
                    //return;
                }
                LoadDropDownLists();
                BindGridView();

                // Đảm bảo kích thước ảnh không đổi bằng CSS
                AddImageSizeCSS();
            }
        }

        private void AddImageSizeCSS()
        {
            string css = @"
                <style type='text/css'>
                    .admin-grid img {
                        width: 25px !important;
                        height: 25px !important;
                        object-fit: cover !important;
                        border: 1px solid #ddd;
                        border-radius: 4px;
                    }
                </style>";

            Page.Header.Controls.Add(new Literal { Text = css });
        }

        private void LoadDropDownLists()
        {
            // Load danh sách thương hiệu xe
            LoadBrands();
            // Load danh sách địa điểm
            LoadLocations();
        }

        private void LoadBrands()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT BrandID, BrandName FROM Brands ORDER BY BrandName";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            var brands = new List<object>();
                            while (reader.Read())
                            {
                                brands.Add(new
                                {
                                    BrandID = reader["BrandID"],
                                    BrandName = reader["BrandName"].ToString()
                                });
                            }

                            // Bind to dropdown control
                            // ddlBrand.DataSource = brands;
                            // ddlBrand.DataTextField = "BrandName";
                            // ddlBrand.DataValueField = "BrandID";
                            // ddlBrand.DataBind();
                            // ddlBrand.Items.Insert(0, new ListItem("-- Chọn thương hiệu --", ""));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error or show message
                throw new Exception("Lỗi khi load danh sách thương hiệu: " + ex.Message);
            }
        }

        private void LoadLocations()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT LocationID, LocationName FROM Locations ORDER BY LocationName";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            var locations = new List<object>();
                            while (reader.Read())
                            {
                                locations.Add(new
                                {
                                    LocationID = reader["LocationID"],
                                    LocationName = reader["LocationName"].ToString()
                                });
                            }

                            // Bind to dropdown control
                            // ddlLocation.DataSource = locations;
                            // ddlLocation.DataTextField = "LocationName";
                            // ddlLocation.DataValueField = "LocationID";
                            // ddlLocation.DataBind();
                            // ddlLocation.Items.Insert(0, new ListItem("-- Chọn địa điểm --", ""));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error or show message
                throw new Exception("Lỗi khi load danh sách địa điểm: " + ex.Message);
            }
        }

        protected void gvXe_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Không cần xử lý selected index nữa vì edit trực tiếp trong GridView
        }

        private Car GetCarById(int carId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"SELECT c.*, b.BrandName, l.LocationName 
                                  FROM Cars c 
                                  LEFT JOIN Brands b ON c.BrandID = b.BrandID
                                  LEFT JOIN Locations l ON c.LocationID = l.LocationID
                                  WHERE c.CarID = @CarID";

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
                                    IsAvailable = Convert.ToBoolean(reader["IsAvailable"]),
                                    IsDisplay = Convert.ToBoolean(reader["IsDisplay"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    UpdatedDate = reader["UpdatedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["UpdatedDate"]) : null,
                                    BrandName = reader["BrandName"]?.ToString(),
                                    LocationName = reader["LocationName"]?.ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy thông tin xe: " + ex.Message);
            }

            return null;
        }


        private void BindGridView()
        {
            List<Car> cars = GetDanhSachCars();
            gvXe.DataSource = cars;
            gvXe.DataKeyNames = new string[] { "ID" }; // Sử dụng property ID tương thích
            gvXe.DataBind();
        }

        private List<Car> GetDanhSachCars()
        {
            List<Car> cars = new List<Car>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"SELECT c.*, b.BrandName, l.LocationName 
                                  FROM Cars c 
                                  LEFT JOIN Brands b ON c.BrandID = b.BrandID
                                  LEFT JOIN Locations l ON c.LocationID = l.LocationID
                                  WHERE c.IsDisplay = 1
                                  ORDER BY c.CreatedDate DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cars.Add(new Car
                                {
                                    CarID = Convert.ToInt32(reader["CarID"]),
                                    BrandID = Convert.ToInt32(reader["BrandID"]),
                                    LocationID = reader["LocationID"] != DBNull.Value ? (int?)Convert.ToInt32(reader["LocationID"]) : null,
                                    CarName = reader["CarName"].ToString(),
                                    Price = Convert.ToDecimal(reader["Price"]),
                                    Description = reader["Description"]?.ToString(),
                                    Year = reader["Year"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Year"]) : null,
                                    IsAvailable = Convert.ToBoolean(reader["IsAvailable"]),
                                    IsDisplay = Convert.ToBoolean(reader["IsDisplay"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    UpdatedDate = reader["UpdatedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["UpdatedDate"]) : null,
                                    BrandName = reader["BrandName"]?.ToString(),
                                    LocationName = reader["LocationName"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách xe: " + ex.Message);
            }

            return cars;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // Ẩn tất cả form khi tìm kiếm
            pnlChiTietXe.Visible = false;
            pnlThemTinTuc.Visible = false;

            // Hủy edit mode nếu đang edit
            gvXe.EditIndex = -1;

            string searchKeyword = txtSearch.Text.Trim();

            List<Car> filteredCars = SearchCars(searchKeyword, null, null);
            gvXe.DataSource = filteredCars;
            gvXe.DataBind();
        }

        private List<Car> SearchCars(string keyword, int? brandId, int? locationId)
        {
            List<Car> cars = new List<Car>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"SELECT c.*, b.BrandName, l.LocationName 
                                  FROM Cars c 
                                  LEFT JOIN Brands b ON c.BrandID = b.BrandID
                                  LEFT JOIN Locations l ON c.LocationID = l.LocationID
                                  WHERE c.IsDisplay = 1";

                    List<SqlParameter> parameters = new List<SqlParameter>();

                    if (!string.IsNullOrEmpty(keyword))
                    {
                        sql += " AND (c.CarName LIKE @Keyword OR c.Description LIKE @Keyword)";
                        parameters.Add(new SqlParameter("@Keyword", "%" + keyword + "%"));
                    }

                    if (brandId.HasValue)
                    {
                        sql += " AND c.BrandID = @BrandID";
                        parameters.Add(new SqlParameter("@BrandID", brandId.Value));
                    }

                    if (locationId.HasValue)
                    {
                        sql += " AND c.LocationID = @LocationID";
                        parameters.Add(new SqlParameter("@LocationID", locationId.Value));
                    }

                    sql += " ORDER BY c.CreatedDate DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                cars.Add(new Car
                                {
                                    CarID = Convert.ToInt32(reader["CarID"]),
                                    BrandID = Convert.ToInt32(reader["BrandID"]),
                                    LocationID = reader["LocationID"] != DBNull.Value ? (int?)Convert.ToInt32(reader["LocationID"]) : null,
                                    CarName = reader["CarName"].ToString(),
                                    Price = Convert.ToDecimal(reader["Price"]),
                                    Description = reader["Description"]?.ToString(),
                                    Year = reader["Year"] != DBNull.Value ? (int?)Convert.ToInt32(reader["Year"]) : null,
                                    IsAvailable = Convert.ToBoolean(reader["IsAvailable"]),
                                    IsDisplay = Convert.ToBoolean(reader["IsDisplay"]),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    UpdatedDate = reader["UpdatedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["UpdatedDate"]) : null,
                                    BrandName = reader["BrandName"]?.ToString(),
                                    LocationName = reader["LocationName"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tìm kiếm xe: " + ex.Message);
            }

            return cars;
        }

        protected void btnThemXe_Click(object sender, EventArgs e)
        {
            // Ẩn form tin tức nếu đang hiển thị
            pnlThemTinTuc.Visible = false;

            // Hiển thị form thêm xe
            pnlChiTietXe.Visible = true;
            lblTitle.Text = "Thêm xe mới";
            ViewState["EditingCarID"] = null;

            // Reset form
            txtTenXe.Text = "";
            txtGia.Text = "";
            txtMoTa.Text = "";
            chkTrangThai.Checked = true;

            // Hủy edit mode của GridView nếu đang edit
            gvXe.EditIndex = -1;
            BindGridView();
        }

        protected void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                // Xử lý upload file hình ảnh
                string imagePath = "";
                if (fuHinhAnh.HasFile)
                {
                    string fileName = Path.GetFileName(fuHinhAnh.FileName);
                    string fileExtension = Path.GetExtension(fileName).ToLower();

                    // Kiểm tra định dạng file
                    if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif")
                    {
                        // Tạo tên file unique
                        string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                        string uploadPath = Server.MapPath("~/images/cars/");

                        // Tạo thư mục nếu chưa tồn tại
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        // Lưu file
                        string fullPath = Path.Combine(uploadPath, uniqueFileName);
                        fuHinhAnh.SaveAs(fullPath);
                        imagePath = "/images/cars/" + uniqueFileName;
                    }
                }

                Car car = new Car
                {
                    BrandID = 1, // Default value - cần thêm dropdown để chọn
                    LocationID = null,
                    CarName = txtTenXe.Text.Trim(),
                    Price = Convert.ToDecimal(txtGia.Text.Trim()),
                    Description = txtMoTa.Text.Trim(),
                    Year = DateTime.Now.Year, // Default current year
                    IsAvailable = true,
                    IsDisplay = chkTrangThai.Checked,
                    ImagePath = imagePath,
                    CreatedDate = DateTime.Now
                };

                // Chỉ thêm mới xe (không có chức năng update từ form này nữa)
                InsertCar(car);

                pnlChiTietXe.Visible = false;
                ViewState["EditingCarID"] = null;
                BindGridView();

                Response.Write("<script>alert('Thêm xe mới thành công!');</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Có lỗi xảy ra: " + ex.Message.Replace("'", "\\'") + "');</script>");
            }
        }

        private void InsertCar(Car car)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"INSERT INTO Cars (BrandID, LocationID, CarName, Price, Description, Year, IsAvailable, IsDisplay, CreatedDate)
                                   VALUES (@BrandID, @LocationID, @CarName, @Price, @Description, @Year, @IsAvailable, @IsDisplay, @CreatedDate)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@BrandID", car.BrandID);
                        cmd.Parameters.AddWithValue("@LocationID", car.LocationID.HasValue ? (object)car.LocationID.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@CarName", car.CarName);
                        cmd.Parameters.AddWithValue("@Price", car.Price);
                        cmd.Parameters.AddWithValue("@Description", !string.IsNullOrEmpty(car.Description) ? car.Description : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", car.Year.HasValue ? (object)car.Year.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsAvailable", car.IsAvailable);
                        cmd.Parameters.AddWithValue("@IsDisplay", car.IsDisplay);
                        cmd.Parameters.AddWithValue("@CreatedDate", car.CreatedDate);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm xe mới: " + ex.Message);
            }
        }

        private void UpdateCar(Car car)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"UPDATE Cars SET 
                                   BrandID = @BrandID, 
                                   LocationID = @LocationID,
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
                        cmd.Parameters.AddWithValue("@BrandID", car.BrandID);
                        cmd.Parameters.AddWithValue("@LocationID", car.LocationID.HasValue ? (object)car.LocationID.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@CarName", car.CarName);
                        cmd.Parameters.AddWithValue("@Price", car.Price);
                        cmd.Parameters.AddWithValue("@Description", !string.IsNullOrEmpty(car.Description) ? car.Description : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Year", car.Year.HasValue ? (object)car.Year.Value : DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsAvailable", car.IsAvailable);
                        cmd.Parameters.AddWithValue("@IsDisplay", car.IsDisplay);
                        cmd.Parameters.AddWithValue("@UpdatedDate", car.UpdatedDate.HasValue ? (object)car.UpdatedDate.Value : DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật thông tin xe: " + ex.Message);
            }
        }

        protected void btnHuy_Click(object sender, EventArgs e)
        {
            pnlChiTietXe.Visible = false;
            ViewState["EditingCarID"] = null;
        }

        protected void gvXe_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvXe.PageIndex = e.NewPageIndex;
            BindGridView();
        }

        protected void gvXe_RowEditing(object sender, GridViewEditEventArgs e)
        {
            // Ẩn tất cả form khi bắt đầu edit trong GridView
            pnlChiTietXe.Visible = false;
            pnlThemTinTuc.Visible = false;

            // Set edit index và bind lại data
            gvXe.EditIndex = e.NewEditIndex;
            BindGridView();
        }

        protected void gvXe_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                int carId = Convert.ToInt32(gvXe.DataKeys[e.RowIndex].Value);

                // Get values from GridView edit controls  
                TextBox txtTenXeEdit = (TextBox)gvXe.Rows[e.RowIndex].FindControl("txtTenXe");
                TextBox txtGiaEdit = (TextBox)gvXe.Rows[e.RowIndex].FindControl("txtGia");
                FileUpload fuHinhAnhEdit = (FileUpload)gvXe.Rows[e.RowIndex].FindControl("fuHinhAnh");

                if (txtTenXeEdit == null || txtGiaEdit == null)
                {
                    Response.Write("<script>alert('Không thể lấy dữ liệu từ form edit!');</script>");
                    return;
                }

                string carName = txtTenXeEdit.Text.Trim();
                decimal price = Convert.ToDecimal(txtGiaEdit.Text.Trim());

                // Get existing car data
                Car car = GetCarById(carId);
                if (car != null)
                {
                    car.CarName = carName;
                    car.Price = price;
                    car.UpdatedDate = DateTime.Now;

                    // Handle file upload in edit mode if needed
                    if (fuHinhAnhEdit != null && fuHinhAnhEdit.HasFile)
                    {
                        string fileName = Path.GetFileName(fuHinhAnhEdit.FileName);
                        string fileExtension = Path.GetExtension(fileName).ToLower();

                        if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif")
                        {
                            string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                            string uploadPath = Server.MapPath("~/images/cars/");

                            if (!Directory.Exists(uploadPath))
                            {
                                Directory.CreateDirectory(uploadPath);
                            }

                            string fullPath = Path.Combine(uploadPath, uniqueFileName);
                            fuHinhAnhEdit.SaveAs(fullPath);
                            car.ImagePath = "/images/cars/" + uniqueFileName;
                        }
                    }

                    // Cập nhật vào database
                    UpdateCar(car);

                    Response.Write("<script>alert('Cập nhật thành công!');</script>");
                }

                // Thoát edit mode và refresh data
                gvXe.EditIndex = -1;
                BindGridView();
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Có lỗi xảy ra khi cập nhật: " + ex.Message.Replace("'", "\\'") + "');</script>");
            }
        }

        protected void gvXe_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            // Hủy edit mode, không lưu thay đổi
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
            }
            catch (Exception ex)
            {
                // Handle error
                // lblError.Text = "Có lỗi xảy ra khi xóa: " + ex.Message;
            }
        }

        private void DeleteCar(int carId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Soft delete - khuyến nghị sử dụng
                    string sql = "UPDATE Cars SET IsDisplay = 0, UpdatedDate = @UpdatedDate WHERE CarID = @CarID";

                    // Hoặc hard delete nếu cần:
                    // string sql = "DELETE FROM Cars WHERE CarID = @CarID";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@CarID", carId);
                        cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa xe: " + ex.Message);
            }
        }

        // News section methods remain the same
        protected void btnThemTinTuc_Click(object sender, EventArgs e)
        {
            // Ẩn form thêm xe nếu đang hiển thị
            pnlChiTietXe.Visible = false;

            // Hiển thị form thêm tin tức
            pnlThemTinTuc.Visible = true;
            lblTieuDeTinTuc.Text = "Thêm tin tức mới";

            // Reset form tin tức
            txtTieuDe.Text = "";
            txtNoiDung.Text = "";
            chkTinTucHienThi.Checked = true;

            // Hủy edit mode của GridView nếu đang edit
            gvXe.EditIndex = -1;
            BindGridView();
        }

        protected void btnLuuTinTuc_Click(object sender, EventArgs e)
        {
            try
            {
                // Validation
                if (string.IsNullOrEmpty(txtTieuDe.Text.Trim()))
                {
                    Response.Write("<script>alert('Vui lòng nhập tiêu đề tin tức!');</script>");
                    return;
                }

                if (string.IsNullOrEmpty(txtNoiDung.Text.Trim()))
                {
                    Response.Write("<script>alert('Vui lòng nhập nội dung tin tức!');</script>");
                    return;
                }

                // Xử lý upload file hình ảnh cho tin tức
                string imagePath = "";
                if (fuHinhAnhTinTuc.HasFile)
                {
                    string fileName = Path.GetFileName(fuHinhAnhTinTuc.FileName);
                    string fileExtension = Path.GetExtension(fileName).ToLower();

                    // Kiểm tra định dạng file
                    if (fileExtension == ".jpg" || fileExtension == ".jpeg" || fileExtension == ".png" || fileExtension == ".gif")
                    {
                        // Tạo tên file unique
                        string uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                        string uploadPath = Server.MapPath("~/images/news/");

                        // Tạo thư mục nếu chưa tồn tại
                        if (!Directory.Exists(uploadPath))
                        {
                            Directory.CreateDirectory(uploadPath);
                        }

                        // Lưu file
                        string fullPath = Path.Combine(uploadPath, uniqueFileName);
                        fuHinhAnhTinTuc.SaveAs(fullPath);
                        imagePath = "/images/news/" + uniqueFileName;
                    }
                    else
                    {
                        Response.Write("<script>alert('Chỉ chấp nhận file ảnh: .jpg, .jpeg, .png, .gif');</script>");
                        return;
                    }
                }

                // Tạo slug từ tiêu đề
                string slug = GenerateSlug(txtTieuDe.Text.Trim());

                // Tạo summary từ nội dung (500 ký tự đầu)
                string summary = txtNoiDung.Text.Trim();
                if (summary.Length > 500)
                {
                    summary = summary.Substring(0, 497) + "...";
                }

                // Lưu tin tức vào database
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"INSERT INTO News (Title, Slug, Summary, Content, ImagePath, IsPublished, PublishedDate, CreatedBy, CreatedDate)
                                   VALUES (@Title, @Slug, @Summary, @Content, @ImagePath, @IsPublished, @PublishedDate, @CreatedBy, @CreatedDate)";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Title", txtTieuDe.Text.Trim());
                        cmd.Parameters.AddWithValue("@Slug", slug);
                        cmd.Parameters.AddWithValue("@Summary", summary);
                        cmd.Parameters.AddWithValue("@Content", txtNoiDung.Text.Trim());
                        cmd.Parameters.AddWithValue("@ImagePath", !string.IsNullOrEmpty(imagePath) ? imagePath : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsPublished", chkTinTucHienThi.Checked);
                        cmd.Parameters.AddWithValue("@PublishedDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@CreatedBy", 1); // Default admin user ID - thay bằng user thực tế
                        cmd.Parameters.AddWithValue("@CreatedDate", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                }

                // Ẩn form và reset
                pnlThemTinTuc.Visible = false;
                txtTieuDe.Text = "";
                txtNoiDung.Text = "";
                chkTinTucHienThi.Checked = true;

                Response.Write("<script>alert('Thêm tin tức thành công!');</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Có lỗi xảy ra: " + ex.Message.Replace("'", "\\'") + "');</script>");
            }
        }

        private string GenerateSlug(string title)
        {
            if (string.IsNullOrEmpty(title))
                return "";

            // Chuyển về lowercase
            string slug = title.ToLower();

            // Loại bỏ dấu tiếng Việt (cơ bản)
            slug = slug.Replace("á", "a").Replace("à", "a").Replace("ả", "a").Replace("ã", "a").Replace("ạ", "a")
                      .Replace("ă", "a").Replace("ắ", "a").Replace("ằ", "a").Replace("ẳ", "a").Replace("ẵ", "a").Replace("ặ", "a")
                      .Replace("â", "a").Replace("ấ", "a").Replace("ầ", "a").Replace("ẩ", "a").Replace("ẫ", "a").Replace("ậ", "a")
                      .Replace("é", "e").Replace("è", "e").Replace("ẻ", "e").Replace("ẽ", "e").Replace("ẹ", "e")
                      .Replace("ê", "e").Replace("ế", "e").Replace("ề", "e").Replace("ể", "e").Replace("ễ", "e").Replace("ệ", "e")
                      .Replace("í", "i").Replace("ì", "i").Replace("ỉ", "i").Replace("ĩ", "i").Replace("ị", "i")
                      .Replace("ó", "o").Replace("ò", "o").Replace("ỏ", "o").Replace("õ", "o").Replace("ọ", "o")
                      .Replace("ô", "o").Replace("ố", "o").Replace("ồ", "o").Replace("ổ", "o").Replace("ỗ", "o").Replace("ộ", "o")
                      .Replace("ơ", "o").Replace("ớ", "o").Replace("ờ", "o").Replace("ở", "o").Replace("ỡ", "o").Replace("ợ", "o")
                      .Replace("ú", "u").Replace("ù", "u").Replace("ủ", "u").Replace("ũ", "u").Replace("ụ", "u")
                      .Replace("ư", "u").Replace("ứ", "u").Replace("ừ", "u").Replace("ử", "u").Replace("ữ", "u").Replace("ự", "u")
                      .Replace("ý", "y").Replace("ỳ", "y").Replace("ỷ", "y").Replace("ỹ", "y").Replace("ỵ", "y")
                      .Replace("đ", "d");

            // Thay thế khoảng trắng và ký tự đặc biệt thành dấu gạch ngang
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"-+", "-");
            slug = slug.Trim('-');

            // Thêm timestamp để đảm bảo unique
            slug = slug + "-" + DateTime.Now.Ticks.ToString().Substring(12);

            return slug;
        }

        protected void btnHuyTinTuc_Click(object sender, EventArgs e)
        {
            pnlThemTinTuc.Visible = false;

            // Reset form
            txtTieuDe.Text = "";
            txtNoiDung.Text = "";
            chkTinTucHienThi.Checked = true;
        }

        // Event handlers cho GridView tin tức
        protected void gvNews_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView gv = (GridView)sender;
            gv.PageIndex = e.NewPageIndex;
            BindNewsGridView();
        }

        protected void gvNews_RowEditing(object sender, GridViewEditEventArgs e)
        {
            GridView gv = (GridView)sender;
            pnlChiTietXe.Visible = false;
            pnlThemTinTuc.Visible = false;

            gv.EditIndex = e.NewEditIndex;
            BindNewsGridView();
        }

        protected void gvNews_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                GridView gv = (GridView)sender;
                int newsId = Convert.ToInt32(gv.DataKeys[e.RowIndex].Value);

                TextBox txtTitleEdit = (TextBox)gv.Rows[e.RowIndex].FindControl("txtTitle");
                TextBox txtSummaryEdit = (TextBox)gv.Rows[e.RowIndex].FindControl("txtSummary");

                if (txtTitleEdit != null)
                {
                    // Lấy tin tức hiện tại
                    News news = GetNewsById(newsId);
                    if (news != null)
                    {
                        news.Title = txtTitleEdit.Text.Trim();
                        news.Summary = txtSummaryEdit?.Text.Trim();
                        news.Slug = GenerateSlug(news.Title);
                        news.UpdatedDate = DateTime.Now;

                        UpdateNews(news);
                        Response.Write("<script>alert('Cập nhật tin tức thành công!');</script>");
                    }
                }

                gv.EditIndex = -1;
                BindNewsGridView();
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Có lỗi xảy ra khi cập nhật tin tức: " + ex.Message.Replace("'", "\\'") + "');</script>");
            }
        }

        protected void gvNews_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            GridView gv = (GridView)sender;
            gv.EditIndex = -1;
            BindNewsGridView();
        }

        protected void gvNews_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                GridView gv = (GridView)sender;
                int newsId = Convert.ToInt32(gv.DataKeys[e.RowIndex].Value);

                DeleteNews(newsId);
                BindNewsGridView();

                Response.Write("<script>alert('Xóa tin tức thành công!');</script>");
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('Có lỗi xảy ra khi xóa tin tức: " + ex.Message.Replace("'", "\\'") + "');</script>");
            }
        }

        private void BindNewsGridView()
        {
            List<News> newsList = GetDanhSachNews();
            // Assuming you have a GridView named gvNews in your ASPX
            // gvNews.DataSource = newsList;
            // gvNews.DataBind();
        }

        private News GetNewsById(int newsId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"SELECT n.*, u.Username as CreatedByName 
                                  FROM News n 
                                  LEFT JOIN Users u ON n.CreatedBy = u.UserID
                                  WHERE n.NewsID = @NewsID";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@NewsID", newsId);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new News
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
                                    UpdatedDate = reader["UpdatedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["UpdatedDate"]) : null,
                                    CreatedByName = reader["CreatedByName"]?.ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy thông tin tin tức: " + ex.Message);
            }

            return null;
        }

        private List<News> GetDanhSachNews()
        {
            List<News> newsList = new List<News>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"SELECT n.*, u.Username as CreatedByName 
                                  FROM News n 
                                  LEFT JOIN Users u ON n.CreatedBy = u.UserID
                                  ORDER BY n.CreatedDate DESC";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                newsList.Add(new News
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
                                    UpdatedDate = reader["UpdatedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["UpdatedDate"]) : null,
                                    CreatedByName = reader["CreatedByName"]?.ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error nếu cần
                Response.Write("<script>alert('Lỗi khi lấy danh sách tin tức: " + ex.Message.Replace("'", "\\'") + "');</script>");
            }

            return newsList;
        }

        private void UpdateNews(News news)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"UPDATE News SET 
                                   Title = @Title, 
                                   Slug = @Slug,
                                   Summary = @Summary,
                                   Content = @Content, 
                                   ImagePath = @ImagePath,
                                   IsPublished = @IsPublished,
                                   UpdatedDate = @UpdatedDate
                                   WHERE NewsID = @NewsID";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@NewsID", news.NewsID);
                        cmd.Parameters.AddWithValue("@Title", news.Title);
                        cmd.Parameters.AddWithValue("@Slug", news.Slug);
                        cmd.Parameters.AddWithValue("@Summary", !string.IsNullOrEmpty(news.Summary) ? news.Summary : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Content", !string.IsNullOrEmpty(news.Content) ? news.Content : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ImagePath", !string.IsNullOrEmpty(news.ImagePath) ? news.ImagePath : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@IsPublished", news.IsPublished);
                        cmd.Parameters.AddWithValue("@UpdatedDate", news.UpdatedDate.HasValue ? (object)news.UpdatedDate.Value : DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật tin tức: " + ex.Message);
            }
        }

        private void DeleteNews(int newsId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Soft delete - set IsPublished = false
                    string sql = "UPDATE News SET IsPublished = 0, UpdatedDate = @UpdatedDate WHERE NewsID = @NewsID";

                    // Hoặc hard delete nếu cần:
                    // string sql = "DELETE FROM News WHERE NewsID = @NewsID";

                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@NewsID", newsId);
                        cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa tin tức: " + ex.Message);
            }
        }
    }
}