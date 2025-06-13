using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;

public class IndustryTypeServiceTests
{
    private readonly Mock<IRepository<Guid, IndustryType>> _repoMock = new();
    private readonly IndustryTypeService _service;

    public IndustryTypeServiceTests()
    {
        _service = new IndustryTypeService(_repoMock.Object);
    }

    [Fact]
    public async Task AddIndustryType_Success()
    {
        var dto = new IndustryTypeRegister { Name = "IT" };
        _repoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<IndustryType>());
        _repoMock.Setup(x => x.Add(It.IsAny<IndustryType>()))
            .ReturnsAsync((IndustryType s) => s);

        var result = await _service.AddIndustryType(dto);

        Assert.Equal("IT", result.Name);
        Assert.NotEqual(Guid.Empty, result.guid);
    }

    [Fact]
    public async Task AddIndustryType_ThrowsFieldRequiredException_WhenNameIsEmpty()
    {
        var dto = new IndustryTypeRegister { Name = "" };

        await Assert.ThrowsAsync<FieldRequiredException>(() => _service.AddIndustryType(dto));
    }

    [Fact]
    public async Task AddIndustryType_ThrowsDuplicateEntryException_WhenNameExists()
    {
        var dto = new IndustryTypeRegister { Name = "IT" };
        var existing = new List<IndustryType> { new IndustryType { Name = "IT" } };
        _repoMock.Setup(x => x.GetAll()).ReturnsAsync(existing);

        await Assert.ThrowsAsync<DuplicateEntryException>(() => _service.AddIndustryType(dto));
    }

    [Fact]
    public async Task AddIndustryType_ThrowsRegistrationException_OnGeneralError()
    {
        var dto = new IndustryTypeRegister { Name = "IT" };
        _repoMock.Setup(x => x.GetAll()).ThrowsAsync(new Exception("db error"));

        await Assert.ThrowsAsync<RegistrationException>(() => _service.AddIndustryType(dto));
    }
}