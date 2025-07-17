using JobPortal.Models;
using JobPortal.Models.DTOs;

public class SeekerRegisterResponseMapper
{
    public SeekerRegisterResponseDto MapResponse(Seeker seeker, IEnumerable<Skill> skills)
    {
        return new SeekerRegisterResponseDto
        {
            FirstName = seeker.FirstName,
            LastName = seeker.LastName,
            About = seeker.About,
            skills = skills.Select(s => s.Name).ToList()
        };
    }
}

public class RegisterSeekerResponseMapper
{
    public SeekerRegisterResponseDto MapResponse(Seeker seeker)
    {
        return new SeekerRegisterResponseDto
        {
            FirstName = seeker.FirstName,
            LastName = seeker.LastName,
            About = seeker.About,

        };
    }
}
