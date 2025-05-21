using System;
using HospitalManagement.Interfaces;
using HospitalManagement.Exceptions;
namespace HospitalManagement.Repositories;

public abstract class Repository<K, T> : IRepository<K, T> where T : class
{
    protected List<T> _items = new List<T>();
    protected abstract K GenerateId();
    public abstract T GetById(K id);
    public abstract ICollection<T> GetAll();

    public T Add(T item)
    {
        var prop = typeof(T).GetProperty("Id");
        if (prop != null)
        {
            prop.SetValue(item, GenerateId());
        }
        if (_items.Contains(item))
        {
            throw new DuplicateEntityException("Appointment with ID Already Exists");
        }
        _items.Add(item);
        return item;
    }

}