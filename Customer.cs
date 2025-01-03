using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShopFromUML
{
    public class Customer
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public Customer() { }

        public Customer(int id, string name, string email, string phone)
        {
            ID = id;
            Name = name;
            Email = email;
            Phone = phone;
        }
    }
}
