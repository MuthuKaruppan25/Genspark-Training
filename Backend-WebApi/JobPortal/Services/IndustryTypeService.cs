using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using log4net;

public class IndustryTypeService : IIndustryTypeService
{
    private readonly IRepository<Guid, IndustryType> _repository;
    private static readonly ILog _errorLogger = LogManager.GetLogger("ErrorFileAppender");
    private static readonly ILog _dataLogger = LogManager.GetLogger("DataChangeFileAppender");

    public IndustryTypeService(IRepository<Guid, IndustryType> repository)
    {
        _repository = repository;
    }

    public async Task<IndustryType> AddIndustryType(IndustryTypeRegister industryTypeRegister)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(industryTypeRegister.Name))
                throw new FieldRequiredException("Skill name is required.");

            var existingSkills = await _repository.GetAll();
            if (existingSkills.Any(s => s.Name.ToLower() == industryTypeRegister.Name.ToLower()))
                throw new DuplicateEntryException("Skill already exists.");

            var skill = new IndustryType { guid = Guid.NewGuid(), Name = industryTypeRegister.Name };
            await _repository.Add(skill);

            _dataLogger.Info($"Industry type added: Name = {skill.Name}, Id = {skill.guid}");
            return skill;
        }
        catch (FieldRequiredException ex)
        {
            _errorLogger.Error("Field required exception during industry type registration", ex);
            throw;
        }
        catch (DuplicateEntryException ex)
        {
            _errorLogger.Error("Duplicate industry type registration attempt", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Industry type registration failed due to an unexpected error", ex);
            throw new RegistrationException("Skills cannot be registered", ex);
        }
    }
    public async Task UpdateIndustryType(Guid industryTypeId, IndustryTypeRegister industryTypeRegister)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(industryTypeRegister.Name))
                throw new FieldRequiredException("Industry type name is required.");

            var existingIndustryType = await _repository.Get(industryTypeId);
            if (existingIndustryType == null)
                throw new RecordNotFoundException("Industry type not found.");

            existingIndustryType.Name = industryTypeRegister.Name;
            await _repository.Update(industryTypeId, existingIndustryType);

            _dataLogger.Info($"Industry type updated: Id = {industryTypeId}, Name = {industryTypeRegister.Name}");
        }
        catch (FieldRequiredException ex)
        {
            _errorLogger.Error("Field required exception during industry type update", ex);
            throw;
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Error("Record not found during industry type update", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Unexpected error during industry type update", ex);
            throw;
        }
    }
}
