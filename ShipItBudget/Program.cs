// Kopplar ihop lagring, logik och UI.

using ShipItBudget.UI;

DataStore dataStore = new DataStore();

BudgetService budgetService = new BudgetService(dataStore);

MainMenu mainMenu = new MainMenu(budgetService);
mainMenu.Show();
