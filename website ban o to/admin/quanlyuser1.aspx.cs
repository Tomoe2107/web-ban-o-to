using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace website_ban_o_to.admin
{
    public partial class quanlyuser1 : System.Web.UI.Page
    {
        private string connectionString = "Data Source=DESKTOP-NMTRDC3\\MSSQLSERVER01;Initial Catalog=BanOto;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Kiểm tra session thay vì User.Identity
                if (Session["IsLoggedIn"] == null || !Convert.ToBoolean(Session["IsLoggedIn"]))
                {
                    Response.Redirect("~/dangnhap1.aspx");
                    return;
                }

                // Kiểm tra quyền admin từ session
                bool isAdmin = Session["IsAdmin"] != null && Convert.ToBoolean(Session["IsAdmin"]);
                string role = Session["Role"]?.ToString();

                if (!isAdmin && role != "Admin")
                {
                    Response.Write("<script>alert('Bạn không có quyền truy cập trang này!'); window.location='~/trangchu1.aspx';</script>");
                    return;
                }

                // Debug log
                System.Diagnostics.Debug.WriteLine($"User access granted - Role: {role}, IsAdmin: {isAdmin}");

                BindGridView();
            }
        }

        private void BindGridView()
        {
            try
            {
                DataTable dtUsers = GetDanhSachUsers();
                gvUsers.DataSource = dtUsers;
                gvUsers.DataBind();
            }
            catch (Exception ex)
            {
                // Log lỗi hoặc hiển thị thông báo
                System.Diagnostics.Debug.WriteLine($"BindGridView Error: {ex.Message}");
                Response.Write($"<script>alert('Lỗi: {ex.Message}');</script>");
            }
        }

        private DataTable GetDanhSachUsers()
        {
            DataTable dt = new DataTable();
            string query = @"SELECT UserID as ID, Username, FullName as HoTen, Email, Role, IsActive 
                           FROM Users 
                           ORDER BY UserID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        private DataTable TimKiemUsers(string keyword)
        {
            DataTable dt = new DataTable();
            string query = @"SELECT UserID as ID, Username, FullName as HoTen, Email, Role, IsActive 
                           FROM Users 
                           WHERE Username LIKE @keyword 
                              OR FullName LIKE @keyword 
                              OR Email LIKE @keyword
                           ORDER BY UserID";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@keyword", "%" + keyword + "%");
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }

        public string GetRoleName(string role)
        {
            switch (role)
            {
                case "Admin": return "Quản trị viên";
                case "User": return "Người dùng";
                case "Staff": return "Nhân viên";
                default: return role;
            }
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string keyword = txtSearch.Text.Trim();
                if (!string.IsNullOrEmpty(keyword))
                {
                    DataTable dtUsers = TimKiemUsers(keyword);
                    gvUsers.DataSource = dtUsers;
                    gvUsers.DataBind();
                }
                else
                {
                    BindGridView();
                }
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert('Lỗi tìm kiếm: {ex.Message}');</script>");
            }
        }

        protected void btnThemUser_Click(object sender, EventArgs e)
        {
            pnlChiTietUser.Visible = true;
            lblTitle.Text = "Thêm người dùng mới";
            ViewState["EditMode"] = false;
            ViewState["EditUserID"] = null;
            ClearForm();
        }

        private void ClearForm()
        {
            txtUsername.Text = "";
            txtPassword.Text = "";
            txtHoTen.Text = "";
            txtEmail.Text = "";
            ddlRole.SelectedValue = "User";
            chkIsActive.Checked = true;
        }

        protected void btnLuu_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateForm())
                {
                    bool isEditMode = ViewState["EditMode"] != null && (bool)ViewState["EditMode"];

                    if (isEditMode)
                    {
                        int userID = Convert.ToInt32(ViewState["EditUserID"]);
                        CapNhatUser(userID);
                        Response.Write("<script>alert('Cập nhật người dùng thành công!');</script>");
                    }
                    else
                    {
                        ThemUser();
                        Response.Write("<script>alert('Thêm người dùng thành công!');</script>");
                    }

                    pnlChiTietUser.Visible = false;
                    BindGridView();
                }
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert('Lỗi: {ex.Message}');</script>");
            }
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrEmpty(txtUsername.Text.Trim()))
            {
                Response.Write("<script>alert('Vui lòng nhập tên đăng nhập!');</script>");
                return false;
            }

            bool isEditMode = ViewState["EditMode"] != null && (bool)ViewState["EditMode"];
            if (!isEditMode && string.IsNullOrEmpty(txtPassword.Text.Trim()))
            {
                Response.Write("<script>alert('Vui lòng nhập mật khẩu!');</script>");
                return false;
            }

            if (string.IsNullOrEmpty(txtHoTen.Text.Trim()))
            {
                Response.Write("<script>alert('Vui lòng nhập họ tên!');</script>");
                return false;
            }

            if (string.IsNullOrEmpty(txtEmail.Text.Trim()))
            {
                Response.Write("<script>alert('Vui lòng nhập email!');</script>");
                return false;
            }

            // Kiểm tra username đã tồn tại (chỉ khi thêm mới)
            if (!isEditMode)
            {
                if (KiemTraUsernameTonTai(txtUsername.Text.Trim()))
                {
                    Response.Write("<script>alert('Tên đăng nhập đã tồn tại!');</script>");
                    return false;
                }
            }

            // Kiểm tra email đã tồn tại (chỉ khi thêm mới)
            if (!isEditMode)
            {
                if (KiemTraEmailTonTai(txtEmail.Text.Trim()))
                {
                    Response.Write("<script>alert('Email đã tồn tại!');</script>");
                    return false;
                }
            }

            return true;
        }

        private bool KiemTraUsernameTonTai(string username)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Username = @username";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private bool KiemTraEmailTonTai(string email)
        {
            string query = "SELECT COUNT(*) FROM Users WHERE Email = @email";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email);
                    conn.Open();
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        private void ThemUser()
        {
            string query = @"INSERT INTO Users (Username, Password, FullName, Email, Role, IsAdmin, IsActive, CreatedDate) 
                           VALUES (@username, @password, @fullname, @email, @role, @isadmin, @isactive, @createddate)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                    cmd.Parameters.AddWithValue("@password", txtPassword.Text.Trim()); // Lưu password plain text
                    cmd.Parameters.AddWithValue("@fullname", txtHoTen.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@role", ddlRole.SelectedValue);
                    cmd.Parameters.AddWithValue("@isadmin", ddlRole.SelectedValue == "Admin" ? 1 : 0);
                    cmd.Parameters.AddWithValue("@isactive", chkIsActive.Checked);
                    cmd.Parameters.AddWithValue("@createddate", DateTime.Now);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void CapNhatUser(int userID)
        {
            string query = @"UPDATE Users 
                           SET Username = @username, FullName = @fullname, Email = @email, 
                               Role = @role, IsAdmin = @isadmin, IsActive = @isactive, UpdatedDate = @updateddate";

            // Chỉ cập nhật password nếu có nhập mới
            if (!string.IsNullOrEmpty(txtPassword.Text.Trim()))
            {
                query += ", Password = @password";
            }

            query += " WHERE UserID = @userid";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userid", userID);
                    cmd.Parameters.AddWithValue("@username", txtUsername.Text.Trim());
                    cmd.Parameters.AddWithValue("@fullname", txtHoTen.Text.Trim());
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text.Trim());
                    cmd.Parameters.AddWithValue("@role", ddlRole.SelectedValue);
                    cmd.Parameters.AddWithValue("@isadmin", ddlRole.SelectedValue == "Admin" ? 1 : 0);
                    cmd.Parameters.AddWithValue("@isactive", chkIsActive.Checked);
                    cmd.Parameters.AddWithValue("@updateddate", DateTime.Now);

                    if (!string.IsNullOrEmpty(txtPassword.Text.Trim()))
                    {
                        cmd.Parameters.AddWithValue("@password", txtPassword.Text.Trim()); // Plain text
                    }

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void btnHuy_Click(object sender, EventArgs e)
        {
            pnlChiTietUser.Visible = false;
        }

        // Các sự kiện GridView
        protected void gvUsers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvUsers.PageIndex = e.NewPageIndex;
            BindGridView();
        }

        protected void gvUsers_RowEditing(object sender, GridViewEditEventArgs e)
        {
            gvUsers.EditIndex = e.NewEditIndex;
            BindGridView();
        }

        protected void gvUsers_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(gvUsers.DataKeys[e.RowIndex].Value);

                // Lấy giá trị từ các control trong EditItemTemplate
                TextBox txtUsernameEdit = (TextBox)gvUsers.Rows[e.RowIndex].FindControl("txtUsername");
                TextBox txtHoTenEdit = (TextBox)gvUsers.Rows[e.RowIndex].FindControl("txtHoTen");
                TextBox txtEmailEdit = (TextBox)gvUsers.Rows[e.RowIndex].FindControl("txtEmail");
                DropDownList ddlRoleEdit = (DropDownList)gvUsers.Rows[e.RowIndex].FindControl("ddlRole");
                CheckBox chkIsActiveEdit = (CheckBox)gvUsers.Rows[e.RowIndex].Cells[5].Controls[0];

                // Cập nhật vào database
                CapNhatUserTrongGrid(id, txtUsernameEdit.Text, txtHoTenEdit.Text, txtEmailEdit.Text,
                                   ddlRoleEdit.SelectedValue, chkIsActiveEdit.Checked);

                gvUsers.EditIndex = -1;
                BindGridView();
                Response.Write("<script>alert('Cập nhật thành công!');</script>");
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert('Lỗi cập nhật: {ex.Message}');</script>");
            }
        }

        private void CapNhatUserTrongGrid(int userID, string username, string fullname, string email, string role, bool isActive)
        {
            string query = @"UPDATE Users 
                           SET Username = @username, FullName = @fullname, Email = @email, 
                               Role = @role, IsAdmin = @isadmin, IsActive = @isactive, UpdatedDate = @updateddate
                           WHERE UserID = @userid";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userid", userID);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@fullname", fullname);
                    cmd.Parameters.AddWithValue("@email", email);
                    cmd.Parameters.AddWithValue("@role", role);
                    cmd.Parameters.AddWithValue("@isadmin", role == "Admin" ? 1 : 0);
                    cmd.Parameters.AddWithValue("@isactive", isActive);
                    cmd.Parameters.AddWithValue("@updateddate", DateTime.Now);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected void gvUsers_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {
            gvUsers.EditIndex = -1;
            BindGridView();
        }

        protected void gvUsers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            try
            {
                int id = Convert.ToInt32(gvUsers.DataKeys[e.RowIndex].Value);
                XoaUser(id);
                BindGridView();
                Response.Write("<script>alert('Xóa người dùng thành công!');</script>");
            }
            catch (Exception ex)
            {
                Response.Write($"<script>alert('Lỗi xóa: {ex.Message}');</script>");
            }
        }

        private void XoaUser(int userID)
        {
            string query = "DELETE FROM Users WHERE UserID = @userid";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userid", userID);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Phương thức để hiển thị form sửa từ GridView (có thể sử dụng thay vì inline edit)
        public void HienThiFormSua(int userID)
        {
            DataTable dtUser = LayThongTinUser(userID);
            if (dtUser.Rows.Count > 0)
            {
                DataRow row = dtUser.Rows[0];
                txtUsername.Text = row["Username"].ToString();
                txtHoTen.Text = row["FullName"].ToString();
                txtEmail.Text = row["Email"].ToString();
                ddlRole.SelectedValue = row["Role"].ToString();
                chkIsActive.Checked = Convert.ToBoolean(row["IsActive"]);
                txtPassword.Text = ""; // Không hiển thị password

                lblTitle.Text = "Cập nhật thông tin người dùng";
                ViewState["EditMode"] = true;
                ViewState["EditUserID"] = userID;
                pnlChiTietUser.Visible = true;
            }
        }

        private DataTable LayThongTinUser(int userID)
        {
            DataTable dt = new DataTable();
            string query = "SELECT * FROM Users WHERE UserID = @userid";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@userid", userID);
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                    adapter.Fill(dt);
                }
            }
            return dt;
        }
    }
}