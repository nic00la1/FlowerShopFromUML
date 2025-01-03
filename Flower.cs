using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShopFromUML
{
    public class Flower
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public float Price { get; set; }
        public int InStock { get; set; }

        public Flower() { }

        public Flower(int id, string name, string color, float price, int inStock)
        {
            ID = id;
            Name = name;
            Color = color;
            Price = price;
            InStock = inStock;
        }
    }
}
