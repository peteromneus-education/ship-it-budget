




namespace ShipItBudget.UI;

public class TransactionMenu
{
    public static void Show(
        BudgetService budgetService,
        CategoryService categoryService)
    {
        Console.WriteLine("\nVälj transaktionstyp:");
        Console.WriteLine("1. Inkomst");
        Console.WriteLine("2. Utgift");

        string choice = Console.ReadLine() ?? "";

        TransactionType type;

        if (choice == "1")
        {
            type = TransactionType.Income;
        }
        else if (choice == "2")
        {
            type = TransactionType.Expense;
        }
        else
        {
            Console.WriteLine("Ogiltigt val!");
            return;
        }

        // Hämtar kategorier för  transaktionstyperna
        List<string> categories =
            categoryService.GetCategoriesByType(type);

        Console.WriteLine("\nVälj kategori:");

        for (int i = 0; i < categories.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {categories[i]}");
        }

        if (!int.TryParse(Console.ReadLine(), out int categoryChoice))
        {
            Console.WriteLine("Ogiltigt val!");
            return;
        }

        if (categoryChoice < 1 || categoryChoice > categories.Count)
        {
            Console.WriteLine("Kategorin finns inte!");
            return;
        }

        string selectedCategory = categories[categoryChoice - 1];

        Console.Write("\nAnge belopp: ");

        if (!decimal.TryParse(Console.ReadLine(), out decimal amount))
        {
            Console.WriteLine("Ogiltigt belopp!");
            return;
        }

        // Sparar transaktionen
        bool added = budgetService.AddTransaction(
            amount,
            type,
            selectedCategory);

        if (added)
        {
            Console.WriteLine("\nTransaktionen sparades!");
        }
        else
        {
            Console.WriteLine("\nTransaktionen kunde inte sparas!");
        }
    }
}
