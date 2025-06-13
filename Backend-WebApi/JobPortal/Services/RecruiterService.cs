using System.Security.Claims;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using JobPortal.Models.DTOs;
using log4net;

public class RecruiterService : IRecruiterService
{
    private static readonly ILog _errorLogger = LogManager.GetLogger("ErrorFileAppender");
    private static readonly ILog _dataLogger = LogManager.GetLogger("DataChangeFileAppender");

    private readonly IRepository<Guid, Recruiter> _recruiterRepository;
    private readonly IRepository<Guid, User> _userRepository;
    private readonly IRepository<Guid, Company> _companyRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly IRepository<Guid, JobPost> _jobPostRepository;
    private readonly ITransactionalRecruiterRegister _transactionalRecruiterRegister;
    private readonly JobPostResMapper _jobPostMapper;
    private readonly UserMapper _userMapper;
    private readonly RecruiterMapper _recruiterMapper;
    private readonly RecruiterRegisterResponseMapper _responseMapper;

    public RecruiterService(
        IRepository<Guid, Recruiter> recruiterRepository,
        IRepository<Guid, User> userRepository,
        IRepository<Guid, Company> companyRepository,
        ITransactionalRecruiterRegister transactionalRecruiterRegister,
        IRepository<Guid, JobPost> jobPostRepository,
        IEncryptionService encryptionService)
    {
        _recruiterRepository = recruiterRepository;
        _userRepository = userRepository;
        _companyRepository = companyRepository;
        _encryptionService = encryptionService;
        _jobPostRepository = jobPostRepository;
        _transactionalRecruiterRegister = transactionalRecruiterRegister;
        _jobPostMapper = new JobPostResMapper();
        _userMapper = new UserMapper();
        _recruiterMapper = new RecruiterMapper();
        _responseMapper = new RecruiterRegisterResponseMapper();
    }

    public async Task<RecruiterRegisterResponseDto> RegisterCompany(RecruiterRegisterDto dto)
    {
        try
        {
            var result = await _transactionalRecruiterRegister.RegisterCompany(dto);
            _dataLogger.Info($"Recruiter registered successfully: Email={dto.Email}, Company={dto.CompanyName}");
            return result;
        }
        catch (DuplicateEntryException ex)
        {
            _errorLogger.Error("Duplicate entry during registration", ex);
            throw;
        }
        catch (FieldRequiredException ex)
        {
            _errorLogger.Error("Field required error during registration", ex);
            throw;
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Error("Company or user record not found during registration", ex);
            throw;
        }
        catch (RegistrationException ex)
        {
            _errorLogger.Error("Registration failed", ex);
            throw new RegistrationException("Recruiter cannot be registered", ex);
        }
    }

    public async Task<Recruiter> GetRecruiterById(Guid recruiterId)
    {
        try
        {
            var recruiter = await _recruiterRepository.Get(recruiterId);
            if (recruiter == null || recruiter.IsDeleted)
                throw new RecordNotFoundException("Recruiter not found");

            return recruiter;
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Warn("Recruiter not found by ID", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Failed to retrieve recruiter", ex);
            throw new FetchDataException("Failed to retrieve recruiter", ex);
        }
    }

    public async Task<IEnumerable<JobPostDto>> GetRecruiterJobPosts(Guid recruiterId)
    {
        try
        {
            var allPosts = await _jobPostRepository.GetAll();
            var recruiterPosts = allPosts
                .Where(p => !p.IsDeleted && p.RecruiterID == recruiterId)
                .OrderByDescending(p => p.PostedDate)
                .Select(p => _jobPostMapper.MapToDto(p))
                .ToList();

            return recruiterPosts;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Failed to fetch job posts for recruiter", ex);
            throw new FetchDataException("Failed to fetch job posts for recruiter", ex);
        }
    }

    public async Task<Recruiter> GetRecruiterByUsername(string username)
    {
        try
        {
            var users = await _userRepository.GetAll();
            var user = users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            if (user == null)
                throw new RecordNotFoundException("User not found.");

            var recruiters = await _recruiterRepository.GetAll();
            var recruiter = recruiters.FirstOrDefault(r => r.UserId == user.guid && !r.IsDeleted);
            if (recruiter == null)
                throw new RecordNotFoundException("Recruiter not found.");

            return recruiter;
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Warn("Recruiter not found by username", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Failed to get recruiter details", ex);
            throw new FetchDataException("Failed to get recruiter details", ex);
        }
    }

    public async Task UpdateRecruiterDetails(Guid recruiterId, RecruiterUpdateDto updateDto)
    {
        try
        {
            var recruiter = await _recruiterRepository.Get(recruiterId);
            if (recruiter == null || recruiter.IsDeleted)
                throw new RecordNotFoundException("Recruiter not found");

            recruiter.FirstName = updateDto.FirstName;
            recruiter.LastName = updateDto.LastName;
            recruiter.PhoneNumber = updateDto.PhoneNumber;
            recruiter.Designation = updateDto.Designation;
            recruiter.CompanyId = updateDto.CompanyId;

            await _recruiterRepository.Update(recruiterId, recruiter);
            _dataLogger.Info($"Recruiter updated successfully: Id={recruiterId}, Name={updateDto.FirstName} {updateDto.LastName}");
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Warn("Recruiter not found for update", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Failed to update recruiter details", ex);
            throw new UpdateException("Failed to update recruiter details", ex);
        }
    }

    public async Task SoftDeleteRecruiter(Guid recruiterId, ClaimsPrincipal user)
    {
        try
        {
            var claimUsername = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var users = await _userRepository.GetAll();
            var u = users.FirstOrDefault(u => u.Username == claimUsername);

            var recruiter = await _recruiterRepository.Get(recruiterId);
            if (recruiter == null)
                throw new RecordNotFoundException("Recruiter not found");

            if (u == null || u.guid != recruiter.UserId)
                throw new UnauthorizedAccessException("You are not authorized to delete this recruiter.");

            recruiter.IsDeleted = true;
            await _recruiterRepository.Update(recruiterId, recruiter);
            _dataLogger.Info($"Recruiter soft-deleted: Id={recruiterId}");
        }
        catch (UnauthorizedAccessException ex)
        {
            _errorLogger.Error("Unauthorized soft delete attempt", ex);
            throw;
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Warn("Recruiter not found for deletion", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Failed to delete recruiter", ex);
            throw new UpdateException("Failed to delete recruiter", ex);
        }
    }
}
