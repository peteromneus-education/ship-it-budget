using System;
using System.Data.Common;
using System.Runtime;


public class Transaction
{
    public Guid Id {get; set;}// Guid skapar unikt id nummer
    public decimal Amount {get; set;}
    public TransactionType Type {get; set;}
    public DateTime Date {get; set;} 
    public string Category {get; set;}


    public static bool Isvalid(decimal amount, string category) //Valederings metod som kollar så en transaktion är Valid
    {
        if(amount <= 0) return false;
        if(string.IsNullOrWhiteSpace(category)) return false;
        return true;

    }

    public Transaction(decimal amount, TransactionType type, string category ) // Konstruktör 
    {
        if(Isvalid(amount, category) == false) // sätter stopp för en transaktion som inte uppfyller kraven i Isvalid metoden
        {
            throw new ArgumentException("Ogiltiga värden uppgavs"); //brytter den nya transaktionen
        }

        Id = Guid.NewGuid();
        Amount = amount; 
        Type = type;
        Date = DateTime.Now; // Sätter så datumet blir direkt när en transaktion skapas
        Category = category;

    }
}



