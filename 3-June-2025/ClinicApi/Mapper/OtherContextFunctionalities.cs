using SecondWebApi.Contexts;
using SecondWebApi.Interfaces;
using SecondWebApi.Models.Dtos;

public class OtherFuncinalitiesImplementation : IOtherContextFunctionities
{
    private readonly ClinicContext _clinicContext;

    public OtherFuncinalitiesImplementation(ClinicContext clinicContext)
    {
        _clinicContext = clinicContext;
    }

    public async Task<ICollection<DoctorsBySpecialityResponseDto>> GetDoctorsBySpeciality(string specilaity)
    {
        var result = await _clinicContext.DoctorsBySpeciality(specilaity);
        return result;
    }
}