using System.Text.Json.Serialization;

public class Transaction
{
    [JsonInclude]
    public Guid Id { get; private set; }// Guid skapar unikt id nummer
    public decimal Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public DateTime Date { get; private set; }
    public string Category { get; private set; }

    public void Update(decimal amount, TransactionType type, DateTime date, string category) //metod som gör det möjligt att upptadera utanför klassen
    {
        if (IsValid(amount, category) == false) // sätter stopp för en transaktion som inte uppfyller kraven i Isvalid metoden
        {
            throw new ArgumentException("Ogiltiga värden uppgavs"); //brytter den nya transaktionen
        }
        Amount = amount;
        Type = type;
        Date = date;
        Category = category;
    }

    public static bool IsValid(decimal amount, string category) //Valederings metod som kollar så en transaktion är Valid
    {
        if (amount <= 0) return false;
        if (string.IsNullOrWhiteSpace(category)) return false;
        return true;

    }

    public Transaction(decimal amount, TransactionType type, string category, DateTime date) // Konstruktör 
    {
        if (IsValid(amount, category) == false) // sätter stopp för en transaktion som inte uppfyller kraven i Isvalid metoden
        {
            throw new ArgumentException("Ogiltiga värden uppgavs"); //brytter den nya transaktionen
        }

        Id = Guid.NewGuid();
        Amount = amount;
        Type = type;
        Date = date;
        Category = category;
    }
}
