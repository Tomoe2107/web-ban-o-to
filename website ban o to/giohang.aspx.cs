using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace website_ban_o_to
{
    public partial class giohang : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadCart();
        }

        private void LoadCart()
        {
            DataTable cart = Session["GioHang"] as DataTable;

            if (cart == null)
            {
                cart = new DataTable();
                cart.Columns.Add("Id");
                cart.Columns.Add("Name");
                cart.Columns.Add("Price", typeof(decimal));
                cart.Columns.Add("ImageUrl");
            }

            rptCartItems.DataSource = cart;
            rptCartItems.DataBind();

            decimal tongTien = 0;
            foreach (DataRow row in cart.Rows)
            {
                tongTien += Convert.ToDecimal(row["Price"]);
            }
            lblTongTien.Text = tongTien.ToString("N0") + " triệu";
        }

        protected void btnRemove_Command(object sender, CommandEventArgs e)
        {
            DataTable cart = Session["GioHang"] as DataTable;
            if (cart != null)
            {
                DataRow[] rows = cart.Select("Id=" + e.CommandArgument.ToString());
                if (rows.Length > 0)
                    cart.Rows.Remove(rows[0]);
                Session["GioHang"] = cart;
            }
            LoadCart();
        }

        protected void btnThanhToan_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/thanhtoan.aspx");
        }
    }
}