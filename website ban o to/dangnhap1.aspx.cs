using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using website_ban_o_to.Models; // Import User model

namespace website_ban_o_to
{
    public partial class dangnhap1 : System.Web.UI.Page
    {
        #region Properties and Constants

        private string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString;
        private const string SESSION_USER = "LoggedInUser";
        private const int MAX_LOGIN_ATTEMPTS = 5;
        private const int LOCKOUT_MINUTES = 30;

        #endregion

        #region Page Events

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    // Check if already logged in
                    if (IsUserLoggedIn())
                    {
                        RedirectToUserHomePage();
                        return;
                    }

                    // Initialize page
                    InitializePage();
                }
            }
            catch (Exception ex)
            {
                LogError("Page_Load", ex);
                ShowError("Có lỗi xảy ra khi tải trang.");
            }
        }

        private void InitializePage()
        {
            // Hide error message initially
            if (lblError != null)
                lblError.Visible = false;

            // Clear any existing form data
            if (txtUsername != null) txtUsername.Text = "";
            if (txtPassword != null) txtPassword.Text = "";

            // Focus on username field
            if (txtUsername != null)
                txtUsername.Focus();

            LogMessage("Login page initialized");
        }

        #endregion

        #region Login Process

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                LogMessage("Login attempt started");

                // Validate connection string
                if (string.IsNullOrEmpty(connectionString))
                {
                    ShowError("Lỗi cấu hình hệ thống. Vui lòng liên hệ quản trị viên!");
                    LogError("btnLogin_Click", new Exception("Connection string is null or empty"));
                    return;
                }

                // Get and validate input
                string username = txtUsername?.Text?.Trim() ?? "";
                string password = txtPassword?.Text?.Trim() ?? "";

                if (!ValidateInput(username, password))
                    return;

                // Check if account is locked
                if (IsAccountLocked(username))
                {
                    ShowError($"Tài khoản đã bị khóa do đăng nhập sai quá nhiều lần. Vui lòng thử lại sau {LOCKOUT_MINUTES} phút.");
                    return;
                }

                // Attempt login
                User user = AuthenticateUser(username, password);

                if (user != null)
                {
                    ProcessSuccessfulLogin(user);
                }
                else
                {
                    ProcessFailedLogin(username);
                }
            }
            catch (SqlException sqlEx)
            {
                LogError("btnLogin_Click - SQL", sqlEx);
                ShowError("Lỗi kết nối cơ sở dữ liệu. Vui lòng thử lại sau!");
            }
            catch (Exception ex)
            {
                LogError("btnLogin_Click - General", ex);
                ShowError("Có lỗi xảy ra. Vui lòng thử lại!");
            }
        }

        private bool ValidateInput(string username, string password)
        {
            if (string.IsNullOrEmpty(username))
            {
                ShowError("Vui lòng nhập tên đăng nhập!");
                txtUsername?.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(password))
            {
                ShowError("Vui lòng nhập mật khẩu!");
                txtPassword?.Focus();
                return false;
            }

            if (username.Length < 3)
            {
                ShowError("Tên đăng nhập phải có ít nhất 3 ký tự!");
                txtUsername?.Focus();
                return false;
            }

            if (password.Length < 6)
            {
                ShowError("Mật khẩu phải có ít nhất 6 ký tự!");
                txtPassword?.Focus();
                return false;
            }

            return true;
        }

        private User AuthenticateUser(string username, string password)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT UserID, Username, Password, FullName, Email, Phone, Role, IsAdmin, IsActive, CreatedDate, UpdatedDate
                        FROM Users 
                        WHERE Username = @Username AND IsActive = 1";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@Username", username);

                    conn.Open();
                    LogMessage($"Database connection opened for user authentication: {username}");

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows && reader.Read())
                        {
                            string storedPassword = reader["Password"].ToString();

                            // In production, use proper password hashing (BCrypt, etc.)
                            // For now, simple comparison
                            if (VerifyPassword(password, storedPassword))
                            {
                                User user = MapUserFromReader(reader);
                                LogMessage($"Authentication successful for user: {username}");
                                return user;
                            }
                            else
                            {
                                LogMessage($"Password verification failed for user: {username}");
                            }
                        }
                        else
                        {
                            LogMessage($"User not found or inactive: {username}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("AuthenticateUser", ex);
                throw;
            }

            return null;
        }

        private User MapUserFromReader(SqlDataReader reader)
        {
            return new User
            {
                UserID = Convert.ToInt32(reader["UserID"]),
                Username = reader["Username"].ToString(),
                Password = reader["Password"].ToString(), // Don't store in session
                FullName = reader["FullName"].ToString(),
                Email = reader["Email"].ToString(),
                Phone = reader["Phone"]?.ToString(),
                Role = reader["Role"].ToString(),
                IsAdmin = Convert.ToBoolean(reader["IsAdmin"]),
                IsActive = Convert.ToBoolean(reader["IsActive"]),
                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                UpdatedDate = reader["UpdatedDate"] != DBNull.Value ? Convert.ToDateTime(reader["UpdatedDate"]) : (DateTime?)null
            };
        }

        private bool VerifyPassword(string inputPassword, string storedPassword)
        {
            // Simple comparison for now
            // TODO: Implement proper password hashing (BCrypt, Argon2, etc.)
            return inputPassword == storedPassword;
        }

        private void ProcessSuccessfulLogin(User user)
        {
            try
            {
                // Clear failed login attempts
                ClearFailedLoginAttempts(user.Username);

                // ============= CẬP NHẬT SESSION CHO MENU =============
                // Store user information in session for menu control
                Session["UserID"] = user.UserID;
                Session["Username"] = user.Username;
                Session["FullName"] = user.FullName;
                Session["Email"] = user.Email;
                Session["Role"] = user.Role;
                Session["isAdmin"] = user.IsAdmin;

                // Store complete user object
                var sessionUser = new User
                {
                    UserID = user.UserID,
                    Username = user.Username,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role,
                    IsAdmin = user.IsAdmin,
                    IsActive = user.IsActive,
                    CreatedDate = user.CreatedDate,
                    UpdatedDate = user.UpdatedDate
                };

                Session[SESSION_USER] = sessionUser;

            
                // Store session tracking for single login enforcement
                Application[$"User_{user.UserID}_SessionID"] = Session.SessionID;

                // Update last login time
                UpdateLastLoginTime(user.UserID);

                LogMessage($"User logged in successfully: {user.Username} (Role: {user.Role}, IsAdmin: {user.IsAdmin})");

                // ============= HIỂN THỊ THÔNG BÁO THÀNH CÔNG =============
                ShowSuccess($"Đăng nhập thành công! Chào mừng {user.FullName}");

                // Delay redirect để user thấy thông báo
                string script = $@"
                    setTimeout(function() {{
                        window.location.href = '{GetRedirectUrl(user)}';
                    }}, 1500);";

                ScriptManager.RegisterStartupScript(this, GetType(), "LoginSuccess", script, true);
            }
            catch (Exception ex)
            {
                LogError("ProcessSuccessfulLogin", ex);
                ShowError("Đăng nhập thành công nhưng có lỗi trong quá trình xử lý. Vui lòng thử lại!");
            }
        }

        private void CreateRememberMeCookie(User user)
        {
            try
            {
                HttpCookie userCookie = new HttpCookie("UserLogin");
                userCookie["UserID"] = user.UserID.ToString();
                userCookie["Username"] = user.Username;
                userCookie["FullName"] = user.FullName;
                userCookie["IsAdmin"] = user.IsAdmin.ToString();
                userCookie.Expires = DateTime.Now.AddDays(30); // Remember for 30 days
                userCookie.HttpOnly = true; // Security: prevent XSS access
                userCookie.Secure = Request.IsSecureConnection; // HTTPS only if available

                Response.Cookies.Add(userCookie);
                LogMessage($"Remember me cookie created for user: {user.Username}");
            }
            catch (Exception ex)
            {
                LogError("CreateRememberMeCookie", ex);
                // Don't fail login if cookie creation fails
            }
        }

        private string GetRedirectUrl(User user)
        {
            string redirectUrl = "~/trangchu1.aspx"; // Default

            if (user.IsAdmin || user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                redirectUrl = "~/admin/quanly1.aspx";
            }

            // Check if there's a return URL
            string returnUrl = Request.QueryString["ReturnUrl"];
            if (!string.IsNullOrEmpty(returnUrl) && IsValidReturnUrl(returnUrl))
            {
                redirectUrl = returnUrl;
            }

            return ResolveUrl(redirectUrl);
        }

        private void ProcessFailedLogin(string username)
        {
            try
            {
                // Increment failed login attempts
                IncrementFailedLoginAttempts(username);

                // Get current attempts count
                int attempts = GetFailedLoginAttempts(username);
                int remainingAttempts = MAX_LOGIN_ATTEMPTS - attempts;

                LogMessage($"Login failed for user: {username}. Attempts: {attempts}");

                if (remainingAttempts > 0)
                {
                    ShowError($"Tên đăng nhập hoặc mật khẩu không đúng! Còn {remainingAttempts} lần thử.");
                }
                else
                {
                    // Lock account
                    LockAccount(username);
                    ShowError($"Tài khoản đã bị khóa do đăng nhập sai quá {MAX_LOGIN_ATTEMPTS} lần. Vui lòng thử lại sau {LOCKOUT_MINUTES} phút.");
                }

                // Clear password field for security
                if (txtPassword != null)
                    txtPassword.Text = "";
            }
            catch (Exception ex)
            {
                LogError("ProcessFailedLogin", ex);
                ShowError("Tên đăng nhập hoặc mật khẩu không đúng!");
            }
        }

        private bool IsValidReturnUrl(string returnUrl)
        {
            // Basic validation to prevent open redirect attacks
            return returnUrl.StartsWith("~/") || returnUrl.StartsWith("/");
        }

        #endregion

        #region Account Security Methods

        private bool IsAccountLocked(string username)
        {
            try
            {
                object lockTime = Session[$"AccountLocked_{username}"];
                if (lockTime != null)
                {
                    DateTime lockedAt = (DateTime)lockTime;
                    if (DateTime.Now.Subtract(lockedAt).TotalMinutes < LOCKOUT_MINUTES)
                    {
                        return true;
                    }
                    else
                    {
                        // Unlock account
                        Session.Remove($"AccountLocked_{username}");
                        ClearFailedLoginAttempts(username);
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                LogError("IsAccountLocked", ex);
                return false;
            }
        }

        private void LockAccount(string username)
        {
            Session[$"AccountLocked_{username}"] = DateTime.Now;
            LogMessage($"Account locked: {username}");
        }

        private int GetFailedLoginAttempts(string username)
        {
            object attempts = Session[$"FailedAttempts_{username}"];
            return attempts != null ? (int)attempts : 0;
        }

        private void IncrementFailedLoginAttempts(string username)
        {
            int currentAttempts = GetFailedLoginAttempts(username);
            Session[$"FailedAttempts_{username}"] = currentAttempts + 1;
        }

        private void ClearFailedLoginAttempts(string username)
        {
            Session.Remove($"FailedAttempts_{username}");
            Session.Remove($"AccountLocked_{username}");
        }

        private void UpdateLastLoginTime(int userId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Users SET UpdatedDate = @UpdatedDate WHERE UserID = @UserID";
                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LogError("UpdateLastLoginTime", ex);
                // Don't throw - this is not critical
            }
        }

        #endregion

        #region Helper Methods

        private void ShowError(string message)
        {
            try
            {
                if (lblError != null)
                {
                    lblError.Text = message;
                    lblError.Visible = true;
                    lblError.CssClass = "alert alert-danger";
                }

                LogMessage($"Error shown to user: {message}");
            }
            catch (Exception ex)
            {
                LogError("ShowError", ex);
            }
        }

        private void ShowSuccess(string message)
        {
            try
            {
                if (lblError != null)
                {
                    lblError.Text = message;
                    lblError.Visible = true;
                    lblError.CssClass = "alert alert-success";
                }

                LogMessage($"Success message shown to user: {message}");
            }
            catch (Exception ex)
            {
                LogError("ShowSuccess", ex);
            }
        }

        protected bool IsUserLoggedIn()
        {
            // Check session first
            if (Session["UserID"] != null)
                return true;

            // Check cookie if session is empty
            if (Request.Cookies["UserLogin"] != null)
            {
                var userCookie = Request.Cookies["UserLogin"];
                if (!string.IsNullOrEmpty(userCookie.Value))
                {
                    // Restore session from cookie
                    RestoreSessionFromCookie(userCookie);
                    return true;
                }
            }

            return false;
        }

        private void RestoreSessionFromCookie(HttpCookie userCookie)
        {
            try
            {
                Session["UserID"] = userCookie["UserID"];
                Session["Username"] = userCookie["Username"];
                Session["FullName"] = userCookie["FullName"];
                Session["isAdmin"] = bool.Parse(userCookie["IsAdmin"] ?? "false");

                LogMessage($"Session restored from cookie for user: {userCookie["Username"]}");
            }
            catch (Exception ex)
            {
                LogError("RestoreSessionFromCookie", ex);
                // Clear invalid cookie
                ClearLoginCookie();
            }
        }

        protected User GetCurrentUser()
        {
            return Session[SESSION_USER] as User;
        }

        private void RedirectToUserHomePage()
        {
            User currentUser = GetCurrentUser();
            if (currentUser != null)
            {
                if (currentUser.IsAdmin)
                {
                    Response.Redirect("~/admin/quanly1.aspx", false);
                }
                else
                {
                    Response.Redirect("~/trangchu1.aspx", false);
                }
            }
            else if (Session["UserID"] != null)
            {
                // Basic redirect if only basic session info available
                bool isAdmin = Session["isAdmin"] != null && (bool)Session["isAdmin"];
                if (isAdmin)
                {
                    Response.Redirect("~/admin/quanly1.aspx", false);
                }
                else
                {
                    Response.Redirect("~/trangchu1.aspx", false);
                }
            }
        }

        protected void Logout()
        {
            try
            {
                User currentUser = GetCurrentUser();
                if (currentUser != null)
                {
                    // Remove session tracking
                    Application.Remove($"User_{currentUser.UserID}_SessionID");
                    LogMessage($"User logged out: {currentUser.Username}");
                }

                // Clear session
                Session.Clear();
                Session.Abandon();

                // Clear remember me cookies
                ClearLoginCookie();

                Response.Redirect("~/dangnhap1.aspx?logout=success", false);
            }
            catch (Exception ex)
            {
                LogError("Logout", ex);
            }
        }

        private void ClearLoginCookie()
        {
            HttpCookie userCookie = new HttpCookie("UserLogin");
            userCookie.Expires = DateTime.Now.AddDays(-1);
            Response.Cookies.Add(userCookie);
        }

        private void LogMessage(string message)
        {
            System.Diagnostics.Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] LOGIN: {message}");
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Trace.Write("LoginDebug", message);
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

        #region Page Security

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            // Add security headers
            Response.Headers.Add("X-Frame-Options", "DENY");
            Response.Headers.Add("X-Content-Type-Options", "nosniff");
            Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        }

        #endregion
    }
}