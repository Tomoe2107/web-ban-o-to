using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Diagnostics;
using System.Data;

namespace website_ban_o_to
{
    public partial class trangchu1 : System.Web.UI.Page
    {
        // Lấy connection string từ web.config hoặc hardcode
        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString
            ?? "Data Source=DESKTOP-NMTRDC3\\MSSQLSERVER01;Initial Catalog=BanOto;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True";

        // Biến lưu trữ dữ liệu để tránh query nhiều lần
        private static List<Car> allCars = new List<Car>();
        private static List<Brand> allBrands = new List<Brand>();
        private static List<Location> allLocations = new List<Location>();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    LogMessage("Page_Load: Bắt đầu tải trang");

                    // Load dữ liệu cơ bản
                    LoadBrands();
                    LoadLocations();
                    LoadCars();

                    // Hiển thị tất cả xe ban đầu
                    DisplayCars(allCars);

                    LogMessage("Page_Load: Hoàn thành tải trang");
                }
                catch (Exception ex)
                {
                    LogMessage($"ERROR Page_Load: {ex.Message}");
                    ShowErrorMessage("Có lỗi xảy ra khi tải trang. Vui lòng thử lại.");
                }
            }
        }

        #region Data Loading Methods

        private void LoadBrands()
        {
            LogMessage("LoadBrands: Bắt đầu load danh sách hãng xe");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT BrandID, BrandName FROM Brands WHERE IsActive = 1 ORDER BY BrandName";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        allBrands.Clear();
                        cblBrands.Items.Clear();

                        while (reader.Read())
                        {
                            var brand = new Brand
                            {
                                BrandID = Convert.ToInt32(reader["BrandID"]),
                                BrandName = reader["BrandName"].ToString()
                            };
                            allBrands.Add(brand);

                            // Thêm vào CheckBoxList
                            cblBrands.Items.Add(new ListItem(brand.BrandName, brand.BrandID.ToString()));
                        }
                    }
                }

                LogMessage($"LoadBrands: Đã load {allBrands.Count} hãng xe");
            }
            catch (Exception ex)
            {
                LogMessage($"ERROR LoadBrands: {ex.Message}");
                // Load dữ liệu mặc định nếu lỗi
                LoadDefaultBrands();
            }
        }

        private void LoadDefaultBrands()
        {
            cblBrands.Items.Clear();
            string[] defaultBrands = { "Audi", "BMW", "Chevrolet", "Ford", "Honda", "Hyundai", "Kia", "Toyota", "VinFast" };

            for (int i = 0; i < defaultBrands.Length; i++)
            {
                cblBrands.Items.Add(new ListItem(defaultBrands[i], (i + 1).ToString()));
            }
        }

        private void LoadLocations()
        {
            LogMessage("LoadLocations: Bắt đầu load danh sách địa điểm");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "SELECT LocationID, LocationName FROM Locations WHERE IsActive = 1 ORDER BY LocationName";
                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        allLocations.Clear();
                        ddlRegion.Items.Clear();
                        ddlRegion.Items.Add(new ListItem("Toàn quốc", "0"));

                        while (reader.Read())
                        {
                            var location = new Location
                            {
                                LocationID = Convert.ToInt32(reader["LocationID"]),
                                LocationName = reader["LocationName"].ToString()
                            };
                            allLocations.Add(location);

                            // Thêm vào DropDownList
                            ddlRegion.Items.Add(new ListItem(location.LocationName, location.LocationID.ToString()));
                        }
                    }
                }

                LogMessage($"LoadLocations: Đã load {allLocations.Count} địa điểm");
            }
            catch (Exception ex)
            {
                LogMessage($"ERROR LoadLocations: {ex.Message}");
                // Load dữ liệu mặc định nếu lỗi
                LoadDefaultLocations();
            }
        }

        private void LoadDefaultLocations()
        {
            ddlRegion.Items.Clear();
            ddlRegion.Items.Add(new ListItem("Toàn quốc", "0"));

            string[] provinces = {
                "Hà Nội", "TP HCM", "Đà Nẵng", "Hải Phòng", "Cần Thơ",
                "An Giang", "Bà Rịa - Vũng Tàu", "Bắc Giang", "Bắc Ninh", "Bình Dương"
            };

            for (int i = 0; i < provinces.Length; i++)
            {
                ddlRegion.Items.Add(new ListItem(provinces[i], (i + 1).ToString()));
            }
        }

        private void LoadCars()
        {
            LogMessage("LoadCars: Bắt đầu load danh sách xe");

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            c.CarID,
                            c.CarName,
                            c.Price,
                            c.Description,
                            c.Year,
                            c.Mileage,
                            c.FuelType,
                            c.Transmission,
                            c.ContactPhone,
                            c.ContactName,
                            c.PostDate,
                            b.BrandName,
                            l.LocationName,
                            ci.ImagePath
                        FROM Cars c
                        LEFT JOIN Brands b ON c.BrandID = b.BrandID
                        LEFT JOIN Locations l ON c.LocationID = l.LocationID
                        LEFT JOIN CarImages ci ON c.CarID = ci.CarID AND ci.IsMain = 1
                        WHERE c.IsAvailable = 1 AND c.IsDisplay = 1
                        ORDER BY c.PostDate DESC, c.CarID DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        allCars.Clear();

                        while (reader.Read())
                        {
                            var car = new Car
                            {
                                Id = Convert.ToInt32(reader["CarID"]),
                                Name = reader["CarName"]?.ToString() ?? "Không có tên",
                                Price = reader["Price"]?.ToString() ?? "0",
                                Location = reader["LocationName"]?.ToString() ?? "Chưa xác định",
                                Description = reader["Description"]?.ToString() ?? "",
                                Contact = reader["ContactName"]?.ToString() ?? "",
                                Phone = reader["ContactPhone"]?.ToString() ?? "",
                                ImageUrl = GetImageUrl(reader["ImagePath"]?.ToString()),
                                Brand = reader["BrandName"]?.ToString() ?? "",
                                Year = reader["Year"]?.ToString() ?? "",
                                Mileage = reader["Mileage"]?.ToString() ?? "",
                                FuelType = reader["FuelType"]?.ToString() ?? "",
                                Transmission = reader["Transmission"]?.ToString() ?? "",
                                PostDate = Convert.ToDateTime(reader["PostDate"])
                            };

                            allCars.Add(car);
                        }
                    }
                }

                LogMessage($"LoadCars: Đã load {allCars.Count} xe");
            }
            catch (Exception ex)
            {
                LogMessage($"ERROR LoadCars: {ex.Message}");
                // Load dữ liệu mẫu nếu lỗi database
                LoadSampleCars();
            }
        }

        private void LoadSampleCars()
        {
            LogMessage("LoadSampleCars: Load dữ liệu mẫu");

            allCars = new List<Car>
            {
                new Car
                {
                    Id = 1,
                    Name = "Honda Civic 2020",
                    Price = "580",
                    Location = "Hà Nội",
                    Description = "Xe đẹp, bảo dưỡng định kỳ, không tai nạn",
                    Contact = "Anh Nam",
                    Phone = "0987654321",
                    ImageUrl = "/images/placeholder-car.jpg",
                    Brand = "Honda",
                    Year = "2020",
                    Mileage = "45000",
                    FuelType = "Xăng",
                    Transmission = "Tự động"
                },
                new Car
                {
                    Id = 2,
                    Name = "Toyota Camry 2019",
                    Price = "850",
                    Location = "TP HCM",
                    Description = "Xe gia đình, ít sử dụng, còn rất mới",
                    Contact = "Chị Lan",
                    Phone = "0912345678",
                    ImageUrl = "/images/placeholder-car.jpg",
                    Brand = "Toyota",
                    Year = "2019",
                    Mileage = "32000",
                    FuelType = "Xăng",
                    Transmission = "Tự động"
                },
                new Car
                {
                    Id = 3,
                    Name = "VinFast Lux A2.0",
                    Price = "950",
                    Location = "Đà Nẵng",
                    Description = "Xe VinFast, còn bảo hành, full option",
                    Contact = "Anh Hùng",
                    Phone = "0945678912",
                    ImageUrl = "/images/placeholder-car.jpg",
                    Brand = "VinFast",
                    Year = "2021",
                    Mileage = "18000",
                    FuelType = "Xăng",
                    Transmission = "Tự động"
                }
            };
        }

        #endregion

        #region Event Handlers

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogMessage("btnSearch_Click: Bắt đầu tìm kiếm");
            ApplyFilters();
        }

        protected void btnApplyFilter_Click(object sender, EventArgs e)
        {
            LogMessage("btnApplyFilter_Click: Áp dụng bộ lọc");
            ApplyFilters();
        }

        protected void ddlRegion_SelectedIndexChanged(object sender, EventArgs e)
        {
            LogMessage($"ddlRegion_SelectedIndexChanged: Chọn vùng {ddlRegion.SelectedItem.Text}");
            ApplyFilters();
        }

        protected void rptPagination_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ChangePage")
            {
                int pageNumber = Convert.ToInt32(e.CommandArgument);
                LogMessage($"rptPagination_ItemCommand: Chuyển trang {pageNumber}");

                var filteredCars = GetFilteredCars();
                DisplayCars(filteredCars, pageNumber);
            }
        }

        protected void rptCars_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Car car = (Car)e.Item.DataItem;

                // Format giá tiền
                Label lblPrice = (Label)e.Item.FindControl("lblPrice");
                if (lblPrice != null)
                {
                    if (decimal.TryParse(car.Price, out decimal price))
                    {
                        lblPrice.Text = price.ToString("N0") + " triệu";
                    }
                    else
                    {
                        lblPrice.Text = car.Price + " triệu";
                    }
                }

                // Xử lý hình ảnh
                System.Web.UI.WebControls.Image imgCar = (System.Web.UI.WebControls.Image)e.Item.FindControl("imgCar");
                if (imgCar != null)
                {
                    if (string.IsNullOrEmpty(car.ImageUrl) || car.ImageUrl == "/images/placeholder-car.jpg")
                    {
                        imgCar.ImageUrl = "~/Images/no-image.png";
                        imgCar.AlternateText = "Không có hình ảnh";
                    }
                    else
                    {
                        imgCar.ImageUrl = car.ImageUrl;
                        imgCar.AlternateText = car.Name;
                    }
                }
            }
        }

        #endregion

        #region Filter and Display Methods

        private void ApplyFilters()
        {
            try
            {
                var filteredCars = GetFilteredCars();
                DisplayCars(filteredCars, 1); // Reset về trang 1

                LogMessage($"ApplyFilters: Hiển thị {filteredCars.Count}/{allCars.Count} xe");
            }
            catch (Exception ex)
            {
                LogMessage($"ERROR ApplyFilters: {ex.Message}");
                ShowErrorMessage("Có lỗi khi lọc dữ liệu.");
            }
        }

        private List<Car> GetFilteredCars()
        {
            var filteredCars = new List<Car>(allCars);

            // Lọc theo từ khóa
            string keyword = txtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(keyword))
            {
                filteredCars = filteredCars.Where(c =>
                    c.Name.ToLower().Contains(keyword) ||
                    c.Description.ToLower().Contains(keyword) ||
                    c.Brand.ToLower().Contains(keyword)).ToList();
            }

            // Lọc theo hãng xe
            var selectedBrands = cblBrands.Items.Cast<ListItem>()
                .Where(i => i.Selected).Select(i => i.Text).ToList();

            if (selectedBrands.Any())
            {
                filteredCars = filteredCars.Where(c => selectedBrands.Contains(c.Brand)).ToList();
            }

            // Lọc theo khu vực
            if (ddlRegion.SelectedValue != "0")
            {
                string selectedLocation = ddlRegion.SelectedItem.Text;
                filteredCars = filteredCars.Where(c => c.Location.Contains(selectedLocation)).ToList();
            }

            // Lọc theo giá
            string priceRange = ddlPriceRange.SelectedValue;
            if (priceRange != "0")
            {
                filteredCars = filteredCars.Where(c => CheckPriceInRange(c.Price, priceRange)).ToList();
            }

            return filteredCars;
        }

        private bool CheckPriceInRange(string priceText, string range)
        {
            if (!decimal.TryParse(priceText, out decimal price))
                return false;

            switch (range)
            {
                case "1": return price < 300;
                case "2": return price >= 300 && price <= 500;
                case "3": return price > 500 && price <= 800;
                case "4": return price > 800;
                default: return true;
            }
        }

        private void DisplayCars(List<Car> cars, int currentPage = 1, int pageSize = 8)
        {
            try
            {
                // Phân trang
                var pagedCars = cars.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();

                // Bind dữ liệu xe
                rptCars.DataSource = pagedCars;
                rptCars.DataBind();

                // Cập nhật số lượng
                lblTotalResults.Text = $"Tổng: {cars.Count} tin";

                // Tạo phân trang
                CreatePagination(cars.Count, currentPage, pageSize);

                LogMessage($"DisplayCars: Hiển thị {pagedCars.Count} xe trang {currentPage}");
            }
            catch (Exception ex)
            {
                LogMessage($"ERROR DisplayCars: {ex.Message}");
                ShowErrorMessage("Có lỗi khi hiển thị dữ liệu.");
            }
        }

        private void CreatePagination(int totalRecords, int currentPage, int pageSize)
        {
            int totalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            if (totalPages <= 1)
            {
                rptPagination.DataSource = null;
                rptPagination.DataBind();
                return;
            }

            var pages = new List<Pagination>();

            // Thêm nút Previous
            if (currentPage > 1)
            {
                pages.Add(new Pagination { PageNumber = currentPage - 1, DisplayText = "‹ Trước", IsCurrent = false });
            }

            // Thêm các trang
            int startPage = Math.Max(1, currentPage - 2);
            int endPage = Math.Min(totalPages, currentPage + 2);

            for (int i = startPage; i <= endPage; i++)
            {
                pages.Add(new Pagination
                {
                    PageNumber = i,
                    DisplayText = i.ToString(),
                    IsCurrent = (i == currentPage)
                });
            }

            // Thêm nút Next
            if (currentPage < totalPages)
            {
                pages.Add(new Pagination { PageNumber = currentPage + 1, DisplayText = "Sau ›", IsCurrent = false });
            }

            rptPagination.DataSource = pages;
            rptPagination.DataBind();
        }

        #endregion

        #region Utility Methods

        private string GetImageUrl(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return "~/Images/no-image.png";

            // Nếu đường dẫn đã là URL đầy đủ
            if (imagePath.StartsWith("http"))
                return imagePath;

            // Nếu là đường dẫn tương đối
            if (!imagePath.StartsWith("~/") && !imagePath.StartsWith("/"))
                return "~/Images/" + imagePath;

            return imagePath;
        }

        private void ShowErrorMessage(string message)
        {
            // Có thể hiển thị thông báo lỗi cho user
            lblTotalResults.Text = $"❌ {message}";
            lblTotalResults.ForeColor = System.Drawing.Color.Red;
        }

        private void LogMessage(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss}] {message}");
            HttpContext.Current.Trace.Write("CarDebug", message);
        }

        #endregion

        #region Data Classes

        public class Car
        {
            public int Id { get; set; }
            public string Name { get; set; } = "";
            public string Price { get; set; } = "0";
            public string Location { get; set; } = "";
            public string Description { get; set; } = "";
            public string Contact { get; set; } = "";
            public string Phone { get; set; } = "";
            public string ImageUrl { get; set; } = "";
            public string Brand { get; set; } = "";
            public string Year { get; set; } = "";
            public string Mileage { get; set; } = "";
            public string FuelType { get; set; } = "";
            public string Transmission { get; set; } = "";
            public DateTime PostDate { get; set; } = DateTime.Now;
        }

        public class Brand
        {
            public int BrandID { get; set; }
            public string BrandName { get; set; } = "";
        }

        public class Location
        {
            public int LocationID { get; set; }
            public string LocationName { get; set; } = "";
        }

        public class Pagination
        {
            public int PageNumber { get; set; }
            public string DisplayText { get; set; } = "";
            public bool IsCurrent { get; set; }
        }

        #endregion
    }
}