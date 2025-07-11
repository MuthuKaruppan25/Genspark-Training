

public class ResponsibilitiesAddDto
{
    [RequirementsValidator]
    public string Name { get; set; } = string.Empty;
}

public class ResponsibilitiesUpdateDto
{
    public List<ResponsibilitiesAddDto>? UpdatedResponsibilities { get; set; }
}

