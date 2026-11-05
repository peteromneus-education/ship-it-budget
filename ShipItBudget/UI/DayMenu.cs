// Visar transaktioner och CRUD-menyer för valt datum. Läs input här och låt BudgetService hantera reglerna.

using Spectre.Console;

namespace ShipItBudget.UI;

public class DayMenu
{
    private readonly BudgetService budgetService;

    public DayMenu(BudgetService budgetService)
    {
        this.budgetService = budgetService;
    }

    public void Show(DateTime selectedDate)
    {
        bool keepDisplayingTable = true;

        while (keepDisplayingTable)
        {
            Console.Clear();

            List<Transaction> transactionsForSelectedDate = budgetService.GettransactionbyDate(selectedDate);

            Table transactionsTable = new();
            transactionsTable.Title($"Transaktioner för {selectedDate:yyyy-MM-dd}");
            transactionsTable.ShowRowSeparators();
            transactionsTable.AddColumn("Typ");
            transactionsTable.AddColumn("Kategori");
            transactionsTable.AddColumn("Belopp");

            if (transactionsForSelectedDate.Count == 0)
            {
                transactionsTable.AddRow("Inga transaktioner", "", "");
            }
            else
            {
                foreach (Transaction transaction in transactionsForSelectedDate)
                {
                    string transactionTypeText = TransactionTypeText.DisplayText(transaction.Type);

                    transactionsTable.AddRow(transactionTypeText, Markup.Escape(transaction.Category),
                        $"{transaction.Amount} kr");
                }
            }

            AnsiConsole.Write(transactionsTable);
            Console.WriteLine();

            string choice = AnsiConsole.Prompt(
    new SelectionPrompt<string>()
        .Title("Välj ett alternativ:")
        .WrapAround()
        .HighlightStyle(new Style(Color.Violet, Color.Black, Decoration.RapidBlink))
        .AddChoices("Ny transaktion")
        .AddChoices("Redigera transaktion")
        .AddChoices("Tillbaka"));

            switch (choice)
            {
                case "Ny transaktion":
                    TransactionForm transactionForm = new(budgetService);
                    transactionForm.ShowAdd(selectedDate);
                    break;

                case "Redigera transaktion":
                    if (transactionsForSelectedDate.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[red]Inga transaktioner att redigera![/]");
                        Console.ReadKey();
                        break;
                    }
                    var selectedTransaction = AnsiConsole.Prompt(
                    new SelectionPrompt<Transaction>()
                        .Title("Välj en transaktion att redigera:")
                        .WrapAround()
                        .HighlightStyle(new Style(Color.Violet, Color.Black, Decoration.RapidBlink))
                        .UseConverter(transaction => $"{TransactionTypeText.DisplayText(transaction.Type)} - {Markup.Escape(transaction.Category)} - {transaction.Amount} kr")
                        .AddChoices(transactionsForSelectedDate));

                    AnsiConsole.MarkupLine($"\n[green]Vald transaktion:[/] {TransactionTypeText.DisplayText(selectedTransaction.Type)} - {Markup.Escape(selectedTransaction.Category)} - {selectedTransaction.Amount} kr");
                    Console.ReadKey(true);
                    break;

                case "Tillbaka":
                    keepDisplayingTable = false;
                    break;
            }
        }
    }
}
