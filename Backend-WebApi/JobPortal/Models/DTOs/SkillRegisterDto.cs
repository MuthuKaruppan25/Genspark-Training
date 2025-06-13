

public class SkillRegisterDto
{
    [TextValidator]
    public string Name { get; set; } = string.Empty;
}