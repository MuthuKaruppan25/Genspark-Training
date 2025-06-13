

public class RequirementsAddDto
{
    [RequirementsValidator]
    public string Name { get; set; } = string.Empty;
}