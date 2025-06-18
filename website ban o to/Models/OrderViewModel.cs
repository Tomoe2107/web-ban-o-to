using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace website_ban_o_to.Models
{
    public class OrderViewModel
    {
        public Order Order { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
        public User User { get; set; }
    }
}