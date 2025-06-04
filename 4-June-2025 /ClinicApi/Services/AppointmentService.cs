using System;
using System.Security.Claims;
using System.Threading.Tasks;
using SecondWebApi.Interfaces;
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

public class AppointmentService : IAppointmentService
{
    private readonly IRepository<string, Appointment> _appointmentRepository;
    private readonly IRepository<int, Doctor> _doctorRepository;
    private readonly IRepository<int, Patient> _patientRepository;
    private readonly AppointmentMapper appointmentMapper;

    public AppointmentService(
        IRepository<string, Appointment> appointmentRepository,
        IRepository<int, Doctor> doctorRepository,
        IRepository<int, Patient> patientRepository)
    {
        _appointmentRepository = appointmentRepository;
        _doctorRepository = doctorRepository;
        _patientRepository = patientRepository;
        appointmentMapper = new AppointmentMapper();
    }

    public async Task<Appointment?> CreateAppointment(AppointmentAddDto appointmentAddDto)
    {
        try
        {
            var doctor = await _doctorRepository.Get(appointmentAddDto.DoctorId);
            var patient = await _patientRepository.Get(appointmentAddDto.PatientId);

            if (doctor == null || patient == null)
                throw new Exception("Patient Or Doctor Not Found");

            var appointment = appointmentMapper.MapAddAppointment(appointmentAddDto);

            return await _appointmentRepository.Add(appointment!);
        }
        catch (Exception ex)
        {
            // You can use a logger here (e.g., ILogger) instead of Console
            throw new Exception(ex.Message);

        }
    }

    public async Task<Appointment?> CancelAppointment(string appointmentNumber, ClaimsPrincipal user)
    {
        try
        {
            var email = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(email))
            {
                throw new Exception("User is not Authorized");
            }
            

            var doctors = await _doctorRepository.GetAll();
            var requestingDoctor = doctors.FirstOrDefault(d => d.Email == email);
            if (requestingDoctor == null)
            {
                throw new Exception("Doctor with Email Not Found");
            }
            
            

            var appointment = await _appointmentRepository.Get(appointmentNumber);

            if (requestingDoctor.Id != appointment.DoctorId)
            {
                throw new Exception("Cancellation of Appointments Not Allowed for other patients");
            }
            if (appointment == null)
                    throw new Exception("Appointment Not Found");



            appointment.Status = "Cancelled";
            return await _appointmentRepository.Update(appointmentNumber, appointment);
        }
        catch (Exception ex)
        {
            
            throw new Exception(ex.Message);

        }
    }

    public async Task<Appointment?> GetAppointment(string appointmentNumber)
    {
        try
        {
            var appointment = await _appointmentRepository.Get(appointmentNumber);
            if (appointment == null)
                throw new Exception("Appointment Not Found");
            return appointment;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);

        }
    }
}
