using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

namespace SecondWebApi.Misc;

public class DoctorMapper
{
    public Doctor? MapDoctorAddRequest(DoctorAddDto doctorAddDto)
    {
        Doctor doctor = new();
        doctor.Name = doctorAddDto.Name;
        doctor.YearsOfExperience = doctorAddDto.YearsOfExperience;
        return doctor;
    }
}