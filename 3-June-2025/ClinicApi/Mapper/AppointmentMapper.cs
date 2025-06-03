

using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

public class AppointmentMapper
{
    public Appointment? MapAddAppointment(AppointmentAddDto appointmentAddDto)
    {
        Appointment appointment = new();
        appointment.AppointmnetNumber = Guid.NewGuid().ToString();
        appointment.DoctorId = appointmentAddDto.DoctorId;
        appointment.PatientId = appointmentAddDto.PatientId;
        appointment.AppointmnetDateTime = appointmentAddDto.AppointmnetDateTime;
        appointment.Status = "Active";
        return appointment;
    }
}