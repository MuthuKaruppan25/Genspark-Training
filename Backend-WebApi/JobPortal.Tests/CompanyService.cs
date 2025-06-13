using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using JobPortal.Models.DTOs;

public class CompanyServiceTests
{
    private readonly Mock<IRepository<Guid, Company>> _companyRepoMock = new();
    private readonly Mock<IRepository<Guid, IndustryType>> _industryRepoMock = new();
    private readonly Mock<IRepository<Guid, Address>> _addressRepoMock = new();
     private readonly Mock<IRepository<Guid, Recruiter>> _recruiterRepoMock = new();
    private readonly CompanyService _service;

    public CompanyServiceTests()
    {
        _service = new CompanyService(
            _companyRepoMock.Object,
            _industryRepoMock.Object,
            _addressRepoMock.Object,
            _recruiterRepoMock.Object
        );
    }

    [Fact]
    public async Task RegisterCompany_Success()
    {
        var industryType = new IndustryType { guid = Guid.NewGuid(), Name = "IT" };
        var companyGuid = Guid.NewGuid();
        var companyDto = new CompanyRegisterDto
        {
            CompanyName = "TestCo",
            Description = "A test company",
            WebsiteUrl = "https://testco.com",
            industryTypeRegister = new IndustryTypeRegister { Name = "IT" },
            locations = new List<AddressRegisterDto>
            {
                new AddressRegisterDto
                {
                    AddressLine1 = "123 Main St",
                    AddressLine2 = "Suite 100",
                    City = "Metropolis",
                    State = "NY",
                    PostalCode = "12345",
                    Country = "USA",
                    AddressType = "Headquarters"
                }
            }
        };

        _companyRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Company>());
        _industryRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<IndustryType> { industryType });
        _companyRepoMock.Setup(x => x.Add(It.IsAny<Company>())).ReturnsAsync((Company c) => { c.guid = companyGuid; return c; });
        _addressRepoMock.Setup(x => x.Add(It.IsAny<Address>())).ReturnsAsync((Address a) => a);

        var result = await _service.RegisterCompany(companyDto);

        Assert.Equal(companyDto.CompanyName, result.CompanyName);
        Assert.NotNull(result.industryType);
        Assert.Equal("IT", result.industryType.Name);
        Assert.NotNull(result.locations);
    }

    [Fact]
    public async Task RegisterCompany_ThrowsDuplicateEntryException()
    {
        var companyDto = new CompanyRegisterDto
        {
            CompanyName = "TestCo",
            industryTypeRegister = new IndustryTypeRegister { Name = "IT" }
        };
        var existingCompany = new Company { CompanyName = "TestCo" };
        _companyRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Company> { existingCompany });

        await Assert.ThrowsAsync<DuplicateEntryException>(() => _service.RegisterCompany(companyDto));
    }

    [Fact]
    public async Task RegisterCompany_ThrowsFieldRequiredException_WhenIndustryTypeMissing()
    {
        var companyDto = new CompanyRegisterDto
        {
            CompanyName = "TestCo",
            industryTypeRegister = null
        };
        _companyRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Company>());

        await Assert.ThrowsAsync<FieldRequiredException>(() => _service.RegisterCompany(companyDto));
    }

    [Fact]
    public async Task RegisterCompany_ThrowsFieldRequiredException_WhenIndustryTypeNameEmpty()
    {
        var companyDto = new CompanyRegisterDto
        {
            CompanyName = "TestCo",
            industryTypeRegister = new IndustryTypeRegister { Name = "" }
        };
        _companyRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Company>());

        await Assert.ThrowsAsync<FieldRequiredException>(() => _service.RegisterCompany(companyDto));
    }

    [Fact]
    public async Task RegisterCompany_ThrowsRecordNotFoundException_WhenIndustryTypeNotFound()
    {
        var companyDto = new CompanyRegisterDto
        {
            CompanyName = "TestCo",
            industryTypeRegister = new IndustryTypeRegister { Name = "IT" }
        };
        _companyRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Company>());
        _industryRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<IndustryType>());

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.RegisterCompany(companyDto));
    }

    [Fact]
    public async Task RegisterCompany_ThrowsRegistrationException_OnGeneralException()
    {
        var companyDto = new CompanyRegisterDto
        {
            CompanyName = "TestCo",
            industryTypeRegister = new IndustryTypeRegister { Name = "IT" }
        };
        _companyRepoMock.Setup(x => x.GetAll()).ThrowsAsync(new Exception("db error"));

        await Assert.ThrowsAsync<RegistrationException>(() => _service.RegisterCompany(companyDto));
    }

    [Fact]
    public async Task GetRecruitersInCompany_ReturnsRecruiters()
    {
        var companyId = Guid.NewGuid();
        var recruiters = new List<Recruiter>
        {
            new Recruiter { guid = Guid.NewGuid(), IsDeleted = false },
            new Recruiter { guid = Guid.NewGuid(), IsDeleted = false }
        };
        var company = new Company { guid = companyId, recruiters = recruiters };

        _companyRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<Company> { company });

        var result = await _service.GetRecruitersInCompany(companyId);

        Assert.Equal(2, result.Count());
    }


    [Fact]
    public async Task GetCompanyLocations_ReturnsLocations()
    {
        var companyId = Guid.NewGuid();
        var addresses = new List<Address>
        {
            new Address { guid = Guid.NewGuid(), companyId = companyId },
            new Address { guid = Guid.NewGuid(), companyId = companyId }
        };
        _addressRepoMock.Setup(x => x.GetAll()).ReturnsAsync(addresses);

        var result = await _service.GetCompanyLocations(companyId);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task UpdateCompanyDetails_Success()
    {
        var companyId = Guid.NewGuid();
        var company = new Company { guid = companyId, CompanyName = "OldName" };
        var updateDto = new CompanyUpdateDto
        {
            CompanyName = "NewName",
            Description = "NewDesc",
            WebsiteUrl = "https://new.com"
        };

        _companyRepoMock.Setup(x => x.Get(companyId)).ReturnsAsync(company);
        _companyRepoMock.Setup(x => x.Update(companyId, It.IsAny<Company>())).ReturnsAsync(company);

        await _service.UpdateCompanyDetails(companyId, updateDto);

        _companyRepoMock.Verify(x => x.Update(companyId, It.Is<Company>(c =>
            c.CompanyName == "NewName" &&
            c.Description == "NewDesc" &&
            c.WebsiteUrl == "https://new.com"
        )), Times.Once);
    }

    [Fact]
    public async Task UpdateCompanyDetails_ThrowsRecordNotFoundException()
    {
        var companyId = Guid.NewGuid();
        var updateDto = new CompanyUpdateDto
        {
            CompanyName = "NewName",
            Description = "NewDesc",
            WebsiteUrl = "https://new.com"
        };

        _companyRepoMock.Setup(x => x.Get(companyId)).ReturnsAsync((Company)null);

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.UpdateCompanyDetails(companyId, updateDto));
    }

    [Fact]
    public async Task UpdateCompanyLocations_Success()
    {
        var companyId = Guid.NewGuid();
        var company = new Company { guid = companyId };
        var oldAddresses = new List<Address>
        {
            new Address { guid = Guid.NewGuid(), companyId = companyId }
        };
        var newLocations = new List<AddressRegisterDto>
        {
            new AddressRegisterDto
            {
                AddressLine1 = "New St",
                City = "New City",
                State = "NC",
                PostalCode = "99999",
                Country = "USA",
                AddressType = "Branch"
            }
        };

        _companyRepoMock.Setup(x => x.Get(companyId)).ReturnsAsync(company);
        _addressRepoMock.Setup(x => x.GetAll()).ReturnsAsync(oldAddresses);
        _addressRepoMock.Setup(x => x.Delete(It.IsAny<Guid>())).ReturnsAsync((Address)null);
        _addressRepoMock.Setup(x => x.Add(It.IsAny<Address>())).ReturnsAsync((Address a) => a);

        await _service.UpdateCompanyLocations(companyId, newLocations);

        _addressRepoMock.Verify(x => x.Delete(It.IsAny<Guid>()), Times.Exactly(oldAddresses.Count));
        _addressRepoMock.Verify(x => x.Add(It.IsAny<Address>()), Times.Exactly(newLocations.Count));
    }

    [Fact]
    public async Task UpdateCompanyLocations_ThrowsRecordNotFoundException()
    {
        var companyId = Guid.NewGuid();
        var newLocations = new List<AddressRegisterDto>
        {
            new AddressRegisterDto
            {
                AddressLine1 = "New St",
                City = "New City",
                State = "NC",
                PostalCode = "99999",
                Country = "USA",
                AddressType = "Branch"
            }
        };

        _companyRepoMock.Setup(x => x.Get(companyId)).ReturnsAsync((Company)null);

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.UpdateCompanyLocations(companyId, newLocations));
    }
}