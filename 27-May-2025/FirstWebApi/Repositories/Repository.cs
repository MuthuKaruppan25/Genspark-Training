


public abstract class Repository<K, T> : IRepository<K, T> where T : class
{
    protected readonly List<T> _items = new List<T>();

    public T Add(T item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item), "Item cannot be null");
        }


        _items.Add(item);
        return item;
    }

    public T Update(K id, T item)
    {
        if (id == null)
        {
            throw new ArgumentNullException(nameof(id), "ID cannot be null");
        }
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item), "Item cannot be null");
        }
        var index = _items.FindIndex(i => i.Equals(GetById(id)));
        if (index < 0)
        {
            throw new KeyNotFoundException($"Item with ID {id} not found");
        }
        _items[index] = item;
       
        return item;
    }

    public void Delete(K id)
    {
        if (id == null)
        {
            throw new ArgumentNullException(nameof(id), "ID cannot be null");
        }

        var item = GetById(id);
        if (item == null)
        {
            throw new KeyNotFoundException($"Item with ID {id} not found");
        }

        _items.Remove(item);
    }

    public abstract T GetById(K id);
    public abstract IEnumerable<T> GetAll();
  
}