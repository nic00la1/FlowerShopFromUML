using FlowerShopFromUML;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using ConsoleTables;

internal class Program
{
    private static async Task Main()
    {
        // Tworzenie instancji DatabaseManager
        DatabaseManager dbManager = new();
        ShopActions shopActions = new();

        // Wczytywanie danych z bazy danych
        Shop shop = new(
            "Fajna Kwiaciarnia Nicoli",
            "Miasto Kwiatkowo, ul. Różowa 77",
            new List<Flower>(),
            new List<Bouquet>(),
            new List<Order>(),
            new List<Customer>()
        );
        await dbManager.LoadDataAsync(shop);

        // Sprawdzenie, czy dane są już zainicjalizowane
        if (!shop.Flowers.Any() && !shop.Bouquets.Any() && !shop.Orders.Any() &&
            !shop.Customers.Any())
        {
            // Inicjalizacja danych
            List<Flower> flowers = new()
            {
                new Flower("Róża", "Czerwona", 2.50f, 200),
                new Flower("Tulipan", "Żółty", 1.75f, 150),
                new Flower("Słonecznik", "Żółty", 3.00f, 100),
                new Flower("Stokrotka", "Biała", 1.25f, 250),
                new Flower("Lilia", "Różowa", 4.50f, 50),
                new Flower("Storczyk", "Fioletowy", 10.00f, 70),
                new Flower("Goździk", "Różowy", 2.00f, 300),
                new Flower("Chryzantema", "Pomarańczowa", 1.50f, 120),
                new Flower("Piwonia", "Biała", 5.00f, 80),
                new Flower("Hortensja", "Niebieska", 6.00f, 60)
            };

            List<Bouquet> bouquets = new()
            {
                new Bouquet("Letnia Rozkosz", new List<FlowerCopy>
                {
                    new("Róża", "Czerwona", 10),
                    new("Tulipan", "Żółty", 10)
                }, 10.75f, 15),

                new Bouquet("Kolory Jesieni", new List<FlowerCopy>
                {
                    new("Storczyk", "Fioletowy", 10),
                    new("Goździk", "Różowy", 5)
                }, 8.25f, 20),

                new Bouquet("Elegancka Czystość", new List<FlowerCopy>
                {
                    new("Róża", "Czerwona", 3),
                    new("Piwonia", "Biała", 3),
                    new("Hortensja", "Niebieska", 3)
                }, 21.00f, 5)
            };

            Customer customer1 = new("Alicja Kowalska", "alicja@example.com",
                "123-456-789");
            Customer customer2 =
                new("Jan Nowak", "jan@example.com", "987-654-321");
            Customer customer3 = new("Karol Wiśniewski", "karol@example.com",
                "555-555-555");

            List<Order> orders = new()
            {
                new Order(1, DateTime.Now, customer1,
                    new List<Bouquet> { bouquets[0], bouquets[1] },
                    bouquets[0].Price + bouquets[1].Price, "Oczekujące"),
                new Order(2, DateTime.Now, customer2,
                    new List<Bouquet> { bouquets[2] }, bouquets[2].Price,
                    "Oczekujące")
            };

            // Dodanie zainicjalizowanych danych do sklepu
            shop.Flowers.AddRange(flowers);
            shop.Bouquets.AddRange(bouquets);
            shop.Orders.AddRange(orders);
            shop.Customers.AddRange(new List<Customer>
                { customer1, customer2, customer3 });

            // Zapisanie danych do bazy danych
            await dbManager.SaveDataAsync(shop);
        }

        // Tworzenie instancji MenuHandler
        MenuHandler menuHandler = new(shopActions, shop, dbManager);

        // Pętla interaktywna
        while (true)
        {
            int selectedIndex = menuHandler.DisplayMenu();
            switch (selectedIndex)
            {
                case 0:
                    shopActions.DisplayShopInfo(shop);
                    break;
                case 1:
                    await shopActions.AddNewBouquetAsync(shop, dbManager);
                    break;
                case 2:
                    await shopActions.AddNewCustomerAsync(shop, dbManager);
                    break;
                case 3:
                    await shopActions.CreateNewOrderAsync(shop, dbManager);
                    break;
                case 4:
                    await shopActions.FulfillOrderAsync(shop, dbManager);
                    break;
                case 5:
                    shopActions.DisplayAllCustomers(shop);
                    break;
                case 6:
                    shopActions.SearchCustomerByEmail(shop);
                    break;
                case 7:
                    shopActions.DisplayAllBouquets(shop, dbManager);
                    break;
                case 8:
                    shopActions.DisplayAllFlowers(shop);
                    break;
                case 9:
                    await dbManager.SaveDataAsync(shop);
                    return;
                default:
                    Console.WriteLine("Nieprawidłowy wybór. Spróbuj ponownie.");
                    break;
            }
        }
    }
}
