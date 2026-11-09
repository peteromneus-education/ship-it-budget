namespace ShipItBudget.UI
{
    public static class TransactionTypeText
    {
        public static string DisplayText(TransactionType type)
        {
            return type switch
            {
                TransactionType.Income => "Inkomst",
                TransactionType.Expense => "Utgift",
                _ => "Okänd"
            };
        }
    }
}
