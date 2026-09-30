# Programming and Object-Oriented Development in C#

## Knowledge Check 2: Robust Shopping List

### Purpose

This assignment assesses two things:

1. Your ability to read code written by someone else, understand what it does, and identify its flaws.
2. Your ability to make a program robust so that it does not crash when real-world situations differ from what the programmer originally expected.

The assignment is based on the topics we have covered regarding exception handling, as well as your existing knowledge of classes and objects.

### Structure and Submission

The assignment consists of two parts, which must be submitted in a **public GitHub repository**:

* **Part 1 – Fix a broken program**
* **Part 2 – Extend the program**

You will start from a provided codebase. It compiles successfully without any errors — but it does not work correctly.

You will start from a provided codebase. It compiles successfully without any errors — but it does not work correctly.

# Part 1 — Fix the Program

The starter code is a shopping list application that stores its items in a text file between runs. The program contains **six bugs**:

* Four cause the program to crash.
* One produces incorrect results without crashing.
* One hides the fact that something went wrong.

Your task is to find all six issues, fix them, and describe them in your **README**.

## Getting Started

Start the program. It crashes immediately — read the entire error message and stack trace, not just the first line.

* Enter letters when the program asks for a number.
* Try removing an item that does not exist.
* Rename `items.txt` and start the program again.
* Add an item, save, exit, and restart. Does the list look the same?
* Search for an item that you can see in the list.
* Calculate the total cost manually and compare it with the program’s result.
* Read the code. Not all six bugs reveal themselves during execution.

## Requirements for Passing (Part 1)

* [ ] The program does not crash, regardless of what the user enters in the menu, the price field, or the item number.
* [ ] The program starts even if `items.txt` is missing.
* [ ] Saved data can be loaded correctly — the list looks the same after restarting the program.
* [ ] An item that appears in the list can also be found when searching by name.
* [ ] The total cost is correct.
* [ ] No `catch` block is empty, and the program does not claim that something succeeded when it actually failed.
* [ ] `TryParse` is used for input that is expected to be invalid at times.
* [ ] Existing `catch` blocks handle specific exception types rather than catching only `Exception`.

# Part 2 — Extend the Program

## Item Must Protect Itself

The constructor should reject invalid values instead of silently creating a broken object:

* Empty name — throw an `ArgumentException`.
* Negative price — throw an `ArgumentOutOfRangeException`.

## The List Must Have a Budget Limit

`ShoppingList` should have a maximum budget limit that the total value of the list cannot exceed. An item that would cause the budget to be exceeded must not be added.

How `Add` rejects the operation is your design choice. The topic of exception handling specifically addresses this type of question:

For example, in a simple banking application, a withdrawal might return `false` when there are insufficient funds.

The question is how the caller needs to handle the situation.

Should you throw an exception or return `false`? Choose one approach and justify your decision in the **README**.

There is no single correct answer, but there is a follow-up question:

**What does `Program.cs` need to do with the result?**

`Program.cs` must handle both the invalid object and the exceeded budget limit. The user should receive a clear message, and the program should continue running.

## Requirements for Passing (Part 2)

* [ ] `Item` rejects empty names and negative prices by throwing exceptions.
* [ ] The budget limit is enforced — an item that exceeds the limit is not added.
* [ ] The program does not crash when either of these situations occurs, and the user is informed about what happened.
* [ ] The README explains how you chose to handle an exceeded budget limit and why.

## Extra (Optional, Does Not Affect the Grade)

* Create a custom exception type that inherits from `Exception`.
* Save the budget limit in the file together with the items.
* Use `finally` or `using` where it provides value.