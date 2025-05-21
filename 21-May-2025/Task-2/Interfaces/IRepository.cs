using System;

namespace HospitalManagement.Interfaces;

public interface IRepository<K,T> where T:class
{
    T Add(T item);
    T GetById(K Id);
    ICollection<T> GetAll();

}
