// Håller objekt av typen T i minnet. Implementera samlingshanteringen här och låt JsonFileStorage sköta filformatet.

namespace ShipItBudget
{
    public class DataStore<T>
    {
        private List<T> items = new List<T>();


        public void AddItem(T item)
        {
            items.Add(item);
        }


        public bool RemoveItem(T item)
        {
            return items.Remove(item);
        }


        public T? FindItem(Func<T, bool> condition)
        {
            foreach (T item in items)
            {
                if (condition(item))
                {
                    return item;
                }
            }

            return default;
        }


        public List<T> GetAll()
        {
            return items;
        }
    }
}