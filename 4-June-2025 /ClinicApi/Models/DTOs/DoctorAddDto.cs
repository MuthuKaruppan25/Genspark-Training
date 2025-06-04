
namespace SecondWebApi.Models.Dtos;

public class DoctorAddDto
{
    public string Name { get; set; } = string.Empty;
    public ICollection<SpecialityAddDto>? specialities { get; set; }
    public float YearsOfExperience { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

}