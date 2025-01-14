using FlowerShopFromUML;
using System;
using System.Collections.Generic;
using ConsoleTables;

public class ShopActions
{
    private void DisplayTitle(string title)
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"=== {title} ===");
        Console.ResetColor();
    }

    public void DisplayShopInfo(Shop shop)
    {
        DisplayTitle("Informacje o sklepie");

        ConsoleTable table = new("W³aœciwoœæ", "Wartoœæ");

        table.AddRow("Nazwa", shop.Name)
            .AddRow("Adres", shop.Address)
            .AddRow("Ca³kowita wartoœæ kwiatów",
                shop.CalculateTotalFlowerValue())
            .AddRow("Ca³kowita wartoœæ bukietów",
                shop.CalculateTotalBouquetValue())
            .AddRow("Liczba zamówieñ", shop.Orders.Count)
            .AddRow("Liczba klientów", shop.Customers.Count);

        table.Write();
        Console.WriteLine("Naciœnij dowolny klawisz, aby kontynuowaæ...");
        Console.ReadKey();
    }

    public void AddNewBouquet(Shop shop, DatabaseManager dbManager)
    {
        DisplayTitle("Dodawanie nowego bukietu");
        Console.WriteLine("Podaj nazwê bukietu:");
        string name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Nazwa bukietu nie mo¿e byæ pusta.");
            Console.WriteLine("Naciœnij dowolny klawisz, aby kontynuowaæ...");
            Console.ReadKey();
            return;
        }

        List<Flower> flowers = new();
        while (true)
        {
            Console.WriteLine("Podaj nazwê kwiatu:");
            string flowerName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(flowerName))
            {
                Console.WriteLine("Nazwa kwiatu nie mo¿e byæ pusta.");
                continue;
            }

            Console.WriteLine("Podaj kolor kwiatu:");
            string color = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(color))
            {
                Console.WriteLine("Kolor kwiatu nie mo¿e byæ pusty.");
                continue;
            }

            Console.WriteLine("Podaj cenê kwiatu:");
            if (!float.TryParse(Console.ReadLine(), out float price))
            {
                Console.WriteLine("Nieprawid³owa cena.");
                continue;
            }

            Console.WriteLine("Podaj iloœæ kwiatu:");
            if (!int.TryParse(Console.ReadLine(), out int inStock))
            {
                Console.WriteLine("Nieprawid³owa iloœæ.");
                continue;
            }

            flowers.Add(new Flower(flowerName, color, price, inStock));

            Console.WriteLine(
                "Czy chcesz dodaæ kolejny kwiat do bukietu? (tak/nie):");
            string addMore = Console.ReadLine();
            if (addMore.ToLower() != "tak") break;
        }

        if (flowers.Count == 0)
        {
            Console.WriteLine("Bukiet musi zawieraæ przynajmniej jeden kwiat.");
            Console.WriteLine("Naciœnij dowolny klawisz, aby kontynuowaæ...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Podaj cenê bukietu:");
        if (!float.TryParse(Console.ReadLine(), out float bouquetPrice))
        {
            Console.WriteLine("Nieprawid³owa cena bukietu.");
            Console.WriteLine("Naciœnij dowolny klawisz, aby kontynuowaæ...");
            Console.ReadKey();
            return;
        }

        shop.CreateBouquet(name, flowers, bouquetPrice);
        dbManager.SaveData(shop); // Save data to the database
        Console.WriteLine(
            $"Liczba bukietów po dodaniu nowego: {shop.Bouquets.Count}");
        Console.WriteLine("Naciœnij dowolny klawisz, aby kontynuowaæ...");
        Console.ReadKey();
    }

    public void AddNewCustomer(Shop shop, DatabaseManager dbManager)
    {
        DisplayTitle("Dodawanie nowego klienta");
        Console.WriteLine("Podaj imiê i nazwisko klienta:");
        string name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Imiê i nazwisko nie mog¹ byæ puste.");
            Console.WriteLine("Naciœnij dowolny klawisz, aby kontynuowaæ...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Podaj email klienta:");
        string email = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("Email nie mo¿e byæ pusty.");
            Console.WriteLine("Naciœnij dowolny klawisz, aby kontynuowaæ...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Podaj numer telefonu klienta:");
        string phone = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(phone))
        {
            Console.WriteLine("Numer telefonu nie mo¿e byæ pusty.");
            Console.WriteLine("Naciœnij dowolny klawisz, aby kontynuowaæ...");
            Console.ReadKey();
            return;
        }

        Customer newCustomer = new(name, email, phone);
        shop.AddCustomer(newCustomer);
        dbManager.SaveData(shop); // Save data to the database
        Console.WriteLine(
            $"Liczba klientów po dodaniu nowego: {shop.Customers.Count}");
        Console.WriteLine("Naciœnij dowolny klawisz, aby kontynuowaæ...");
        Console.ReadKey();
    }

    public void CreateNewOrder(Shop shop, DatabaseManager dbManager)
    {
        DisplayTitle("Tworzenie nowego zamówienia");
        Console.WriteLine("Podaj email klienta:");
        string email = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("Email nie mo¿e byæ pusty.");
            Console.WriteLine("Naciœnij dowolny klawisz, aby kontynuowaæ...");
            Console.ReadKey();
            return;
        }

        Customer customer = shop.GetCustomerByEmail(email);
        if (customer == null)
        {
            Console.WriteLine("Klient nie znaleziony.");
            Console.WriteLine("Naciœnij dowolny klawisz, aby kontynuowaæ...");
            Console.ReadKey();
            return;
        }

        List<Bouquet> orderBouquets = new();
        while (true)
        {
            Console.WriteLine(
                "Podaj nazwê bukietu (lub 'koniec' aby zakoñczyæ):");
            string bouquetName = Console.ReadLine();
            if (bouquetName.ToLower() == "koniec") break;
            if (string.IsNullOrWhiteSpace(bouquetName))
            {
                Console.WriteLine("Nazwa bukietu nie mo¿e byæ pusta.");
                continue;
            }

            Bouquet bouquet = shop.Bouquets.Find(b => b.Name == bouquetName);
            if (bouquet != null)
                orderBouquets.Add(bouquet);
            else
                Console.WriteLine("Bukiet nie znaleziony.");
        }

        shop.CreateOrder(customer, orderBouquets);
        dbManager.SaveData(shop); // Save data to the database
        Console.WriteLine(
            $"Liczba zamówieñ po dodaniu nowego: {shop.Orders.Count}");
        Console.WriteLine("Naciœnij dowolny klawisz, aby kontynuowaæ...");
        Console.ReadKey();
    }

    public void FulfillOrder(Shop shop, DatabaseManager dbManager)
    {
        DisplayTitle("Realizacja zamówienia");
        Console.WriteLine("Podaj numer zamówienia do realizacji:");
        string input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input) ||
            !int.TryParse(input, out int orderIndex))
        {
            Console.WriteLine("Nieprawid³owy numer zamówienia.");
            Console.WriteLine("Naciœnij dowolny klawisz, aby kontynuowaæ...");
            Console.ReadKey();
            return;
        }

        if (orderIndex < 0 || orderIndex >= shop.Orders.Count)
        {
            Console.WriteLine("Nieprawid³owy numer zamówienia.");
            Console.WriteLine("Naciœnij dowolny klawisz, aby kontynuowaæ...");
            Console.ReadKey();
            return;
        }

        shop.FulfillOrder(shop.Orders[orderIndex]);
        dbManager.SaveData(shop); // Save data to the database
        Console.WriteLine(
            $"Liczba zamówieñ po realizacji: {shop.Orders.Count}");
        Console.WriteLine("Naciœnij dowolny klawisz, aby kontynuowaæ...");
        Console.ReadKey();
    }

    public void SearchCustomerByEmail(Shop shop)
    {
        DisplayTitle("Wyszukiwanie klienta po emailu");
        Console.WriteLine("Podaj email klienta:");
        string email = Console.ReadLine();
        Customer customer = shop.GetCustomerByEmail(email);
        if (customer != null)
            Console.WriteLine(
                $"Znaleziony klient: {customer.Name}, Email: {customer.Email}, Telefon: {customer.Phone}");
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Klient nie znaleziony.");
            Console.ResetColor();
        }

        Console.WriteLine("Naciœnij dowolny klawisz, aby kontynuowaæ...");
        Console.ReadKey();
    }

    public void DisplayAllCustomers(Shop shop)
    {
        DisplayTitle("Lista wszystkich klientów");
        if (shop.Customers.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Brak klientów.");
            Console.ResetColor();
        } else
        {
            ConsoleTable table = new("Imiê i nazwisko", "Email", "Telefon");
            foreach (Customer customer in shop.Customers)
                table.AddRow(customer.Name, customer.Email, customer.Phone);
            table.Write();
        }

        Console.WriteLine("Naciœnij dowolny klawisz, aby kontynuowaæ...");
        Console.ReadKey();
    }
}
