# Programming and Object-Oriented Development in C#

## Knowledge Check 2: Robust Shopping List

### README Requirements

Your README must contain three things:

## 1. Bug Report

Describe the six bugs:

* What happened?
* Why did it happen?
* How did you fix it?

A few lines per bug are sufficient.

## 2. Design Decision

Explain how `Add` rejects an item when the budget limit would be exceeded, and why you chose that approach.

## 3. Class Diagram

Include a simple diagram of the program after your changes.

Three boxes are sufficient.

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




