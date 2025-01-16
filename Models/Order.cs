using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShopFromUML;

public class Order
{
    public int Id { get; set; } // Unique identifier for the order
    public DateTime OrderDate { get; set; }
    public Customer Customer { get; set; }
    public List<Bouquet> Bouquets { get; set; }
    public float TotalPrice { get; set; }
    public string Status { get; set; } // New property to track order status


    public Order(int id,
                 DateTime orderDate,
                 Customer customer,
                 List<Bouquet> bouquets,
                 float totalPrice,
                 string status
    )
    {
        Id = id;
        OrderDate = orderDate;
        Customer = customer;
        Bouquets = bouquets;
        TotalPrice = totalPrice;
        Status = status;
    }
}
