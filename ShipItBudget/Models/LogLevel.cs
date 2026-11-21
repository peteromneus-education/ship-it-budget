using System.Text.Json.Serialization;

// Skiljer vanliga händelser från fel i loggen.
// Sparar värdet som text ("Info", "Error") i stället för ett tal i log.json.
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LogLevel
{
    Info,
    Error
}
