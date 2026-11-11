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

        private Columns BuildDashboard(DateTime today)
        {
            Dictionary<string, decimal> expensesByCategory = budgetService.GetExpenseTotalsByCategory(today.Month, today.Year);

            decimal totalIncome = budgetService.GetTotalincome(today.Month, today.Year);
            decimal totalExpense = budgetService.GetTotalexpense(today.Month, today.Year);
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
                .BorderColor(Color.SkyBlue1)
                .Padding(2, 1);

            BreakdownChart chartExpensesPerCategory = new BreakdownChart()
                .Width(40);

            int colorIndex = 0;

            foreach (KeyValuePair<string, decimal> expensePair in expensesByCategory)
            {
                chartExpensesPerCategory
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
                .BorderColor(Color.Gold1)
                .Padding(2, 1);

            return new Columns(panelStats, panelChart)
                .Padding(2, 0)
                .Collapse();
        }

        public void Show()
        {
            bool keepMainMenuOpen = true;

            Rule separatorRule = new Rule()
                .LeftJustified()
                .RuleStyle(new Style(Color.CadetBlue));

            while (keepMainMenuOpen)
            {
                DateTime today = DateTime.Today.Date;

                Rule headerRule = new Rule($"[yellow]{today:Y}[/]")
                    .LeftJustified()
                    .RuleStyle(new Style(Color.CadetBlue));

                Console.Clear();
                AnsiConsole.Write(headerRule);
                AnsiConsole.WriteLine();
                AnsiConsole.Write(BuildDashboard(today));
                AnsiConsole.WriteLine();
                AnsiConsole.Write(separatorRule);

                string choice = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("")
            .WrapAround()
            .HighlightStyle(new Style(Color.Yellow, Color.Black))
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
                        Console.Clear();
                        calendarView.Show();
                        break;

                    case "Redigera transaktion":
                        AnsiConsole.MarkupLine("[yellow]Funktionen är inte implementerad än.[/]");
                        Console.ReadKey(true);
                        break;

                    case "Redigera budget":
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
