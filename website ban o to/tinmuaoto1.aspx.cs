using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace website_ban_o_to
{
    public partial class tinmuaoto1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                rptNews.DataSource = GetNews();
                rptNews.DataBind();
            }
        }

        private List<News> GetNews()
        {
            return new List<News>
            {
                new News { Id = 1, Title = "VinFast ra mắt mẫu xe điện mới", Summary = "Mẫu xe điện mới nhất của VinFast có thiết kế hiện đại...", ImageUrl = "/images/news1.jpg", Date = DateTime.Now.AddDays(-2) },
                new News { Id = 2, Title = "Honda Civic 2025 trình làng", Summary = "Honda chính thức giới thiệu Civic 2025 với nhiều nâng cấp...", ImageUrl = "/images/news2.jpg", Date = DateTime.Now.AddDays(-5) }
            };
        }
    }

    public class News
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string ImageUrl { get; set; }
        public DateTime Date { get; set; }
    }
}
