
using JobPortal.Contexts;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using JobPortal.Models.DTOs;

public class TransactionalRecruiterRegister : ITransactionalRecruiterRegister
{
    private readonly IRepository<Guid, User> _userRepository;
    private readonly IRepository<Guid, Recruiter> _recruiterRepository;
    private readonly IRepository<Guid, Company> _companyRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly JobContext _context;

    private readonly UserMapper _userMapper;
    private readonly RecruiterMapper _recruiterMapper;
    private readonly RecruiterRegisterResponseMapper _responseMapper;

    public TransactionalRecruiterRegister(
        IRepository<Guid, User> userRepository,
        IRepository<Guid, Recruiter> recruiterRepository,
        IRepository<Guid, Company> companyRepository,
        IEncryptionService encryptionService,
        JobContext context)
    {
        _userRepository = userRepository;
        _recruiterRepository = recruiterRepository;
        _companyRepository = companyRepository;
        _encryptionService = encryptionService;
        _context = context;

        _userMapper = new UserMapper();
        _recruiterMapper = new RecruiterMapper();
        _responseMapper = new RecruiterRegisterResponseMapper();
    }

    public async Task<RecruiterRegisterResponseDto> RegisterCompany(RecruiterRegisterDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var users = await _userRepository.GetAll();
            if (users.Any(u => u.Username.ToLower() == dto.Email.ToLower()))
                throw new DuplicateEntryException("User with the given email already exists.");

            if (string.IsNullOrWhiteSpace(dto.CompanyName))
                throw new FieldRequiredException("Company name is required.");

            var companies = await _companyRepository.GetAll();
            var company = companies.FirstOrDefault(c => c.CompanyName.ToLower() == dto.CompanyName.ToLower() && !c.IsDeleted);

            if (company == null)
                throw new RecordNotFoundException($"Company with name '{dto.CompanyName}' not found.");

            var encryptModel = new EncryptModel { Data = dto.Password };
            var encrypted = await _encryptionService.EncryptData(encryptModel);
            string role = "Recruiter";

            var user = _userMapper.MapUser(dto.Email, encrypted, role);
            await _userRepository.Add(user!);

            var recruiter = _recruiterMapper.MapRecruiter(dto, user!.guid, company.guid);
            await _recruiterRepository.Add(recruiter);

            await transaction.CommitAsync();

            return _responseMapper.MapResponse(dto, user.PasswordHash);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

 
}