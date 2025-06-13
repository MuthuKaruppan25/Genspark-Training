using System.ComponentModel.DataAnnotations;

public class SeekerUpdateDto
{
    [TextValidator]
    public string FirstName { get; set; } = string.Empty;
    [TextValidator]
    public string LastName { get; set; } = string.Empty;
    [TextValidator]
    public string Education { get; set; } = string.Empty;
    [Required]
    public int Experience { get; set; }
}
