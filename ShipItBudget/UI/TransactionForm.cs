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
            Console.WriteLine("LÄGG TILL NY TRANSAKTION\n");

            decimal amount = AskAmount(null);
            TransactionType type = AskType(null);
            string category = AskCategory(null);
            DateTime date = AskDate(formDate);

            bool success = budgetService.AddTransaction(amount, type, category, date) == ServiceResult.Success;
            ShowResult(success, "Transaktion tillagd!", "Fel vid tillägg av transaktion!");
        }

        public void ShowEdit(Transaction transaction)
        {
            Console.Clear();
            Console.WriteLine("REDIGERA TRANSAKTION\n");
            AnsiConsole.MarkupLine("[grey]Tryck Enter för att behålla nuvarande värde.[/]\n");

            decimal amount = AskAmount(transaction.Amount);
            TransactionType type = AskType(transaction.Type);
            string category = AskCategory(transaction.Category);
            DateTime date = AskDate(transaction.Date);

            bool success = budgetService.UpdateTransaction(transaction.Id, amount, type, date, category) == ServiceResult.Success;

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

        private TransactionType AskType(TransactionType? currentType)
        {
            SelectionPrompt<TransactionType> prompt = new SelectionPrompt<TransactionType>()
                .Title("Välj typ:")
                .WrapAround()
                .HighlightStyle(new Style(Color.Violet, Color.Black, Decoration.RapidBlink))
                .UseConverter(TransactionTypeText.DisplayText)
                .AddChoices(TransactionType.Income, TransactionType.Expense);

            if (currentType != null)
            {
                prompt.DefaultValue(currentType.Value);
            }

            return AnsiConsole.Prompt(prompt);
        }

        private string AskCategory(string? currentCategory)
        {
            SelectionPrompt<string> prompt = new SelectionPrompt<string>()
                .Title("Välj kategori:")
                .WrapAround()
                .HighlightStyle(new Style(Color.Violet, Color.Black, Decoration.RapidBlink))
                .UseConverter(Markup.Escape)
                .AddChoices("Boende", "Mat", "Transport", "Underhållning");

            if (currentCategory != null)
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
