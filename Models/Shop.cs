using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlowerShopFromUML;

public class Shop
{
    public string Name { get; set; }
    public string Address { get; set; }
    public List<Flower> Flowers { get; set; }
    public List<Bouquet> Bouquets { get; set; }
    public List<Order> Orders { get; set; }
    public List<Customer> Customers { get; set; }
    private int _nextOrderId = 1; // Counter for generating unique order IDs

    public int NextOrderId
    {
        get => _nextOrderId;
        set => _nextOrderId = value;
    }


    public Shop(string name,
                string address,
                List<Flower> flowers,
                List<Bouquet> bouquets,
                List<Order> orders,
                List<Customer> customers
    )
    {
        Name = name;
        Address = address;
        Flowers = flowers;
        Bouquets = bouquets;
        Orders = orders;
        Customers = customers;
    }

    public float CalculateTotalFlowerValue()
    {
        float totalValue = 0;
        foreach (Flower flower in Flowers)
            totalValue += flower.Price * flower.InStock;
        return totalValue;
    }

    public float CalculateTotalBouquetValue()
    {
        float totalValue = 0;
        foreach (Bouquet bouquet in Bouquets)
            totalValue += bouquet.Price * bouquet.InStock;
        return totalValue;
    }

    public bool CreateBouquet(string name,
                              List<FlowerCopy> flowers,
                              float price,
                              out string errorMessage
    )
    {
        errorMessage = string.Empty;
        Bouquet? existingBouquet = Bouquets.Find(b => b.Name == name);
        if (existingBouquet != null)
        {
            if (existingBouquet.Flowers.SequenceEqual(flowers))
            {
                existingBouquet.InStock++;
                foreach (FlowerCopy flower in flowers)
                {
                    Flower? shopFlower = Flowers.Find(f =>
                        f.Name == flower.Name && f.Color == flower.Color);
                    if (shopFlower != null)
                        shopFlower.InStock -= flower.Count;
                }
            } else
            {
                errorMessage =
                    "Bukiet o tej samej nazwie, ale innej zawartości już istnieje.";
                return false;
            }
        } else
        {
            Bouquets.Add(new Bouquet(name, flowers, price, 1));
            foreach (FlowerCopy flower in flowers)
            {
                Flower? shopFlower = Flowers.Find(f =>
                    f.Name == flower.Name && f.Color == flower.Color);
                if (shopFlower != null)
                    shopFlower.InStock -= flower.Count;
            }
        }

        return true;
    }

    public void CreateOrder(Customer customer, List<Bouquet> bouquets)
    {
        float totalPrice = bouquets.Sum(b => b.Price);
        Order newOrder = new(_nextOrderId++, DateTime.Now, customer, bouquets,
            totalPrice, "Oczekujące");
        Orders.Add(newOrder);
    }

    public void FulfillOrder(Order order)
    {
        order.Status = "Zrealizowane";
        foreach (Bouquet bouquet in order.Bouquets)
        {
            Bouquet? shopBouquet = Bouquets.Find(b => b.Name == bouquet.Name);
            if (shopBouquet != null)
            {
                shopBouquet.InStock--;
                if (shopBouquet.InStock <= 0)
                    Bouquets.Remove(shopBouquet);
            }
        }
    }

    public void AddCustomer(Customer customer)
    {
        Customers.Add(customer);
    }

    public Customer GetCustomerByEmail(string email)
    {
        return Customers.Find(c => c.Email == email);
    }
}
