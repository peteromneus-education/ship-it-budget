using System.Globalization;
using Spectre.Console;

namespace ShipItBudget.UI
{
    public class TransactionForm
    {
        private readonly BudgetService budgetService;

        public TransactionForm(BudgetService budgetService)
        {
            this.budgetService = budgetService;
        }

        public void ShowAdd(DateTime formDate)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold]LÄGG TILL NY TRANSAKTION[/]\n");

            TransactionType? type = AskType(null, true);
            if (type == null) return;

            decimal amount = AskAmount(null);
            string category = AskCategory(type.Value, null);
            DateTime date = AskDate(formDate);



             // fråga ja elr nej innan man spara
            bool confirm = AnsiConsole.Prompt(
            new SelectionPrompt<bool>()
            .Title("\nVill du spara transaktionen?")
            .AddChoices(true, false)
            .UseConverter(choice => choice ? "Ja" : "Nej"));

            if (!confirm)
            {
            AnsiConsole.MarkupLine("[yellow]Åtgärden avbröts. Inget sparades.[/]");
            Thread.Sleep(1000);
            return; // Hoppar ur metoden utan att uppdatera
            }

            bool success = budgetService.AddTransaction(amount, type.Value, category, date) == ServiceResult.Success;

            ShowResult(success, "Transaktion tillagd!", "Fel vid tillägg av transaktion!");
        }

        public void ShowEdit(Transaction transaction)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold]REDIGERA TRANSAKTION[/]\n");
            AnsiConsole.MarkupLine("[grey]Tryck Enter för att behålla nuvarande värde.[/]\n");

            TransactionType? type = AskType(transaction.Type, true);
            if (type == null) return;

            decimal amount = AskAmount(transaction.Amount);
            string category = AskCategory(type.Value, transaction.Category);
            DateTime date = AskDate(transaction.Date);



            // fråga ja elr nej innan man spara
            bool confirm = AnsiConsole.Prompt(
            new SelectionPrompt<bool>()
            .Title("\nVill du spara transaktionen?")
            .AddChoices(true, false)
            .UseConverter(choice => choice ? "Ja" : "Nej"));

            if (!confirm)
            {
            AnsiConsole.MarkupLine("[yellow]Åtgärden avbröts. Inget sparades.[/]");
            Thread.Sleep(1000);
            return; // Hoppar ur metoden utan att uppdatera
            }

            bool success = budgetService.UpdateTransaction(transaction.Id, amount, type.Value, date, category) == ServiceResult.Success;
            ShowResult(success, "Transaktion uppdaterad!", "Fel vid uppdatering av transaktion!"); //Ge olika text beroende om de e true elr false på Success
        }

        private decimal AskAmount(decimal? currentAmount)
        {
            TextPrompt<decimal> prompt = new TextPrompt<decimal>("Ange belopp:")
            {
                Culture = new CultureInfo("sv-SE") // Decimalkomma, t.ex. 12,50
            }
                .Validate(amount => amount > 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Beloppet måste vara större än 0[/]"));

            if (currentAmount != null)
            {
                prompt.DefaultValue(currentAmount.Value);
            }

            return AnsiConsole.Prompt(prompt);
        }

       /*  private TransactionType? AskType(TransactionType? currentType, bool allowCancel = false)
        {
            const string backLabel = "[red]Gå tillbaka[/]";
            string incomeLabel = TransactionTypeText.DisplayText(TransactionType.Income);
            string expenseLabel = TransactionTypeText.DisplayText(TransactionType.Expense);

            var prompt = new SelectionPrompt<string>()
                .Title("Välj typ:")
                .WrapAround()
                .HighlightStyle(new Style(Color.Violet, Color.Black, Decoration.RapidBlink));

            if (allowCancel)
            {
                prompt.AddChoice(backLabel);
            }

            prompt.AddChoices(incomeLabel, expenseLabel);

            if (currentType != null)
            {
                prompt.DefaultValue(TransactionTypeText.DisplayText(currentType.Value));
            }

            string result = AnsiConsole.Prompt(prompt);

            if (result == backLabel) return null;
            if (result == incomeLabel) return TransactionType.Income;
            return TransactionType.Expense;
        } */

        private TransactionType? AskType(TransactionType? currentType, bool allowCancel = false)
        {
            TransactionType[] types = { TransactionType.Income, TransactionType.Expense };
            string[] labels = { "Inkomst", "Utgift" };

            int selectedIndex = currentType == TransactionType.Expense ? 1 : 0;
            TransactionType? result = null;

            AnsiConsole.Live(RenderMenu(selectedIndex))
                .Start(ctx =>
                {
                    ctx.Refresh(); // rita menyn första gången

                    while (true)
                    {
                        var key = Console.ReadKey(true);

                        if (key.Key == ConsoleKey.UpArrow)
                        {
                            selectedIndex = (selectedIndex - 1 + types.Length) % types.Length;
                            ctx.UpdateTarget(RenderMenu(selectedIndex));
                            ctx.Refresh();
                        }
                        else if (key.Key == ConsoleKey.DownArrow)
                        {
                            selectedIndex = (selectedIndex + 1) % types.Length;
                            ctx.UpdateTarget(RenderMenu(selectedIndex));
                            ctx.Refresh();
                        }
                        else if (key.Key == ConsoleKey.Enter)
                        {
                            result = types[selectedIndex];
                            break;
                        }
                        else if (key.Key == ConsoleKey.Escape && allowCancel)
                        {
                            result = null;
                            break;
                        }
                    }
                });

            return result;

            Panel RenderMenu(int index)
            {
                var lines = labels.Select((label, i) =>
                    i == index ? $"[violet]> {label}[/]" : $"  {label}");

                string cancelText = allowCancel ? " [grey](Tryck ESC för att backa)[/]" : "";

                return new Panel(new Markup(string.Join("\n", lines)))
                    .Header($"Välj typ{cancelText}")
                    .BorderColor(Color.Violet);
            }
        }




        private string AskCategory(TransactionType type, string? currentCategory)
        {
            string[] availableCategories = type == TransactionType.Income
                ? Categories.Income
                : Categories.Expense;

            SelectionPrompt<string> prompt = new SelectionPrompt<string>()
                .Title("Välj kategori:")
                .WrapAround()
                .HighlightStyle(new Style(Color.Violet, Color.Black, Decoration.RapidBlink))
                .AddChoices(availableCategories);

            if (currentCategory != null && availableCategories.Contains(currentCategory))
            {
                prompt.DefaultValue(currentCategory);
            }

            return AnsiConsole.Prompt(prompt);
        }


        private DateTime AskDate(DateTime defaultDate)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<DateTime>("Ange datum:")
                    .DefaultValue(defaultDate)
                    .ShowDefaultValue());
        }

        private static void ShowResult(bool success, string successMessage, string errorMessage)
        {
            if (success)
            {
                AnsiConsole.MarkupLine($"\n[green]{Markup.Escape(successMessage)}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"\n[red]{Markup.Escape(errorMessage)}[/]");
            }

            AnsiConsole.MarkupLine("[grey]Tryck valfri tangent för att fortsätta.[/]");
            Console.ReadKey(true);
        }
    }
}
