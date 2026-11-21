using System.Text.Json.Serialization;

// Vilken typ av händelse som loggas.
// Sparar värdet som text ("TransactionAdded") i stället för ett tal i log.json, så att filen går att läsa.
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LogEvent
{
    TransactionAdded,
    TransactionUpdated,
    TransactionDeleted,
    BudgetAdded,
    BudgetUpdated,
    BudgetRemoved,
    LoadFailed,
    SaveFailed
}
