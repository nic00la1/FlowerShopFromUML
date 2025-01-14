﻿using FlowerShopFromUML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ConsoleTables;

internal class Program
{
    private static void Main()
    {
        // Tworzenie instancji DatabaseManager
        DatabaseManager dbManager = new();

        // Wczytywanie danych z bazy danych
        Shop shop = new(
            "Fajna Kwiaciarnia Nicoli",
            "Miasto Kwiatkowo, ul. Różowa 77",
            new List<Flower>(),
            new List<Bouquet>(),
            new List<Order>(),
            new List<Customer>()
        );
        dbManager.LoadData(shop);

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
                    AddNewBouquet(shop, dbManager);
                    break;
                case 4:
                    AddNewCustomer(shop, dbManager);
                    break;
                case 5:
                    CreateNewOrder(shop, dbManager);
                    break;
                case 6:
                    FulfillOrder(shop, dbManager);
                    break;
                case 7:
                    DisplayAllCustomers(shop);
                    break;
                case 8:
                    SearchCustomerByEmail(shop);
                    break;
                case 9:
                    dbManager.SaveData(shop);
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
            "Wyświetl wszystkich klientów",
            "Wyszukaj klienta po emailu",
            "Wyjdź"
        };

        int selectedIndex = 0;

        ConsoleKey key;
        do
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("*** Witaj w Kwiaciarni Nicoli ***");
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
        Console.WriteLine(" \n    .-./\\_.-.");
        Console.WriteLine("   /         \\");
        Console.WriteLine("  (           )");
        Console.WriteLine("   \\         /");
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

    private static void AddNewBouquet(Shop shop, DatabaseManager dbManager)
    {
        Console.Clear();
        Console.WriteLine("Podaj nazwę bukietu:");
        string name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Nazwa bukietu nie może być pusta.");
            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
            return;
        }

        List<Flower> flowers = new();
        while (true)
        {
            Console.WriteLine("Podaj nazwę kwiatu:");
            string flowerName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(flowerName))
            {
                Console.WriteLine("Nazwa kwiatu nie może być pusta.");
                continue;
            }

            Console.WriteLine("Podaj kolor kwiatu:");
            string color = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(color))
            {
                Console.WriteLine("Kolor kwiatu nie może być pusty.");
                continue;
            }

            Console.WriteLine("Podaj cenę kwiatu:");
            if (!float.TryParse(Console.ReadLine(), out float price))
            {
                Console.WriteLine("Nieprawidłowa cena.");
                continue;
            }

            Console.WriteLine("Podaj ilość kwiatu:");
            if (!int.TryParse(Console.ReadLine(), out int inStock))
            {
                Console.WriteLine("Nieprawidłowa ilość.");
                continue;
            }

            flowers.Add(new Flower(flowerName, color, price, inStock));

            Console.WriteLine(
                "Czy chcesz dodać kolejny kwiat do bukietu? (tak/nie):");
            string addMore = Console.ReadLine();
            if (addMore.ToLower() != "tak") break;
        }

        if (flowers.Count == 0)
        {
            Console.WriteLine("Bukiet musi zawierać przynajmniej jeden kwiat.");
            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Podaj cenę bukietu:");
        if (!float.TryParse(Console.ReadLine(), out float bouquetPrice))
        {
            Console.WriteLine("Nieprawidłowa cena bukietu.");
            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
            return;
        }

        shop.CreateBouquet(name, flowers, bouquetPrice);
        dbManager.SaveData(shop); // Save data to the database
        Console.WriteLine(
            $"Liczba bukietów po dodaniu nowego: {shop.Bouquets.Count}");
        Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    private static void AddNewCustomer(Shop shop, DatabaseManager dbManager)
    {
        Console.Clear();
        Console.WriteLine("Podaj imię i nazwisko klienta:");
        string name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Imię i nazwisko nie mogą być puste.");
            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Podaj email klienta:");
        string email = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("Email nie może być pusty.");
            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Podaj numer telefonu klienta:");
        string phone = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(phone))
        {
            Console.WriteLine("Numer telefonu nie może być pusty.");
            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
            return;
        }

        Customer newCustomer = new(name, email, phone);
        shop.AddCustomer(newCustomer);
        dbManager.SaveData(shop); // Save data to the database
        Console.WriteLine(
            $"Liczba klientów po dodaniu nowego: {shop.Customers.Count}");
        Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    private static void CreateNewOrder(Shop shop, DatabaseManager dbManager)
    {
        Console.Clear();
        Console.WriteLine("Podaj email klienta:");
        string email = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(email))
        {
            Console.WriteLine("Email nie może być pusty.");
            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
            return;
        }

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
            if (string.IsNullOrWhiteSpace(bouquetName))
            {
                Console.WriteLine("Nazwa bukietu nie może być pusta.");
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
            $"Liczba zamówień po dodaniu nowego: {shop.Orders.Count}");
        Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    private static void FulfillOrder(Shop shop, DatabaseManager dbManager)
    {
        Console.Clear();
        Console.WriteLine("Podaj numer zamówienia do realizacji:");
        string input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input) ||
            !int.TryParse(input, out int orderIndex))
        {
            Console.WriteLine("Nieprawidłowy numer zamówienia.");
            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
            return;
        }

        if (orderIndex < 0 || orderIndex >= shop.Orders.Count)
        {
            Console.WriteLine("Nieprawidłowy numer zamówienia.");
            Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
            Console.ReadKey();
            return;
        }

        shop.FulfillOrder(shop.Orders[orderIndex]);
        dbManager.SaveData(shop); // Save data to the database
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
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Klient nie znaleziony.");
            Console.ResetColor();
        }

        Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }

    private static void DisplayAllCustomers(Shop shop)
    {
        Console.Clear();
        if (shop.Customers.Count == 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Brak klientów.");
            Console.ResetColor();
        } else
        {
            ConsoleTable table = new("Imię i nazwisko", "Email", "Telefon");
            foreach (Customer customer in shop.Customers)
                table.AddRow(customer.Name, customer.Email, customer.Phone);
            table.Write();
        }

        Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
        Console.ReadKey();
    }
}
