using FlowerShopFromUML;
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
        MenuHandler menuHandler = new();
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
        dbManager.LoadData(shop);

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
                    shopActions.AddNewBouquet(shop, dbManager);
                    break;
                case 2:
                    shopActions.AddNewCustomer(shop, dbManager);
                    break;
                case 3:
                    shopActions.CreateNewOrder(shop, dbManager);
                    break;
                case 4:
                    shopActions.FulfillOrder(shop, dbManager);
                    break;
                case 5:
                    shopActions.DisplayAllCustomers(shop);
                    break;
                case 6:
                    shopActions.SearchCustomerByEmail(shop);
                    break;
                case 7:
                    dbManager.SaveData(shop);
                    return;
                default:
                    Console.WriteLine("Nieprawidłowy wybór. Spróbuj ponownie.");
                    break;
            }
        }
    }
}
