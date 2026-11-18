using System.Text.Json.Serialization;

// Vilken typ av händelse som loggas.
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
