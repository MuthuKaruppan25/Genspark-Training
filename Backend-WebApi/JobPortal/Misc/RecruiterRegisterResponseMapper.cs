using JobPortal.Models.DTOs;

public class RecruiterRegisterResponseMapper
{
    public RecruiterRegisterResponseDto MapResponse(RecruiterRegisterDto dto, byte[] passwordHash)
    {
        return new RecruiterRegisterResponseDto
        {
            
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber,
            Designation = dto.Designation,
            Username = dto.Email,
            PasswordHash = passwordHash,
            CompanyName = dto.CompanyName
        };
    }
}
