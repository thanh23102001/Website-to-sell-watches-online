using System;
using System.Collections.Generic;

#nullable disable

namespace WebShop.Models
{
    public partial class Product
    {
        public Product()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public int? Price { get; set; }
        public int? Number { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
