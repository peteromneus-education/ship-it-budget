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
            AnsiConsole.MarkupLine("[bold yellow]LÄGG TILL NY TRANSAKTION[/]\n");
            
            TransactionType? type = AskType(null);
            if (type == null) return;

            decimal amount = AskAmount(null);
            string category = AskCategory(null, type);
            DateTime date = AskDate(formDate);

            bool success = budgetService.AddTransaction(amount, type.Value, category, date) == ServiceResult.Success;
            ShowResult(success, "Transaktion tillagd!", "Fel vid tillägg av transaktion!");
        }

        public void ShowEdit(Transaction transaction)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[bold yellow]REDIGERA TRANSAKTION[/]\n");
            AnsiConsole.MarkupLine("[grey]Tryck Enter för att behålla nuvarande värde.[/]\n");

            TransactionType? type = AskType(transaction.Type);
            if (type == null) return;

            decimal amount = AskAmount(transaction.Amount);
            string category = AskCategory(transaction.Category, type);
            DateTime date = AskDate(transaction.Date);

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

        private TransactionType? AskType(TransactionType? currentType)
        {
            bool cancelled = false;

            SelectionPrompt<TransactionType> prompt = new SelectionPrompt<TransactionType>()
                .Title("Välj typ [grey](Esc för att avbryta)[/]:")
                .WrapAround()
                .HighlightStyle(new Style(Color.Yellow, Color.Black, Decoration.Bold | Decoration.RapidBlink))
                .UseConverter(TransactionTypeText.DisplayText)
                .AddChoices(TransactionType.Income, TransactionType.Expense)
                .AddCancelResult(() =>
                {
                    cancelled = true; // Körs när användaren trycker Esc
                    return TransactionType.Income; // Måste returnera något, men TransactionType.Income används inte!
                });

            if (currentType != null)
            {
                prompt.DefaultValue(currentType.Value);
            }

            TransactionType chosenType = AnsiConsole.Prompt(prompt);

            if (cancelled)
            {
                return null;
            }

            AnsiConsole.MarkupLine($"Typ: {TransactionTypeText.DisplayText(chosenType)}");
            return chosenType;
        }

        private string AskCategory(string? currentCategory, TransactionType? type)
        {
            bool cancelled = false;

            SelectionPrompt<string> prompt = new SelectionPrompt<string>()
                .Title("Välj kategori:")
                .WrapAround()
                .HighlightStyle(new Style(Color.Yellow, Color.Black, Decoration.Bold | Decoration.RapidBlink))
                .UseConverter(Markup.Escape)
                .AddChoices(type == TransactionType.Income ? Categories.Income : Categories.Expense)
                .AddCancelResult(() =>
                {
                    cancelled = true; // Körs bara när användaren trycker Esc
                    return ""; // Måste returnera något, men värdet används inte
                });

            if (currentCategory != null)
            {
                prompt.DefaultValue(currentCategory);
            }

            string chosenCategory = AnsiConsole.Prompt(prompt);
            
            if (cancelled)
            {
                return null!;
            }

            AnsiConsole.MarkupLine($"Kategori: {Markup.Escape(chosenCategory)}");
            return chosenCategory;
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
