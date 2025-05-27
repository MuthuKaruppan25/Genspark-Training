using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace SecondWebApi.Models;
public class Appointmnet
{
    [Key]
    public string AppointmnetNumber { get; set; } = string.Empty;
    public int PatientId { get; set; }
    public int DoctorId { get; set; }
    public DateTime AppointmnetDateTime { get; set; }

    public string Status { get; set; } = string.Empty;
    [ForeignKey("DoctorId")]
    public Doctor? Doctor { get; set; }
    [ForeignKey("PatientId")]
    public Patient? Patient { get; set; }
}