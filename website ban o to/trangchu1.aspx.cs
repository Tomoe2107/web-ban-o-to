using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using website_ban_o_to.Models; // Import models đã tạo

namespace website_ban_o_to
{
    public partial class trangchu1 : System.Web.UI.Page
    {
        #region Properties and Fields

        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
        private const int PAGE_SIZE = 12;
        private const string SESSION_USER = "LoggedInUser";
        private const string SESSION_CARS = "CachedCars";

        private List<CarDetailViewModel> CachedCars
        {
            get
            {
                if (Session[SESSION_CARS] == null)
                {
                    Session[SESSION_CARS] = LoadCarsFromDatabase();
                }
                return (List<CarDetailViewModel>)Session[SESSION_CARS];
            }
            set { Session[SESSION_CARS] = value; }
        }

        private User CurrentUser
        {
            get { return Session[SESSION_USER] as User; }
            set { Session[SESSION_USER] = value; }
        }

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra multiple login attempt
                CheckMultipleLoginAttempt();

                if (!IsPostBack)
                {
                    LogMessage("Page_Load: Khởi tạo trang");

                    // Kiểm tra trạng thái đăng nhập
                    CheckLoginStatus();

                    // Load dữ liệu từ database
                    LoadBrandsFromDatabase();
                    LoadLocationsFromDatabase();

                    // Hiển thị tất cả xe
                    DisplayCars(CachedCars);

                    LogMessage($"Page_Load: Hoàn thành. Loaded {CachedCars.Count} cars");
                }
            }
            catch (Exception ex)
            {
                LogError("Page_Load", ex);
                ShowMessage("Có lỗi xảy ra khi tải trang. Vui lòng thử lại.", "error");
            }
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            // Đảm bảo trạng thái UI đúng
            UpdateLoginUI();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            // Update login UI
            UpdateLoginUI();

            // Update user activity if logged in
            UpdateUserActivity();

            // Add security headers
            Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
            Response.Headers.Add("X-Content-Type-Options", "nosniff");
            Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        }

        protected void Session_End(object sender, EventArgs e)
        {
            // Clean up application state when session ends
            if (CurrentUser != null)
            {
                Application.Remove($"User_{CurrentUser.UserID}_SessionID");
            }
        }

        #endregion

        #region Authentication Methods

        private void CheckMultipleLoginAttempt()
        {
            // Kiểm tra nếu user đã đăng nhập ở tab/cửa sổ khác
            if (CurrentUser != null)
            {
                string currentSessionId = Session.SessionID;
                string storedSessionId = Application[$"User_{CurrentUser.UserID}_SessionID"] as string;

                if (!string.IsNullOrEmpty(storedSessionId) && storedSessionId != currentSessionId)
                {
                    // Đăng xuất user hiện tại
                    Session.Clear();
                    CurrentUser = null;

                    ShowMessage("Tài khoản này đã được đăng nhập ở nơi khác. Bạn đã bị đăng xuất tự động.", "warning");
                    Response.Redirect("~/dangnhap1.aspx", false);
                    return;
                }

                // Cập nhật session ID hiện tại
                Application[$"User_{CurrentUser.UserID}_SessionID"] = currentSessionId;
            }
        }

        private void CheckLoginStatus()
        {
            // Kiểm tra nếu có thông tin đăng nhập từ session hoặc cookie
            if (CurrentUser == null)
            {
                // Thử restore từ cookie (Remember Me)
                HttpCookie loginCookie = Request.Cookies["RememberLogin"];
                if (loginCookie != null && !string.IsNullOrEmpty(loginCookie.Value))
                {
                    try
                    {
                        int userId = Convert.ToInt32(loginCookie.Value);
                        CurrentUser = GetUserById(userId);
                        if (CurrentUser != null && CurrentUser.IsActive)
                        {
                            LogMessage($"User restored from cookie: {CurrentUser.Username}");

                            // ✅ THÊM DÒNG NÀY - Set Session khi restore từ cookie
                            Session["UserID"] = CurrentUser.UserID; // hoặc userId
                            Session["Username"] = CurrentUser.Username; // optional
                        }
                        else
                        {
                            // Cookie không hợp lệ, xóa nó
                            loginCookie.Expires = DateTime.Now.AddDays(-1);
                            Response.Cookies.Add(loginCookie);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogError("CheckLoginStatus - Cookie restore", ex);
                    }
                }
            }
            else
            {
                // ✅ THÊM PHẦN NÀY - Đảm bảo Session được set nếu CurrentUser có sẵn
                if (Session["UserID"] == null && CurrentUser != null)
                {
                    Session["UserID"] = CurrentUser.UserID;
                    Session["Username"] = CurrentUser.Username;
                }
            }
        }
        private void UpdateLoginUI()
        {
            if (CurrentUser != null)
            {
                // Hiển thị thông tin user
                pnlUserInfo.Visible = true;
                lblUserName.Text = CurrentUser.FullName;

                LogMessage($"UI Updated for user: {CurrentUser.Username}");
            }
            else
            {
                // Ẩn thông tin user
                pnlUserInfo.Visible = false;
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            try
            {
                LogMessage($"User logout: {CurrentUser?.Username}");

                // Xóa session tracking
                if (CurrentUser != null)
                {
                    Application.Remove($"User_{CurrentUser.UserID}_SessionID");
                }

                // Xóa cookie Remember Me
                HttpCookie loginCookie = new HttpCookie("RememberLogin");
                loginCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(loginCookie);

                // Clear session
                Session.Clear();
                CurrentUser = null;

                ShowMessage("Đăng xuất thành công!", "success");

                // Reload page to update UI
                Response.Redirect(Request.RawUrl, false);
            }
            catch (Exception ex)
            {
                LogError("btnLogout_Click", ex);
                ShowMessage("Có lỗi khi đăng xuất.", "error");
            }
        }

        // Update user activity timestamp
        private void UpdateUserActivity()
        {
            if (CurrentUser != null)
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        string query = "UPDATE Users SET UpdatedDate = @UpdatedDate WHERE UserID = @UserID";
                        SqlCommand cmd = new SqlCommand(query, conn);
                        cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);
                        cmd.Parameters.AddWithValue("@UserID", CurrentUser.UserID);

                        conn.Open();
                        cmd.ExecuteNonQuery();

                        // Update the current user object
                        CurrentUser.UpdatedDate = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    LogError("UpdateUserActivity", ex);
                    // Don't throw - this is not critical
                }
            }
        }

        #endregion

        #region Data Loading Methods

        private void LoadBrandsFromDatabase()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT BrandID, BrandName 
                        FROM Brands 
                        WHERE IsActive = 1 
                        ORDER BY BrandName";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    cblBrands.Items.Clear();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            cblBrands.Items.Add(new ListItem(
                                reader["BrandName"].ToString(),
                                reader["BrandID"].ToString()
                            ));
                        }
                    }
                }

                LogMessage($"Loaded {cblBrands.Items.Count} brands from database");
            }
            catch (Exception ex)
            {
                LogError("LoadBrandsFromDatabase", ex);
                LoadDefaultBrands(); // Fallback
            }
        }

        private void LoadLocationsFromDatabase()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT LocationID, LocationName 
                        FROM Locations 
                        WHERE IsActive = 1 
                        ORDER BY LocationName";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    ddlLocation.Items.Clear();
                    ddlLocation.Items.Add(new ListItem("Toàn quốc", "0"));

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ddlLocation.Items.Add(new ListItem(
                                reader["LocationName"].ToString(),
                                reader["LocationID"].ToString()
                            ));
                        }
                    }
                }

                LogMessage($"Loaded {ddlLocation.Items.Count - 1} locations from database");
            }
            catch (Exception ex)
            {
                LogError("LoadLocationsFromDatabase", ex);
                LoadDefaultLocations(); // Fallback
            }
        }

        private List<CarDetailViewModel> LoadCarsFromDatabase()
        {
            var cars = new List<CarDetailViewModel>();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT 
                            c.CarID, c.CarName, c.Price, c.Description, c.Year, c.IsAvailable, c.CreatedDate,
                            c.BrandID, c.LocationID,
                            b.BrandName, l.LocationName,
                            ci.ImagePath
                        FROM Cars c
                        LEFT JOIN Brands b ON c.BrandID = b.BrandID
                        LEFT JOIN Locations l ON c.LocationID = l.LocationID
                        LEFT JOIN CarImages ci ON c.CarID = ci.CarID AND ci.IsMain = 1
                        WHERE c.IsDisplay = 1
                        ORDER BY c.CreatedDate DESC, c.CarID DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    conn.Open();

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var car = CreateCarViewModelFromReader(reader);
                            if (car != null)
                            {
                                cars.Add(car);
                            }
                        }
                    }
                }

                LogMessage($"Loaded {cars.Count} cars from database");
            }
            catch (Exception ex)
            {
                LogError("LoadCarsFromDatabase", ex);
                ShowMessage("Không thể tải dữ liệu xe từ database.", "error");
            }

            return cars;
        }

        private CarDetailViewModel CreateCarViewModelFromReader(SqlDataReader reader)
        {
            try
            {
                return new CarDetailViewModel
                {
                    Car = new Car
                    {
                        CarID = GetSafeInt(reader, "CarID"),
                        CarName = GetSafeString(reader, "CarName"),
                        Price = GetSafeDecimal(reader, "Price"),
                        Description = GetSafeString(reader, "Description"),
                        Year = GetSafeNullableInt(reader, "Year"),
                        IsAvailable = GetSafeBool(reader, "IsAvailable"),
                        CreatedDate = GetSafeDateTime(reader, "CreatedDate"),
                        BrandID = (int)GetSafeNullableInt(reader, "BrandID"),
                        LocationID = GetSafeNullableInt(reader, "LocationID")
                    },
                    Brand = new Brand
                    {
                        BrandID = GetSafeNullableInt(reader, "BrandID") ?? 0,
                        BrandName = GetSafeString(reader, "BrandName", "Không xác định")
                    },
                    Location = new Location
                    {
                        LocationID = GetSafeNullableInt(reader, "LocationID") ?? 0,
                        LocationName = GetSafeString(reader, "LocationName", "Chưa xác định")
                    },
                    Images = new List<CarImage>
                    {
                        new CarImage
                        {
                            CarID = GetSafeInt(reader, "CarID"),
                            ImagePath = GetSafeString(reader, "ImagePath", ""),
                            IsMain = true
                        }
                    }
                };
            }
            catch (Exception ex)
            {
                LogError("CreateCarViewModelFromReader", ex);
                return null;
            }
        }

        private User GetUserById(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT UserID, Username, FullName, Email, Phone, Role, IsActive 
                        FROM Users 
                        WHERE UserID = @UserID AND IsActive = 1";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                Username = reader["Username"].ToString(),
                                FullName = reader["FullName"].ToString(),
                                Email = reader["Email"].ToString(),
                                Phone = reader["Phone"]?.ToString(),
                                Role = reader["Role"].ToString(),
                                IsActive = Convert.ToBoolean(reader["IsActive"])
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("GetUserById", ex);
            }

            return null;
        }

        #endregion

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

        private int? GetSafeNullableInt(SqlDataReader reader, string columnName)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? (int?)null : reader.GetInt32(ordinal);
            }
            catch
            {
                return null;
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

        private bool GetSafeBool(SqlDataReader reader, string columnName, bool defaultValue = false)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? defaultValue : reader.GetBoolean(ordinal);
            }
            catch
            {
                return defaultValue;
            }
        }

        private DateTime GetSafeDateTime(SqlDataReader reader, string columnName, DateTime? defaultValue = null)
        {
            try
            {
                int ordinal = reader.GetOrdinal(columnName);
                return reader.IsDBNull(ordinal) ? (defaultValue ?? DateTime.Now) : reader.GetDateTime(ordinal);
            }
            catch
            {
                return defaultValue ?? DateTime.Now;
            }
        }

        #endregion

        #region Event Handlers

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            LogMessage($"Search clicked with keyword: {txtSearch.Text}");
            ApplyFilters();
        }

        protected void btnApplyFilter_Click(object sender, EventArgs e)
        {
            LogMessage("Apply filter clicked");
            ApplyFilters();
        }

        protected void btnClearFilter_Click(object sender, EventArgs e)
        {
            ClearAllFilters();
            ApplyFilters();
        }

        protected void cblBrands_SelectedIndexChanged(object sender, EventArgs e)
        {
            LogMessage("Brand selection changed");
            ApplyFilters();
        }

        protected void ddlLocation_SelectedIndexChanged(object sender, EventArgs e)
        {
            LogMessage($"Location changed to: {ddlLocation.SelectedItem.Text}");
            ApplyFilters();
        }

        protected void rptPagination_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "ChangePage")
            {
                int pageNumber = Convert.ToInt32(e.CommandArgument);
                var filteredCars = GetFilteredCars();
                DisplayCars(filteredCars, pageNumber);

                LogMessage($"Page changed to: {pageNumber}");
            }
        }

        protected void rptCars_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var carViewModel = (CarDetailViewModel)e.Item.DataItem;

                if (carViewModel?.Car != null)
                {
                    BindCarData(e.Item, carViewModel);
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
                DisplayCars(filteredCars, 1); // Reset to page 1

                // Update filter display
                UpdateFilterDisplay();

                LogMessage($"Filters applied. Showing {filteredCars.Count} of {CachedCars.Count} cars");
            }
            catch (Exception ex)
            {
                LogError("ApplyFilters", ex);
                ShowMessage("Có lỗi khi lọc dữ liệu.", "error");
            }
        }

        private List<CarDetailViewModel> GetFilteredCars()
        {
            var filteredCars = new List<CarDetailViewModel>(CachedCars);

            // Filter by search keyword
            string keyword = txtSearch.Text.Trim().ToLower();
            if (!string.IsNullOrEmpty(keyword))
            {
                filteredCars = filteredCars.Where(c =>
                    c.Car.CarName.ToLower().Contains(keyword) ||
                    c.Car.Description.ToLower().Contains(keyword) ||
                    c.Brand.BrandName.ToLower().Contains(keyword)
                ).ToList();
            }

            // Filter by selected brands
            var selectedBrands = cblBrands.Items.Cast<ListItem>()
                .Where(i => i.Selected).Select(i => Convert.ToInt32(i.Value)).ToList();

            if (selectedBrands.Any())
            {
                filteredCars = filteredCars.Where(c =>
                    selectedBrands.Any(brandId => c.Brand.BrandName == cblBrands.Items
                        .Cast<ListItem>().First(x => x.Value == brandId.ToString()).Text)
                ).ToList();
            }

            // Filter by location
            if (ddlLocation.SelectedValue != "0")
            {
                string selectedLocationName = ddlLocation.SelectedItem.Text;
                filteredCars = filteredCars.Where(c =>
                    c.Location.LocationName.Contains(selectedLocationName)
                ).ToList();
            }

            // Filter by year
            if (ddlYear.SelectedValue != "0")
            {
                int selectedYear = Convert.ToInt32(ddlYear.SelectedValue);
                filteredCars = filteredCars.Where(c => c.Car.Year == selectedYear).ToList();
            }

            // Filter by price range
            if (ddlPriceRange.SelectedValue != "0")
            {
                filteredCars = filteredCars.Where(c => CheckPriceInRange(c.Car.Price, ddlPriceRange.SelectedValue)).ToList();
            }

            return filteredCars;
        }

        private bool CheckPriceInRange(decimal price, string range)
        {
            // Giá được lưu theo triệu VND (ví dụ: 500 = 500 triệu)
            switch (range)
            {
                case "1": return price < 300; // Dưới 300 triệu
                case "2": return price >= 300 && price <= 500; // 300-500 triệu
                case "3": return price > 500 && price <= 800; // 500-800 triệu
                case "4": return price > 800 && price <= 1200; // 800-1200 triệu
                case "5": return price > 1200; // Trên 1200 triệu
                default: return true;
            }
        }

        private void DisplayCars(List<CarDetailViewModel> cars, int currentPage = 1)
        {
            try
            {
                // Validate input
                if (cars == null)
                {
                    cars = new List<CarDetailViewModel>();
                }

                // Filter out null items and validate data
                cars = cars.Where(c => c?.Car != null).ToList();

                // Log data for debugging
                LogMessage($"DisplayCars: Preparing to display {cars.Count} cars on page {currentPage}");
                foreach (var car in cars.Take(3)) // Log first 3 cars for debugging
                {
                    LogMessage($"Car: {car.Car.CarName}, Brand: {car.Brand?.BrandName}, Location: {car.Location?.LocationName}");
                }

                // Pagination
                var pagedCars = cars.Skip((currentPage - 1) * PAGE_SIZE).Take(PAGE_SIZE).ToList();

                // Bind data
                rptCars.DataSource = pagedCars;
                rptCars.DataBind();

                // Update results count
                lblTotalResults.Text = $"Tổng: {cars.Count} tin";

                // Show/hide no results panel
                pnlNoResults.Visible = !cars.Any();

                // Create pagination
                if (cars.Count > PAGE_SIZE)
                {
                    CreatePagination(cars.Count, currentPage, PAGE_SIZE);
                }
                else
                {
                    rptPagination.DataSource = null;
                    rptPagination.DataBind();
                }

                LogMessage($"DisplayCars: Successfully displayed {pagedCars.Count} cars on page {currentPage}");
            }
            catch (Exception ex)
            {
                LogError("DisplayCars", ex);
                ShowMessage("Có lỗi khi hiển thị dữ liệu.", "error");

                // Show empty state
                rptCars.DataSource = new List<CarDetailViewModel>();
                rptCars.DataBind();
                pnlNoResults.Visible = true;
                lblTotalResults.Text = "Tổng: 0 tin";
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

            var pages = new List<PaginationItem>();

            // Previous button
            if (currentPage > 1)
            {
                pages.Add(new PaginationItem { PageNumber = currentPage - 1, DisplayText = "‹ Trước", IsCurrent = false });
            }

            // First page
            if (currentPage > 3)
            {
                pages.Add(new PaginationItem { PageNumber = 1, DisplayText = "1", IsCurrent = false });
                if (currentPage > 4)
                {
                    pages.Add(new PaginationItem { PageNumber = -1, DisplayText = "...", IsCurrent = false });
                }
            }

            // Page numbers around current page
            int startPage = Math.Max(1, currentPage - 2);
            int endPage = Math.Min(totalPages, currentPage + 2);

            for (int i = startPage; i <= endPage; i++)
            {
                pages.Add(new PaginationItem
                {
                    PageNumber = i,
                    DisplayText = i.ToString(),
                    IsCurrent = (i == currentPage)
                });
            }

            // Last page
            if (currentPage < totalPages - 2)
            {
                if (currentPage < totalPages - 3)
                {
                    pages.Add(new PaginationItem { PageNumber = -1, DisplayText = "...", IsCurrent = false });
                }
                pages.Add(new PaginationItem { PageNumber = totalPages, DisplayText = totalPages.ToString(), IsCurrent = false });
            }

            // Next button
            if (currentPage < totalPages)
            {
                pages.Add(new PaginationItem { PageNumber = currentPage + 1, DisplayText = "Sau ›", IsCurrent = false });
            }

            rptPagination.DataSource = pages;
            rptPagination.DataBind();
        }

        private void UpdateFilterDisplay()
        {
            var filterParts = new List<string>();

            // Search keyword
            if (!string.IsNullOrEmpty(txtSearch.Text.Trim()))
            {
                filterParts.Add($"Từ khóa: '{txtSearch.Text.Trim()}'");
            }

            // Selected brands
            var selectedBrands = cblBrands.Items.Cast<ListItem>()
                .Where(i => i.Selected).Select(i => i.Text).ToList();
            if (selectedBrands.Any())
            {
                filterParts.Add($"Hãng: {string.Join(", ", selectedBrands)}");
            }

            // Location
            if (ddlLocation.SelectedValue != "0")
            {
                filterParts.Add($"Khu vực: {ddlLocation.SelectedItem.Text}");
            }

            // Year
            if (ddlYear.SelectedValue != "0")
            {
                filterParts.Add($"Năm: {ddlYear.SelectedItem.Text}");
            }

            // Price range
            if (ddlPriceRange.SelectedValue != "0")
            {
                filterParts.Add($"Giá: {ddlPriceRange.SelectedItem.Text}");
            }

            lblCurrentFilter.Text = filterParts.Any() ?
                $"Bộ lọc: {string.Join(" | ", filterParts)}" : "";
        }

        private void ClearAllFilters()
        {
            txtSearch.Text = "";

            // Clear brand selection
            foreach (ListItem item in cblBrands.Items)
            {
                item.Selected = false;
            }

            ddlLocation.SelectedValue = "0";
            ddlYear.SelectedValue = "0";
            ddlPriceRange.SelectedValue = "0";

            lblCurrentFilter.Text = "";

            LogMessage("All filters cleared");
        }

        #endregion

        #region Data Binding Methods

        private void BindCarData(RepeaterItem item, CarDetailViewModel carViewModel)
        {
            try
            {
                // Bind car image
                var imgCar = (Image)item.FindControl("imgCar");
                if (imgCar != null)
                {
                    string imagePath = carViewModel.Images?.FirstOrDefault()?.ImagePath ?? "";
                    imgCar.ImageUrl = GetImageUrl(imagePath);
                    imgCar.AlternateText = carViewModel.Car.CarName ?? "Car Image";
                }

                // Bind car link
                var lnkCarName = (HyperLink)item.FindControl("lnkCarName");
                if (lnkCarName != null)
                {
                    lnkCarName.NavigateUrl = $"~/chitietsanpham.aspx?id={carViewModel.Car.CarID}";
                    lnkCarName.Text = carViewModel.Car.CarName ?? "Unknown Car";
                }

                // Bind price
                var litPrice = item.FindControl("litPrice") as Literal;
                if (litPrice != null)
                {
                    litPrice.Text = FormatPrice(carViewModel.Car.Price);
                }

                // Bind year
                var litYear = item.FindControl("litYear") as Literal;
                if (litYear != null)
                {
                    litYear.Text = carViewModel.Car.Year?.ToString() ?? "N/A";
                }

                // Bind brand
                var litBrand = item.FindControl("litBrand") as Literal;
                if (litBrand != null)
                {
                    litBrand.Text = carViewModel.Brand?.BrandName ?? "Không xác định";
                }

                // Bind location
                var litLocation = item.FindControl("litLocation") as Literal;
                if (litLocation != null)
                {
                    litLocation.Text = carViewModel.Location?.LocationName ?? "Chưa xác định";
                }

                // Bind description
                var litDescription = item.FindControl("litDescription") as Literal;
                if (litDescription != null)
                {
                    litDescription.Text = TruncateText(carViewModel.Car.Description, 100);
                }

                // Bind created date
                var litCreatedDate = item.FindControl("litCreatedDate") as Literal;
                if (litCreatedDate != null)
                {
                    litCreatedDate.Text = carViewModel.Car.CreatedDate.ToString("dd/MM/yyyy");
                }

                // Bind availability
                var litAvailability = item.FindControl("litAvailability") as Literal;
                if (litAvailability != null)
                {
                    litAvailability.Text = carViewModel.Car.IsAvailable ? "✅ Còn hàng" : "❌ Đã bán";
                }

                LogMessage($"Bound data for car: {carViewModel.Car.CarName} (ID: {carViewModel.Car.CarID})");
            }
            catch (Exception ex)
            {
                LogError("BindCarData", ex);

                // Set default values in case of error
                SetDefaultValues(item);
            }
        }


        private void SetDefaultValues(RepeaterItem item)
        {
            try
            {
                var imgCar = (Image)item.FindControl("imgCar");
                if (imgCar != null)
                {
                    imgCar.ImageUrl = GetImageUrl("");
                    imgCar.AlternateText = "No Image";
                }

                var lnkCarName = (HyperLink)item.FindControl("lnkCarName");
                if (lnkCarName != null)
                {
                    lnkCarName.Text = "Không có thông tin";
                    lnkCarName.NavigateUrl = "#";
                }

                var litPrice = item.FindControl("litPrice") as Literal;
                if (litPrice != null) litPrice.Text = "Liên hệ";

                var litYear = item.FindControl("litYear") as Literal;
                if (litYear != null) litYear.Text = "N/A";

                var litBrand = item.FindControl("litBrand") as Literal;
                if (litBrand != null) litBrand.Text = "Không xác định";

                var litLocation = item.FindControl("litLocation") as Literal;
                if (litLocation != null) litLocation.Text = "Chưa xác định";

                var litDescription = item.FindControl("litDescription") as Literal;
                if (litDescription != null) litDescription.Text = "Không có mô tả";

                var litCreatedDate = item.FindControl("litCreatedDate") as Literal;
                if (litCreatedDate != null) litCreatedDate.Text = DateTime.Now.ToString("dd/MM/yyyy");

                var litAvailability = item.FindControl("litAvailability") as Literal;
                if (litAvailability != null) litAvailability.Text = "❓ Không rõ";
            }
            catch (Exception ex)
            {
                LogError("SetDefaultValues", ex);
            }
        }

        #endregion

        #region Utility Methods

        protected string GetImageUrl(object imagePath)
        {
            try
            {
                string path = imagePath?.ToString()?.Trim() ?? "";

                if (string.IsNullOrEmpty(path))
                    return ResolveUrl("~/Images/no-image.png");

                // If already a full URL
                if (path.StartsWith("http://", StringComparison.OrdinalIgnoreCase) ||
                    path.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                    return path;

                // If relative path
                if (!path.StartsWith("~/") && !path.StartsWith("/"))
                    return ResolveUrl("~/Images/Cars/" + path);

                return ResolveUrl(path);
            }
            catch (Exception ex)
            {
                LogError("GetImageUrl", ex);
                return ResolveUrl("~/Images/no-image.png");
            }
        }

        protected string FormatPrice(object price)
        {
            if (price == null) return "Liên hệ";

            if (decimal.TryParse(price.ToString(), out decimal priceValue))
            {
                if (priceValue >= 1000)
                {
                    return $"{priceValue:N0} triệu";
                }
                else
                {
                    return $"{priceValue:N0} triệu";
                }
            }

            return price.ToString();
        }

        protected string TruncateText(object text, int maxLength)
        {
            string str = text?.ToString() ?? "";
            if (str.Length <= maxLength) return str;

            return str.Substring(0, maxLength) + "...";
        }

        private void ShowMessage(string message, string type = "info")
        {
            lblMessage.Text = message;
            pnlMessage.Visible = true;

            // Set CSS class based on type
            switch (type.ToLower())
            {
                case "success":
                    divMessage.Attributes["class"] = "alert alert-success";
                    break;
                case "warning":
                    divMessage.Attributes["class"] = "alert alert-warning";
                    break;
                case "error":
                    divMessage.Attributes["class"] = "alert alert-error";
                    break;
                default:
                    divMessage.Attributes["class"] = "alert";
                    break;
            }

            // Auto-hide after delay using client script
            string script = $"setTimeout(function(){{ document.getElementById('{pnlMessage.ClientID}').style.display='none'; }}, 5000);";
            ClientScript.RegisterStartupScript(this.GetType(), "HideMessage", script, true);
        }

        private void LogMessage(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] {message}");
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Trace.Write("CarListingDebug", message);
            }
        }

        private void LogError(string method, Exception ex)
        {
            string errorMessage = $"ERROR in {method}: {ex.Message}";
            if (ex.InnerException != null)
            {
                errorMessage += $" | Inner: {ex.InnerException.Message}";
            }

            LogMessage(errorMessage);

            // Log to Event Log in production
            try
            {
                System.Diagnostics.EventLog.WriteEntry("CarWebsite", errorMessage,
                    System.Diagnostics.EventLogEntryType.Error);
            }
            catch
            {
                // Ignore logging errors
            }
        }

        #endregion

        #region Fallback Data Methods

        private void LoadDefaultBrands()
        {
            cblBrands.Items.Clear();
            string[] defaultBrands = { "Audi", "BMW", "Chevrolet", "Ford", "Honda", "Hyundai", "Kia", "Toyota", "VinFast" };

            for (int i = 0; i < defaultBrands.Length; i++)
            {
                cblBrands.Items.Add(new ListItem(defaultBrands[i], (i + 1).ToString()));
            }

            LogMessage("Loaded default brands as fallback");
        }

        private void LoadDefaultLocations()
        {
            ddlLocation.Items.Clear();
            ddlLocation.Items.Add(new ListItem("Toàn quốc", "0"));

            string[] provinces = {
                "Hà Nội", "TP HCM", "Đà Nẵng", "Hải Phòng", "Cần Thơ",
                "An Giang", "Bà Rịa - Vũng Tàu", "Bắc Giang", "Bắc Ninh", "Bình Dương"
            };

            for (int i = 0; i < provinces.Length; i++)
            {
                ddlLocation.Items.Add(new ListItem(provinces[i], (i + 1).ToString()));
            }

            LogMessage("Loaded default locations as fallback");
        }

        #endregion

        #region Helper Classes

        public class PaginationItem
        {
            public int PageNumber { get; set; }
            public string DisplayText { get; set; } = "";
            public bool IsCurrent { get; set; } = false;
        }

        #endregion
    }
}