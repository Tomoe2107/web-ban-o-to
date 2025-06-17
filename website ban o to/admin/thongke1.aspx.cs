using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace website_ban_o_to.admin
{
    public partial class thongke1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (!User.Identity.IsAuthenticated || !User.IsInRole("Admin"))
                {
                    //Response.Redirect("~/dangnhap.aspx");
                }
                HienThiTheoThang();
            }
        }

        protected void ddlLoaiThongKe_SelectedIndexChanged(object sender, EventArgs e)
        {
            divThang.Style["display"] = "none";
            divQuy.Style["display"] = "none";
            divNam.Style["display"] = "none";

            switch (ddlLoaiThongKe.SelectedValue)
            {
                case "0": divThang.Style["display"] = "block"; break;
                case "1": divQuy.Style["display"] = "block"; break;
                case "2": divNam.Style["display"] = "block"; break;
            }
        }

        protected void btnXemThongKe_Click(object sender, EventArgs e)
        {
            switch (ddlLoaiThongKe.SelectedValue)
            {
                case "0": HienThiTheoThang(); break;
                case "1": HienThiTheoQuy(); break;
                case "2": HienThiTheoNam(); break;
            }
        }

        private void HienThiTheoThang()
        {
            int thang = int.Parse(ddlThang.SelectedValue);
            int nam = int.Parse(ddlNam.SelectedValue);

            DataTable dt = LayDuLieuDoanhThu(thang, nam);
            //chartDoanhThu.Series["DoanhThu"].Points.Clear();

            decimal tongDoanhThu = 0;
            decimal caoNhat = 0, thapNhat = decimal.MaxValue;

            foreach (DataRow row in dt.Rows)
            {
                string ngay = Convert.ToDateTime(row["NgayDat"]).ToString("dd/MM");

                decimal doanhThu = Convert.ToDecimal(row["TongTien"]);

              //  chartDoanhThu.Series["DoanhThu"].Points.AddXY(ngay, doanhThu);

                tongDoanhThu += doanhThu;
                caoNhat = Math.Max(caoNhat, doanhThu);
                thapNhat = Math.Min(thapNhat, doanhThu);
            }

            lblTongDoanhThu.Text = tongDoanhThu.ToString("N0") + " triệu";
            lblDonHangCaoNhat.Text = caoNhat.ToString("N0") + " triệu";
            lblDonHangThapNhat.Text = (dt.Rows.Count == 0 ? "0" : thapNhat.ToString("N0")) + " triệu";
            lblTongDonHang.Text = dt.Rows.Count.ToString();

            gvChiTietDonHang.DataSource = dt;
            gvChiTietDonHang.DataBind();

            gvSanPhamBanChay.DataSource = LayXeBanChay(thang, nam);
            gvSanPhamBanChay.DataBind();
        }

        private void HienThiTheoQuy()
        {
            int quy = int.Parse(ddlQuy.SelectedValue);
            int nam = int.Parse(ddlNamQuy.SelectedValue);
            // Có thể làm tương tự như theo tháng, gộp 3 tháng trong quý
        }

        private void HienThiTheoNam()
        {
            int nam = int.Parse(ddlNamThongKe.SelectedValue);
            // Tính tổng theo năm, chia theo tháng hoặc sản phẩm tùy ý
        }

        private DataTable LayDuLieuDoanhThu(int thang, int nam)
        {
            // Dữ liệu mẫu - thay bằng truy vấn thực tế từ DB
            DataTable dt = new DataTable();
            dt.Columns.Add("MaDon");
            dt.Columns.Add("NgayDat", typeof(DateTime));
            dt.Columns.Add("KhachHang");
            dt.Columns.Add("TongTien", typeof(decimal));
            dt.Columns.Add("TrangThai");

            dt.Rows.Add("DH01", new DateTime(nam, thang, 2), "Nguyen Van A", 250, "Đã giao");
            dt.Rows.Add("DH02", new DateTime(nam, thang, 5), "Tran Thi B", 300, "Đang xử lý");
            dt.Rows.Add("DH03", new DateTime(nam, thang, 12), "Le Van C", 100, "Đã giao");

            return dt;
        }

        private DataTable LayXeBanChay(int thang, int nam)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TenXe");
            dt.Columns.Add("SoLuong", typeof(int));
            dt.Columns.Add("DoanhThu", typeof(decimal));

            dt.Rows.Add("Toyota Vios", 5, 500);
            dt.Rows.Add("Mazda CX5", 3, 720);
            dt.Rows.Add("VinFast Lux A2.0", 2, 620);

            return dt;
        }

        protected void gvChiTietDonHang_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvChiTietDonHang.PageIndex = e.NewPageIndex;
            HienThiTheoThang();
        }
    }
}