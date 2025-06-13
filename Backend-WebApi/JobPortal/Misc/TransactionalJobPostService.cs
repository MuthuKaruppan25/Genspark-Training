using JobPortal.Contexts;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using JobPortal.Models.DTOs;

public class TransactionalJobPostService : ITransactionalJobPostService
{
    private readonly IRepository<Guid, JobPost> _jobPostRepository;
    private readonly IRepository<Guid, Recruiter> _recruiterRepository;
    private readonly IRepository<Guid, Requirements> _requirementRepository;
    private readonly IRepository<Guid, Responsibilities> _responsibilityRepository;
    private readonly IRepository<Guid, Skill> _skillRepository;
    private readonly IRepository<Guid, PostSkills> _postSkillRepository;
    private readonly JobContext _context;

    private readonly JobPostMapper _jobPostMapper;
    private readonly RequirementMapper _requirementMapper;
    private readonly ResponsibilityMapper _responsibilityMapper;
    private readonly PostSkillMapper _postSkillMapper;
    private readonly JobPostResponseMapper _responseMapper;

    public TransactionalJobPostService(
        IRepository<Guid, JobPost> jobPostRepository,
        IRepository<Guid, Recruiter> recruiterRepository,
        IRepository<Guid, Requirements> requirementRepository,
        IRepository<Guid, Responsibilities> responsibilityRepository,
        IRepository<Guid, Skill> skillRepository,
        IRepository<Guid, PostSkills> postSkillRepository,
        JobContext context)
    {
        _jobPostRepository = jobPostRepository;
        _recruiterRepository = recruiterRepository;
        _requirementRepository = requirementRepository;
        _responsibilityRepository = responsibilityRepository;
        _skillRepository = skillRepository;
        _postSkillRepository = postSkillRepository;
        _context = context;

        _jobPostMapper = new JobPostMapper();
        _requirementMapper = new RequirementMapper();
        _responsibilityMapper = new ResponsibilityMapper();
        _postSkillMapper = new PostSkillMapper();
        _responseMapper = new JobPostResponseMapper();
    }

    public async Task<JobPostRegisterResponseDto> AddJobPostAsync(JobPostDto jobPostDto)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var recruiter = await _recruiterRepository.Get(jobPostDto.RecruiterId);
            if (recruiter == null)
                throw new RecordNotFoundException("Recruiter not found");
            if(jobPostDto.LastDate < DateTime.UtcNow.AddDays(2))
                throw new FieldRequiredException("Last date must be at least 2 days from now.");

            var jobPost = _jobPostMapper.Map(jobPostDto);
            await _jobPostRepository.Add(jobPost);

            if (jobPostDto.requirements == null || jobPostDto.requirements.Count == 0)
                throw new FieldRequiredException("Requirements not provided.");

            var mappedReqs = _requirementMapper.MapList(jobPostDto.requirements, jobPost.guid);
            foreach (var req in mappedReqs)
            {
                await _requirementRepository.Add(req);
            }

            if (jobPostDto.responsibilities == null || jobPostDto.responsibilities.Count == 0)
                throw new FieldRequiredException("Responsibilities not provided.");

            var mappedResps = _responsibilityMapper.MapList(jobPostDto.responsibilities, jobPost.guid);
            foreach (var resp in mappedResps)
            {
                await _responsibilityRepository.Add(resp);
            }

            if (jobPostDto.skills == null || jobPostDto.skills.Count == 0)
                throw new FieldRequiredException("Skills not provided.");

            var allSkills = await _skillRepository.GetAll();
            var skillEntities = allSkills.Where(s => jobPostDto.skills.Any(ds => ds.Name.ToLower() == s.Name.ToLower())).ToList();

            var postSkills = _postSkillMapper.MapSeekerSkills(jobPost.guid, skillEntities);
            foreach (var skill in postSkills)
            {
                await _postSkillRepository.Add(skill);
            }

            await transaction.CommitAsync();
            return _responseMapper.Map(jobPost);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
