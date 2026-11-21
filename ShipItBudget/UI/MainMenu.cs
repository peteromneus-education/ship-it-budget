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

        // today skickas in som parameter, så att rubrik och statistik garanterat gäller samma månad.
        private Columns BuildDashboard(DateTime today)
        {
            Dictionary<string, decimal> expensesByCategory = budgetService.GetExpenseTotalsByCategory(today.Month, today.Year);

            decimal totalIncome = budgetService.GetTotalIncome(today.Month, today.Year);
            decimal totalExpense = budgetService.GetTotalExpense(today.Month, today.Year);
            MonthlyBudget? budget = budgetService.GetBudget(today.Month, today.Year);
            decimal? remainingBudget = budgetService.GetRemainingBudget(today.Month, today.Year);

            string budgetText = "Ingen budget satt";
            if (budget != null)
            {
                budgetText = $"{budget.Limit} kr";
            }

            string remainingText = "Ingen budget satt";
            if (remainingBudget != null)
            {
                remainingText = $"{remainingBudget} kr";
            }

            Color[] chartColors = { Color.Red, Color.Blue, Color.Yellow, Color.Green, Color.Orange1 };

            Panel panelStats = new Panel(new Markup(
                $"Budget: [yellow]{budgetText}[/]\n" +
                $"Inkomst: {totalIncome} kr\n" +
                $"Utgifter: {totalExpense} kr\n" +
                $"Återstående budget: {remainingText}"))
                .Header("Statistik")
                .RoundedBorder()
                .BorderColor(Color.SkyBlue1)
                .Padding(2, 1);

            BreakdownChart chartExpensesPerCategory = new BreakdownChart()
                .Width(40);

            int colorIndex = 0;

            foreach (KeyValuePair<string, decimal> expensePair in expensesByCategory)
            {
                chartExpensesPerCategory
                    // BreakdownChart kräver double. % (modulo) börjar om på första färgen när färgerna tar slut.
                    .AddItem(expensePair.Key, (double)expensePair.Value, chartColors[colorIndex % chartColors.Length]);

                colorIndex++;
            }

            Panel panelChart;

            if (expensesByCategory.Count != 0)
            {
                panelChart = new Panel(chartExpensesPerCategory);
            }
            else
            {
                panelChart = new Panel(new Markup("Inga registrerade utgifter ännu"));
            }

            panelChart
                .Header("Utgifter per kategori")
                .RoundedBorder()
                .BorderColor(Color.Gold1)
                .Padding(2, 1);

            // Columns lägger panelerna bredvid varandra. Collapse gör dem inte bredare än innehållet.
            return new Columns(panelStats, panelChart)
                .Padding(2, 0)
                .Collapse();
        }

        public void Show()
        {
            // Lokal variabel som återställs vid varje anrop, så att Program.cs kan visa menyn igen om sparandet misslyckas.
            bool keepMainMenuOpen = true;

            Rule separatorRule = new Rule()
                .LeftJustified()
                .RuleStyle(new Style(Color.Gray));

            while (keepMainMenuOpen)
            {
                DateTime today = DateTime.Today.Date;

                Rule headerRule = new Rule($"[yellow]{today:Y}[/]")
                    .LeftJustified()
                    .RuleStyle(new Style(Color.Gray));

                Console.Clear();
                AnsiConsole.Write(headerRule);
                AnsiConsole.WriteLine();
                AnsiConsole.Write(BuildDashboard(today));
                AnsiConsole.WriteLine();
                AnsiConsole.Write(separatorRule);

                // SelectionPrompt returnerar den valda texten, som jämförs i switch nedan. Texterna måste vara exakt lika på båda ställena.
                string choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("")
            .WrapAround()
            .HighlightStyle(new Style(Color.Yellow, Color.Black, Decoration.Bold | Decoration.RapidBlink))
            .AddChoices("Visa kalendern")
            .AddChoices("Ny transaktion")
            .AddChoices("Lägg till/redigera budget")
            .AddChoices("Avsluta programmet"));

                switch (choice)
                {
                    case "Visa kalendern":
                        CalendarView calendarView = new CalendarView(budgetService);
                        Console.Clear();
                        calendarView.Show();
                        break;

                    case "Ny transaktion":
                        TransactionForm transactionForm = new TransactionForm(budgetService);
                        transactionForm.ShowAdd(DateTime.Today);
                        break;

                    case "Lägg till/redigera budget":
                        new BudgetForm(budgetService).Show(DateTime.Today);
                        break;

                    case "Avsluta programmet":
                        keepMainMenuOpen = false;
                        break;

                    default:
                        AnsiConsole.MarkupLine("[yellow]Ogiltigt val![/]");
                        Console.ReadKey(true);
                        break;
                }
            }
        }
    }
}
