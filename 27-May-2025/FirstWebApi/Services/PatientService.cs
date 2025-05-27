

public class PatientService
{
    private readonly IRepository<int, Patient> _repository;

    public PatientService(IRepository<int, Patient> repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository), "Repository cannot be null");
    }

    public Patient? AddPatient(Patient patient)
    {
        try
        {
            var existingPatient = _repository.GetById(patient.Id);
            if (existingPatient != null)
            {
                throw new InvalidOperationException($"Patient with ID {patient.Id} already exists");
            }
            if (patient == null)
            {
                throw new ArgumentNullException(nameof(patient), "Patient cannot be null");
            }
            _repository.Add(patient);
            return patient;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding patient: {ex.Message}");
            return null;
        }
    }
    public void DeletePatient(int id)
    {
        try
        {

            _repository.Delete(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting patient with ID {id}: {ex.Message}");
        }
    }
    public Patient? UpdatePatient(int id, Patient patient)
    {
        try
        {
            return _repository.Update(id, patient);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating patient with ID {id}: {ex.Message}");
            return null;
        }
    }
    public Patient? GetPatientById(int id)
    {
        try
        {
            return _repository.GetById(id);
        }

        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving patient with ID {id}: {ex.Message}");
            return null;
        }
    }
    public IEnumerable<Patient> GetAllPatients()
    {
        try
        {
            var patients = _repository.GetAll();
            if (patients.Count() == 0)
            {
                throw new KeyNotFoundException("No patients found in the repository");
            }
            return patients;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving all patients: {ex.Message}");
            return Enumerable.Empty<Patient>();
        }
    }
}