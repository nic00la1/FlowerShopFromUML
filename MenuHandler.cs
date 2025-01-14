using System;
using System.Collections.Generic;

public class MenuHandler
{
    public int DisplayMenu()
    {
        string[] options =
        {
            "Wyœwietl informacje o sklepie",
            "Dodaj nowy bukiet",
            "Dodaj nowego klienta",
            "Utwórz nowe zamówienie",
            "Zrealizuj zamówienie",
            "Wyœwietl wszystkich klientów",
            "Wyszukaj klienta po emailu",
            "WyjdŸ"
        };

        int selectedIndex = 0;

        ConsoleKey key;
        do
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("*** Witaj w Kwiaciarni Nicoli ***");
            Console.ResetColor();
            Console.WriteLine("Wybierz akcjê (u¿yj strza³ek):\n");

            for (int i = 0; i < options.Length; i++)
                if (i == selectedIndex)
                {
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Green;
                    Console.WriteLine($">> {options[i]} <<");
                    Console.ResetColor();
                }
                else
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
