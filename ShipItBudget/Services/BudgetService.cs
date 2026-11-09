public class BudgetService
{
    private List<Transaction> transactions = new List<Transaction>();
    //JASON ?

    public List<Transaction> GetsortedbyDate() //metod som sotera efter datumet
    {
        return transactions.OrderByDescending(transaction => transaction.Date).ToList();
    }

    public List<Transaction> GettransactionbyDate(DateTime ChoosenDate) // metod som hittar transaktioner efter ett visst datum
    {
        return transactions.Where(transaction => transaction.Date.Date == ChoosenDate.Date).ToList(); //Date.Date så man bar få datum
    }

    public bool DeleteTransaction(Guid Id) //metod för att ta bort transaction
    {
        var transaction = transactions.FirstOrDefault(transaction => transaction.Id == Id); //letar upp rätt Id 
        if (transaction == null)
        {
            return false;
        }
        transactions.Remove(transaction);
        return true;
    }

    public string? UpdateTransaction(Guid Id, decimal newamount, TransactionType newType, DateTime newDate, string newcategory) //string? så man kan sriva i retrun
    {
        var transaction = transactions.FirstOrDefault(transaction => transaction.Id == Id); //samma princip som Deleate metoden
        if (transaction == null)
        {
            return "Hittade inte transaktionen";
        }
        if (Transaction.IsValid(newamount, newcategory) == false)//använder valedringsmetoden på dem nya värderna
        {
            return "Fel! Ogiltiga värden";
        }
        //Sätter de nya väderna genom Uptade metoden från Transaction klassen
        transaction.Update(newamount, newType, newDate, newcategory);

        return null;
    }

    public bool AddTransaction(decimal amount, TransactionType type, string category, DateTime? date = null)
    {
        if (Transaction.IsValid(amount, category) == false)
        {
            return false;
        }

        Transaction newtransaction = new Transaction(amount, type, category, date ?? DateTime.Now);//Nytt obejekt, konstrukton sätter id och datum om inget annat anges

        transactions.Add(newtransaction);
        return true;
    }
}
