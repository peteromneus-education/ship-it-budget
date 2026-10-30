// Visar kalendern och hanterar datumval med piltangenter. Avsluta Live Display innan DayMenu öppnas.

using Spectre.Console;

namespace ShipItBudget.UI;

public class CalendarView
{
    public void Show()
    {
        DateTime selectedDate = DateTime.Today;

        bool keepRunning = true;

        AnsiConsole.Live(BuildCalendarTable()) // BuildCalendarTable() bygger kalandervyn och AnsiConsole omsluter den i en Live Display
            .Start(liveDisplayContext =>
            {
                liveDisplayContext.Refresh(); // Uppdaterar Live Display med initialt innehåll utan att man behöver trycka en tangent

                while (keepRunning)
                {
                    ConsoleKeyInfo keyPress = Console.ReadKey(true);

                    switch (keyPress.Key)
                    {
                        case ConsoleKey.RightArrow:
                            selectedDate = selectedDate.AddDays(1);
                            liveDisplayContext.UpdateTarget(BuildCalendarTable()); // ersätter innehållet i Live Display
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
                        case ConsoleKey.Enter:
                            // Öppna DayMenu för valt datum
                            // DayMenu.Show(selectedDate);
                            break;
                        case ConsoleKey.Escape:
                            keepRunning = false;
                            break;
                    }
                }
            });

        Table BuildCalendarTable()
        {
            string[] weekCells = new string[7];
            Table calendarTable = new();

            calendarTable.Title(selectedDate.ToString("yyyy-MM-dd"));

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

            Array.Fill(weekCells, "");

            DateTime firstDayOfMonth = new(selectedDate.Year, selectedDate.Month, 1);

            int daysInMonth = DateTime.DaysInMonth(selectedDate.Year, selectedDate.Month);

            int emptyCells = (int)firstDayOfMonth.DayOfWeek - 1;

            if (emptyCells == -1)
            {
                emptyCells = 6;
            }

            int weekdayColumnIndex = emptyCells;

            for (int dayOfMonth = 1; dayOfMonth <= daysInMonth; dayOfMonth++)
            {
                if (dayOfMonth == selectedDate.Day)
                {
                    string selectedDayLine = dayOfMonth.ToString().PadRight(14);
                    string emptyCellLine = new(' ', 14);

                    weekCells[weekdayColumnIndex] = $"[black on cyan]{selectedDayLine}\n{emptyCellLine}\n{emptyCellLine}[/]";
                }
                else
                {
                    weekCells[weekdayColumnIndex] = $"{dayOfMonth}\n\n";
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
