
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

namespace SecondWebApi.Misc;

public class SpecialityMapper
{
    public Speciality? MapSpecialityAddRequest(SpecialityAddDto specialityAddDto)
    {
        Speciality speciality = new();
        speciality.Name = specialityAddDto.Name;
        return speciality;
    }

    public DoctorSpeciality? MapDoctorSpecialityAddRequest(int doctorId, int specialityId)
    {
        DoctorSpeciality doctorSpeciality = new();
        doctorSpeciality.DoctorId = doctorId;
        doctorSpeciality.SpecialityId = specialityId;
        return doctorSpeciality;
    }
}