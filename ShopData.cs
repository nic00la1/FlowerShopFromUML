using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShopFromUML;

public class ShopData
{
    public List<Flower> Flowers { get; set; } = new();
    public List<Bouquet> Bouquets { get; set; } = new();
    public List<Order> Orders { get; set; } = new();
    public List<Customer> Customers { get; set; } = new();
}
