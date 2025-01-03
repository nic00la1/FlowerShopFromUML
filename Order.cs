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
    }
}
