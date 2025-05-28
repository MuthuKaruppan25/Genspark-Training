

using System.Threading.Tasks;
using SecondWebApi.Interfaces;
using SecondWebApi.Models;
using SecondWebApi.Models.Dtos;

public class SpecialityService : ISpecialityService
{
    private readonly IRepository<int, Speciality> _repository;
    public SpecialityService(IRepository<int, Speciality> repository)
    {
        _repository = repository;
    }

    public async Task<Speciality?> AddSpeciality(SpecialityAddDto specialityAddDto)
    {
        try
        {
            var speciality = new Speciality { Name = specialityAddDto.Name, Status = "Created" };
            var spec = await _repository.Add(new Speciality { Name = specialityAddDto.Name });
            return spec;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error Adding Speciality :{ex.Message}");
            return null;
        }
    }

    public async Task<IEnumerable<Speciality>?> GetSpecialities()
    {
        try
        {
            var specialities = await _repository.GetAll();
            return specialities;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error Adding Speciality :{ex.Message}");
             return new List<Speciality>();
        }
    }

}