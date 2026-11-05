// Kopplar ihop lagring, logik och UI.

using ShipItBudget.UI;

BudgetService budgetService = new BudgetService();

MainMenu mainMenu = new MainMenu(budgetService);
mainMenu.Show();
