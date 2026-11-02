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

    public Transaction(decimal amount, TransactionType type, string category ) // Konstruktör 
    {
        if(amount <= 0) // sätter stopp för en transaktion som är mindre än 0, Oklart om den ska vara kvar
        {
            throw new ArgumentException("Beloppet måste va större än 0"); //brytter den nya transaktionen
        }

        Id = Guid.NewGuid();
        Amount = amount; 
        Type = type;
        Date = DateTime.Now; // Sätter så datumet blir direkt när en transaktion skapas
        Category = category;

    }
}



