using System;
using System.Collections.Generic;
using FlowerShopFromUML;

public class MenuHandler
{
    private readonly ShopActions _shopActions;
    private readonly Shop _shop;
    private readonly DatabaseManager _dbManager;

    public MenuHandler(ShopActions shopActions,
                       Shop shop,
                       DatabaseManager dbManager
    )
    {
        _shopActions = shopActions;
        _shop = shop;
        _dbManager = dbManager;
    }

    public int DisplayMenu()
    {
        string[] mainOptions =
        {
            "Wyœwietl informacje o sklepie",
            "Opcje bukietów",
            "Opcje klientów",
            "WyjdŸ"
        };

        string[] bouquetOptions =
        {
            "Dodaj nowy bukiet",
            "Wyœwietl wszystkie bukiety",
            "Powrót do g³ównego menu"
        };

        string[] customerOptions =
        {
            "Dodaj nowego klienta",
            "Wyœwietl wszystkich klientów",
            "Wyszukaj klienta po emailu",
            "Powrót do g³ównego menu"
        };

        while (true)
        {
            int selectedIndex = DisplayOptions(mainOptions,
                "*** Witaj w Kwiaciarni Nicoli ***");

            switch (selectedIndex)
            {
                case 0:
                    return 0; // Wyœwietl informacje o sklepie
                case 1:
                    DisplaySubMenu(bouquetOptions, "Opcje bukietów");
                    break;
                case 2:
                    DisplaySubMenu(customerOptions, "Opcje klientów");
                    break;
                case 3:
                    return 9; // WyjdŸ
                default:
                    Console.WriteLine("Nieprawid³owy wybór. Spróbuj ponownie.");
                    break;
            }
        }
    }

    private void DisplaySubMenu(string[] options, string title)
    {
        while (true)
        {
            int selectedIndex = DisplayOptions(options, title);

            if (selectedIndex == options.Length - 1)
                break; // Powrót do g³ównego menu

            switch (title)
            {
                case "Opcje bukietów":
                    if (selectedIndex == 0)
                        ExecuteAction(1); // Dodaj nowy bukiet
                    if (selectedIndex == 1)
                        ExecuteAction(7); // Wyœwietl wszystkie bukiety
                    break;
                case "Opcje klientów":
                    if (selectedIndex == 0)
                        ExecuteAction(2); // Dodaj nowego klienta
                    if (selectedIndex == 1)
                        ExecuteAction(5); // Wyœwietl wszystkich klientów
                    if (selectedIndex == 2)
                        ExecuteAction(6); // Wyszukaj klienta po emailu
                    break;
            }
        }
    }

    private int DisplayOptions(string[] options, string title)
    {
        int selectedIndex = 0;
        ConsoleKey key;

        do
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(title);
            Console.ResetColor();
            Console.WriteLine("Wybierz akcjê (u¿yj strza³ek):\n");

            for (int i = 0; i < options.Length; i++)
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.WriteLine($">> {options[i]} <<\n");
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

    private void ExecuteAction(int action)
    {
        switch (action)
        {
            case 0:
                _shopActions.DisplayShopInfo(_shop);
                break;
            case 1:
                _shopActions.AddNewBouquet(_shop, _dbManager);
                break;
            case 2:
                _shopActions.AddNewCustomer(_shop, _dbManager);
                break;
            case 3:
                _shopActions.CreateNewOrder(_shop, _dbManager);
                break;
            case 4:
                _shopActions.FulfillOrder(_shop, _dbManager);
                break;
            case 5:
                _shopActions.DisplayAllCustomers(_shop);
                break;
            case 6:
                _shopActions.SearchCustomerByEmail(_shop);
                break;
            case 7:
                _shopActions.DisplayAllBouquets(_shop, _dbManager);
                break;
            case 8:
                _shopActions.DisplayAllFlowers(_shop);
                break;
            case 9:
                _dbManager.SaveData(_shop);
                break;
            default:
                Console.WriteLine("Nieprawid³owy wybór. Spróbuj ponownie.");
                break;
        }
    }

    private void DrawRose()
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
}
