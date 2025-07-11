

public class RequirementsAddDto
{
    [RequirementsValidator]
    public string Name { get; set; } = string.Empty;
}
public class RequirementsUpdateDto
{
    public List<RequirementsAddDto>? UpdatedRequirements { get; set; }
}

