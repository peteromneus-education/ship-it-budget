using System.Globalization;
using Spectre.Console;

namespace ShipItBudget.UI
{
    public class BudgetForm
    {
        private readonly BudgetService budgetService;

        public BudgetForm(BudgetService budgetService)
        {
            this.budgetService = budgetService;
        }

        public void Show(DateTime month)
        {
            MonthlyBudget? budget = budgetService.GetBudget(month.Month, month.Year);

            string currentText = "Ingen budget satt";
            if (budget != null)
            {
                currentText = $"{budget.Limit} kr";
            }
            AnsiConsole.MarkupLine($"Budget för {month:Y}: [yellow]{currentText}[/]\n");

            decimal newLimit = AskLimit(budget?.Limit);

            ServiceResult serviceResult;

            if (budget == null)
            {
                serviceResult = budgetService.AddBudget(month.Month, month.Year, newLimit);
            }
            else
            {
                serviceResult = budgetService.UpdateMonthlyBudget(month.Month, month.Year, newLimit);
            }

            switch (serviceResult)
            {
                case ServiceResult.Success:
                    AnsiConsole.MarkupLine("[green]Budget uppdaterad![/]");
                    break;

                case ServiceResult.NotFound:
                    AnsiConsole.MarkupLine("[red]Ingen budget hittades[/]");
                    break;

                case ServiceResult.InValidInput:
                    AnsiConsole.MarkupLine("[red]Ogiltig budgetgräns[/]");
                    break;

                case ServiceResult.AlreadyExist:
                    AnsiConsole.MarkupLine("[red]Det finns redan en budget för månaden[/]");
                    break;
            }

            AnsiConsole.MarkupLine("[grey]Tryck valfri tangent för att fortsätta[/]");
            Console.ReadKey(true);
        }

        private decimal AskLimit(decimal? currentLimit)
        {
            TextPrompt<decimal> prompt = new TextPrompt<decimal>("Ange budgetgräns:")
            {
                Culture = new CultureInfo("sv-SE")
            }
                .Validate(limit => limit >= 0
                    ? ValidationResult.Success()
                    : ValidationResult.Error("[red]Budgetgränsen kan inte vara negativ[/]"));

            if (currentLimit != null)
            {
                prompt.DefaultValue(currentLimit.Value);
            }

            return AnsiConsole.Prompt(prompt);
        }
    }
}
