public class DoctorRepository : Repository<int, Doctor>
{
    public override Doctor GetById(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentException("ID cannot be null or empty", nameof(id));
        }

        var doctor = _items.FirstOrDefault(d => d.Id == id);
        if (doctor == null)
        {
            throw new KeyNotFoundException($"Doctor with ID {id} not found");
        }
        return doctor;
    }

    public override IEnumerable<Doctor> GetAll()
    {
        return _items;
    }

    
}