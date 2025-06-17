using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace website_ban_o_to
{
    public partial class banoto1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void btnDangTin_Click(object sender, EventArgs e)
        {
            string tenXe = txtTenXe.Text;
            string gia = txtGia.Text;
            string moTa = txtMoTa.Text;
            string lienHe = txtLienHe.Text;

            // (Tùy chọn: xử lý hình ảnh ở đây)

            // Lưu thông tin vào database (tạm thời demo)
            lblThongBao.Text = $"Đăng tin thành công: {tenXe} - {gia} triệu";
            lblThongBao.ForeColor = System.Drawing.Color.Green;

            // Reset form
            txtTenXe.Text = "";
            txtGia.Text = "";
            txtMoTa.Text = "";
            txtLienHe.Text = "";
        }
    }
}