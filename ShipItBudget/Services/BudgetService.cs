using System;
using ShipItBudget;
public enum ServiceResult
{
    Success,
    NotFound,
    InValidInput,
    AlreadyExist
}
public class BudgetService
{
    private readonly DataStore<Transaction> _transactionStore;
    private readonly DataStore<MonthlyBudget> _budgetStore;

    public BudgetService(DataStore<Transaction> transactionStore, DataStore<MonthlyBudget> budgetStore)
    {
        _transactionStore = transactionStore;
        _budgetStore = budgetStore;
    }
    

    public List<Transaction> GetSortedbyDate() //metod som sotera efter datumet
    {
        return _transactionStore.GetAll().OrderByDescending(transaction => transaction.Date).ToList();
    }

    public List<Transaction> GetTransactionbyDate(DateTime ChoosenDate) // metod som hittar transaktioner efter ett visst datum
    {
        return _transactionStore.GetAll().Where(transaction => transaction.Date.Date == ChoosenDate.Date).ToList(); //Date.Date så man bar få datum
    }

    public ServiceResult DeleteTransaction(Guid Id) //metod för att ta bort transaction
    {
        var transaction = _transactionStore.GetAll().FirstOrDefault(transaction => transaction.Id == Id); //letar upp rätt Id 
        if (transaction == null)
        {
            return ServiceResult.NotFound;
        }
        _transactionStore.RemoveItem(transaction);
        return ServiceResult.Success;
    }

    public ServiceResult UpdateTransaction(Guid Id, decimal newamount, TransactionType newType, DateTime newDate, string newcategory) //string? så man kan sriva i retrun
    {
        var transaction = _transactionStore.GetAll().FirstOrDefault(transaction => transaction.Id == Id); //samma princip som Deleate metoden
        if (transaction == null)
        {
            return ServiceResult.NotFound;
        }
        if (Transaction.IsValid(newamount, newcategory) == false)//använder valedringsmetoden på dem nya värderna
        {
            return ServiceResult.InValidInput;
        }
        //Sätter de nya väderna genom Uptade metoden från Transaction klassen
        transaction.Update(newamount, newType, newDate, newcategory);

        return ServiceResult.Success;
    }

    public ServiceResult AddTransaction(decimal amount, TransactionType type, string category, DateTime? date = null)
    {
        if (Transaction.IsValid(amount, category) == false)
        {
            return ServiceResult.InValidInput;
        }

        Transaction newtransaction = new Transaction(amount, type, category, date ?? DateTime.Now);//Nytt obejekt, konstrukton sätter id och datum om inget annat anges

        _transactionStore.AddItem(newtransaction);
        return ServiceResult.Success;
    }
    public ServiceResult AddBudget(int month, int year, decimal limit)
    {
        string? FelMeddelande = MonthlyBudget.IsValid(month, year, limit); // Använder IsValid, får ur rätt fel beroende på felet
        if(FelMeddelande != null)
        {
            return ServiceResult.InValidInput;
        }

        bool exits = _budgetStore.GetAll().Any(budget => budget.Month == month && budget.Year == year); //kollar iaf de finns en budget redan
        if(exits)
        {
            return ServiceResult.AlreadyExist;
        }

        MonthlyBudget nyBudget = new MonthlyBudget(month, year, limit); // skapa nytt obejekt
        _budgetStore.AddItem(nyBudget); // lägger till
        return ServiceResult.Success;
    }
    public MonthlyBudget? GetBudget(int month, int year) // metod för att hämta rätt budget, kommer retunera null ifall den ej hittas
    {
        return _budgetStore.GetAll().FirstOrDefault(budget => budget.Month == month && budget.Year == year);
    }
    public ServiceResult UpdateMonthlyBudget(int month, int year, decimal newlimit)// metod för att uppdatera Budgeten obs ändast gränsen
    {
        MonthlyBudget? budget = GetBudget(month, year); // Använder getmetoden ifall jag får null så hittar den ej
        if(budget == null)
        {
            return ServiceResult.NotFound;
        }
        string? felMeddelande = budget.Update(newlimit); // använder update metoden från MonlyBudget klassen 
        if (felMeddelande != null)
        {
        return ServiceResult.InValidInput; // få tillbaka felet
        }
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
        return ServiceResult.Success; 
    }
    public decimal GetTotalincome(int month, int year)// Hitta rätt månad och summera inkomsterna 
    {
    return _transactionStore.GetAll().Where(t => t.Date.Month == month && t.Date.Year == year && t.Type == TransactionType.Income).Sum(t => t.Amount); // t för transaction
    }
    public decimal GetTotalexpense(int month, int year)// Hitta rätt månad och summera utgifterna 
    {
    return _transactionStore.GetAll().Where(t => t.Date.Month == month && t.Date.Year == year && t.Type == TransactionType.Expense).Sum(t => t.Amount); // t för transaction
    }
    public decimal GetIncomeDay(int day, int month, int year) // hitta rätt dag och summeera inkomsterna
    {
    return _transactionStore.GetAll().Where(t => t.Date.Day == day && t.Date.Month == month && t.Date.Year == year && t.Type == TransactionType.Income).Sum(t => t.Amount);
    }
    public decimal GetExpenseDay(int day, int month, int year) // hitta rätt dag och summeera utgifterna
    {
    return _transactionStore.GetAll().Where(t => t.Date.Day == day && t.Date.Month == month && t.Date.Year == year && t.Type == TransactionType.Expense).Sum(t => t.Amount);
    }

    public decimal ? GetRemainingBudget(int month, int year) // ? kan va null
    {
        MonthlyBudget? budget = GetBudget(month, year);
        if (budget == null) // om man inte hitta någon budget
        {
            return null;
        }
        decimal Expenses = GetTotalexpense(month, year);
        return budget.Limit - Expenses; // kan bli negativt tal
    }

}
