using Xunit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using JobPortal.Contexts;
using JobPortal.Models;
using JobPortal.Models.DTOs;
using JobPortal.Exceptions;
using JobPortal.Interfaces;

public class TransactionalRecruiterRegisterTests : IDisposable
{
    private readonly JobContext _context;
    private readonly TransactionalRecruiterRegister _service;
    private readonly Mock<IRepository<Guid, User>> _userRepoMock = new();
    private readonly Mock<IRepository<Guid, Recruiter>> _recruiterRepoMock = new();
    private readonly Mock<IRepository<Guid, Company>> _companyRepoMock = new();
    private readonly Mock<IEncryptionService> _encryptionMock = new();

    public TransactionalRecruiterRegisterTests()
    {
        var options = new DbContextOptionsBuilder<JobContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning))
            .Options;
        _context = new JobContext(options);

        _service = new TransactionalRecruiterRegister(
            _userRepoMock.Object,
            _recruiterRepoMock.Object,
            _companyRepoMock.Object,
            _encryptionMock.Object,
            _context
        );
    }

    [Fact]
    public async Task RegisterCompany_Success()
    {
        var company = new Company { guid = Guid.NewGuid(), CompanyName = "TestCo" };
        var user = new User { guid = Guid.NewGuid(), Username = "rec@mail.com" };
        var recruiter = new Recruiter { guid = Guid.NewGuid(), UserId = user.guid, CompanyId = company.guid };

        var dto = new RecruiterRegisterDto
        {
            Email = "rec@mail.com",
            Password = "password",
            CompanyName = "TestCo"
        };

        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User>());
        _companyRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Company> { company });
        _userRepoMock.Setup(x => x.Add(It.IsAny<User>())).ReturnsAsync(user);
        _recruiterRepoMock.Setup(x => x.Add(It.IsAny<Recruiter>())).ReturnsAsync(recruiter);
        _encryptionMock.Setup(x => x.EncryptData(It.IsAny<EncryptModel>()))
            .ReturnsAsync(new EncryptModel { Data = "encrypted" });

        var result = await _service.RegisterCompany(dto);

        Assert.NotNull(result);
     
        Assert.Equal(dto.CompanyName, result.CompanyName);
    }

    [Fact]
    public async Task RegisterCompany_Throws_DuplicateEntryException()
    {
        var dto = new RecruiterRegisterDto
        {
            Email = "rec@mail.com",
            Password = "password",
            CompanyName = "TestCo"
        };

        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User> { new User { Username = "rec@mail.com" } });

        await Assert.ThrowsAsync<DuplicateEntryException>(() => _service.RegisterCompany(dto));
    }

    [Fact]
    public async Task RegisterCompany_Throws_FieldRequiredException_WhenCompanyNameMissing()
    {
        var dto = new RecruiterRegisterDto
        {
            Email = "rec@mail.com",
            Password = "password",
            CompanyName = ""
        };

        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User>());

        await Assert.ThrowsAsync<FieldRequiredException>(() => _service.RegisterCompany(dto));
    }

    [Fact]
    public async Task RegisterCompany_Throws_RecordNotFoundException_WhenCompanyNotFound()
    {
        var dto = new RecruiterRegisterDto
        {
            Email = "rec@mail.com",
            Password = "password",
            CompanyName = "NotExistCo"
        };

        _userRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<User>());
        _companyRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Company>());

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.RegisterCompany(dto));
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}