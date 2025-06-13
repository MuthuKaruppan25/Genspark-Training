using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using JobPortal.Contexts;
using JobPortal.Models;
using JobPortal.Models.DTOs;
using JobPortal.Repositories;
using JobPortal.Exceptions;
using Microsoft.EntityFrameworkCore;
using JobPortal.Interfaces;
using Microsoft.EntityFrameworkCore.Diagnostics;

public class TransactionalJobPostServiceTests : IDisposable
{
    private readonly JobContext _context;
    private readonly TransactionalJobPostService _service;
    private readonly Mock<IRepository<Guid, JobPost>> _jobPostRepoMock = new();
    private readonly Mock<IRepository<Guid, Recruiter>> _recruiterRepoMock = new();
    private readonly Mock<IRepository<Guid, Requirements>> _requirementRepoMock = new();
    private readonly Mock<IRepository<Guid, Responsibilities>> _responsibilityRepoMock = new();
    private readonly Mock<IRepository<Guid, Skill>> _skillRepoMock = new();
    private readonly Mock<IRepository<Guid, PostSkills>> _postSkillRepoMock = new();

    public TransactionalJobPostServiceTests()
    {
        var options = new DbContextOptionsBuilder<JobContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        _context = new JobContext(options);

        _service = new TransactionalJobPostService(
            _jobPostRepoMock.Object,
            _recruiterRepoMock.Object,
            _requirementRepoMock.Object,
            _responsibilityRepoMock.Object,
            _skillRepoMock.Object,
            _postSkillRepoMock.Object,
            _context
        );
    }

    [Fact]
    public async Task AddJobPostAsync_Success()
    {
        var recruiterId = Guid.NewGuid();
        var jobPostDto = new JobPostDto
        {
            RecruiterId = recruiterId,
            Title = "Backend Developer",
            Description = "Develop APIs",
            requirements = new List<RequirementsAddDto> { new RequirementsAddDto { Name = "Req1" } },
            responsibilities = new List<ResponsibilitiesAddDto> { new ResponsibilitiesAddDto { Name = "Resp1" } },
            skills = new List<SkillRegisterDto> { new SkillRegisterDto { Name = "C#" } }
        };
        jobPostDto.LastDate = DateTime.UtcNow.AddDays(3);

        var recruiter = new Recruiter { guid = recruiterId };
        var jobPost = new JobPost { guid = Guid.NewGuid(), Title = "Backend Developer" };
        var requirement = new Requirements { guid = Guid.NewGuid(), Name = "Req1" };
        var responsibility = new Responsibilities { guid = Guid.NewGuid(), Name = "Resp1" };
        var skill = new Skill { guid = Guid.NewGuid(), Name = "C#" };
        var postSkill = new PostSkills { guid = Guid.NewGuid(), Skill = skill };

        _recruiterRepoMock.Setup(x => x.Get(recruiterId)).ReturnsAsync(recruiter);
        _jobPostRepoMock.Setup(x => x.Add(It.IsAny<JobPost>())).ReturnsAsync(jobPost);
        _requirementRepoMock.Setup(x => x.Add(It.IsAny<Requirements>())).ReturnsAsync(requirement);
        _responsibilityRepoMock.Setup(x => x.Add(It.IsAny<Responsibilities>())).ReturnsAsync(responsibility);
        _skillRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Skill> { skill });
        _postSkillRepoMock.Setup(x => x.Add(It.IsAny<PostSkills>())).ReturnsAsync(postSkill);

        var result = await _service.AddJobPostAsync(jobPostDto);

        Assert.NotNull(result);
        Assert.Equal(jobPost.Title, result.Title);
    }

    [Fact]
    public async Task AddJobPostAsync_Throws_RecordNotFoundException_WhenRecruiterNotFound()
    {
        var jobPostDto = new JobPostDto
        {
            RecruiterId = Guid.NewGuid(),
            Title = "Backend Developer",
            requirements = new List<RequirementsAddDto> { new RequirementsAddDto { Name = "Req1" } },
            responsibilities = new List<ResponsibilitiesAddDto> { new ResponsibilitiesAddDto { Name = "Resp1" } },
            skills = new List<SkillRegisterDto> { new SkillRegisterDto { Name = "C#" } }
        };

        _recruiterRepoMock.Setup(x => x.Get(It.IsAny<Guid>())).ReturnsAsync((Recruiter)null);

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.AddJobPostAsync(jobPostDto));
    }

    [Fact]
    public async Task AddJobPostAsync_Throws_FieldRequiredException_WhenRequirementsMissing()
    {
        var recruiterId = Guid.NewGuid();
        var jobPostDto = new JobPostDto
        {
            RecruiterId = recruiterId,
            Title = "Backend Developer",
            requirements = null,
            responsibilities = new List<ResponsibilitiesAddDto> { new ResponsibilitiesAddDto { Name = "Resp1" } },
            skills = new List<SkillRegisterDto> { new SkillRegisterDto { Name = "C#" } }
        };

        var recruiter = new Recruiter { guid = recruiterId };
        _recruiterRepoMock.Setup(x => x.Get(recruiterId)).ReturnsAsync(recruiter);

        await Assert.ThrowsAsync<FieldRequiredException>(() => _service.AddJobPostAsync(jobPostDto));
    }

    [Fact]
    public async Task AddJobPostAsync_Throws_FieldRequiredException_WhenResponsibilitiesMissing()
    {
        var recruiterId = Guid.NewGuid();
        var jobPostDto = new JobPostDto
        {
            RecruiterId = recruiterId,
            Title = "Backend Developer",
            requirements = new List<RequirementsAddDto> { new RequirementsAddDto { Name = "Req1" } },
            responsibilities = null,
            skills = new List<SkillRegisterDto> { new SkillRegisterDto { Name = "C#" } }
        };

        var recruiter = new Recruiter { guid = recruiterId };
        _recruiterRepoMock.Setup(x => x.Get(recruiterId)).ReturnsAsync(recruiter);

        await Assert.ThrowsAsync<FieldRequiredException>(() => _service.AddJobPostAsync(jobPostDto));
    }

    [Fact]
    public async Task AddJobPostAsync_Throws_FieldRequiredException_WhenSkillsMissing()
    {
        var recruiterId = Guid.NewGuid();
        var jobPostDto = new JobPostDto
        {
            RecruiterId = recruiterId,
            Title = "Backend Developer",
            requirements = new List<RequirementsAddDto> { new RequirementsAddDto { Name = "Req1" } },
            responsibilities = new List<ResponsibilitiesAddDto> { new ResponsibilitiesAddDto { Name = "Resp1" } },
            skills = null
        };

        var recruiter = new Recruiter { guid = recruiterId };
        _recruiterRepoMock.Setup(x => x.Get(recruiterId)).ReturnsAsync(recruiter);

        await Assert.ThrowsAsync<FieldRequiredException>(() => _service.AddJobPostAsync(jobPostDto));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}