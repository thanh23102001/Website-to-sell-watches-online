using System;
using System.Collections.Generic;

#nullable disable

namespace WebShop.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public int? Purchases { get; set; }
        public int? Customer { get; set; }
        public int? TotalPrice { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Note { get; set; }

        public virtual Account CustomerNavigation { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
