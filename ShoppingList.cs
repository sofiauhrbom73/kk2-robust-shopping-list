/// <summary>
/// Manages shopping list items, enforces the budget, and handles file storage.
/// </summary>
class ShoppingList
{
    private List<Item> items = new List<Item>();
    private string path;
    private int budgetLimit = 500;

    /// <summary>
    /// Creates a shopping list that stores its items at the specified file path.
    /// </summary>
    /// <param name="path">The file path used to load and save items.</param>
    public ShoppingList(string path)
    {
        this.path = path;
    }

    /// <summary>
    /// Loads saved items and runs the interactive shopping-list menu.
    /// </summary>
    public void Run()
    {
        Load();

        // Keep accepting menu choices until the user selects Exit.
        while (true)
        {
            Console.WriteLine();
            Print();
            Console.WriteLine();
            Console.WriteLine("1. Lägg till vara");
            Console.WriteLine("2. Ta bort vara");
            Console.WriteLine("3. Spara");
            Console.WriteLine("4. Sök vara");
            Console.WriteLine("5. Avsluta");
            Console.Write("Välj: ");

            // TryParse lets the program handle non-numeric input without crashing.
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

                int price;
                while (true)
                {
                    Console.Write("Pris: ");
                    if (int.TryParse(Console.ReadLine(), out price))
                    {
                        break;
                    }

                    Console.WriteLine("Ange ett giltigt pris.");
                }

                // Item validation and budget errors are shown to the user; the menu continues.
                try
                {
                    Add(new Item(name, price));
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

                RemoveAt(number);
            }
            else if (choice == 3)
            {
                Save();
            }
            else if (choice == 4)
            {
                Console.Write("Namn att söka efter: ");
                string wanted = Console.ReadLine();
                Item found = Find(wanted);

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
    }

    /// <summary>
    /// Adds an item unless doing so would exceed the budget limit.
    /// </summary>
    /// <param name="item">The item to add.</param>
    /// <exception cref="BudgetExceededException">The item would exceed the budget limit.</exception>
    public void Add(Item item)
    {
        if (Total() + item.Price > budgetLimit)
        {
            throw new BudgetExceededException($"Budgetgränsen på {budgetLimit} kr har överskridits.");
        }

        items.Add(item);
    }

    /// <summary>
    /// Removes an item using the 1-based number shown to the user.
    /// </summary>
    /// <param name="number">The item's displayed number.</param>
    public void RemoveAt(int number)
    {
        if (number < 1 || number > items.Count)
        {
            Console.WriteLine("Ange ett giltigt artikelnummer.");
            return;
        }

        items.RemoveAt(number - 1);
    }

    /// <summary>
    /// Calculates the combined price of all items.
    /// </summary>
    /// <returns>The total price in kronor.</returns>
    public int Total()
    {
        int sum = 0;

        for (int i = 0; i < items.Count; i++)
        {
            sum += items[i].Price;
        }

        return sum;
    }

    /// <summary>
    /// Finds the first item with the specified name.
    /// </summary>
    /// <param name="name">The name to search for.</param>
    /// <returns>The matching item, or null if no match exists.</returns>
    public Item Find(string name)
    {
        foreach (Item item in items)
        {
            if (item.Name == name)
            {
                return item;
            }
        }

        return null;
    }

    /// <summary>
    /// Prints each item and the current total to the console.
    /// </summary>
    public void Print()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {items[i]}");
        }

        Console.WriteLine($"Total: {Total()} kr");
    }

    /// <summary>
    /// Saves items to the configured file, one item per line as "price;name".
    /// </summary>
    public void Save()
    {
        StreamWriter writer = null;
        bool saveSucceeded = false;

        try
        {
            writer = new StreamWriter(path);

            foreach (Item item in items)
            {
                writer.WriteLine($"{item.Price};{item.Name}");
            }

            writer.Flush();
            saveSucceeded = true;
        }
        catch (IOException ex)
        {
            Console.WriteLine($"Det gick inte att spara listan: {ex.Message}");
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Du har inte behörighet att spara listan: {ex.Message}");
        }
        finally
        {
            // Always close the file, including when writing fails.
            if (writer != null)
            {
                try
                {
                    writer.Dispose();
                }
                catch (IOException ex)
                {
                    saveSucceeded = false;
                    Console.WriteLine($"Det gick inte att stänga filen: {ex.Message}");
                }
                catch (UnauthorizedAccessException ex)
                {
                    saveSucceeded = false;
                    Console.WriteLine($"Du har inte behörighet att stänga filen: {ex.Message}");
                }
            }
        }

        if (saveSucceeded)
        {
            Console.WriteLine("Listan har sparats.");
        }
    }

    /// <summary>
    /// Loads valid items from the configured file; does nothing if the file is missing.
    /// </summary>
    public void Load()
    {
        if (!File.Exists(path))
            return;

        string[] lines = File.ReadAllLines(path);

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string[] parts = line.Split(';');

            if (parts.Length != 2)
                continue;

            // Skip malformed rows rather than adding invalid item data.
            if (!int.TryParse(parts[0], out int price))
                continue;

            items.Add(new Item(parts[1], price));
        }
    }
}
