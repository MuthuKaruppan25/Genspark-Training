
namespace JobPortal.Models.DTOs;

public class RecruiterRegisterDto
{
    [EmailValidation]
    public string Email { get; set; } = string.Empty;
    [PasswordValidation]
    public string Password { get; set; } = string.Empty;
    [TextValidator]
    public string FirstName { get; set; } = string.Empty;
    [TextValidator]
    public string LastName { get; set; } = string.Empty;
    [PhoneValidation]
    public string PhoneNumber { get; set; } = string.Empty;
    [TextValidator]
    public string Designation { get; set; } = string.Empty;
    [TextValidator]
    public string CompanyName { get; set; } = string.Empty;
    
}