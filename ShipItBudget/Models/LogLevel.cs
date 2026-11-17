using System.Text.Json.Serialization;

// Skiljer vanliga händelser från fel i loggen.
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LogLevel
{
    Info,
    Error
}
