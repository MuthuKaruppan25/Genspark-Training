using JobPortal.Models;
using JobPortal.Models.DTOs;

public class SeekerMapper
{
    public Seeker MapSeeker(SeekerRegisterDto dto, Guid userId)
    {
        return new Seeker
        {
            guid = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Experience = dto.Experience,
            About = dto.About,
            Education = dto.Education,
            UserId = userId,
            IsDeleted = false
        };
    }
}
