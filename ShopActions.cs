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

    private void DisplayMessageAndWait(string message,
                                       ConsoleColor color = ConsoleColor.White
    )
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
        Console.WriteLine("Naciœnij dowolny klawisz, aby kontynuowaæ...");
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
                Console.WriteLine("Wartoœæ nie mo¿e byæ pusta.");
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
            if (!isValid) Console.WriteLine("Nieprawid³owa wartoœæ.");
        } while (!isValid);

        return value;
    }

    public void DisplayShopInfo(Shop shop)
    {
        DisplayTitle("Informacje o sklepie");

        ConsoleTable table = new("W³aœciwoœæ", "Wartoœæ");

        table.AddRow("Nazwa", shop.Name)
            .AddRow("Adres", shop.Address)
            .AddRow("Ca³kowita wartoœæ kwiatów",
                $"{shop.CalculateTotalFlowerValue()} z³")
            .AddRow("Ca³kowita wartoœæ bukietów",
                $"{shop.CalculateTotalBouquetValue()} z³")
            .AddRow("Liczba zamówieñ", shop.Orders.Count)
            .AddRow("Liczba klientów", shop.Customers.Count);

        table.Write();
        DisplayMessageAndWait("");
    }

    public void AddNewBouquet(Shop shop, DatabaseManager dbManager)
    {
        DisplayTitle("Dodawanie nowego bukietu");
        string name = GetInput("Podaj nazwê bukietu:");
        if (string.IsNullOrWhiteSpace(name))
        {
            DisplayMessageAndWait("Nazwa bukietu nie mo¿e byæ pusta.",
                ConsoleColor.Red);
            return;
        }

        List<Flower> flowers = new();
        while (true)
        {
            string flowerName = GetInput("Podaj nazwê kwiatu:");
            string color = GetInput("Podaj kolor kwiatu:");
            float price = GetValidatedInput("Podaj cenê kwiatu:",
                input => (float.TryParse(input, out float result), result));
            int inStock = GetValidatedInput("Podaj iloœæ kwiatu:",
                input => (int.TryParse(input, out int result), result));

            flowers.Add(new Flower(flowerName, color, price, inStock));

            string addMore =
                GetInput(
                    "Czy chcesz dodaæ kolejny kwiat do bukietu? (tak/nie):",
                    true);
            if (addMore.ToLower() != "tak") break;
        }

        if (flowers.Count == 0)
        {
            DisplayMessageAndWait(
                "Bukiet musi zawieraæ przynajmniej jeden kwiat.",
                ConsoleColor.Red);
            return;
        }

        float bouquetPrice = GetValidatedInput("Podaj cenê bukietu:",
            input => (float.TryParse(input, out float result), result));

        List<FlowerCopy> flowerCopies = flowers
            .Select(f => new FlowerCopy(f.Name, f.Color, f.InStock)).ToList();

        if (!shop.CreateBouquet(name, flowerCopies, bouquetPrice,
                out string errorMessage))
        {
            DisplayMessageAndWait(errorMessage, ConsoleColor.Red);
            return;
        }

        dbManager.SaveData(shop); // Save data to the database
        DisplayMessageAndWait(
            $"Liczba bukietów po dodaniu nowego: {shop.Bouquets.Count}");
    }

    public void AddNewCustomer(Shop shop, DatabaseManager dbManager)
    {
        DisplayTitle("Dodawanie nowego klienta");
        string name = GetInput("Podaj imiê i nazwisko klienta:");
        string email = GetInput("Podaj email klienta:");
        string phone = GetInput("Podaj numer telefonu klienta:");

        Customer newCustomer = new(name, email, phone);
        shop.AddCustomer(newCustomer);
        dbManager.SaveData(shop); // Save data to the database
        DisplayMessageAndWait(
            $"Liczba klientów po dodaniu nowego: {shop.Customers.Count}");
    }

    public void CreateNewOrder(Shop shop, DatabaseManager dbManager)
    {
        DisplayTitle("Tworzenie nowego zamówienia");
        string email = GetInput("Podaj email klienta:");
        Customer customer = shop.GetCustomerByEmail(email);
        if (customer == null)
        {
            DisplayMessageAndWait("Klient nie znaleziony.", ConsoleColor.Red);
            return;
        }

        List<Bouquet> orderBouquets = new();
        while (true)
        {
            string bouquetName =
                GetInput("Podaj nazwê bukietu (lub 'koniec' aby zakoñczyæ):",
                    true);
            if (bouquetName.ToLower() == "koniec") break;
            if (string.IsNullOrWhiteSpace(bouquetName))
            {
                Console.WriteLine("Nazwa bukietu nie mo¿e byæ pusta.");
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
            $"Liczba zamówieñ po dodaniu nowego: {shop.Orders.Count}");
    }

    public void FulfillOrder(Shop shop, DatabaseManager dbManager)
    {
        DisplayTitle("Realizacja zamówienia");
        int orderIndex = GetValidatedInput(
            "Podaj numer zamówienia do realizacji:",
            input => (int.TryParse(input, out int result), result));

        if (orderIndex < 0 || orderIndex >= shop.Orders.Count)
        {
            DisplayMessageAndWait("Nieprawid³owy numer zamówienia.",
                ConsoleColor.Red);
            return;
        }

        shop.FulfillOrder(shop.Orders[orderIndex]);
        dbManager.SaveData(shop); // Save data to the database
        DisplayMessageAndWait(
            $"Liczba zamówieñ po realizacji: {shop.Orders.Count}");
    }

    public void SearchCustomerByEmail(Shop shop)
    {
        DisplayTitle("Wyszukiwanie klienta po emailu");
        string email = GetInput("Podaj email klienta:");
        Customer customer = shop.Customers.Find(c =>
            c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
        if (customer != null)
        {
            Console.Write($"Znaleziony klient: {customer.Name}, Email: ");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(customer.Email);
            Console.ResetColor();
            Console.WriteLine($", Telefon: {customer.Phone}");
        } else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Klient nie znaleziony.");
            Console.ResetColor();
        }

        DisplayMessageAndWait("");
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
            ConsoleTable table = new("Nr", "Imiê i nazwisko", "Email",
                "Telefon");
            int index = 1;
            foreach (Customer customer in shop.Customers)
                table.AddRow(index++, customer.Name, customer.Email,
                    customer.Phone);
            table.Write();
        }

        DisplayMessageAndWait("");
    }

    public void DisplayAllBouquets(Shop shop)
    {
        DisplayTitle("Lista wszystkich bukietów");
        if (shop.Bouquets.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Brak bukietów.");
            Console.ResetColor();
        } else
        {
            ConsoleTable table = new("Nr", "Nazwa", "Cena [z³]",
                "Iloœæ w magazynie");
            int index = 1;
            foreach (Bouquet bouquet in shop.Bouquets)
                table.AddRow(index++, bouquet.Name, bouquet.Price,
                    bouquet.InStock);
            table.Write();
        }

        DisplayMessageAndWait("");
    }

    public void DisplayAllFlowers(Shop shop)
    {
        DisplayTitle("Lista wszystkich kwiatów");
        if (shop.Flowers.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Brak kwiatów.");
            Console.ResetColor();
        } else
        {
            ConsoleTable table = new("Nr", "Nazwa", "Kolor", "Cena [z³]",
                "Iloœæ w magazynie");
            int index = 1;
            foreach (Flower flower in shop.Flowers)
                table.AddRow(index++, flower.Name, flower.Color, flower.Price,
                    flower.InStock);
            table.Write();
        }

        DisplayMessageAndWait("");
    }
}
