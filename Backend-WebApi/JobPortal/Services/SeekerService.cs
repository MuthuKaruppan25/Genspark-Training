using System.Security.Claims;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using JobPortal.Models.DTOs;
using log4net;

public class SeekerService : ISeekerService
{
    private readonly IRepository<Guid, User> _userRepository;
    private readonly IRepository<Guid, Skill> _skillRepository;
    private readonly IRepository<Guid, Seeker> _seekerRepository;
    private readonly IRepository<Guid, SeekerSkills> _seekerSkillsRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly ISeekerPagedGet _seekerPagedGet;
    private readonly ITransactionalSeekerService _transactionalSeekerService;
    private readonly UserMapper _userMapper;
    private readonly SeekerMapper _seekerMapper;
    private readonly SeekerSkillsMapper _skillsMapper;
    private readonly SeekerRegisterResponseMapper _responseMapper;

    private static readonly ILog _errorLogger = LogManager.GetLogger("ErrorFileAppender");
    private static readonly ILog _dataLogger = LogManager.GetLogger("DataChangeFileAppender");

    public SeekerService(
        IRepository<Guid, User> userRepository,
        IRepository<Guid, Skill> skillRepository,
        IRepository<Guid, Seeker> seekerRepository,
        IRepository<Guid, SeekerSkills> seekerSkillsRepository,
        ITransactionalSeekerService transactionalSeekerService,
        ISeekerPagedGet seekerPagedGet,
        IEncryptionService encryptionService)
    {
        _userRepository = userRepository;
        _skillRepository = skillRepository;
        _seekerRepository = seekerRepository;
        _seekerSkillsRepository = seekerSkillsRepository;
        _encryptionService = encryptionService;
        _seekerPagedGet = seekerPagedGet;
        _transactionalSeekerService = transactionalSeekerService;
        _userMapper = new UserMapper();
        _seekerMapper = new SeekerMapper();
        _skillsMapper = new SeekerSkillsMapper();
        _responseMapper = new SeekerRegisterResponseMapper();
    }

    public async Task<SeekerRegisterResponseDto> RegisterSeeker(SeekerRegisterDto dto)
    {
        try
        {
            var result = await _transactionalSeekerService.RegisterSeekerWithTransaction(dto);
            _dataLogger.Info($"Seeker registered: Email = {dto.Email}, SeekerId = {result.FirstName}");
            return result;
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Error("Seeker registration failed", ex);
            throw;
        }
        catch (DuplicateEntryException ex)
        {
            _errorLogger.Error("Seeker registration failed", ex);
            throw;
        }
        catch (FieldRequiredException ex)
        {
            _errorLogger.Error("Seeker registration failed", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Seeker registration failed", ex);
            throw new RegistrationException("Seeker cannot be registered", ex);
        }
    }

    public async Task<IEnumerable<SeekerRegisterResponseDto>> GetPagedSeekers(int pageNumber, int pageSize)
    {
        try
        {
            var seekers = await _seekerPagedGet.GetPaged(pageNumber, pageSize);
            if (seekers == null || !seekers.Any())
                throw new NoRecordsFoundException("No seekers found");

            var responses = seekers.OrderBy(s => s.FirstName).Select(s =>
            {
                var skillEntities = s.seekerSkills?.Select(ss => ss.skill!).ToList() ?? new List<Skill>();
                return _responseMapper.MapResponse(s, skillEntities);
            });

            return responses;
        }
        catch (NoRecordsFoundException ex)
        {
            _errorLogger.Error("Failed to fetch paged seekers", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Failed to fetch paged seekers", ex);
            throw new FetchDataException("Failed to fetch paged seekers", ex);
        }
    }

    public async Task<SeekerWithApplicationsDto> GetSeekerWithApplications(string username)
    {
        try
        {
            var users = await _userRepository.GetAll();
            var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null) throw new RecordNotFoundException("User not found");

            var seekers = await _seekerRepository.GetAll();
            var seeker = seekers.FirstOrDefault(s => s.UserId == user.guid);
            if (seeker == null) throw new RecordNotFoundException("Seeker not found");

            var applications = seeker.jobApplications?.Where(j => !j.IsDeleted).ToList() ?? new List<JobApplication>();

            var applicationDtos = applications
                .OrderByDescending(app => app.AppliedOn)
                .Select(app => new JobApplicationDto
                {
                    JobTitle = app.jobPost?.Title ?? "N/A",
                    JobDescription = app.jobPost?.Description ?? "N/A",
                    AppliedOn = app.AppliedOn,
                    Status = app.Status
                })
                .ToList();

            return new SeekerWithApplicationsDto
            {
                SeekerId = seeker.guid,
                FirstName = seeker.FirstName,
                LastName = seeker.LastName,
                Email = user.Username,
                Applications = applicationDtos
            };
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Error("Failed to fetch paged seekers", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Failed to fetch seeker with applications", ex);
            throw new FetchDataException("Seeker cannot be fetched", ex);
        }
    }

    public async Task<List<SkillRegisterDto>> GetSeekerSkills(string username)
    {
        try
        {
            var users = await _userRepository.GetAll();
            var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null) throw new RecordNotFoundException("User not found");

            var seekers = await _seekerRepository.GetAll();
            var seeker = seekers.FirstOrDefault(s => s.UserId == user.guid);
            if (seeker == null) throw new RecordNotFoundException("Seeker not found");

            var seekerSkillIds = seeker.seekerSkills?.Select(s => s.SkillId).ToList() ?? new List<Guid>();
            var allSkills = await _skillRepository.GetAll();

            var skillDtos = allSkills
                .Where(s => seekerSkillIds.Contains(s.guid))
                .OrderBy(s => s.Name)
                .Select(s => new SkillRegisterDto { Name = s.Name })
                .ToList();

            return skillDtos;
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Error("Failed to fetch paged seekers", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Failed to fetch seeker skills", ex);
            throw new FetchDataException("Skills cannot be fetched", ex);
        }
    }

    public async Task UpdateSeekerDetails(string username, SeekerUpdateDto dto, ClaimsPrincipal userPrincipal)
    {
        try
        {
            var claimUsername = userPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claimUsername == null || !claimUsername.Equals(username, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("You are not authorized to delete this seeker.");
            var users = await _userRepository.GetAll();
            var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null) throw new RecordNotFoundException("User not found");

            var seekers = await _seekerRepository.GetAll();
            var seeker = seekers.FirstOrDefault(s => s.UserId == user.guid);
            if (seeker == null) throw new RecordNotFoundException("Seeker not found");

            seeker.FirstName = dto.FirstName;
            seeker.LastName = dto.LastName;
            seeker.Education = dto.Education;
            seeker.Experience = dto.Experience;

            await _seekerRepository.Update(seeker.guid, seeker);
            _dataLogger.Info($"Updated seeker details for {username} (ID: {seeker.guid})");
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Error("Failed to fetch paged seekers", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Failed to update seeker details", ex);
            throw new UpdateException("Seeker details cannot be updated", ex);
        }
    }

    public async Task UpdateSeekerSkills(string username, List<SkillRegisterDto> skillNames, ClaimsPrincipal userPrincipal)
    {
        try
        {
            var claimUsername = userPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claimUsername == null || !claimUsername.Equals(username, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("You are not authorized to delete this seeker.");
            if (skillNames == null || skillNames.Count == 0)
                throw new FieldRequiredException("At least one skill is required.");

            var users = await _userRepository.GetAll();
            var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null)
                throw new RecordNotFoundException("User not found");

            var seekers = await _seekerRepository.GetAll();
            var seeker = seekers.FirstOrDefault(s => s.UserId == user.guid);
            if (seeker == null)
                throw new RecordNotFoundException("Seeker not found");

            var allSkills = await _skillRepository.GetAll();

            var validSkills = allSkills
                .Where(s => skillNames.Any(sn => sn.Name.Equals(s.Name, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            if (validSkills.Count != skillNames.Count)
                throw new RecordNotFoundException("Some provided skills are not valid.");

            var oldSkills = seeker.seekerSkills?.ToList() ?? new();
            foreach (var skill in oldSkills)
            {
                await _seekerSkillsRepository.Delete(skill.guid);
            }

            var newSeekerSkills = _skillsMapper.MapSeekerSkills(seeker.guid, validSkills);
            foreach (var skill in newSeekerSkills)
            {
                await _seekerSkillsRepository.Add(skill);
            }

            _dataLogger.Info($"Updated skills for seeker {username} (ID: {seeker.guid}) with skills: {string.Join(", ", validSkills.Select(s => s.Name))}");
        }
        catch (FieldRequiredException ex)
        {
            _errorLogger.Error("Failed to fetch paged seekers", ex);
            throw;
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Error("Failed to fetch paged seekers", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Failed to update seeker skills", ex);
            throw new UpdateException("Seeker skills cannot be updated", ex);
        }
    }

    public async Task<SeekerRegisterResponseDto> GetSeekerByUsername(string username)
    {
        try
        {

            var users = await _userRepository.GetAll();
            var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null)
                throw new RecordNotFoundException("User not found.");

            var seekers = await _seekerRepository.GetAll();
            var seeker = seekers.FirstOrDefault(s => s.UserId == user.guid && !s.IsDeleted);
            if (seeker == null)
                throw new RecordNotFoundException("Seeker not found.");

            var skills = seeker.seekerSkills?.Select(ss => ss.skill!).ToList() ?? new();
            return _responseMapper.MapResponse(seeker, skills);
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Error("Failed to fetch paged seekers", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Failed to fetch seeker by username", ex);
            throw new FetchDataException("Failed to get seeker details", ex);
        }
    }

    public async Task SoftDeleteSeekerAsync(string username, ClaimsPrincipal userPrincipal)
    {
        try
        {

            var claimUsername = userPrincipal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (claimUsername == null || !claimUsername.Equals(username, StringComparison.OrdinalIgnoreCase))
                throw new UnauthorizedAccessException("You are not authorized to delete this seeker.");

            var users = await _userRepository.GetAll();
            var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase) && u.IsDeleted!);
            if (user == null)
                throw new RecordNotFoundException("User not found.");

            var seekers = await _seekerRepository.GetAll();
            var seeker = seekers.FirstOrDefault(s => s.UserId == user.guid);
            if (seeker == null)
                throw new RecordNotFoundException("Seeker not found.");

            seeker.IsDeleted = true;
            await _seekerRepository.Update(seeker.guid, seeker);
            _dataLogger.Info($"Soft deleted seeker: {seeker.guid} ({username})");
        }
        catch (UnauthorizedAccessException ex)
        {
            _errorLogger.Error("Unauthorized soft delete attempt", ex);
            throw;
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Error("Failed to fetch paged seekers", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Failed to soft delete seeker", ex);
            throw new UpdateException("Failed to soft delete the seeker.", ex);
        }
    }
    public async Task<IEnumerable<SeekerRegisterResponseDto>> SearchSeekersByName(string name)
    {
        var seekers = await _seekerRepository.GetAll();
        var matchedSeekers = seekers
            .Where(s => !s.IsDeleted &&
                        (s.FirstName.Contains(name, StringComparison.OrdinalIgnoreCase) ||
                         s.LastName.Contains(name, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        if (!matchedSeekers.Any())
            throw new NoRecordsFoundException("No seekers found with the given name.");

        return matchedSeekers.Select(seeker =>
        {
            var skills = seeker.seekerSkills?.Select(ss => ss.skill!).ToList() ?? new();
            return _responseMapper.MapResponse(seeker, skills);
        });
    }
    public async Task<IEnumerable<SeekerRegisterResponseDto>> SearchSeekersBySkills(List<SkillRegisterDto> skillDtos)
    {
        if (skillDtos == null || !skillDtos.Any())
            throw new FieldRequiredException("At least one skill is required for search.");

        var allSkills = await _skillRepository.GetAll();
        var matchedSkills = allSkills
            .Where(s => skillDtos.Any(dto => dto.Name.Equals(s.Name, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        if (matchedSkills.Count != skillDtos.Count)
            throw new RecordNotFoundException("Some provided skills were not found in the system.");

        var skillIds = matchedSkills.Select(s => s.guid).ToHashSet();

        var allSeekers = await _seekerRepository.GetAll();
        var filteredSeekers = allSeekers
            .Where(s => !s.IsDeleted && s.seekerSkills != null &&
                        s.seekerSkills.Any(ss => skillIds.Contains(ss.SkillId)))
            .ToList();

        if (!filteredSeekers.Any())
            throw new NoRecordsFoundException("No seekers found with the given skills.");

        return filteredSeekers.Select(seeker =>
        {
            var skills = seeker.seekerSkills?.Select(ss => ss.skill!).ToList() ?? new();
            return _responseMapper.MapResponse(seeker, skills);
        });
    }
    public async Task<IEnumerable<SeekerRegisterResponseDto>> SearchSeekersByEducation(string education)
    {
        if (string.IsNullOrWhiteSpace(education))
            throw new FieldRequiredException("Education value is required.");

        var seekers = await _seekerRepository.GetAll();
        var matchedSeekers = seekers
            .Where(s => !s.IsDeleted &&
                        s.Education.Contains(education, StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (!matchedSeekers.Any())
            throw new NoRecordsFoundException("No seekers found with the given education.");

        return matchedSeekers.Select(seeker =>
        {
            var skills = seeker.seekerSkills?.Select(ss => ss.skill!).ToList() ?? new();
            return _responseMapper.MapResponse(seeker, skills);
        });
    }



}
