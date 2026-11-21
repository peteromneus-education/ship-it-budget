// Läser och skriver JSON med generiska metoder. Fel kastas vidare till DataStore, som hanterar dem.

using System.Text.Json;

public static class JsonFileStorage
{
    public static void Save<T>(string jsonFilePath, List<T> data)
    {
        JsonSerializerOptions jsonOptions = new()
        {
            // Radbrytningar och indrag, så att JSON-filen går lättare att läsa för människor.
            WriteIndented = true
        };

        // Skapar mappen om den inte finns, annars får vi undantag när vi försöker skriva filen.
        // ! säger till kompilatorn att GetDirectoryName inte returnerar null här, eftersom sökvägen alltid innehåller en mapp.
        Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath)!);
        string json = JsonSerializer.Serialize(data, jsonOptions);
        File.WriteAllText(jsonFilePath, json);
    }

    public static List<T> Load<T>(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
        {
            // Ingen fil vid första start. [] skapar en tom samling av den typ metoden returnerar, här List<T>.
            return [];
        }

        string json = File.ReadAllText(jsonFilePath);
        // ?? [] ger en tom lista om Deserialize returnerar null, t.ex. när filen bara innehåller "null".
        return JsonSerializer.Deserialize<List<T>>(json) ?? [];
    }
}
