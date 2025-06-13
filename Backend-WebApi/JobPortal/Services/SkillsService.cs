using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using JobPortal.Models.DTOs;
using log4net;

public class SkillsService : ISkillsService
{
    private static readonly ILog _errorLogger = LogManager.GetLogger("ErrorFileAppender");
    private static readonly ILog _dataLogger = LogManager.GetLogger("DataChangeFileAppender");

    private readonly IRepository<Guid, Skill> _skillRepository;

    public SkillsService(IRepository<Guid, Skill> skillRepository)
    {
        _skillRepository = skillRepository;
    }

    public async Task<Skill> AddSkill(SkillRegisterDto skillRegisterDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(skillRegisterDto.Name))
                throw new FieldRequiredException("Skill name is required.");

            var existingSkills = await _skillRepository.GetAll();
            if (existingSkills.Any(s => s.Name.ToLower() == skillRegisterDto.Name.ToLower()))
                throw new DuplicateEntryException("Skill already exists.");

            var skill = new Skill { Name = skillRegisterDto.Name };
            await _skillRepository.Add(skill);

            _dataLogger.Info($"Skill added: {skillRegisterDto.Name}");
            return skill;
        }
        catch (FieldRequiredException ex)
        {
            _errorLogger.Warn("Missing skill name in AddSkill", ex);
            throw;
        }
        catch (DuplicateEntryException ex)
        {
            _errorLogger.Warn($"Duplicate skill attempted: {skillRegisterDto.Name}", ex);
            throw;
        }
        catch (RegistrationException ex)
        {
            _errorLogger.Error("Skill registration failed", ex);
            throw new RegistrationException("Skills cannot be registered", ex);
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Unexpected error occurred during AddSkill", ex);
            throw;
        }
    }
    public async Task UpdateSkill(Guid skillId, SkillRegisterDto skillRegisterDto)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(skillRegisterDto.Name))
                throw new FieldRequiredException("Skill name is required.");

            var existingSkills = await _skillRepository.GetAll();
            if (existingSkills.Any(s => s.Name.ToLower() == skillRegisterDto.Name.ToLower() && s.guid != skillId))
                throw new DuplicateEntryException("Skill already exists.");

            var skill = await _skillRepository.Get(skillId);
            if (skill == null)
                throw new RecordNotFoundException("Skill not found");

            skill.Name = skillRegisterDto.Name;
            await _skillRepository.Update(skillId, skill);

            _dataLogger.Info($"Skill updated: {skillRegisterDto.Name}");
        }
        catch (FieldRequiredException ex)
        {
            _errorLogger.Warn("Missing skill name in UpdateSkill", ex);
            throw;
        }
        catch (DuplicateEntryException ex)
        {
            _errorLogger.Warn($"Duplicate skill attempted: {skillRegisterDto.Name}", ex);
            throw;
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Warn($"Skill not found for update: {skillId}", ex);
            throw;
        }
        catch (RegistrationException ex)
        {
            _errorLogger.Error("Skill update failed", ex);
            throw new RegistrationException("Skills cannot be updated", ex);
        }
    }
}
