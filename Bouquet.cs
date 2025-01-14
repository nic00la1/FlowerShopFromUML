using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShopFromUML;

public class Bouquet
{
    public string Name { get; set; }
    public List<Flower> Flowers { get; set; }
    public float Price { get; set; }
    public int InStock { get; set; }

    public Bouquet(string name, List<Flower> flowers, float price, int inStock)
    {
        Name = name;
        Flowers = flowers;
        Price = price;
        InStock = inStock;
    }
}
