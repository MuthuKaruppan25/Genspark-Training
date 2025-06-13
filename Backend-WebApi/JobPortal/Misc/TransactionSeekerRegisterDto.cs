using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using JobPortal.Models.DTOs;

using Microsoft.EntityFrameworkCore;
using JobPortal.Contexts;

public class TransactionalSeekerService : ITransactionalSeekerService
{
    private readonly JobContext _context;
    private readonly IRepository<Guid, Skill> _skillRepository;
    private readonly IEncryptionService _encryptionService;
    private readonly UserMapper _userMapper;
    private readonly SeekerMapper _seekerMapper;
    private readonly SeekerSkillsMapper _skillsMapper;
    private readonly SeekerRegisterResponseMapper _responseMapper;

    public TransactionalSeekerService(
        JobContext context,
        IRepository<Guid, Skill> skillRepository,
        IEncryptionService encryptionService)
    {
        _context = context;
        _skillRepository = skillRepository;
        _encryptionService = encryptionService;
        _userMapper = new UserMapper();
        _seekerMapper = new SeekerMapper();
        _skillsMapper = new SeekerSkillsMapper();
        _responseMapper = new SeekerRegisterResponseMapper();
    }
        public async Task<SeekerRegisterResponseDto> RegisterSeekerWithTransaction(SeekerRegisterDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var userExists = await _context.users
                .AnyAsync(u => u.Username.ToLower() == dto.Email.ToLower());

            if (userExists)
                throw new DuplicateEntryException("User with the given name already exists.");

            if (dto.skills == null || dto.skills.Count == 0)
                throw new FieldRequiredException("At least one skill is required.");

            var allSkills = await _skillRepository.GetAll();
            var skillEntities = allSkills
                .Where(s => dto.skills.Any(ds => ds.Name.ToLower() == s.Name.ToLower()))
                .ToList();

            if (skillEntities.Count != dto.skills.Count)
                throw new RecordNotFoundException("One or more skills are not found in the system.");

            var encryptModel = new EncryptModel { Data = dto.Password };
            var encrypted = await _encryptionService.EncryptData(encryptModel);

            string role = "Seeker";
            var user = _userMapper.MapUser(dto.Email, encrypted, role);
            await _context.users.AddAsync(user!);
            await _context.SaveChangesAsync(); // to get user.guid

            var seeker = _seekerMapper.MapSeeker(dto, user.guid);
            await _context.seekers.AddAsync(seeker);
            await _context.SaveChangesAsync(); // to get seeker.guid

            var seekerSkills = _skillsMapper.MapSeekerSkills(seeker.guid, skillEntities);
            await _context.seekerSkills.AddRangeAsync(seekerSkills);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            return _responseMapper.MapResponse(seeker, skillEntities);
        }
        catch (RecordNotFoundException )
        {
            await transaction.RollbackAsync();
            throw;
        }
        catch (DuplicateEntryException)
        {
            await transaction.RollbackAsync();
            throw;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new RegistrationException("Seeker registration failed. Rolled back.", ex);
        }
    }
}

