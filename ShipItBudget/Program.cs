// Kopplar ihop lagring, logik och UI.
using ShipItBudget;
using ShipItBudget.UI;

Console.OutputEncoding = System.Text.Encoding.UTF8;

DataStore<Transaction> transactionStore = new();
DataStore<MonthlyBudget> budgetStore = new();

BudgetService budgetService = new BudgetService(transactionStore, budgetStore);

MainMenu mainMenu = new MainMenu(budgetService);
mainMenu.Show();
