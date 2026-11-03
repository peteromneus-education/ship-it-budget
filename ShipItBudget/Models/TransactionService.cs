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

    public bool DetelateTransaction(Guid Id) //metod för att ta bort transaction
    {
        var transaction = transactions.FirstOrDefault(transaction => transaction.Id == Id); //letar upp rätt Id 
        if(transaction == null)
        {
            return false;
        }
        transactions.Remove(transaction);
        return true;
    }

    public bool UpdateTransaction(Guid Id, decimal newamount, TransactionType newType, DateTime newDate, string newcatagory)
    {
        var transaction = transactions.FirstOrDefault(transaction => transaction.Id == Id); //samma princip som Deleate metoden
        if(transaction == null)
        {
            return false;
        }
        if(Transaction.Isvalid(newamount, newcatagory) == false)//använder valedringsmetoden på dem nya värderna
        {
            return false;
        }
        //Sätter de nya väderna 
        transaction.Amount = newamount;
        transaction.Type = newType;
        transaction.Date = newDate;
        transaction.Category = newcatagory;

        return true;
    }



    

}