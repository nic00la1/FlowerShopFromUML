using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShopFromUML
{
    public class Bouquet
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public List<Flower> Flowers { get; set; }
        public float Price { get; set; }
        public int InStock { get; set; }

        public Bouquet() 
        {
            Flowers = new List<Flower>();
        }

        public Bouquet(int id, string name, List<Flower> flowers, float price, int inStock)
        {
            ID = id;
            Name = name;
            Flowers = flowers ?? new List<Flower>(); // Jeśli nie ma kwiatków to utwórz nową listę kwiatów
            Price = price;
            InStock = inStock;
        }
    }
}
