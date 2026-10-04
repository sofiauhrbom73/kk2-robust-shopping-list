### 1. Bug Report

Describe the six bugs:

* What happened?
* Why did it happen?
* How did you fix it?

A few lines per bug are sufficient.

## Bug 1

* What happened?

This tells me that the file is found and the problem is in line 90 in ShoppingList.cs.

Unhandled exception. System.IndexOutOfRangeException: Index was outside the bounds of the array.
   at ShoppingList.Load() in C:\Users\Sofia\OneDrive\Dokument\Repository\kk2-robust-shopping-list\ShoppingList.cs:line 90
   at Program.<Main>$(String[] args) in C:\Users\Sofia\OneDrive\Dokument\Repository\kk2-robust-shopping-list\Program.cs:line 2

* Why did it happen?

Search for error with print in Load method:

 public void Load()
    {
        string text = File.ReadAllText(path);
        string[] lines = text.Split('\n');

        foreach (string line in lines)
        {
            Console.WriteLine($"'{line}'");

            string[] parts = line.Split(';');
            Console.WriteLine($"Length = {parts.Length}");

            items.Add(new Item(parts[1], int.Parse(parts[0])));
        }
    }

This tells me that the last row is empty or not correct:

'15;Mjölk
Length = 2
'32;Bröd
Length = 2
'89;Ost
Length = 2
''
Length = 1

* How did you fix it?

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

This version improves the robustness of the program by handling several potential error conditions:

It does not crash if the file is missing.
It does not crash when encountering empty lines in the file.
It does not crash if a line does not contain the expected semicolon separator.
It does not crash if the price value cannot be parsed as a valid integer.

The most likely cause of the IndexOutOfRangeException is that parts[1] is accessed without first verifying that the array contains at least two elements. If parts.Length < 2, attempting to access parts[1] will throw an IndexOutOfRangeException.

Result from dotnet run:

1. Mjölk - 15 kr
2. Bröd - 32 kr
3. Ost - 89 kr
Totalt: 121 kr

1. Lägg till vara
2. Ta bort vara
3. Spara
4. Sök vara
5. Avsluta
Välj:


## Bug 2

* What happened?

After fixing the first bug, I see that the total amount is wrong.

* Why did it happen?

Because, the first item in the list is excluded from the total calculation.

for (int i = 1; i < items.Count; i++)

This is likely the defect referred to in the assignment as "produces an incorrect result without causing the program to crash."

public int Total()
    {
        int sum = 0;

        for (int i = 1; i < items.Count; i++)
        {
            sum += items[i].Price;
        }

        return sum;
    }

* How did you fix it?

Start to count from 0.

for (int i = 0; i < items.Count; i++)

## Bug 3

* What happened?

The Save() method appends an extra line break at the end of the file:

File.WriteAllText(path, string.Join("\r\n", lines) + "\r\n");

This creates an empty line at the end of the file. When the file is loaded, the empty line is split and processed as data, causing an IndexOutOfRangeException.

* Why did it happen?

The file was saved with an empty line.

* How did you fix it?

I fixed the issue by removing the unnecessary trailing line break:

File.WriteAllText(path, string.Join("\r\n", lines));

I also have the validation in Load from the first bug fix to skip empty lines and malformed records.

## Bug 4

* What happened?

The program displayed the message:

Console.WriteLine("Listan är sparad.");

even when the file was not successfully saved.

* Why did it happen?

The Save() method contains an empty catch block:

try
{
    File.WriteAllText(path, string.Join("\r\n", lines));
}
catch
{
}

This catches and suppresses all exceptions without reporting them. As a result, if an error occurs during the save operation (for example, an invalid path, missing directory, or insufficient permissions), the exception is ignored and the program continues execution.

The user is then shown the message:

Console.WriteLine("Listan är sparad.");

which incorrectly indicates that the save operation was successful.

* How did you fix it?

try
{
    File.WriteAllText(path, string.Join("\r\n", lines));
    Console.WriteLine("The list has been saved.");
}
catch (Exception ex)
{
    Console.WriteLine($"Failed to save the list: {ex.Message}");
}

This solution ensures that save errors are visible to the user and prevents the program from falsely reporting a successful save operation.

## Bug 5

* What happened?

I got an ArgumentOutOfRangeException when trying to remove an item that was not in the list.

1. Lägg till vara
2. Ta bort vara
3. Spara
4. Sök vara
5. Avsluta
Välj: 2
Nummer: 0
Unhandled exception. System.ArgumentOutOfRangeException: Index was out of range. Must be non-negative and less than the size of the collection. (Parameter 'index')
   at System.Collections.Generic.List`1.RemoveAt(Int32 index)
   at ShoppingList.RemoveAt(Int32 number) in C:\Users\Sofia\OneDrive\Dokument\Repository\kk2-robust-shopping-list\ShoppingList.cs:line 20
   at Program.<Main>$(String[] args) in C:\Users\Sofia\OneDrive\Dokument\Repository\kk2-robust-shopping-list\Program.cs:line 30

* Why did it happen?

The problem is that RemoveAt() assumes the user always enters a valid number.

* How did you fix it?

public void RemoveAt(int number)
{
    if (number < 1 || number > items.Count)
    {
        Console.WriteLine("Please enter a valid item number.");
        return;
    }

    items.RemoveAt(number - 1);
}

## Bug 6

* What happened?

The program crashed when the I entered invalid input in the menu, for example: abc or pressed Enter.

1. Lägg till vara
2. Ta bort vara
3. Spara
4. Sök vara
5. Avsluta
Välj: abc
Unhandled exception. System.FormatException: The input string 'abc' was not in a correct format.
   at System.Number.ThrowFormatException[TChar](ReadOnlySpan`1 value)
   at System.Int32.Parse(String s)
   at Program.<Main>$(String[] args) in C:\Users\Sofia\OneDrive\Dokument\Repository\kk2-robust-shopping-list\Program.cs:line 16

* Why did it happen?

The code converts user input directly to an integer without validation:

int choice = int.Parse(Console.ReadLine());

* How did you fix it?

if (!int.TryParse(Console.ReadLine(), out int choice))
{
    Console.WriteLine("Please enter a valid number.");
    continue;
}