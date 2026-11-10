// Håller objekt av typen T i minnet. Implementera samlingshanteringen här och låt JsonFileStorage sköta filformatet.
public class DataStore
{
    // Tillfälliga egenskaper så att koden kompilerar
    public List<MonthlyBudget> MonthlyBudgets { get; set; } = new List<MonthlyBudget>();
    public List<Transaction> Transactions { get; set; } = new List<Transaction>();
}