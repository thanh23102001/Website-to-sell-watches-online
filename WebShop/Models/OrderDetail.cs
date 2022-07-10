using System;
using System.Collections.Generic;

#nullable disable

namespace WebShop.Models
{
    public partial class OrderDetail
    {
        public int Idcart { get; set; }
        public int? Idorder { get; set; }
        public int? Product { get; set; }
        public int? Number { get; set; }

        public virtual Order IdorderNavigation { get; set; }
        public virtual Product ProductNavigation { get; set; }
    }
}
