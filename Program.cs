ShoppingList list = new ShoppingList("items.txt");
list.Load();

while (true)
{
    Console.WriteLine();
    list.Print();
    Console.WriteLine();
    Console.WriteLine("1. Lägg till vara");
    Console.WriteLine("2. Ta bort vara");
    Console.WriteLine("3. Spara");
    Console.WriteLine("4. Sök vara");
    Console.WriteLine("5. Avsluta");
    Console.Write("Välj: ");

    if (!int.TryParse(Console.ReadLine(), out int choice))
    {
        Console.WriteLine("Ange ett giltigt nummer.");
        continue;
    }

    if (choice < 1 || choice > 5)
    {
        Console.WriteLine("Välj ett giltigt menyalternativ.");
        continue;
    }

    if (choice == 1)
    {
        Console.Write("Namn: ");
        string name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Ange ett namn på varan.");
            continue;
        }
        Console.Write("Pris: ");
        if (!int.TryParse(Console.ReadLine(), out int price))
        {
            Console.WriteLine("Ange ett giltigt pris.");
            continue;
        }

        try
        {
            list.Add(new Item(name, price));
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message);
        }
        catch (BudgetExceededException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    else if (choice == 2)
    {
        Console.Write("Nummer: ");

        if (!int.TryParse(Console.ReadLine(), out int number))
        {
            Console.WriteLine("Ange ett giltigt artikelnummer.");
            continue;
        }

        list.RemoveAt(number);
    }
    else if (choice == 3)
    {
        list.Save();
    }
    else if (choice == 4)
    {
        Console.Write("Namn att söka efter: ");
        string wanted = Console.ReadLine();
        Item found = list.Find(wanted);

        if (found == null)
        {
            Console.WriteLine("Varan finns inte i listan.");
        }
        else
        {
            Console.WriteLine($"Hittade: {found}");
        }
    }
    else if (choice == 5)
    {
        break;
    }
}
