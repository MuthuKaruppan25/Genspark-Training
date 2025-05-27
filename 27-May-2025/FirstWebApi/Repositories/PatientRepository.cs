
public class PatientRepository : Repository<int, Patient>
{
    public override Patient GetById(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "ID must be greater than zero");
        }

        var parient = _items.FirstOrDefault(p => p.Id == id);
        if (parient == null)
        {
            throw new KeyNotFoundException($"Patient with ID {id} not found");
        }
        return parient;
    }

    public override IEnumerable<Patient> GetAll()
    {
        return _items;
    }
    
}