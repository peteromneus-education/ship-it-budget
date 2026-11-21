// Håller objekt av typen T i minnet. Implementera samlingshanteringen här och låt JsonFileStorage sköta filformatet.

using System.Text.Json;

namespace ShipItBudget
{
    public class DataStore<T>
    {
        private List<T> items = new List<T>();

        private string filePath;
        // Sätts om inläsningen misslyckas. Då vägrar SaveToJson att spara, så att en trasig fil inte skrivs över med en tom lista.
        private bool loadFailed = false;

        // => är en kortform för en metod med en enda rad. this.filePath är fältet, filePath är parametern.
        public DataStore(string filePath) => this.filePath = filePath;

        public void AddItem(T item)
        {
            items.Add(item);
        }

        public bool RemoveItem(T item)
        {
            return items.Remove(item);
        }

        // Func<T, bool> är ett villkor som skickas in som parameter, t.ex. t => t.Id == id.
        public T? FindItem(Func<T, bool> condition)
        {
            foreach (T item in items)
            {
                if (condition(item))
                {
                    return item;
                }
            }

            // default är null för klasser, alltså "hittades inte".
            return default;
        }

        public List<T> GetAll()
        {
            return items;
        }

        // Ladda och spara data till JSON med hjälp av JsonFileStorage. Rapportera filfel och trasig JSON så att befintliga data inte skrivs över av misstag.
        public bool LoadFromJson()
        {
            try
            {
                items = JsonFileStorage.Load<T>(filePath);
                return true;
            }
            // when fångar bara de listade undantagstyperna. Oväntade fel får krascha programmet, så att de syns och kan rättas.
            catch (Exception ex) when (ex is
                JsonException or
                IOException or
                InvalidOperationException or
                ArgumentException or
                UnauthorizedAccessException)
            { loadFailed = true; return false; }
        }

        // Spara data till JSON med hjälp av JsonFileStorage. Rapportera filfel så att befintliga data inte skrivs över av misstag.
        public bool SaveToJson()
        {
            if (loadFailed)
            { return false; }

            try
            {
                JsonFileStorage.Save<T>(filePath, items);
                return true;
            }
            catch (Exception ex) when (ex is
                IOException or
                InvalidOperationException or
                ArgumentException or
                UnauthorizedAccessException)
            { return false; }
        }
    }
}
