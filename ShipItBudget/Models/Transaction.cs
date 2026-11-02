using System;
using System.Data.Common;
using System.Runtime;
// Beskriver en inkomst eller utgift. Använd Id för att hitta rätt transaktion vid ändring och radering.
/* en ny transaktion får ett unikt ID och sparas via datalagringen.

Belopp måste vara större än noll och inkomst/utgift anges med TransactionType.

Obligatoriska uppgifter valideras innan transaktionen läggs till.

Tjänsten kan hämta och sortera transaktioner för ett valt datum med LINQ. I BudgetService

Flera transaktioner på samma datum och dagar utan transaktioner fungerar */

public class Transaction
{
    public Guid Id {get; set;}// Guid skapar unikt id nummer
    public decimal Amount {get; set;}
    public TransactionType Type {get; set;}
    public DateTime Date {get; set;} 
    public string Category {get; set;}

    public Transaction(decimal amount, TransactionType type, string category ) // Konstruktör 
    {
      
        Id = Guid.NewGuid();
        Amount = amount; 
        Type = type;
        Date = DateTime.Now; // Sätter så datumet blir direkt när en transaktion skapas
        Category = category;

    }

}



