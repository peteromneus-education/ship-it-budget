// Kopplar ihop lagring, logik och UI.
using ShipItBudget;
using ShipItBudget.UI;
using Spectre.Console;

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

WelcomeScreen.Show();

if (!TryLoad(transactionStore, logger, transactionsPath))
{
    return;
}

if (!TryLoad(budgetStore, logger, budgetsPath))
{
    return;
}

MainMenu mainMenu = new MainMenu(budgetService);

bool keepRunning = true;
while (keepRunning)
{
    mainMenu.Show();

    // Försök spara båda filerna, även om den första misslyckas.
    bool transactionsSaved = TrySave(transactionStore, logger, transactionsPath);
    bool budgetsSaved = TrySave(budgetStore, logger, budgetsPath);

    if (transactionsSaved && budgetsSaved)
    {
        keepRunning = false;
    }
    else
    {
        // Användaren väljer antingen att avsluta och förlora ändringarna, eller gå tillbaka till menyn.
        bool quitAnyway = AnsiConsole.Confirm("\nVill du avsluta ändå? Osparade ändringar förloras.", defaultValue: false);
        keepRunning = !quitAnyway;
    }
}

// Försöker ladda en DataStore och visar ett felmeddelande om det misslyckas. Avslutar programmet om det inte går att läsa filen.
static bool TryLoad<T>(DataStore<T> store, Logger logger, string path)
{
    if (!store.LoadFromJson())
    {
        logger.Error(LogEvent.LoadFailed, $"{path} kunde inte läsas.");
        AnsiConsole.MarkupLine($"\n[red]{Markup.Escape(path)} kunde inte läsas och programmet kommer därför att avslutas.[/] \n\n" +
            $"Dina data finns kvar. Kontakta support för hjälp.");
        AnsiConsole.MarkupLine($"\n[grey]Tryck valfri tangent för att avsluta programmet.[/]");
        Console.ReadKey(true);
        return false;
    }
    return true;
}

// Sparar en DataStore och visar ett felmeddelande om det misslyckas.
static bool TrySave<T>(DataStore<T> store, Logger logger, string path)
{
    if (!store.SaveToJson())
    {
        logger.Error(LogEvent.SaveFailed, $"{path} kunde inte sparas.");
        AnsiConsole.MarkupLine($"\n[red]Programmet har stött på ett problem. {Markup.Escape(path)} kunde inte sparas.[/]\n" +
            "Notera dina ändringar och kontakta support.");
        return false;
    }
    return true;
}
