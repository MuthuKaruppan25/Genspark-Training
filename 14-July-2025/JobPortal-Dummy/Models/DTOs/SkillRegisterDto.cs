

public class SkillRegisterDto
{
    [TextValidator]
    public string Name { get; set; } = string.Empty;
}

public class SkillsUpdateDto
{
    public List<SkillRegisterDto>? UpdatedSkills { get; set; }
}

