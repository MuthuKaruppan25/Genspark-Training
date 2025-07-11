
using JobPortal.Models;

public class Skill
{
    public Guid guid { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public ICollection<SeekerSkills>? seekerSkills { get; set; }
    public ICollection<PostSkills>? postSkills{ get; set; }

}