using System;
namespace HospitalManagement.Models;

public class Appointment
{
    public int Id { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public int PatientAge { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string Reason { get; set; } = string.Empty;

    // public Appointment(int Id,string patientName, int patientAge, DateTime appointmentDate, string reason)
    // {
    //     this.PatientName = patientName;
    //     this.PatientAge = patientAge;
    //     this.AppointmentDate = appointmentDate;
    //     this.Reason = reason;
    // }

    public override string ToString()
    {
        return "Appointment ID : " + Id + "\nPatient Name: " + PatientName + "\nPatient Age: " + PatientAge + "\nAppointment Date: " + AppointmentDate + "\nReason: " + Reason;
    }
}