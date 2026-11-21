// Visar kalendern och hanterar datumval med piltangenter. Live Display avslutas innan DayMenu öppnas.

using Spectre.Console;

namespace ShipItBudget.UI;

public class CalendarView
{
    private readonly BudgetService budgetService;

    public CalendarView(BudgetService budgetService)
    {
        this.budgetService = budgetService;
    }

    public void Show() // Visar kalendern och hanterar datumval med piltangenter. Avslutar Live Display innan DayMenu öppnas.
    {
        DateTime selectedDate = DateTime.Today;

        bool keepCalendarOpen = true;

        while (keepCalendarOpen) // Håller kalendern öppen tills användaren trycker på Esc.
        {
            bool keepRunning = true;
            bool shouldOpenDayMenu = false;

            AnsiConsole.Live(BuildCalendarTable()) // BuildCalendarTable() bygger kalendervyn och AnsiConsole omsluter den i en Live Display
            .AutoClear(true) // Rensar Live Display när den avslutas
                .Start(liveDisplayContext =>
            {
                liveDisplayContext.Refresh(); // Uppdaterar Live Display med initialt innehåll utan att man behöver trycka en tangent

                while (keepRunning) // Håller Live Display öppen tills användaren trycker på Esc eller Enter
                {
                    ConsoleKeyInfo keyPress = Console.ReadKey(true);

                    switch (keyPress.Key)
                    {
                        case ConsoleKey.RightArrow:
                            selectedDate = selectedDate.AddDays(1);
                            liveDisplayContext.UpdateTarget(BuildCalendarTable()); // Uppdaterar Live Display med nytt innehåll
                            break;
                        case ConsoleKey.LeftArrow:
                            selectedDate = selectedDate.AddDays(-1);
                            liveDisplayContext.UpdateTarget(BuildCalendarTable());
                            break;
                        case ConsoleKey.UpArrow:
                            selectedDate = selectedDate.AddDays(-7);
                            liveDisplayContext.UpdateTarget(BuildCalendarTable());
                            break;
                        case ConsoleKey.DownArrow:
                            selectedDate = selectedDate.AddDays(7);
                            liveDisplayContext.UpdateTarget(BuildCalendarTable());
                            break;
                        case ConsoleKey.PageDown:
                            selectedDate = selectedDate.AddMonths(1);
                            liveDisplayContext.UpdateTarget(BuildCalendarTable());
                            break;
                        case ConsoleKey.PageUp:
                            selectedDate = selectedDate.AddMonths(-1);
                            liveDisplayContext.UpdateTarget(BuildCalendarTable());
                            break;
                        case ConsoleKey.Enter: // Avslutar Live Display och öppnar DayMenu för valt datum
                            shouldOpenDayMenu = true;
                            keepRunning = false;
                            break;
                        case ConsoleKey.Escape:
                            keepRunning = false;
                            keepCalendarOpen = false;
                            break;
                    }
                }
            });

            if (shouldOpenDayMenu) // Om användaren tryckte på Enter, öppna DayMenu för valt datum
            {
                DayMenu dayMenu = new DayMenu(budgetService);
                dayMenu.Show(selectedDate);
            }
        }

        Table BuildCalendarTable() // Bygger kalendervyn med hjälp av Spectre.Console.Table och returnerar den som ett Table-objekt.
        {
            string[] weekCells = new string[7]; // Skapar en array med 7 celler för att representera en vecka i kalendern.

            Table calendarTable = new();

            calendarTable
                .Title(selectedDate.ToString("yyyy-MM-dd"))
                .Caption(
                "[grey]Piltangenter: Byt dag | PageUp/PageDown: Byt månad | Enter: Visa/redigera | Esc: Tillbaka[/]")
                .ShowRowSeparators()
                .DoubleEdgeBorder()

                .AddColumn("Måndag")
                .AddColumn("Tisdag")
                .AddColumn("Onsdag")
                .AddColumn("Torsdag")
                .AddColumn("Fredag")
                .AddColumn("Lördag")
                .AddColumn("Söndag");

            foreach (TableColumn weekdayColumn in calendarTable.Columns)
            {
                weekdayColumn.Width = 12;
                weekdayColumn.Padding = new Padding(0, 0); // Ingen padding, så att cellerna inte får extra utrymme.
            }

            Array.Fill(weekCells, ""); // Fyller arrayen med tomma strängar för att representera tomma celler i kalendern.

            DateTime firstDayOfMonth = new(selectedDate.Year, selectedDate.Month, 1); // Första dagen i månaden; behövs för att räkna ut vilken veckodag månaden börjar på.

            int daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);

            int emptyCells = (int)firstDayOfMonth.DayOfWeek - 1; // Tomma celler före månadens första dag. DayOfWeek börjar på söndag (0) men kalendern på måndag, därför -1.

            if (emptyCells == -1) // Söndag ger -1, så då blir hela första veckan tom (6 celler).
            {
                emptyCells = 6;
            }

            int weekdayColumnIndex = emptyCells; // Kolumnen där månadens första dag hamnar (0 = måndag, 6 = söndag).

            for (int dayOfMonth = 1; dayOfMonth <= daysInMonth; dayOfMonth++) // Fyller kalendern dag för dag med dagnummer, inkomster och utgifter. Vald dag markeras med gul bakgrund, söndagar med röd text.
            {
                DateTime cellDate = new(selectedDate.Year, selectedDate.Month, dayOfMonth);

                List<Transaction> transactionsForCellDate = budgetService.GetTransactionsByDate(cellDate);

                List<Transaction> incomeForCellDate = transactionsForCellDate
                    .Where(transaction => transaction.Type == TransactionType.Income)
                    .ToList();

                List<Transaction> expenseForCellDate = transactionsForCellDate
                    .Where(transaction => transaction.Type == TransactionType.Expense)
                    .ToList();

                decimal? incomeTotal = null;
                decimal? expenseTotal = null;

                if (incomeForCellDate.Count > 0)
                {
                    incomeTotal = incomeForCellDate.Sum(
                        transaction => transaction.Amount);
                }

                if (expenseForCellDate.Count > 0)
                {
                    expenseTotal = expenseForCellDate.Sum(
                        transaction => transaction.Amount);
                }

                string incomeSummaryText = string.Empty;
                string expenseSummaryText = string.Empty;

                if (incomeTotal != null)
                {
                    incomeSummaryText = $"[green]+ {incomeTotal} kr[/]";
                }

                if (expenseTotal != null)
                {
                    expenseSummaryText = $"[red]- {expenseTotal} kr[/]";
                }

                string dayNumberText = dayOfMonth.ToString();

                if (cellDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    dayNumberText = $"[red]{dayOfMonth}[/]";
                }

                if (dayOfMonth == selectedDate.Day)
                {
                    weekCells[weekdayColumnIndex] =
                        $"[black on yellow]{dayOfMonth}[/] [rapidblink]◀[/]\n" +
                        $"{incomeSummaryText}\n" +
                        $"{expenseSummaryText}";
                }
                else
                {
                    weekCells[weekdayColumnIndex] =
                        $"{dayNumberText}\n" +
                        $"{incomeSummaryText}\n" +
                        $"{expenseSummaryText}";
                }

                weekdayColumnIndex++;

                if (weekdayColumnIndex > 6)
                {
                    calendarTable.AddRow(weekCells);

                    weekCells = new string[7];
                    Array.Fill(weekCells, "");

                    weekdayColumnIndex = 0;
                }
            }

            if (weekdayColumnIndex > 0)
            {
                calendarTable.AddRow(weekCells);
            }

            return calendarTable;
        }
    }
}
