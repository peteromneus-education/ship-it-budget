// Läser och skriver JSON med generiska metoder. Rapportera filfel och trasig JSON så att befintliga data inte skrivs över av misstag.

using System.Text.Json;

public static class JsonFileStorage
{
    public static void Save<T>(string jsonFilePath, List<T> data)
    {
        JsonSerializerOptions jsonOptions = new()
        {
            WriteIndented = true
        };

        // Skapar mappen om den inte finns, annars får vi undantag när vi försöker skriva filen.
        Directory.CreateDirectory(Path.GetDirectoryName(jsonFilePath)!);
        string json = JsonSerializer.Serialize(data, jsonOptions);
        File.WriteAllText(jsonFilePath, json);
    }

    public static List<T> Load<T>(string jsonFilePath)
    {
        if (!File.Exists(jsonFilePath))
        {
            return [];
        }

        string json = File.ReadAllText(jsonFilePath);
        return JsonSerializer.Deserialize<List<T>>(json) ?? [];
    }
}
