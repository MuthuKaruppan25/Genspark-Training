
using System.ComponentModel.DataAnnotations;

public class JobPostUpdateDto
{
    [TextValidator]
    public string Title { get; set; } = string.Empty;
  
    public string Description { get; set; } = string.Empty;
    [TextValidator]
    public string EmploymentType { get; set; } = string.Empty;
    [TextValidator]
    public string EmploymentPosition { get; set; } = string.Empty;
    

    [Required]
    public int Minsalary { get; set; } 
    [Required]
    public int  Maxsalary { get; set; } 
    [Required]
    public DateTime ClosingDate { get; set; }
    
}