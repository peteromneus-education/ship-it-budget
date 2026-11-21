// Beskriver månadens utgiftsgräns. Year och Month identifierar budgeten; utfallet beräknas från transaktionerna.

public class MonthlyBudget
{
    public int Month {get; private set;}
    public int Year {get; private set;}
    public decimal Limit {get; private set;}

    // Följer samma princip som Transaction-klassen med konstruktor och valideringsmetod. Olika felmeddelanden beroende på fel
    public MonthlyBudget(int month, int year, decimal limit)
    {
        string? errorMessage = IsValid(month, year, limit);
        if(errorMessage != null)
        {
            throw new(errorMessage);
        }
        Month = month;
        Year = year;
        Limit = limit;
    }

    public static string? IsValid(int month, int year, decimal limit) // Samma som i Transaction-klassen: kontrollerar att en budget uppfyller kraven
    {
        if(month < 1 || month > 12)
        return "Ange en giltig månad 1-12";

        if(year < 2025 || year > 2100)
        return "Obs! Ange ett giltigt årtal";

        if(limit < 0)
        return "Budget gränsen kan inte va negativ";

        return null;
    }

    public string? Update(decimal newLimit) // Ändrar gränsen; månad och år går inte att ändra
    {
        if(newLimit < 0)
        {
            return "Budget gränsen kan inte va negativ";
        }
        Limit = newLimit;
        return null;
    }
}
