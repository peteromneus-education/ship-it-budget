// Visar kalendern och hanterar datumval med piltangenter. Avsluta Live Display innan DayMenu öppnas.

using Spectre.Console;

namespace ShipItBudget.UI;

public class CalendarView
{
    public void Show() // Visar kalendern och hanterar datumval med piltangenter. Avslutar Live Display innan DayMenu öppnas.
    {
        DateTime selectedDate = DateTime.Today;

        //// Skapa exempeldata för demonstration/test med hjälp av klassen ExampleTransaction.
        List<ExampleTransaction> exampleTransactions =
        [
            new ExampleTransaction
            {
                Date = DateTime.Today,
                Amount = 500m,
                IsIncome = true
            },
            new ExampleTransaction
            {
                Date = DateTime.Today.AddDays(1),
                Amount = 100m,
                IsIncome = false
            },
            new ExampleTransaction
            {
                Date = DateTime.Today,
                Amount = 200m,
                IsIncome = true
            },
        ];
        ////

        bool keepCalendarOpen = true;

        while (keepCalendarOpen) // Håller kalendern öppen tills användaren trycker på Esc.
        {
            bool keepRunning = true;
            bool shouldOpenDayMenu = false;

            AnsiConsole.Live(BuildCalendarTable()) // BuildCalendarTable() bygger kalandervyn och AnsiConsole omsluter den i en Live Display
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

            if (shouldOpenDayMenu) // Om användaren tryckte på Enter, öppna DayMenu för valt datum och skicka med transaktionerna för det datumet
            {
                List<ExampleTransaction> transactionsForSelectedDate = exampleTransactions
                    .Where(transaction => transaction.Date.Date == selectedDate.Date)
                    .ToList();
                DayMenu.Show(selectedDate, transactionsForSelectedDate);
            }
        }

        Table BuildCalendarTable() // Bygger kalendervyn med hjälp av Spectre.Console.Table och returnerar den som en Table-objekt.
        {
            string[] weekCells = new string[7]; // Skapar en array med 7 celler för att representera en vecka i kalendern.
            
            Table calendarTable = new();

            calendarTable.Title(selectedDate.ToString("yyyy-MM-dd"));
            calendarTable.Caption(
                "[grey]Piltangenter: Byt dag | PageUp/PageDown: Byt månad | Enter: Visa detaljer | Esc: Avsluta\n" +
                "EXEMPELDATA – tillfälliga belopp[/]");
            calendarTable.ShowRowSeparators();

            calendarTable.AddColumn("Måndag");
            calendarTable.AddColumn("Tisdag");
            calendarTable.AddColumn("Onsdag");
            calendarTable.AddColumn("Torsdag");
            calendarTable.AddColumn("Fredag");
            calendarTable.AddColumn("Lördag");
            calendarTable.AddColumn("Söndag");

            foreach (TableColumn weekdayColumn in calendarTable.Columns)
            {
                weekdayColumn.Width = 14;
                weekdayColumn.Padding = new Padding(0, 0, 0, 0);
            }

            Array.Fill(weekCells, ""); // Fyller arrayen med tomma strängar för att representera tomma celler i kalendern.

            DateTime firstDayOfMonth = new(selectedDate.Year, selectedDate.Month, 1); // Skapar ett DateTime-objekt för den första dagen i den valda månaden som behövs för att beräkna vilken veckodag den första dagen i månaden infaller på.

            int daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);

            int emptyCells = (int)firstDayOfMonth.DayOfWeek - 1; // Beräknar antalet tomma celler som behövs för att fylla ut kalendern innan den första dagen i månaden. Eftersom DayOfWeek enum börjar på söndag (0) och vi vill att måndag ska vara den första kolumnen, drar vi ifrån 1. Om första dagen är en söndag, sätts emptyCells till 6 för att fylla ut hela veckan.

            if (emptyCells == -1) // Om första dagen i månaden är en söndag, sätts emptyCells till 6 för att fylla ut hela veckan.
            {
                emptyCells = 6;
            }

            int weekdayColumnIndex = emptyCells; // Sätter indexet för den första cellen i kalendern som ska fyllas med en dag i månaden. Om första dagen i månaden är en måndag, sätts weekdayColumnIndex till 0. Om första dagen är en söndag, sätts weekdayColumnIndex till 6.

            for (int dayOfMonth = 1; dayOfMonth <= daysInMonth; dayOfMonth++) // Loopar igenom alla dagar i den valda månaden och fyller kalendern med dagnummer, inkomster och utgifter för varje dag. Om dagen är vald, markeras den med en grön bakgrund. Om dagen är en söndag, markeras den med röd text.
            {
                DateTime cellDate = new(selectedDate.Year, selectedDate.Month, dayOfMonth);

                List<ExampleTransaction> transactionsForCellDate = exampleTransactions
                    .Where(transaction => transaction.Date.Date == cellDate.Date)
                    .ToList();

                List<ExampleTransaction> incomeForCellDate = transactionsForCellDate
                    .Where(transaction => transaction.IsIncome)
                    .ToList();

                List<ExampleTransaction> expenseForCellDate = transactionsForCellDate
                    .Where(transaction => transaction.IsIncome == false)
                    .ToList();

                decimal? exampleIncomeTotal = null;
                decimal? exampleExpenseTotal = null;

                if (incomeForCellDate.Count > 0)
                {
                    exampleIncomeTotal = incomeForCellDate.Sum(
                        transaction => transaction.Amount);
                }

                if (expenseForCellDate.Count > 0)
                {
                    exampleExpenseTotal = expenseForCellDate.Sum(
                        transaction => transaction.Amount);
                }

                string incomeSummaryText = string.Empty;
                string expenseSummaryText = string.Empty;

                if (exampleIncomeTotal != null)
                {
                    incomeSummaryText = $"[green]+ {exampleIncomeTotal} kr[/]";
                }

                if (exampleExpenseTotal != null)
                {
                    expenseSummaryText = $"[red]- {exampleExpenseTotal} kr[/]";
                }

                string dayNumberText = dayOfMonth.ToString();

                if (cellDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    dayNumberText = $"[red]{dayOfMonth}[/]";
                }

                if (dayOfMonth == selectedDate.Day)
                {
                    string selectedDayLine = dayOfMonth.ToString().PadRight(14);
                    string selectedIncomeLine = string.Empty;
                    string selectedExpenseLine = string.Empty;

                    if (exampleIncomeTotal != null)
                    {
                        selectedIncomeLine = $"+ {exampleIncomeTotal} kr";
                    }

                    if (exampleExpenseTotal != null)
                    {
                        selectedExpenseLine = $"- {exampleExpenseTotal} kr";
                    }

                    selectedIncomeLine = selectedIncomeLine.PadRight(14);
                    selectedExpenseLine = selectedExpenseLine.PadRight(14);

                    weekCells[weekdayColumnIndex] =
                        $"[black on green]{selectedDayLine}\n" +
                        $"[black]{selectedIncomeLine}[/]\n" +
                        $"[black]{selectedExpenseLine}[/][/]";
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
