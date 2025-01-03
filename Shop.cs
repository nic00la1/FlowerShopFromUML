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

        public Shop() 
        {
            Flowers = new List<Flower>();
            Bouquets = new List<Bouquet>();
            Orders = new List<Order>();
        }

        public Shop(string name, string adress, List<Flower> flowers, List<Bouquet> bouquets, List<Order> orders)
        {
            Name = name;
            Adress = adress;
            Flowers = flowers ?? new List<Flower>(); // Jesli nie ma kwiatkow to utworz nowa liste kwiatow
            Bouquets = bouquets ?? new List<Bouquet>(); // -,,- 
            Orders = orders ?? new List<Order>(); // -,,-
        }
    }
}
