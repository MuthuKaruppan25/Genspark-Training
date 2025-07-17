using JobPortal.Models;

public class PostSkillMapper
{
    public List<PostSkills> MapSeekerSkills( Guid PostId,List<Skill> matchedSkills)
    {
        return matchedSkills.Select(skill => new PostSkills
        {
            guid = Guid.NewGuid(),
            SkillId = skill.guid,
            JobPostId = PostId,
            
        }).ToList();
    }
}
