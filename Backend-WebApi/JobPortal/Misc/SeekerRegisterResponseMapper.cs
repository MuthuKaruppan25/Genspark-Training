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
            Experience = seeker.Experience,
            About = seeker.About,
            Education = seeker.Education,
            skills = skills.Select(s => s.Name).ToList()
        };
    }
}
