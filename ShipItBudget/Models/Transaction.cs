using System.Text.Json.Serialization;

public class Transaction
{
    [JsonInclude] // Behövs för JSON-serialisering eftersom setter är privat.
    public Guid Id { get; private set; } // Guid är ett unikt id
    public decimal Amount { get; private set; }
    public TransactionType Type { get; private set; }
    public DateTime Date { get; private set; }
    public string Category { get; private set; }

    public void Update(decimal amount, TransactionType type, DateTime date, string category) // Gör det möjligt att uppdatera transaktionen utanför klassen
    {
        if (IsValid(amount, category) == false) // Stoppar en transaktion som inte uppfyller kraven i IsValid
        {
            throw new ArgumentException("Ogiltiga värden uppgavs"); // Avbryter
        }
        Amount = amount;
        Type = type;
        Date = date;
        Category = category;
    }

    public static bool IsValid(decimal amount, string category) // Valideringsmetod som kontrollerar att en transaktion är giltig
    {
        if (amount <= 0) return false;
        if (string.IsNullOrWhiteSpace(category)) return false;
        return true;

    }

    public Transaction(decimal amount, TransactionType type, string category, DateTime date) // Skapar en transaktion
    {
        if (IsValid(amount, category) == false) // Stoppar en transaktion som inte uppfyller kraven i IsValid
        {
            throw new ArgumentException("Ogiltiga värden uppgavs"); // Avbryter
        }

        Id = Guid.NewGuid();
        Amount = amount;
        Type = type;
        Date = date;
        Category = category;
    }
}
