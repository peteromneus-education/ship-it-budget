
namespace ShipItBudget
{
    public class CategoryService
    {
        // för inkomster
        private List<string> incomeCategories = new List<string>
        {
            "Lön",
            "Bidrag",
            "Övrigt"
        };

        // för utgifter
        private List<string> expenseCategories = new List<string>
        {
            "Mat",
            "Hyra",
            "Transport",
            "Aktivitet",
            "Övrigt"
        };

        // Hämtar beroende på transaktionstyp
        public List<string> GetCategoriesByType(TransactionType type)
        {
            if (type == TransactionType.Income)
            {
                return incomeCategories;
            }
            else
            {
                return expenseCategories;
            }
        }
    }
}
