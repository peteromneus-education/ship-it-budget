// Håller objekt av typen T i minnet. Implementera samlingshanteringen här och låt JsonFileStorage sköta filformatet.
public class DataStore //Tiflällig kod så att koden funkar
{
    
    public List<MonthlyBudget> MonthlyBudgets { get; set; } = new List<MonthlyBudget>();
    public List<Transaction> Transactions { get; set; } = new List<Transaction>();
}