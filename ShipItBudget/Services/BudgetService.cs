using ShipItBudget;

public enum ServiceResult //Fasta val för resultat
{
    Success,
    NotFound,
    InvalidInput,
    AlreadyExists
}

public class BudgetService
{
    private readonly DataStore<Transaction> _transactionStore; //Kopplingar till datalagringen 
    private readonly DataStore<MonthlyBudget> _budgetStore;
    private readonly Logger _logger; //Loggar händelser

    //Konstruktor som kopplar in lagring och logger
    public BudgetService(DataStore<Transaction> transactionStore, DataStore<MonthlyBudget> budgetStore, Logger logger)
    {
        _transactionStore = transactionStore;
        _budgetStore = budgetStore;
        _logger = logger;
    }

    public List<Transaction> GetTransactionsByDate(DateTime chosenDate) // Hittar transaktionerna för ett visst datum, sorterade
    {
        return _transactionStore.GetAll()
            .Where(transaction => transaction.Date.Date == chosenDate.Date) // .Date jämför bara datumet, inte klockslaget
            .OrderBy(transaction => transaction.Type) // Inkomster före utgifter
            .ThenByDescending(transaction => transaction.Amount) // Inom varje typ: störst belopp först
            .ToList();
    }

    public ServiceResult DeleteTransaction(Guid id) // Tar bort en transaktion
    {
        Transaction? transaction = _transactionStore.GetAll().FirstOrDefault(transaction => transaction.Id == id); // Letar upp rätt id
        if (transaction == null)
        {
            return ServiceResult.NotFound;
        }
        _transactionStore.RemoveItem(transaction);
        _logger.Info(LogEvent.TransactionDeleted, $"Transaktion {id} borttagen: {transaction.Type}, {transaction.Amount} kr, {transaction.Category}, {transaction.Date:yyyy-MM-dd}");
        return ServiceResult.Success;
    }

    public ServiceResult UpdateTransaction(Guid id, decimal newAmount, TransactionType newType, DateTime newDate, string newCategory) // Uppdaterar en befintlig transaktion
    {
        Transaction? transaction = _transactionStore.GetAll().FirstOrDefault(transaction => transaction.Id == id); // Samma princip som DeleteTransaction
        if (transaction == null)
        {
            return ServiceResult.NotFound;
        }
        if (Transaction.IsValid(newAmount, newCategory) == false) // Validerar de nya värdena
        {
            return ServiceResult.InvalidInput;
        }

        string before = $"{transaction.Type}, {transaction.Amount} kr, {transaction.Category}, {transaction.Date:yyyy-MM-dd}";
        transaction.Update(newAmount, newType, newDate, newCategory);
        _logger.Info(LogEvent.TransactionUpdated, $"Transaktion {id} ändrad: {before} → {newType}, {newAmount} kr, {newCategory}, {newDate:yyyy-MM-dd}");

        return ServiceResult.Success;
    }

    public ServiceResult AddTransaction(decimal amount, TransactionType type, string category, DateTime? date = null)
    {
        if (Transaction.IsValid(amount, category) == false)
        {
            return ServiceResult.InvalidInput;
        }

        Transaction newTransaction = new Transaction(amount, type, category, date ?? DateTime.Now); // Nytt objekt. Konstruktorn sätter id; utan angivet datum används dagens datum

        _transactionStore.AddItem(newTransaction);
        _logger.Info(LogEvent.TransactionAdded, $"Transaktion {newTransaction.Id} tillagd: {newTransaction.Type}, {newTransaction.Amount} kr, {newTransaction.Category}, {newTransaction.Date:yyyy-MM-dd}");
        return ServiceResult.Success;
    }

    public ServiceResult AddBudget(int month, int year, decimal limit)
    {
        string? errorMessage = MonthlyBudget.IsValid(month, year, limit); // IsValid returnerar ett felmeddelande, eller null om budgeten är giltig
        if (errorMessage != null)
        {
            return ServiceResult.InvalidInput;
        }

        bool exists = _budgetStore.GetAll().Any(budget => budget.Month == month && budget.Year == year); // Kontrollerar om det redan finns en budget för månaden
        if (exists)
        {
            return ServiceResult.AlreadyExists;
        }

        MonthlyBudget newBudget = new MonthlyBudget(month, year, limit); // Skapar nytt objekt
        _budgetStore.AddItem(newBudget); // Lägger till
        _logger.Info(LogEvent.BudgetAdded, $"Budget {year}-{month:00} tillagd: {limit} kr");
        return ServiceResult.Success;
    }
    public MonthlyBudget? GetBudget(int month, int year) // Hämtar budgeten för månaden, eller null om den inte finns
    {
        return _budgetStore.GetAll().FirstOrDefault(budget => budget.Month == month && budget.Year == year);
    }

    public ServiceResult UpdateMonthlyBudget(int month, int year, decimal newLimit) // Uppdaterar budgeten, obs endast gränsen
    {
        MonthlyBudget? budget = GetBudget(month, year); // Använder GetBudget; null betyder att budgeten inte finns
        if (budget == null)
        {
            return ServiceResult.NotFound;
        }
        decimal oldLimit = budget.Limit;
        string? errorMessage = budget.Update(newLimit);
        if (errorMessage != null)
        {
            return ServiceResult.InvalidInput;
        }
        _logger.Info(LogEvent.BudgetUpdated, $"Budget {year}-{month:00} ändrad: {oldLimit} kr → {newLimit} kr");
        return ServiceResult.Success;
    }

    public ServiceResult RemoveBudget(int month, int year)
    {
        MonthlyBudget? budget = GetBudget(month, year);
        if (budget == null)
        {
            return ServiceResult.NotFound;
        }

        _budgetStore.RemoveItem(budget);
        _logger.Info(LogEvent.BudgetRemoved, $"Budget {year}-{month:00} borttagen: {budget.Limit} kr");
        return ServiceResult.Success;
    }
    public decimal GetTotalIncome(int month, int year) // Hittar rätt månad och summerar inkomsterna
    {
        return _transactionStore.GetAll().Where(t => t.Date.Month == month && t.Date.Year == year && t.Type == TransactionType.Income).Sum(t => t.Amount); // t för transaction
    }

    public decimal GetTotalExpense(int month, int year) // Hittar rätt månad och summerar utgifterna
    {
        return _transactionStore.GetAll().Where(t => t.Date.Month == month && t.Date.Year == year && t.Type == TransactionType.Expense).Sum(t => t.Amount); // t för transaction
    }

    public Dictionary<string, decimal> GetExpenseTotalsByCategory(int month, int year) // Dictionary där varje utgiftskategori får sin summa
    {
        return _transactionStore.GetAll()
            .Where(transaction => transaction.Date.Month == month && transaction.Date.Year == year && transaction.Type == TransactionType.Expense)
            .GroupBy(transaction => transaction.Category)
            .ToDictionary(group => group.Key,
            group => group.Sum(transaction => transaction.Amount));
    }

    public decimal? GetRemainingBudget(int month, int year) // decimal? eftersom svaret är null när ingen budget finns
    {
        MonthlyBudget? budget = GetBudget(month, year);
        if (budget == null) // Om ingen budget hittas
        {
            return null;
        }
        decimal expenses = GetTotalExpense(month, year);
        return budget.Limit - expenses; // Kan bli ett negativt tal
    }
}
