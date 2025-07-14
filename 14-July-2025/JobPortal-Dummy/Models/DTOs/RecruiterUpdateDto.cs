using System.ComponentModel.DataAnnotations;

public class RecruiterUpdateDto
{
    [TextValidator]
    public string FirstName { get; set; } = string.Empty;
    [TextValidator]
    public string LastName { get; set; } = string.Empty;
    [PhoneValidation]
    public string PhoneNumber { get; set; } = string.Empty;
    [TextValidator]
    public string Designation { get; set; } = string.Empty;
    [Required]
    public Guid CompanyId { get; set; }
}