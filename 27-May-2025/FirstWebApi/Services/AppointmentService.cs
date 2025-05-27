public class AppointmentService
{
    private readonly IRepository<int, Appointment> _repository;
    private readonly DoctorService _doctorService;
    private readonly PatientService _patientService;
    public AppointmentService(IRepository<int, Appointment> repository, DoctorService doctorService, PatientService patientService)
    {
        _repository = repository;
        _doctorService = doctorService;
        _patientService = patientService;

    }
    public Appointment? CreateAppointment(Appointment appointment)
    {
        try
        {
            var app = _repository.GetById(appointment.Id);
            if (app != null)
            {
                throw new InvalidOperationException($"Appointment with ID {appointment.Id} already exists.");
            }
            var doctor = _doctorService.GetDoctorById(appointment.DoctorId);
            if (doctor == null)
            {
                throw new ArgumentException($"Doctor with ID {appointment.DoctorId} does not exist.");
            }
            var patient = _patientService.GetPatientById(appointment.PatientId);
            if (patient == null)
            {
                throw new ArgumentException($"Patient with ID {appointment.PatientId} does not exist.");
            }
            if (appointment.AppointmentDate < DateTime.Now)
            {
                throw new ArgumentException("Appointment date cannot be in the past.");
            }
            if (string.IsNullOrWhiteSpace(appointment.Description))
            {
                throw new ArgumentException("Appointment description cannot be empty.");
            }
            _repository.Add(appointment);
            return appointment;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error validating appointment: {ex.Message}");
            return null;
        }

    }
    public void CancelAppointment(int id)
    {
        try
        {
            _repository.Delete(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error cancelling appointment with ID {id}: {ex.Message}");
        }
    }
    public Appointment? GetAppointmentById(int id)
    {
        try
        {
            return _repository.GetById(id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving appointment with ID {id}: {ex.Message}");
            return null;
        }
    }
    public IEnumerable<Appointment> GetAllAppointments()
    {
        try
        {
            var appointments = _repository.GetAll();
            if (appointments.Count() == 0)
            {
                throw new KeyNotFoundException("No appointments found in the repository");
            }
            return appointments;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving all appointments: {ex.Message}");
            return Enumerable.Empty<Appointment>();
        }
    }

    public Appointment? RescheduleAppointment(int id, Appointment updatedAppointment)
    {
        try
        {
            if(updatedAppointment.AppointmentDate < DateTime.Now)
            {
                throw new ArgumentException("Appointment date cannot be in the past.");
            }
            var appointments = _repository.GetAll();

            bool hasConflict = appointments.Any(other =>
                other.Id != id &&
                (other.DoctorId == updatedAppointment.DoctorId || other.PatientId == updatedAppointment.PatientId) &&
                Math.Abs((other.AppointmentDate - updatedAppointment.AppointmentDate).TotalMinutes) < 30
            );

            if (hasConflict)
            {
                throw new InvalidOperationException("Cannot reschedule: another appointment for this doctor or patient is within 30 minutes.");
            }

            return _repository.Update(id, updatedAppointment);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating appointment: {ex.Message}");
            return null;
        }
    }

}