// Kopplar ihop lagring, logik och UI.
using ShipItBudget;
using ShipItBudget.UI;

Console.OutputEncoding = System.Text.Encoding.UTF8;

string storageFolder = Path.Combine(AppContext.BaseDirectory, "Storage");
string transactionsPath = Path.Combine(storageFolder, "transactions.json");
string budgetsPath = Path.Combine(storageFolder, "budgets.json");

DataStore<Transaction> transactionStore = new(transactionsPath);
DataStore<MonthlyBudget> budgetStore = new(budgetsPath);

// Loggen skapas först, så att fel vid inläsningen kan loggas.
string logPath = Path.Combine(storageFolder, "log.json");
DataStore<LogEntry> logStore = new(logPath);
logStore.LoadFromJson();
Logger logger = new Logger(logStore);

BudgetService budgetService = new BudgetService(transactionStore, budgetStore, logger);

// Ladda data från JSON-filerna innan programmet startar.
if (!transactionStore.LoadFromJson())
{
    logger.Error(LogEvent.LoadFailed, $"Kunde inte läsa {transactionsPath}");
}
if (!budgetStore.LoadFromJson())
{
    logger.Error(LogEvent.LoadFailed, $"Kunde inte läsa {budgetsPath}");
}

// Ladda data från JSON-filerna innan programmet startar.
transactionStore.LoadFromJson();
budgetStore.LoadFromJson();

MainMenu mainMenu = new MainMenu(budgetService);
mainMenu.Show();

// Spara data till JSON-filerna innan programmet avslutas.
if (!transactionStore.SaveToJson())
{
    logger.Error(LogEvent.SaveFailed, $"Kunde inte spara {transactionsPath}");
}
if (!budgetStore.SaveToJson())
{
    logger.Error(LogEvent.SaveFailed, $"Kunde inte spara {budgetsPath}");
}
