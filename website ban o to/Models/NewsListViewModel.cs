using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace website_ban_o_to.Models
{
    public class NewsListViewModel
    {
        public List<News> NewsList { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
    }
}