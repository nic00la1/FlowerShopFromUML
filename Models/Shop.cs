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

    public void CreateBouquet(string name, List<Flower> flowers, float price)
    {
        Bouquet? existingBouquet = Bouquets.Find(b => b.Name == name);
        if (existingBouquet != null)
        {
            if (existingBouquet.Flowers.SequenceEqual(flowers))
            {
                existingBouquet.InStock++;
                foreach (Flower flower in flowers)
                {
                    Flower? shopFlower =
                        Flowers.Find(f => f.Name == flower.Name);
                    if (shopFlower != null)
                        shopFlower.InStock -= flower.InStock;
                }
            } else
                throw new InvalidOperationException(
                    "Bouquet with the same name but different content already exists.");
        } else
        {
            Bouquets.Add(new Bouquet(name, flowers, price, 1));
            foreach (Flower flower in flowers)
            {
                Flower? shopFlower = Flowers.Find(f => f.Name == flower.Name);
                if (shopFlower != null)
                    shopFlower.InStock -= flower.InStock;
            }
        }
    }

    public void CreateOrder(Customer customer, List<Bouquet> bouquets)
    {
        float totalPrice = bouquets.Sum(b => b.Price);
        Orders.Add(new Order(DateTime.Now, customer, bouquets, totalPrice));
    }

    public void FulfillOrder(Order order)
    {
        Orders.Remove(order);
        foreach (Bouquet bouquet in order.Bouquets)
        {
            Bouquet? shopBouquet = Bouquets.Find(b => b.Name == bouquet.Name);
            if (shopBouquet != null)
            {
                shopBouquet.InStock -= bouquet.InStock;
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
