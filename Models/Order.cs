using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShopFromUML;

public class Order
{
    public DateTime OrderDate { get; set; }
    public Customer Customer { get; set; }
    public List<Bouquet> Bouquets { get; set; }
    public float TotalPrice { get; set; }

    public Order(
        DateTime orderDate,
        Customer customer,
        List<Bouquet> bouquets,
        float totalPrice
    )
    {
        OrderDate = orderDate;
        Customer = customer;
        Bouquets =
            bouquets ??
            new List<Bouquet>(); // Jeśli nie ma bukietów to utwórz nową listę bukietów
        TotalPrice = totalPrice;
    }
}
