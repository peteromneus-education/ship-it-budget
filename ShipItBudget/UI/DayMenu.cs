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

            List<Transaction> transactionsForSelectedDate = budgetService.GetTransactionbyDate(selectedDate);

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
        .AddChoices("Radera transaktion")
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
                        Console.ReadKey(true);
                        break;
                    }

                    Transaction transactionToEdit = ChooseTransaction(transactionsForSelectedDate, "Välj en transaktion att redigera:");
                    TransactionForm editForm = new(budgetService);
                    editForm.ShowEdit(transactionToEdit);
                    break;

                case "Radera transaktion":
                    if (transactionsForSelectedDate.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[red]Inga transaktioner att radera![/]");
                        Console.ReadKey(true);
                        break;
                    }

                    Transaction transactionToDelete = ChooseTransaction(transactionsForSelectedDate, "Välj en transaktion att radera:");

                    if (ConfirmDelete(transactionToDelete))
                    {
                        bool deleted = budgetService.DeleteTransaction(transactionToDelete.Id) == ServiceResult.Success;

                        if (deleted)
                        {
                            AnsiConsole.MarkupLine("\n[green]Transaktionen raderades.[/]");
                        }
                        else
                        {
                            AnsiConsole.MarkupLine("\n[red]Transaktionen kunde inte hittas.[/]");
                        }

                        Console.ReadKey(true);
                    }
                    break;

                case "Tillbaka":
                    keepDisplayingTable = false;
                    break;
            }
        }
    }

    private Transaction ChooseTransaction(List<Transaction> transactions, string title)
    {
        return AnsiConsole.Prompt(
            new SelectionPrompt<Transaction>()
                .Title(title)
                .WrapAround()
                .HighlightStyle(new Style(Color.Violet, Color.Black, Decoration.RapidBlink))
                .UseConverter(DescribeTransaction)
                .AddChoices(transactions));
    }

    private bool ConfirmDelete(Transaction transaction)
    {
        ConfirmationPrompt prompt = new ConfirmationPrompt($"Vill du radera {DescribeTransaction(transaction)}?")
        {
            DefaultValue = false // Enter betyder "nej", så att inget raderas av misstag
        };

        return AnsiConsole.Prompt(prompt);
    }

    private static string DescribeTransaction(Transaction transaction)
    {
        return $"{TransactionTypeText.DisplayText(transaction.Type)} - {Markup.Escape(transaction.Category)} - {transaction.Amount} kr";
    }
}
