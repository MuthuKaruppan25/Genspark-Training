using Microsoft.EntityFrameworkCore;
using SecondWebApi.Models;
namespace SecondWebApi.Contexts;

public class ClinicContext : DbContext
{
    public ClinicContext(DbContextOptions<ClinicContext> options) : base(options)
    {

    }

    public DbSet<Patient> patients { get; set; }
    public DbSet<Speciality> specialities
    {
        get; set;
    }
    public DbSet<Appointmnet> appointmnets { get; set; }
    public DbSet<Doctor> doctors { get; set; }
    public DbSet<DoctorSpeciality> doctorSpecialities { get; set; }

    
 }

