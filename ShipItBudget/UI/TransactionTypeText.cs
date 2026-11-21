namespace ShipItBudget.UI
{
    public static class TransactionTypeText
    {
        public static string DisplayText(TransactionType type)
        {
            // Switch-uttryck: varje enumvärde översätts till svensk text. _ fångar alla andra värden.
            return type switch
            {
                TransactionType.Income => "Inkomst",
                TransactionType.Expense => "Utgift",
                _ => "Okänd"
            };
        }
    }
}
