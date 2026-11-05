using Spectre.Console;

namespace ShipItBudget.UI
{
    public class MainMenu
    {
        private readonly BudgetService budgetService;

        public MainMenu(BudgetService budgetService)
        {
            this.budgetService = budgetService;
        }

        public void Show()
        {
            bool keepMainMenuOpen = true;

            while (keepMainMenuOpen)
            {
                Console.Clear();

                Console.WriteLine("HUVUDMENY\n");

                string choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("Välj ett alternativ:")
            .WrapAround()
            .HighlightStyle(new Style(Color.Violet, Color.Black, Decoration.RapidBlink))
            .AddChoices("Ny transaktion")
            .AddChoiceGroup("Visa kalendern")
            .AddChoiceGroup("Redigering", new[]
            {
            "Redigera transaktion",
            "Redigera budget"
            })
            .AddChoices("Avsluta programmet"));

                switch (choice)
                {
                    case "Ny transaktion":
                        TransactionForm transactionForm = new TransactionForm(budgetService);
                        transactionForm.ShowAdd(DateTime.Today);
                        break;
                    case "Visa kalendern":
                        CalendarView calendarView = new CalendarView(budgetService);
                        calendarView.Show();
                        break;
                    case "Redigera transaktion":
                        break;
                    case "Redigera budget":
                        break;
                    case "Avsluta programmet":
                        keepMainMenuOpen = false;
                        break;
                    default:
                        AnsiConsole.MarkupLine("[red]Ogiltigt val![/]");
                        break;
                }
            }
        }

    }
}
