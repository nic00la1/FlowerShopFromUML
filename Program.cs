using FlowerShopFromUML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

internal class Program
{
    private static void Main()
    {
        // Wczytywanie danych z pliku JSON
        string jsonFilePath =
            "C:\\Users\\Admin\\source\\repos\\nic00la1\\FlowerShopFromUML\\data.json";
        string jsonData = File.ReadAllText(jsonFilePath);

        // Deserializacja danych
        ShopData data = JsonSerializer.Deserialize<ShopData>(jsonData);

        // Tworzenie sklepu na podstawie danych
        Shop shop = new(
            "Flower Shop",
            "123 Flower St, Garden City",
            data.Flowers,
            data.Bouquets,
            data.Orders,
            data.Customers
        );

        // Pętla interaktywna
        while (true)
        {
            int selectedIndex = DisplayMenu();
            switch (selectedIndex)
            {
                case 0:
                    DisplayShopInfo(shop);
                    break;
                case 1:
                    DisplayTotalFlowerValue(shop);
                    break;
                case 2:
                    DisplayTotalBouquetValue(shop);
                    break;
                case 3:
                    AddNewBouquet(shop);
                    break;
                case 4:
                    AddNewCustomer(shop);
                    break;
                case 5:
                    CreateNewOrder(shop);
                    break;
                case 6:
                    FulfillOrder(shop);
                    break;
                case 7:
                    SearchCustomerByEmail(shop);
                    break;
                case 8:
                    return;
                default:
                    Console.WriteLine("Nieprawidłowy wybór. Spróbuj ponownie.");
                    break;
            }
        }
    }

    private static int DisplayMenu()
    {
        string[] options =
        {
            "Wyświetl informacje o sklepie",
            "Wyświetl wartość wszystkich kwiatów",
            "Wyświetl wartość wszystkich bukietów",
            "Dodaj nowy bukiet",
            "Dodaj nowego klienta",
            "Utwórz nowe zamówienie",
            "Zrealizuj zamówienie",
            "Wyszukaj klienta po emailu",
            "Wyjdź"
        };

        int selectedIndex = 0;

        ConsoleKey key;
        do
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("*** Witaj w Kwiaciarni ***");
            Console.ResetColor();
            Console.WriteLine("Wybierz akcję (użyj strzałek):\n");

            for (int i = 0; i < options.Length; i++)
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.WriteLine($">> {options[i]} <<");
                    Console.ResetColor();
                } else
                    Console.WriteLine($"   {options[i]}");

            DrawRose();

            key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow)
                selectedIndex = selectedIndex == 0
                    ? options.Length - 1
                    : selectedIndex - 1;
            else if (key == ConsoleKey.DownArrow)
                selectedIndex = selectedIndex == options.Length - 1
                    ? 0
                    : selectedIndex + 1;
        } while (key != ConsoleKey.Enter);

        return selectedIndex;
    }

    private static void DrawRose()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(" \n   .-./\\_.-.");
        Console.WriteLine("   /         \\");
        Console.WriteLine("  (            )");
        Console.WriteLine("   \\          /");
        Console.WriteLine("    `-\\'---'/`");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("        | ");
        Console.WriteLine("        | ");
        Console.WriteLine("        | ");
        Console.WriteLine("        |");
        Console.WriteLine("        |");
        Console.ResetColor();
    }

    private static void DisplayShopInfo(Shop shop)
    {
        Console.Clear();
        Console.WriteLine($"Sklep: {shop.Name}, Adres: {shop.Address}");
        Console.WriteLine($"Liczba kwiatów: {shop.Flowers.Count}");
        Console.WriteLine($"Liczba bukietów: {shop.Bouquets.Count}");
        Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    private static void DisplayTotalFlowerValue(Shop shop)
    {
        Console.Clear();
        float totalFlowerValue = shop.CalculateTotalFlowerValue();
        Console.WriteLine($"Całkowita wartość kwiatów: {totalFlowerValue}");
        Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    private static void DisplayTotalBouquetValue(Shop shop)
    {
        Console.Clear();
        float totalBouquetValue = shop.CalculateTotalBouquetValue();
        Console.WriteLine($"Całkowita wartość bukietów: {totalBouquetValue}");
        Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    private static void AddNewBouquet(Shop shop)
    {
        Console.Clear();
        Console.WriteLine("Podaj nazwę bukietu:");
        string name = Console.ReadLine();

        List<Flower> flowers = new();
        while (true)
        {
            Console.WriteLine(
                "Podaj nazwę kwiatu (lub 'koniec' aby zakończyć):");
            string flowerName = Console.ReadLine();
            if (flowerName.ToLower() == "koniec") break;

            Console.WriteLine("Podaj kolor kwiatu:");
            string color = Console.ReadLine();

            Console.WriteLine("Podaj cenę kwiatu:");
            float price = float.Parse(Console.ReadLine());

            Console.WriteLine("Podaj ilość kwiatu:");
            int inStock = int.Parse(Console.ReadLine());

            flowers.Add(new Flower(flowerName, color, price, inStock));
        }

        Console.WriteLine("Podaj cenę bukietu:");
        float bouquetPrice = float.Parse(Console.ReadLine());

        shop.CreateBouquet(name, flowers, bouquetPrice);
        Console.WriteLine(
            $"Liczba bukietów po dodaniu nowego: {shop.Bouquets.Count}");
        Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    private static void AddNewCustomer(Shop shop)
    {
        Console.Clear();
        Console.WriteLine("Podaj imię i nazwisko klienta:");
        string name = Console.ReadLine();

        Console.WriteLine("Podaj email klienta:");
        string email = Console.ReadLine();

        Console.WriteLine("Podaj numer telefonu klienta:");
        string phone = Console.ReadLine();

        Customer newCustomer = new(name, email, phone);
        shop.AddCustomer(newCustomer);
        Console.WriteLine(
            $"Liczba klientów po dodaniu nowego: {shop.Customers.Count}");
        Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    private static void CreateNewOrder(Shop shop)
    {
        Console.Clear();
        Console.WriteLine("Podaj email klienta:");
        string email = Console.ReadLine();
        Customer customer = shop.GetCustomerByEmail(email);
        if (customer == null)
        {
            Console.WriteLine("Klient nie znaleziony.");
            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
            return;
        }

        List<Bouquet> orderBouquets = new();
        while (true)
        {
            Console.WriteLine(
                "Podaj nazwę bukietu (lub 'koniec' aby zakończyć):");
            string bouquetName = Console.ReadLine();
            if (bouquetName.ToLower() == "koniec") break;

            Bouquet bouquet = shop.Bouquets.Find(b => b.Name == bouquetName);
            if (bouquet != null)
                orderBouquets.Add(bouquet);
            else
                Console.WriteLine("Bukiet nie znaleziony.");
        }

        shop.CreateOrder(customer, orderBouquets);
        Console.WriteLine(
            $"Liczba zamówień po dodaniu nowego: {shop.Orders.Count}");
        Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    private static void FulfillOrder(Shop shop)
    {
        Console.Clear();
        Console.WriteLine("Podaj numer zamówienia do realizacji:");
        int orderIndex = int.Parse(Console.ReadLine());
        if (orderIndex < 0 || orderIndex >= shop.Orders.Count)
        {
            Console.WriteLine("Nieprawidłowy numer zamówienia.");
            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
            return;
        }

        shop.FulfillOrder(shop.Orders[orderIndex]);
        Console.WriteLine(
            $"Liczba zamówień po realizacji: {shop.Orders.Count}");
        Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    private static void SearchCustomerByEmail(Shop shop)
    {
        Console.Clear();
        Console.WriteLine("Podaj email klienta:");
        string email = Console.ReadLine();
        Customer customer = shop.GetCustomerByEmail(email);
        if (customer != null)
            Console.WriteLine(
                $"Znaleziony klient: {customer.Name}, Email: {customer.Email}, Telefon: {customer.Phone}");
        else
            Console.WriteLine("Klient nie znaleziony.");
        Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }
}
