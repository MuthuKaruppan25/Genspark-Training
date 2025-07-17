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

            UserId = userId,
            IsDeleted = false
        };
    }
}
