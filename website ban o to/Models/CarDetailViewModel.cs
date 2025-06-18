using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace website_ban_o_to.Models
{
    public class CarDetailViewModel
    {
        public Car Car { get; set; }
        public List<CarImage> Images { get; set; }
        public Brand Brand { get; set; }
        public Location Location { get; set; }
    }
}