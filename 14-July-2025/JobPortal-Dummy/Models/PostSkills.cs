public class PostSkills
{
    public Guid guid { get; set; } = Guid.NewGuid();
    public Guid JobPostId { get; set; }
    public Guid SkillId { get; set; }
    public JobPost JobPost { get; set; } = null!;
    public Skill Skill { get; set; } = null!;
}
