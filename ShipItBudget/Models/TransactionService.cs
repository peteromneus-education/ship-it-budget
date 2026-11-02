using System;

public class TransactionService
{
    public List<Transaction> transactions = new List<Transaction>();
    //JASON ?


    public List<Transaction> GetsortedbyDate() //metod som sotera efter datumet
    {
        return transactions.OrderByDescending(transaction => transaction.Date).ToList();
    }

    public List<Transaction> GettransactionbyDate(DateTime ChoosenDate) // metod som hittar transaktioner efter ett visst datum
    {
        return transactions.Where(transaction => transaction.Date.Date == ChoosenDate.Date).ToList(); //Date.Date så man bar få datum
    }

}