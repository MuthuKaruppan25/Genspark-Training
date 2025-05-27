


public class AppointmentRepository : Repository<int, Appointment>
{
    public override Appointment GetById(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero");
        }

        var appointment = _items.FirstOrDefault(a => a.Id == id);
        if (appointment == null)
        {
            throw new KeyNotFoundException($"Appointment with ID {id} not found");
        }
        return appointment;
    }

    public override IEnumerable<Appointment> GetAll()
    {
        return _items;
    }

    public IEnumerable<Appointment> GetByPatientId(int patientId)
    {
        if (patientId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(patientId), "Patient ID must be greater than zero");
        }

        return _items.Where(a => a.PatientId == patientId);
    }
}