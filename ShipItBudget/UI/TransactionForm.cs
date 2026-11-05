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

            decimal amount = AnsiConsole.Prompt(new TextPrompt<decimal>("Ange belopp:")
                    .Validate(amount =>
                    amount > 0 ? ValidationResult.Success() : ValidationResult.Error("[red]Beloppet måste vara större än 0[/]")));

            var typeChoices = new List<TransactionType> { TransactionType.Income, TransactionType.Expense };

            TransactionType type = AnsiConsole.Prompt(
                            new SelectionPrompt<TransactionType>()
                                .Title("Välj typ:")
                                            .WrapAround()
                        .HighlightStyle(new Style(Color.Violet, Color.Black, Decoration.RapidBlink))
                        .UseConverter(TransactionTypeText.DisplayText)
                        .AddChoices(typeChoices));

            string category = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Välj kategori:")
                                .WrapAround()
            .HighlightStyle(new Style(Color.Violet, Color.Black, Decoration.RapidBlink))
                    .AddChoices("Boende", "Mat", "Transport", "Underhållning"));

            DateTime date = AnsiConsole.Prompt(
                new TextPrompt<DateTime>("Ange datum")
                    .DefaultValue(formDate)
                    .ShowDefaultValue());

            bool success = budgetService.AddTransaction(amount, type, category, date);

            if (!success)
            {
                AnsiConsole.MarkupLine("\n[red]Fel vid tillägg av transaktion![/]");
                Console.ReadKey(true);
            }
            else
            {
                AnsiConsole.MarkupLine("\n[green]Transaktion tillagd! Tryck valfri tangent för att fortsätta.[/]");
                Console.ReadKey(true);
            }
        }
    }


}
