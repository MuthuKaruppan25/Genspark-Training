public class DoctorService
{
    private readonly IRepository<int, Doctor> _repository;

    public DoctorService(IRepository<int, Doctor> repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository), "Repository cannot be null");
    }

    public Doctor? AddDoctor(Doctor doctor)
    {
        try
        {
            var doc = _repository.GetById(doctor.Id);
            if (doc != null)
            {
                throw new InvalidOperationException($"Doctor with ID {doctor.Id} already exists");
            }
            if (doctor == null)
            {
                throw new ArgumentNullException(nameof(doctor), "Doctor cannot be null");
            }
            _repository.Add(doctor);
            return doctor;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding doctor: {ex.Message}");
            return null;
        }
    }
    public Doctor? UpdateDoctor(int id, Doctor doctor)
    {
        try
        {

            return _repository.Update(id, doctor);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating doctor with ID {id}: {ex.Message}");
            return null;
        }
    }

    public void DeleteDoctor(int id)
    {
        try
        {
            _repository.Delete(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting doctor with ID {id}: {ex.Message}");
        }
    }

    public Doctor? GetDoctorById(int id)
    {
        try
        {
            return _repository.GetById(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving doctor with ID {id}: {ex.Message}");
            return null;
        }
    }

    public IEnumerable<Doctor> GetAllDoctors()
    {
        try
        {
            var doctors = _repository.GetAll();
            if (doctors.Count() == 0)
            {
                throw new KeyNotFoundException("No doctors found in the repository");
            }
            return doctors;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving all doctors: {ex.Message}");
            return Enumerable.Empty<Doctor>();
        }
    }
}