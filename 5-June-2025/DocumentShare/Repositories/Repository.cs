


using DocumentShare.Contexts;
using DocumentShare.Interfaces;

namespace DocumentShare.Repositories;

public abstract class Repository<K, T> : IRepository<K, T> where T : class
{
    protected readonly FileContext _clinicContext;

    public Repository(FileContext clinicContext)
    {
        _clinicContext = clinicContext;
    }
    public async Task<T> Add(T item)
    {
        _clinicContext.Add(item);
        await _clinicContext.SaveChangesAsync();
        return item;
    }

    public async Task<T> Delete(K key)
    {
        var item = await Get(key);
        if (item != null)
        {
            _clinicContext.Remove(item);
            await _clinicContext.SaveChangesAsync();
            return item;
        }
        
        throw new Exception("No such item found for deleting");
    }

    public abstract Task<T> Get(K key);


    public abstract Task<IEnumerable<T>> GetAll();


    public async Task<T> Update(K key, T item)
    {
        var myItem = await Get(key);
        if (myItem != null)
        {
            _clinicContext.Entry(myItem).CurrentValues.SetValues(item);
            await _clinicContext.SaveChangesAsync();
            return item;
        }
        throw new Exception("No such item found for updation");
    }
}