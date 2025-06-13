namespace JobPortal.Interfaces;

public interface ISkillsService
{
    Task<Skill> AddSkill(SkillRegisterDto skillRegisterDto);
    Task UpdateSkill(Guid skillId, SkillRegisterDto skillRegisterDto);
}