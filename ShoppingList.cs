using System.Text.Json;
using System.Text.Json.Serialization;

/// <summary>
/// Manages shopping list items, enforces the budget, and handles file storage.
/// </summary>
class ShoppingList
{
    private List<Item> items = new List<Item>();
    private string basePath;
    private string storagePath;
    private int budgetLimit = 500;

    /// <summary>
    /// Creates a shopping list with a seed file and a separate file for saved changes.
    /// </summary>
    /// <param name="basePath">The original item file used when no saved changes exist.</param>
    /// <param name="storagePath">The file path used to load and save local changes.</param>
    public ShoppingList(string basePath, string storagePath)
    {
        this.basePath = basePath;
        this.storagePath = storagePath;
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
            string choiceInput = Console.ReadLine();
            if (choiceInput == null)
            {
                return;
            }

            if (!int.TryParse(choiceInput, out int choice))
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
                if (name == null)
                {
                    return;
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine("Ange ett namn på varan.");
                    continue;
                }

                int price;
                while (true)
                {
                    Console.Write("Pris: ");
                    string priceInput = Console.ReadLine();
                    if (priceInput == null)
                    {
                        return;
                    }

                    if (int.TryParse(priceInput, out price))
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

                string numberInput = Console.ReadLine();
                if (numberInput == null)
                {
                    return;
                }

                if (!int.TryParse(numberInput, out int number))
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
                if (wanted == null)
                {
                    return;
                }

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
        if ((long)Total() + item.Price > budgetLimit)
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
    /// Saves items to the local storage file, one item per line as "price;name".
    /// </summary>
    public void Save()
    {
        StreamWriter writer = null;
        bool saveSucceeded = false;

        try
        {
            writer = new StreamWriter(storagePath);

            foreach (Item item in items)
            {
                writer.WriteLine(JsonSerializer.Serialize(new PersistedItem
                {
                    Name = item.Name,
                    Price = item.Price
                }));
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
    /// Loads valid items from local storage, or from the base file if no local save exists.
    /// </summary>
    public void Load()
    {
        string sourcePath = File.Exists(storagePath) ? storagePath : basePath;

        if (!File.Exists(sourcePath))
            return;

        string[] lines = File.ReadAllLines(sourcePath);
        long loadedTotal = Total();

        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            if (string.IsNullOrWhiteSpace(line))
                continue;

            string name;
            int price;
            try
            {
                if (line.TrimStart().StartsWith("{", StringComparison.Ordinal))
                {
                    PersistedItem savedItem = JsonSerializer.Deserialize<PersistedItem>(line);
                    if (savedItem == null)
                    {
                        Console.WriteLine($"Ogiltig rad {i + 1} i listfilen hoppades över.");
                        continue;
                    }

                    name = savedItem.Name;
                    price = savedItem.Price;
                }
                else
                {
                    int separatorIndex = line.IndexOf(';');
                    if (separatorIndex < 0 ||
                        !int.TryParse(line.Substring(0, separatorIndex), out price))
                    {
                        Console.WriteLine($"Ogiltig rad {i + 1} i listfilen hoppades över.");
                        continue;
                    }

                    name = line.Substring(separatorIndex + 1);
                }
            }
            catch (JsonException)
            {
                Console.WriteLine($"Ogiltig rad {i + 1} i listfilen hoppades över.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(name) || price < 0)
            {
                Console.WriteLine($"Ogiltig rad {i + 1} i listfilen hoppades över.");
                continue;
            }

            if (loadedTotal + price > budgetLimit)
            {
                Console.WriteLine($"Rad {i + 1} överskrider budgetgränsen och hoppades över.");
                continue;
            }

            items.Add(new Item(name, price));
            loadedTotal += price;
        }
    }

    private sealed class PersistedItem
    {
        [JsonRequired]
        public string Name { get; set; }

        [JsonRequired]
        public int Price { get; set; }
    }
}
