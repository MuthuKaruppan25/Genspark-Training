using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models.DTOs;

public class SeekerRegisterDto
{
    [EmailValidation]
    public string Email { get; set; } = string.Empty;
    [PasswordValidation]
    public string Password { get; set; } = string.Empty;
    [TextValidator]
    public string FirstName { get; set; } = string.Empty;
    [TextValidator]
    public string LastName { get; set; } = string.Empty;

    // public string About { get; set; } = string.Empty;

    public ICollection<SkillRegisterDto>? skills { get; set; }

}