using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace website_ban_o_to.Models
{
    public class CartViewModel
    {
        public List<Cart> CartItems { get; set; }
        public decimal TotalAmount { get; set; }
        public int TotalItems { get; set; }
    }

}