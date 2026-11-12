// Kategorier för transaktioner, olika för inkomster och utgifter. Ursprungligen från Firats PR #34.

public static class Categories
{
    public static readonly string[] Income = { "Lön", "Bidrag", "Återbetalning", "Försäljning", "Övrigt" };

    public static readonly string[] Expense = { "Boende", "Mat", "Transport", "Räkningar", "Försäkringar",
  "Hälsa", "Nöje", "Kläder", "Sparande", "Övrigt" };
}