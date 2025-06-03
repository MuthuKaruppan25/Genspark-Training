using Microsoft.EntityFrameworkCore;
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;
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
    public DbSet<Appointment> appointmnets { get; set; }
    public DbSet<Doctor> doctors { get; set; }
    public DbSet<DoctorSpeciality> doctorSpecialities { get; set; }

    public DbSet<DoctorsBySpecialityResponseDto> doctorsBySpeciality { get; set; }

    public DbSet<User> users { get; set; }
    public async Task<List<DoctorsBySpecialityResponseDto>> DoctorsBySpeciality(string speciality)
    {
        return await this.Set<DoctorsBySpecialityResponseDto>()
                    .FromSqlInterpolated($"select * from func_GetDoctorsBySpeciality({speciality})")
                    .ToListAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>().HasOne(p => p.user)
                            .WithOne(u => u.patient)
                            .HasForeignKey<Patient>(p => p.Email)
                            .HasConstraintName("FK_User_Patient")
                            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Doctor>().HasOne(p => p.user)
                                    .WithOne(u => u.doctor)
                                    .HasForeignKey<Doctor>(p => p.Email)
                                    .HasConstraintName("FK_User_Doctor")
                                    .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<Appointment>().HasKey(a => a.AppointmnetNumber).HasName("PK_Appointment_Number");

        modelBuilder.Entity<Appointment>().HasOne(app => app.Patient)
                                            .WithMany(p => p.Appointmnets)
                                            .HasForeignKey(ap => ap.PatientId)
                                            .HasConstraintName("FK_Appoinment_Patient")
                                            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Appointment>().HasOne(app => app.Doctor)
                                            .WithMany(d => d.Appointmnets)
                                            .HasForeignKey(ad => ad.DoctorId)
                                            .HasConstraintName("FK_Appoinment_Doctor")
                                            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DoctorSpeciality>().HasKey(ds => ds.SerialNumber);

        modelBuilder.Entity<DoctorSpeciality>().HasOne(sp => sp.Speciality)
                                                .WithMany(s => s.DoctorSpecialities)
                                                .HasForeignKey(ds => ds.SpecialityId)
                                                .HasConstraintName("FK_Speciality_Spec")
                                                .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<DoctorSpeciality>().HasOne(sp => sp.Doctor)
                                        .WithMany(s => s.DoctorSpecialities)
                                        .HasForeignKey(ds => ds.DoctorId)
                                        .HasConstraintName("FK_Speciality_Doctor")
                                        .OnDelete(DeleteBehavior.Restrict);

    }

}

