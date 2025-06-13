
namespace JobPortal.Models;

public class SeekerSkills
{
    public Guid guid { get; set; } = Guid.NewGuid();
    public Guid SkillId { get; set; }
    public Guid SeekerId { get; set; }

    public Seeker? seeker { get; set; }
    public Skill? skill { get; set; }

}