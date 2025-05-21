using System;
using HospitalManagement.Exceptions;
using HospitalManagement.Models;
namespace HospitalManagement.Repositories;

public class AppointmentRepository: Repository<int,Appointment> 
{
    protected override int GenerateId()
    {
        if(_items.Count == 0)
        return 101;
        else{
            return _items.Max(a => a.Id) + 1;
        }
    }

    public override Appointment GetById(int id)
    {
        var Appointment = _items.FirstOrDefault(e => e.Id == id);
        if(Appointment is null)
        {
             throw new KeyNotFoundException("Appointment with the Given Id Not Found");
        }
        return Appointment;
    }

    public override ICollection<Appointment> GetAll()
    {
        if(_items.Count == 0)
        {
            throw new CollectionEmptyException("No Appointments Found");
        }
        return _items;
    }
}
