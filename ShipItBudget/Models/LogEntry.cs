// En post i log.json: när något hände, hur allvarligt det var, vad det var och detaljer i klartext.
public class LogEntry
{
    // private set: värdena sätts bara i konstruktorn. Konstruktorns parameternamn måste matcha egenskaperna (logEvent/LogEvent), annars går log.json inte att läsa in igen.
    public DateTime Timestamp { get; private set; }
    public LogLevel Level { get; private set; }
    public LogEvent LogEvent { get; private set; }
    public string Message { get; private set; }

    public LogEntry(DateTime timestamp, LogLevel level, LogEvent logEvent, string message)
    {
        Timestamp = timestamp;
        Level = level;
        LogEvent = logEvent;
        Message = message;
    }
}
