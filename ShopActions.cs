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
                $"{shop.CalculateTotalFlowerValue()} z�")
            .AddRow("Ca�kowita warto�� bukiet�w",
                $"{shop.CalculateTotalBouquetValue()} z�")
            .AddRow("Liczba zam�wie�", shop.Orders.Count)
            .AddRow("Liczba klient�w", shop.Customers.Count);

        table.Write();
        DisplayMessageAndWait("");
    }

    private int DisplayMenu(string title,
                            List<string> options,
                            int pageSize = 10
    )
    {
        int selectedIndex = 0;
        int currentPage = 0;
        int totalPages = (int)Math.Ceiling((double)options.Count / pageSize);
        ConsoleKey key;

        do
        {
            Console.Clear();
            DisplayTitle(title);

            int start = currentPage * pageSize;
            int end = Math.Min(start + pageSize, options.Count);

            for (int i = start; i < end; i++)
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"> {options[i]}");
                    Console.ResetColor();
                } else
                    Console.WriteLine($"  {options[i]}");

            Console.WriteLine($"\nStrona {currentPage + 1} z {totalPages}");

            key = Console.ReadKey().Key;

            if (key == ConsoleKey.UpArrow)
            {
                if (selectedIndex > start)
                    selectedIndex--;
                else if (currentPage > 0)
                {
                    currentPage--;
                    selectedIndex = currentPage * pageSize + pageSize - 1;
                } else
                {
                    currentPage = totalPages - 1;
                    selectedIndex = options.Count - 1;
                }
            } else if (key == ConsoleKey.DownArrow)
            {
                if (selectedIndex < end - 1)
                    selectedIndex++;
                else if (currentPage < totalPages - 1)
                {
                    currentPage++;
                    selectedIndex = currentPage * pageSize;
                } else
                {
                    currentPage = 0;
                    selectedIndex = 0;
                }
            } else if (key == ConsoleKey.LeftArrow)
            {
                if (currentPage > 0)
                {
                    currentPage--;
                    selectedIndex = currentPage * pageSize;
                } else
                {
                    currentPage = totalPages - 1;
                    selectedIndex = currentPage * pageSize;
                }
            } else if (key == ConsoleKey.RightArrow)
            {
                if (currentPage < totalPages - 1)
                {
                    currentPage++;
                    selectedIndex = currentPage * pageSize;
                } else
                {
                    currentPage = 0;
                    selectedIndex = 0;
                }
            }
        } while (key != ConsoleKey.Enter);

        return selectedIndex;
    }

    public void AddNewBouquet(Shop shop, DatabaseManager dbManager)
    {
        DisplayTitle("Dodawanie nowego bukietu");

        // Prepare options for the menu
        List<string> bouquetOptions = new();
        foreach (Bouquet bouquet in shop.Bouquets)
        {
            string flowers = string.Join(", ",
                bouquet.Flowers.Select(f =>
                    $"{f.Name} ({f.Color}) x{f.Count}"));
            bouquetOptions.Add(
                $"{bouquet.Name} - {bouquet.Price} z�, {bouquet.InStock} w magazynie - {flowers}");
        }

        bouquetOptions.Add("Wpisz nowy bukiet");

        // Display menu and get selected index
        int selectedBouquetIndex = DisplayMenu("Wybierz bukiet lub wpisz nowy",
            bouquetOptions);

        string name;
        if (selectedBouquetIndex == shop.Bouquets.Count)
            name = GetInput("Podaj nazw� nowego bukietu:");
        else
            name = shop.Bouquets[selectedBouquetIndex].Name;

        if (string.IsNullOrWhiteSpace(name))
        {
            DisplayMessageAndWait("Nazwa bukietu nie mo�e by� pusta.",
                ConsoleColor.Red);
            return;
        }

        List<FlowerCopy> flowerCopies = new();
        while (true)
        {
            // Prepare options for the flower menu
            List<string> flowerOptions = new();
            foreach (Flower flower in shop.Flowers)
                flowerOptions.Add(
                    $"{flower.Name} ({flower.Color}) - {flower.Price} z�, {flower.InStock} w magazynie");
            flowerOptions.Add("Zako�cz wybieranie kwiat�w");

            // Display flower menu and get selected index
            int selectedFlowerIndex = DisplayMenu(
                "Wybierz kwiat z listy (lub zako�cz wybieranie)",
                flowerOptions);

            if (selectedFlowerIndex == shop.Flowers.Count) break;

            Flower selectedFlower = shop.Flowers[selectedFlowerIndex];
            int count = GetValidatedInput(
                $"Podaj ilo�� kwiatu '{selectedFlower.Name}' ({selectedFlower.Color}):",
                input =>
                {
                    bool isValid = int.TryParse(input, out int result) &&
                        result > 0 && result <= selectedFlower.InStock;
                    return (isValid, result);
                });

            flowerCopies.Add(new FlowerCopy(selectedFlower.Name,
                selectedFlower.Color, count));

            string addMore =
                GetInput(
                    "Czy chcesz doda� kolejny kwiat do bukietu? (tak/nie):",
                    true);
            if (addMore.ToLower() != "tak") break;
        }

        if (flowerCopies.Count == 0)
        {
            DisplayMessageAndWait(
                "Bukiet musi zawiera� przynajmniej jeden kwiat.",
                ConsoleColor.Red);
            return;
        }

        float bouquetPrice = GetValidatedInput("Podaj cen� bukietu:",
            input => (float.TryParse(input, out float result), result));

        if (!shop.CreateBouquet(name, flowerCopies, bouquetPrice,
                out string errorMessage))
        {
            DisplayMessageAndWait(errorMessage, ConsoleColor.Red);
            return;
        }

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
            DisplayMessageAndWait("Klient nie znaleziony.", ConsoleColor.Red);
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
            DisplayMessageAndWait("Nieprawid�owy numer zam�wienia.",
                ConsoleColor.Red);
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
        DisplayTitle("Lista wszystkich klient�w");
        if (shop.Customers.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Brak klient�w.");
            Console.ResetColor();
        } else
        {
            ConsoleTable table = new("Nr", "Imi� i nazwisko", "Email",
                "Telefon");
            int index = 1;
            foreach (Customer customer in shop.Customers)
                table.AddRow(index++, customer.Name, customer.Email,
                    customer.Phone);
            table.Write();
        }

        DisplayMessageAndWait("");
    }

    public void DisplayAllBouquets(Shop shop, DatabaseManager dbManager)
    {
        DisplayTitle("Lista wszystkich bukiet�w");
        if (shop.Bouquets.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Brak bukiet�w.");
            Console.ResetColor();
        } else
        {
            ConsoleTable table = new("Nr", "Nazwa", "Cena [z�]",
                "Ilo�� w magazynie", "Kwiaty");
            int index = 1;
            foreach (Bouquet bouquet in shop.Bouquets)
            {
                string flowers = string.Join(", ",
                    bouquet.Flowers.Select(f =>
                        $"{f.Name} ({f.Color}) x{f.Count}"));
                table.AddRow(index++, bouquet.Name, bouquet.Price,
                    bouquet.InStock, flowers);
            }

            table.Write();
        }

        DisplayMessageAndWait("");
    }

    public void DisplayAllFlowers(Shop shop)
    {
        DisplayTitle("Lista wszystkich kwiat�w");
        if (shop.Flowers.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Brak kwiat�w.");
            Console.ResetColor();
        } else
        {
            ConsoleTable table = new("Nr", "Nazwa", "Kolor", "Cena [z�]",
                "Ilo�� w magazynie");
            int index = 1;
            foreach (Flower flower in shop.Flowers)
                table.AddRow(index++, flower.Name, flower.Color, flower.Price,
                    flower.InStock);
            table.Write();
        }

        DisplayMessageAndWait("");
    }
}
