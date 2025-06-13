using JobPortal.Models;

public class SeekerSkillsMapper
{
    public List<SeekerSkills> MapSeekerSkills( Guid seekerId,List<Skill> matchedSkills)
    {
        return matchedSkills.Select(skill => new SeekerSkills
        {
            guid = Guid.NewGuid(),
            SkillId = skill.guid,
            SeekerId = seekerId,
        }).ToList();
    }
}
