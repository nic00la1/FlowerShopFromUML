using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShopFromUML
{
    public class Shop
    {
        public string Name { get; set; }
        public string Adress { get; set; }
        public List<Flower> Flowers { get; set; }
        public List<Bouquet> Bouquets { get; set; }
        public List<Order> Orders { get; set; }
    }
}
