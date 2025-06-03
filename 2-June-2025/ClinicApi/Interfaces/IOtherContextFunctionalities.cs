using SecondWebApi.Models.Dtos;

namespace SecondWebApi.Interfaces;
public interface IOtherContextFunctionities
{
    public Task<ICollection<DoctorsBySpecialityResponseDto>> GetDoctorsBySpeciality(string specilaity);
}