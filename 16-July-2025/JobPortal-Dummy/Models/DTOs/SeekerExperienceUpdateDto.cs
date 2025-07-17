public class SeekerExperienceUpdateDto
{
 public ICollection<ExperienceDetails>? experienceDetails{ get; set; }
}

public class ExperienceDetails
{
    public string CompanyName { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public int FromMonth { get; set; }  // e.g., 1 to 12
    public int FromYear { get; set; }   // e.g., 2020
    public int? ToMonth { get; set; }   // nullable for current job
    public int? ToYear { get; set; }
}