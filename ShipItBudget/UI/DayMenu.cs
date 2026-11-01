// Visar transaktioner och CRUD-menyer för valt datum. Läs input här och låt BudgetService hantera reglerna.

using Spectre.Console;

namespace ShipItBudget.UI;

public class DayMenu
{
    public static void Show(DateTime selectedDate, List<ExampleTransaction> transactionsForSelectedDate)
    {
        Table transactionsTable = new();

        transactionsTable.Title($"Transaktioner för {selectedDate:yyyy-MM-dd}");
        transactionsTable.Caption("Tryck valfri tangent för att återgå till kalendern.");
        transactionsTable.ShowRowSeparators();
        transactionsTable.AddColumn("Typ");
        transactionsTable.AddColumn("Belopp");

        if (transactionsForSelectedDate.Count == 0)
        {
            transactionsTable.AddRow("Inga transaktioner", "");
        }
        else
        {
            foreach (ExampleTransaction transaction in transactionsForSelectedDate)
            {
                string transactionTypeText;

                if (transaction.IsIncome)
                {
                    transactionTypeText = "Inkomst";
                }
                else
                {
                    transactionTypeText = "Utgift";
                }

                transactionsTable.AddRow(
                transactionTypeText,
                $"{transaction.Amount} kr");
            }
        }

        AnsiConsole.Write(transactionsTable);
        Console.ReadKey(true);
        Console.Clear();
    }
}
