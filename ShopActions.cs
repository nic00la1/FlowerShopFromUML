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

    private void DisplayMessageAndWait(string message)
    {
        Console.WriteLine(message);
        Console.WriteLine("Naci�nij dowolny klawisz, aby kontynuowa�...");
        Console.ReadKey();
    }

    private string GetInput(string prompt, bool allowEmpty = false)
    {
        string input;
        do
        {
            Console.WriteLine(prompt);
            input = Console.ReadLine();
            if (!allowEmpty && string.IsNullOrWhiteSpace(input))
                Console.WriteLine("Warto�� nie mo�e by� pusta.");
        } while (!allowEmpty && string.IsNullOrWhiteSpace(input));

        return input;
    }

    private T GetValidatedInput<T>(string prompt,
                                   Func<string, (bool, T)> validator
    )
    {
        T value;
        bool isValid;
        do
        {
            Console.WriteLine(prompt);
            string input = Console.ReadLine();
            (isValid, value) = validator(input);
            if (!isValid) Console.WriteLine("Nieprawid�owa warto��.");
        } while (!isValid);

        return value;
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
        DisplayMessageAndWait("");
    }

    public void AddNewBouquet(Shop shop, DatabaseManager dbManager)
    {
        DisplayTitle("Dodawanie nowego bukietu");
        string name = GetInput("Podaj nazw� bukietu:");
        if (string.IsNullOrWhiteSpace(name))
        {
            DisplayMessageAndWait("Nazwa bukietu nie mo�e by� pusta.");
            return;
        }

        List<Flower> flowers = new();
        while (true)
        {
            string flowerName = GetInput("Podaj nazw� kwiatu:");
            string color = GetInput("Podaj kolor kwiatu:");
            float price = GetValidatedInput("Podaj cen� kwiatu:",
                input => (float.TryParse(input, out float result), result));
            int inStock = GetValidatedInput("Podaj ilo�� kwiatu:",
                input => (int.TryParse(input, out int result), result));

            flowers.Add(new Flower(flowerName, color, price, inStock));

            string addMore =
                GetInput(
                    "Czy chcesz doda� kolejny kwiat do bukietu? (tak/nie):",
                    true);
            if (addMore.ToLower() != "tak") break;
        }

        if (flowers.Count == 0)
        {
            DisplayMessageAndWait(
                "Bukiet musi zawiera� przynajmniej jeden kwiat.");
            return;
        }

        float bouquetPrice = GetValidatedInput("Podaj cen� bukietu:",
            input => (float.TryParse(input, out float result), result));

        shop.CreateBouquet(name, flowers, bouquetPrice);
        dbManager.SaveData(shop); // Save data to the database
        DisplayMessageAndWait(
            $"Liczba bukiet�w po dodaniu nowego: {shop.Bouquets.Count}");
    }

    public void AddNewCustomer(Shop shop, DatabaseManager dbManager)
    {
        DisplayTitle("Dodawanie nowego klienta");
        string name = GetInput("Podaj imi� i nazwisko klienta:");
        string email = GetInput("Podaj email klienta:");
        string phone = GetInput("Podaj numer telefonu klienta:");

        Customer newCustomer = new(name, email, phone);
        shop.AddCustomer(newCustomer);
        dbManager.SaveData(shop); // Save data to the database
        DisplayMessageAndWait(
            $"Liczba klient�w po dodaniu nowego: {shop.Customers.Count}");
    }

    public void CreateNewOrder(Shop shop, DatabaseManager dbManager)
    {
        DisplayTitle("Tworzenie nowego zam�wienia");
        string email = GetInput("Podaj email klienta:");
        Customer customer = shop.GetCustomerByEmail(email);
        if (customer == null)
        {
            DisplayMessageAndWait("Klient nie znaleziony.");
            return;
        }

        List<Bouquet> orderBouquets = new();
        while (true)
        {
            string bouquetName =
                GetInput("Podaj nazw� bukietu (lub 'koniec' aby zako�czy�):",
                    true);
            if (bouquetName.ToLower() == "koniec") break;
            if (string.IsNullOrWhiteSpace(bouquetName))
            {
                Console.WriteLine("Nazwa bukietu nie mo�e by� pusta.");
                continue;
            }

            Bouquet bouquet = shop.Bouquets.Find(b =>
                b.Name.Equals(bouquetName, StringComparison.OrdinalIgnoreCase));
            if (bouquet != null)
                orderBouquets.Add(bouquet);
            else
                Console.WriteLine("Bukiet nie znaleziony.");
        }

        shop.CreateOrder(customer, orderBouquets);
        dbManager.SaveData(shop); // Save data to the database
        DisplayMessageAndWait(
            $"Liczba zam�wie� po dodaniu nowego: {shop.Orders.Count}");
    }

    public void FulfillOrder(Shop shop, DatabaseManager dbManager)
    {
        DisplayTitle("Realizacja zam�wienia");
        int orderIndex = GetValidatedInput(
            "Podaj numer zam�wienia do realizacji:",
            input => (int.TryParse(input, out int result), result));

        if (orderIndex < 0 || orderIndex >= shop.Orders.Count)
        {
            DisplayMessageAndWait("Nieprawid�owy numer zam�wienia.");
            return;
        }

        shop.FulfillOrder(shop.Orders[orderIndex]);
        dbManager.SaveData(shop); // Save data to the database
        DisplayMessageAndWait(
            $"Liczba zam�wie� po realizacji: {shop.Orders.Count}");
    }

    public void SearchCustomerByEmail(Shop shop)
    {
        DisplayTitle("Wyszukiwanie klienta po emailu");
        string email = GetInput("Podaj email klienta:");
        Customer customer = shop.Customers.Find(c =>
            c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        if (customer != null)
            Console.WriteLine(
                $"Znaleziony klient: {customer.Name}, Email: {customer.Email}, Telefon: {customer.Phone}");
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Klient nie znaleziony.");
            Console.ResetColor();
        }

        DisplayMessageAndWait("");
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

        DisplayMessageAndWait("");
    }
}
