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
namespace website_ban_o_to.admin
{


    public partial class quanly1 : System.Web.UI.Page
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;

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
                            //car. = "/images/cars/" + uniqueFileName;
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


    }
}