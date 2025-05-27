
public interface IRepository<K, T> where T : class
{
    T Add(T item);
    void Delete(K id);
    T GetById(K id);

    T Update(K id, T item);
    IEnumerable<T> GetAll();
}