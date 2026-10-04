# Programming and Object-Oriented Development in C#

## Knowledge Check 2: Robust Shopping List

### README Requirements

Your README must contain three things:

## 1. Bug Report

Link to Bugreport.md

[Bug report](Bugreport.md)

## 2. Design Decision

`ShoppingList` has a budget limit of 500 kr. Before adding an item, `Add` checks whether the current total plus the item's price would exceed this limit. If it would, `Add` throws a `BudgetExceededException` and does not add the item.

I chose an exception because exceeding the budget means the requested operation cannot be completed. The custom exception makes this situation clear to the caller. `ShoppingList.Run` catches it, displays the message to the user, and continues running. The same `try` block also handles invalid item values rejected by the `Item` constructor.

In `ShoppingList.Save`, a `finally` block disposes the `StreamWriter` so the file is closed whether saving succeeds or fails. The success message is only shown if writing and closing the file both succeed.

The original `items.txt` is kept as the starter list. Saved changes go to `items.local.txt`, which is ignored by Git. This keeps the starter file unchanged while allowing saved changes to persist between runs on this computer.

## 3. Class Diagram

```text
+------------------------+          +-------------------------------+
|        Program         | uses     |          ShoppingList          |
+------------------------+--------->+-------------------------------+
| Starts the app         |          | Runs menu and handles input     |
| Calls ShoppingList.Run |          | items: List<Item>              |
+------------------------+          | budgetLimit: int                |
                                    | Add(item): checks budget        |
                                    | Total(): int                    |
                                    +---------------+---------------+
                                                    | contains
                                                    v
                                    +-------------------------------+
                                    |             Item              |
                                    +-------------------------------+
                                    | Name: string                  |
                                    | Price: int                    |
                                    | Validates name and price      |
                                    +-------------------------------+

                 ShoppingList.Add may throw BudgetExceededException
```

# Code review
My check of the code before running the program.

1. ShoppingList.cs
 // Reads the file back into the list.
    public void Load()
    {
        string text = File.ReadAllText(path);
        string[] lines = text.Split('\n');

        foreach (string line in lines)
        {
            string[] parts = line.Split(';');
            items.Add(new Item(parts[1], int.Parse(parts[0])));
        }
    }
Need to handle FormatException and FileNotFoundException

2. ShoppingList.cs
// Removes the item the user sees as number 1, 2, 3 ...
    public void RemoveAt(int number)
    {
        items.RemoveAt(number - 1);
    }
Need to handle possible ArgumentOutOfRangeException

3. Program.cs
ShoppingList list = new ShoppingList("items.txt");
Path. Risk if item.txt is not copied to working dir. 

4. Program.cs
Menu choice
 int choice = int.Parse(Console.ReadLine());
Need to handle FormatException

5. Program.cs
if (choice == 1)
    {
        Console.Write("Namn: ");
        string name = Console.ReadLine();
        Console.Write("Pris: ");
        int price = int.Parse(Console.ReadLine());
        list.Add(new Item(name, price));
    }
    else if (choice == 2)
    {
        Console.Write("Nummer: ");
        int number = int.Parse(Console.ReadLine());
        list.RemoveAt(number);
    }
Need to handle FormatException, two places.

6. ShoppingList.cs
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
            File.WriteAllText(path, string.Join("\r\n", lines) + "\r\n");
        }
        catch
        {
        }

        Console.WriteLine("Listan är sparad.");
    }
Need to handle FileNotFoundException. If we cannot find the file, the item will not be saved, because we
dont catch the exception and the message will be "Listan är sparad". Wrong information. 

Let see what I find out after running the program.
