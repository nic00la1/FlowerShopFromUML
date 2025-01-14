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

        ConsoleTable table = new("W�a�ciwo��", "Warto��");

        table.AddRow("Nazwa", shop.Name)
            .AddRow("Adres", shop.Address)
            .AddRow("Ca�kowita warto�� kwiat�w",
                shop.CalculateTotalFlowerValue())
            .AddRow("Ca�kowita warto�� bukiet�w",
                shop.CalculateTotalBouquetValue())
            .AddRow("Liczba zam�wie�", shop.Orders.Count)
            .AddRow("Liczba klient�w", shop.Customers.Count);

        table.Write();
        Console.WriteLine("Naci�nij dowolny klawisz, aby kontynuowa�...");
        Console.ReadKey();
    }

    public void AddNewBouquet(Shop shop, DatabaseManager dbManager)
    {
        DisplayTitle("Dodawanie nowego bukietu");
        Console.WriteLine("Podaj nazw� bukietu:");
        string name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Nazwa bukietu nie mo�e by� pusta.");
            Console.WriteLine("Naci�nij dowolny klawisz, aby kontynuowa�...");
            Console.ReadKey();
            return;
        }

        List<Flower> flowers = new();
        while (true)
        {
            Console.WriteLine("Podaj nazw� kwiatu:");
            string flowerName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(flowerName))
            {
                Console.WriteLine("Nazwa kwiatu nie mo�e by� pusta.");
                continue;
            }

            Console.WriteLine("Podaj kolor kwiatu:");
            string color = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(color))
            {
                Console.WriteLine("Kolor kwiatu nie mo�e by� pusty.");
                continue;
            }

            Console.WriteLine("Podaj cen� kwiatu:");
            if (!float.TryParse(Console.ReadLine(), out float price))
            {
                Console.WriteLine("Nieprawid�owa cena.");
                continue;
            }

            Console.WriteLine("Podaj ilo�� kwiatu:");
            if (!int.TryParse(Console.ReadLine(), out int inStock))
            {
                Console.WriteLine("Nieprawid�owa ilo��.");
                continue;
            }

            flowers.Add(new Flower(flowerName, color, price, inStock));

            Console.WriteLine(
                "Czy chcesz doda� kolejny kwiat do bukietu? (tak/nie):");
            string addMore = Console.ReadLine();
            if (addMore.ToLower() != "tak") break;
        }

        if (flowers.Count == 0)
        {
            Console.WriteLine("Bukiet musi zawiera� przynajmniej jeden kwiat.");
            Console.WriteLine("Naci�nij dowolny klawisz, aby kontynuowa�...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Podaj cen� bukietu:");
        if (!float.TryParse(Console.ReadLine(), out float bouquetPrice))
        {
            Console.WriteLine("Nieprawid�owa cena bukietu.");
            Console.WriteLine("Naci�nij dowolny klawisz, aby kontynuowa�...");
            Console.ReadKey();
            return;
        }

        shop.CreateBouquet(name, flowers, bouquetPrice);
        dbManager.SaveData(shop); // Save data to the database
        Console.WriteLine(
            $"Liczba bukiet�w po dodaniu nowego: {shop.Bouquets.Count}");
        Console.WriteLine("Naci�nij dowolny klawisz, aby kontynuowa�...");
        Console.ReadKey();
    }

    public void AddNewCustomer(Shop shop, DatabaseManager dbManager)
    {
        DisplayTitle("Dodawanie nowego klienta");
        Console.WriteLine("Podaj imi� i nazwisko klienta:");
        string name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Imi� i nazwisko nie mog� by� puste.");
            Console.WriteLine("Naci�nij dowolny klawisz, aby kontynuowa�...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Podaj email klienta:");
        string email = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("Email nie mo�e by� pusty.");
            Console.WriteLine("Naci�nij dowolny klawisz, aby kontynuowa�...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Podaj numer telefonu klienta:");
        string phone = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(phone))
        {
            Console.WriteLine("Numer telefonu nie mo�e by� pusty.");
            Console.WriteLine("Naci�nij dowolny klawisz, aby kontynuowa�...");
            Console.ReadKey();
            return;
        }

        Customer newCustomer = new(name, email, phone);
        shop.AddCustomer(newCustomer);
        dbManager.SaveData(shop); // Save data to the database
        Console.WriteLine(
            $"Liczba klient�w po dodaniu nowego: {shop.Customers.Count}");
        Console.WriteLine("Naci�nij dowolny klawisz, aby kontynuowa�...");
        Console.ReadKey();
    }

    public void CreateNewOrder(Shop shop, DatabaseManager dbManager)
    {
        DisplayTitle("Tworzenie nowego zam�wienia");
        Console.WriteLine("Podaj email klienta:");
        string email = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("Email nie mo�e by� pusty.");
            Console.WriteLine("Naci�nij dowolny klawisz, aby kontynuowa�...");
            Console.ReadKey();
            return;
        }

        Customer customer = shop.GetCustomerByEmail(email);
        if (customer == null)
        {
            Console.WriteLine("Klient nie znaleziony.");
            Console.WriteLine("Naci�nij dowolny klawisz, aby kontynuowa�...");
            Console.ReadKey();
            return;
        }

        List<Bouquet> orderBouquets = new();
        while (true)
        {
            Console.WriteLine(
                "Podaj nazw� bukietu (lub 'koniec' aby zako�czy�):");
            string bouquetName = Console.ReadLine();
            if (bouquetName.ToLower() == "koniec") break;
            if (string.IsNullOrWhiteSpace(bouquetName))
            {
                Console.WriteLine("Nazwa bukietu nie mo�e by� pusta.");
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
            $"Liczba zam�wie� po dodaniu nowego: {shop.Orders.Count}");
        Console.WriteLine("Naci�nij dowolny klawisz, aby kontynuowa�...");
        Console.ReadKey();
    }

    public void FulfillOrder(Shop shop, DatabaseManager dbManager)
    {
        DisplayTitle("Realizacja zam�wienia");
        Console.WriteLine("Podaj numer zam�wienia do realizacji:");
        string input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input) ||
            !int.TryParse(input, out int orderIndex))
        {
            Console.WriteLine("Nieprawid�owy numer zam�wienia.");
            Console.WriteLine("Naci�nij dowolny klawisz, aby kontynuowa�...");
            Console.ReadKey();
            return;
        }

        if (orderIndex < 0 || orderIndex >= shop.Orders.Count)
        {
            Console.WriteLine("Nieprawid�owy numer zam�wienia.");
            Console.WriteLine("Naci�nij dowolny klawisz, aby kontynuowa�...");
            Console.ReadKey();
            return;
        }

        shop.FulfillOrder(shop.Orders[orderIndex]);
        dbManager.SaveData(shop); // Save data to the database
        Console.WriteLine(
            $"Liczba zam�wie� po realizacji: {shop.Orders.Count}");
        Console.WriteLine("Naci�nij dowolny klawisz, aby kontynuowa�...");
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

        Console.WriteLine("Naci�nij dowolny klawisz, aby kontynuowa�...");
        Console.ReadKey();
    }

    public void DisplayAllCustomers(Shop shop)
    {
        DisplayTitle("Lista wszystkich klient�w");
        if (shop.Customers.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Brak klient�w.");
            Console.ResetColor();
        } else
        {
            ConsoleTable table = new("Imi� i nazwisko", "Email", "Telefon");
            foreach (Customer customer in shop.Customers)
                table.AddRow(customer.Name, customer.Email, customer.Phone);
            table.Write();
        }

        Console.WriteLine("Naci�nij dowolny klawisz, aby kontynuowa�...");
        Console.ReadKey();
    }
}
