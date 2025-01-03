using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShopFromUML
{
    public class Order
    {
        public int ID { get; set; }
        public DateTime OrderDate { get; set; }
        public Customer Customer { get; set; }
        public List<Bouquet> Bouquets { get; set; }
        public float TotalPrice { get; set; }

        public Order()
        { 
            Bouquets = new List<Bouquet>();
        }

        public Order(int id, DateTime orderDate, Customer customer, List<Bouquet> bouquets, float totalPrice) 
        {
            ID = id;
            OrderDate = orderDate;
            Customer = customer;
            Bouquets = bouquets ?? new List<Bouquet>(); // Jeśli nie ma bukietów to utwórz nową listę bukietów
            TotalPrice = totalPrice;
        }
    }
}
