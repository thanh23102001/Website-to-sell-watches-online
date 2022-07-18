using System;
using System.Collections.Generic;

#nullable disable

namespace WebShop.Models
{
    public partial class Account
    {
        public Account()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string Fullname { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int? Role { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
