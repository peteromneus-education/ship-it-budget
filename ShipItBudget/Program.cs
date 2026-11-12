// Kopplar ihop lagring, logik och UI.
using ShipItBudget;
using ShipItBudget.UI;

Console.OutputEncoding = System.Text.Encoding.UTF8;

string storageFolder = Path.Combine(AppContext.BaseDirectory, "Storage");
string transactionsPath = Path.Combine(storageFolder, "transactions.json");
string budgetsPath = Path.Combine(storageFolder, "budgets.json");

DataStore<Transaction> transactionStore = new(transactionsPath);
DataStore<MonthlyBudget> budgetStore = new(budgetsPath);

BudgetService budgetService = new BudgetService(transactionStore, budgetStore);

// Ladda data från JSON-filerna innan programmet startar.
transactionStore.LoadFromJson();
budgetStore.LoadFromJson();

MainMenu mainMenu = new MainMenu(budgetService);
mainMenu.Show();

// Spara data till JSON-filerna innan programmet avslutas.
transactionStore.SaveToJson();
budgetStore.SaveToJson();