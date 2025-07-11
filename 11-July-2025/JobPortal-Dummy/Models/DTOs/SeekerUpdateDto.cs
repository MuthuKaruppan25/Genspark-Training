using System;
using System.ComponentModel.DataAnnotations;

public class SeekerUpdateDto
{
    [TextValidator]
    public string FirstName { get; set; } = string.Empty;

    [TextValidator]
    public string LastName { get; set; } = string.Empty;

    [TextValidator]
    public string About { get; set; } = string.Empty;


    
    [RegularExpression("^(Male|Female|Other)$", ErrorMessage = "Gender must be Male, Female, or Other")]
    public string Gender { get; set; } = string.Empty;

    public string ConnectionId { get; set; } = string.Empty;
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }
}
