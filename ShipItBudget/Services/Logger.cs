using ShipItBudget;

// Skapar loggposter och sparar dem direkt i log.json.
public class Logger
{
    private readonly DataStore<LogEntry> _logStore;

    public Logger(DataStore<LogEntry> logStore)
    {
        _logStore = logStore;
    }

    // Info och Error är genvägar till Write med rätt LogLevel, så att anroparen inte behöver ange nivån själv.
    public void Info(LogEvent logEvent, string message)
    {
        Write(LogLevel.Info, logEvent, message);
    }

    public void Error(LogEvent logEvent, string message)
    {
        Write(LogLevel.Error, logEvent, message);
    }

    private void Write(LogLevel level, LogEvent logEvent, string message)
    {
        _logStore.AddItem(new LogEntry(DateTime.Now, level, logEvent, message));

        // Sparas direkt så att loggen finns kvar även om programmet kraschar.
        // Returvärdet ignoreras medvetet: ett loggfel ska inte stoppa programmet.
        _logStore.SaveToJson();
    }
}
