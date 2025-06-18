using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using website_ban_o_to.Models;

namespace website_ban_o_to
{
    public partial class giohang : System.Web.UI.Page
    {
        private string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Kiểm tra người dùng đã đăng nhập chưa
                if (Session["UserID"] == null)
                {
                    Response.Redirect("~/dangnhap1.aspx");
                    return;
                }
                LoadUserPosts();
            }
        }

        private void LoadUserPosts()
        {
            int userID = Convert.ToInt32(Session["UserID"]);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = @"
                        SELECT PostID, CarName, ExpectedPrice, Description, 
                               ContactName, ContactEmail, ContactPhone, 
                               ImagePath, IsApproved, IsActive, 
                               CreatedDate, ApprovedDate
                        FROM UsedCarPosts 
                        WHERE UserID = @UserID AND IsActive = 1
                        ORDER BY CreatedDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userID);

                        conn.Open();
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        rptUserPosts.DataSource = dt;
                        rptUserPosts.DataBind();

                        // Hiển thị thống kê
                        LoadStatistics(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error hoặc hiển thị thông báo lỗi
                lblMessage.Text = "Có lỗi xảy ra khi tải dữ liệu: " + ex.Message;
                lblMessage.CssClass = "error-message";
            }
        }

        private void LoadStatistics(DataTable dt)
        {
            int totalPosts = dt.Rows.Count;
            int approvedPosts = dt.AsEnumerable().Count(row => Convert.ToBoolean(row["IsApproved"]));
            int pendingPosts = totalPosts - approvedPosts;

            lblTotalPosts.Text = totalPosts.ToString();
            lblApprovedPosts.Text = approvedPosts.ToString();
            lblPendingPosts.Text = pendingPosts.ToString();
        }

     

        protected void btnDelete_Command(object sender, CommandEventArgs e)
        {
            int postID = Convert.ToInt32(e.CommandArgument);

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    string query = "UPDATE UsedCarPosts SET IsActive = 0 WHERE PostID = @PostID AND UserID = @UserID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@PostID", postID);
                        cmd.Parameters.AddWithValue("@UserID", Convert.ToInt32(Session["UserID"]));

                        conn.Open();
                        int result = cmd.ExecuteNonQuery();

                        if (result > 0)
                        {
                            lblMessage.Text = "Đã xóa bài đăng thành công!";
                            lblMessage.CssClass = "success-message";
                            LoadUserPosts(); // Reload dữ liệu
                        }
                        else
                        {
                            lblMessage.Text = "Không thể xóa bài đăng!";
                            lblMessage.CssClass = "error-message";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lblMessage.Text = "Có lỗi xảy ra khi xóa: " + ex.Message;
                lblMessage.CssClass = "error-message";
            }
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/banoto1.aspx");
        }

        protected string GetStatusText(object isApproved)
        {
            bool approved = Convert.ToBoolean(isApproved);
            return approved ? "Đã duyệt" : "Chờ duyệt";
        }

        protected string GetStatusClass(object isApproved)
        {
            bool approved = Convert.ToBoolean(isApproved);
            return approved ? "status-approved" : "status-pending";
        }

        protected string FormatPrice(object price)
        {
            if (price == null || price == DBNull.Value)
                return "Liên hệ";

            decimal priceValue = Convert.ToDecimal(price);
            return priceValue.ToString("N0") + " triệu";
        }

        protected string FormatDate(object date)
        {
            if (date == null || date == DBNull.Value)
                return "";

            DateTime dateValue = Convert.ToDateTime(date);
            return dateValue.ToString("dd/MM/yyyy HH:mm");
        }
    }
}