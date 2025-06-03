


using SecondWebApi.Contexts;
using SecondWebApi.Interfaces;
using SecondWebApi.Misc;
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

public class DoctorServiceTransaction : IDoctorService
{
    private readonly ClinicContext _clinicContext;
    DoctorMapper doctorMapper;
    SpecialityMapper specialityMapper;

    public DoctorServiceTransaction(ClinicContext clinicContext)
    {
        doctorMapper = new DoctorMapper();
        specialityMapper = new SpecialityMapper();
        _clinicContext = clinicContext;
    }

    public Task<Doctor> AddDoctor(DoctorAddDto doctorAddDto)
    {
        throw new NotImplementedException();
    }

    // public async Task<Doctor> AddDoctor(DoctorAddDto doctorAddDto)
    // {
    //     try
    //     {

    //         var transaction  = await _clinicContext.
    //     }
    //     catch (Exception ex)
    //     {

    //     }
    // }

    public Task<Doctor> GetDoctorByName(string name)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<DoctorsBySpecialityResponseDto>> GetDoctorsBySpeciality(string speciality)
    {
        throw new NotImplementedException();
    }
}
