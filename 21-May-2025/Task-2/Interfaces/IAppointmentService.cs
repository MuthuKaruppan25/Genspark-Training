using System;
using HospitalManagement.Models;
namespace HospitalManagement.Interfaces;

public interface IAppointmentService
{
    int AddAppointment(Appointment appointment);

    List<Appointment>? SearchAppointment(AppointmentSearchModel appointmentSearchModel);
}