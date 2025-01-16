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
            "Wy�wietl informacje o sklepie",
            "Opcje bukiet�w",
            "Opcje klient�w",
            "Wyjd�"
        };

        string[] bouquetOptions =
        {
            "Dodaj nowy bukiet",
            "Wy�wietl wszystkie bukiety",
            "Powr�t do g��wnego menu"
        };

        string[] customerOptions =
        {
            "Dodaj nowego klienta",
            "Wy�wietl wszystkich klient�w",
            "Wyszukaj klienta po emailu",
            "Powr�t do g��wnego menu"
        };

        while (true)
        {
            int selectedIndex = DisplayOptions(mainOptions,
                "*** Witaj w Kwiaciarni Nicoli ***");

            switch (selectedIndex)
            {
                case 0:
                    return 0; // Wy�wietl informacje o sklepie
                case 1:
                    DisplaySubMenu(bouquetOptions, "Opcje bukiet�w");
                    break;
                case 2:
                    DisplaySubMenu(customerOptions, "Opcje klient�w");
                    break;
                case 3:
                    return 9; // Wyjd�
                default:
                    Console.WriteLine("Nieprawid�owy wyb�r. Spr�buj ponownie.");
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
                break; // Powr�t do g��wnego menu

            switch (title)
            {
                case "Opcje bukiet�w":
                    if (selectedIndex == 0)
                        ExecuteActionAsync(1).Wait(); // Dodaj nowy bukiet
                    if (selectedIndex == 1)
                        ExecuteActionAsync(7)
                            .Wait(); // Wy�wietl wszystkie bukiety
                    break;
                case "Opcje klient�w":
                    if (selectedIndex == 0)
                        ExecuteActionAsync(2).Wait(); // Dodaj nowego klienta
                    if (selectedIndex == 1)
                        ExecuteActionAsync(5)
                            .Wait(); // Wy�wietl wszystkich klient�w
                    if (selectedIndex == 2)
                        ExecuteActionAsync(6)
                            .Wait(); // Wyszukaj klienta po emailu
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
            Console.WriteLine("Wybierz akcj� (u�yj strza�ek):\n");

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

    private async Task ExecuteActionAsync(int action)
    {
        switch (action)
        {
            case 0:
                _shopActions.DisplayShopInfo(_shop);
                break;
            case 1:
                await _shopActions.AddNewBouquetAsync(_shop, _dbManager);
                break;
            case 2:
                await _shopActions.AddNewCustomerAsync(_shop, _dbManager);
                break;
            case 3:
                await _shopActions.CreateNewOrderAsync(_shop, _dbManager);
                break;
            case 4:
                await _shopActions.FulfillOrderAsync(_shop, _dbManager);
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
                await _dbManager.SaveDataAsync(_shop);
                break;
            default:
                Console.WriteLine("Nieprawid�owy wyb�r. Spr�buj ponownie.");
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
