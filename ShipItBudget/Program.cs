// Kopplar ihop lagring, logik och UI.

using ShipItBudget;
using ShipItBudget.UI;

// Skapar tjänsterna
BudgetService budgetService = new BudgetService();
CategoryService categoryService = new CategoryService();

// Visar menyn för att registrera en transaktion
TransactionMenu.Show(budgetService, categoryService);

// Väntar innan kalendern öppnas
Console.WriteLine("\nTryck Enter för att öppna kalendern.");
Console.ReadLine();

// Öppnar kalendern
CalendarView calendarView = new CalendarView();
calendarView.Show();



