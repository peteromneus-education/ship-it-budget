using System;

// Beskriver månadens utgiftsgräns. Year och Month identifierar budgeten; utfallet beräknas från transaktionerna.
/* Låt tjänsten hantera en total utgiftsgräns per månad och beräkna utfallet.

Klart när

En månadsbudget kan skapas, hämtas, ändras och tas bort enligt det överenskomna kontraktet.

År och månad identifierar budgeten; samma månad kan inte få dubbla budgetar.

Budgetgränsen kan vara noll men inte negativ, och månaden måste vara giltig.

Inkomster och utgifter summeras separat för en vald dag och månad med LINQ.

Återstående budget beräknas som utgiftsgräns minus månadens utgifter och kan vara negativ.

Avsaknad av budget skiljs från en budget på noll; beräknade summor lagras inte som extra modellfält. */
public class MonthlyBudget
{
    public int Month {get; private set;}
    public int Year {get; private set;}
    public decimal Limit {get; private set;}

    //Följer samma princip som Transactionklassen med konstruktör och valederings metod. Olika felmeddelande beroende på fel
    public MonthlyBudget(int month, int year, decimal limit)
    {
        string? FelMeddelande = IsValid(month, year, limit);
        if(FelMeddelande != null)
        {
            throw new(FelMeddelande);
        }
        Month = month;
        Year = year;
        Limit = limit;

    }
    public static string? IsValid(int month, int year, decimal limit) //Samma som Transaction klassen kolla så en budget uppfyller kraven
    {
        if(month < 1 || month > 12)
        return "Ange en giltig månad 1-12";

        if(year < 2025 || year > 2100)
        return "Obs! Ange ett giltigt årtal";

        if(limit < 0)
        return "Budget gränsen kan inte va negativ";

        return null;
    }

    public string? Update(decimal newlimit) // Metod för att kunna nå Limiten, månad och år e låst 
    {
        if(newlimit < 0)
        {
            return "Budget gränsen kan inte va negativ";
        }
        Limit = newlimit;
        return null;
    }
}

