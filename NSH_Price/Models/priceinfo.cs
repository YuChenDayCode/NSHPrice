using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NSH_Price.Models
{
    public class area
    {
        public int Id { get; set; }
        public string AreaName { get; set; }
    }

    public class priceinfo
    {
        public int Id { get; set; }
        public decimal UnitPrice { get; set; }
        public int AreaId { get; set; }
        public string AreaName { get; set; }
        public DateTime TheTime { get; set; }
        public DateTime CreateTime { get; set; }

    }
}