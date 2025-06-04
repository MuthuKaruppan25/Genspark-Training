
using System.Security.Claims;
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

namespace SecondWebApi.Interfaces;


public interface IAppointmentService
{
    Task<Appointment?> CreateAppointment(AppointmentAddDto appointmentAddDto);
    Task<Appointment?> CancelAppointment(string appointmentNumber,ClaimsPrincipal user);

    Task<Appointment?> GetAppointment(string appointmentNumber);

    
}