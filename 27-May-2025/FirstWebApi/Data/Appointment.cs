
public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int DoctorId { get; set; }

    public DateTime AppointmentDate { get; set; }
    public string Description { get; set; } = string.Empty; 
    public string Status { get; set; } = "Scheduled"; 
}