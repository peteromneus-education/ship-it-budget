// Kopplar ihop lagring, logik och UI.

 using ShipItBudget.UI;

//CalendarView calendarView = new();
//calendarView.Show();  




namespace ShipItBudget
{
    class Program
    {
        static void Main(string[] args)
        {
            // Skapa DataStore för Transaction
            DataStore<Transaction> transactionStore = new DataStore<Transaction>();

            // Kontrollera tom lista
            Console.WriteLine("Antal transaktioner: " + transactionStore.GetAll().Count);

            // Skapa en transaktion
            Transaction transaction = new Transaction(
                500,
                TransactionType.Expense,
                "Food"
            );

            // Lägg till transaktionen
            transactionStore.AddItem(transaction);

            Console.WriteLine("Antal efter tillägg: " + transactionStore.GetAll().Count);

            // Söka efter transaktionen
            Transaction? foundTransaction = transactionStore.FindItem(
                t => t.Id == transaction.Id
            );

            if (foundTransaction != null)
            {
                Console.WriteLine("Transaktionen hittades!");
            }
            else
            {
                Console.WriteLine("Transaktionen hittades inte.");
            }

            // Ta bort transaktionen
            bool removed = transactionStore.RemoveItem(transaction);

            Console.WriteLine("Borttagen: " + removed);

            // Försöka ta bort samma transaktion igen
            bool removedAgain = transactionStore.RemoveItem(transaction);

            Console.WriteLine("Borttagning igen: " + removedAgain);
        }
    }
}



