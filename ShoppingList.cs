// Holds the items and takes care of loading and saving them.
class ShoppingList
{
    private List<Item> items = new List<Item>();
    private string path;
    private int budgetLimit = 500;

    public ShoppingList(string path)
    {
        this.path = path;
    }

    public void Add(Item item)
    {
        if (Total() + item.Price > budgetLimit)
        {
            throw new BudgetExceededException($"Budgetgränsen på {budgetLimit} kr har överskridits.");
        }

        items.Add(item);
    }

    // Removes the item the user sees as number 1, 2, 3 ...
    public void RemoveAt(int number)
    {
        if (number < 1 || number > items.Count)
        {
            Console.WriteLine("Ange ett giltigt artikelnummer.");
            return;
        }

        items.RemoveAt(number - 1);
    }

    // Adds up the price of every item on the list.
    public int Total()
    {
        int sum = 0;

        for (int i = 0; i < items.Count; i++)
        {
            sum += items[i].Price;
        }

        return sum;
    }

    // Looks up an item by its name. Returns null if there is no such item.
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

    public void Print()
    {
        for (int i = 0; i < items.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {items[i]}");
        }

        Console.WriteLine($"Total: {Total()} kr");
    }

    // Writes one item per line, as "price;name".
    public void Save()
    {
        List<string> lines = new List<string>();

        foreach (Item item in items)
        {
            lines.Add($"{item.Price};{item.Name}");
        }

        try
        {
            File.WriteAllText(path, string.Join("\r\n", lines));
            Console.WriteLine("Listan har sparats.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Det gick inte att spara listan: {ex.Message}");
        }
    }

    // Reads the file back into the list.
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

            if (!int.TryParse(parts[0], out int price))
                continue;

            items.Add(new Item(parts[1], price));
        }
    }
}
