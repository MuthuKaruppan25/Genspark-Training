
using JobPortal.Contexts;
using JobPortal.Interfaces;

namespace JobPortal.Repositories;

public abstract class Repository<K, T> : IRepository<K, T> where T : class
{
    protected readonly JobContext _jobContext;

    public Repository(JobContext jobContext)
    {
        _jobContext = jobContext;
    }
    public async Task<T> Add(T item)
    {
        _jobContext .Add(item);
        await _jobContext.SaveChangesAsync();
        return item;
    }

    public async Task<T> Delete(K key)
    {
        var item = await Get(key);
        if (item != null)
        {
            _jobContext.Remove(item);
            await _jobContext .SaveChangesAsync();
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
            _jobContext .Entry(myItem).CurrentValues.SetValues(item);
            await _jobContext.SaveChangesAsync();
            return item;
        }
        throw new Exception("No such item found for updation");
    }
}