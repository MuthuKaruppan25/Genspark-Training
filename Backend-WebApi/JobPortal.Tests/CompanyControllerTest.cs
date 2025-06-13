using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JobPortal.Controllers;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models.DTOs;
using JobPortal.Models;

public class CompanyControllerTests
{
    private readonly Mock<ICompanyService> _companyServiceMock = new();
    private readonly CompanyController _controller;

    public CompanyControllerTests()
    {
        _controller = new CompanyController(_companyServiceMock.Object);
    }

    [Fact]
    public async Task RegisterCompany_ReturnsOk_WhenSuccess()
    {
        var dto = new CompanyRegisterDto();
        var response = new CompanyRegisterResponseDto();

        _companyServiceMock.Setup(s => s.RegisterCompany(dto)).ReturnsAsync(response);

        var result = await _controller.RegisterCompany(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task RegisterCompany_ReturnsBadRequest_WhenNull()
    {
        var result = await _controller.RegisterCompany(null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Company registration data is required.", badRequest.Value);
    }

    [Fact]
    public async Task RegisterCompany_ReturnsBadRequest_WhenFieldRequired()
    {
        var dto = new CompanyRegisterDto();
        _companyServiceMock.Setup(s => s.RegisterCompany(dto))
            .ThrowsAsync(new FieldRequiredException("Field required"));

        var result = await _controller.RegisterCompany(dto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var errorObj = badRequest.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Field required", errorValue);
    }

    [Fact]
    public async Task RegisterCompany_ReturnsConflict_WhenDuplicateEntry()
    {
        var dto = new CompanyRegisterDto();
        _companyServiceMock.Setup(s => s.RegisterCompany(dto))
            .ThrowsAsync(new DuplicateEntryException("Duplicate"));

        var result = await _controller.RegisterCompany(dto);

        var conflict = Assert.IsType<ConflictObjectResult>(result);
        var errorObj = conflict.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Duplicate", errorValue);
    }

    [Fact]
    public async Task RegisterCompany_ReturnsNotFound_WhenRecordNotFound()
    {
        var dto = new CompanyRegisterDto();
        _companyServiceMock.Setup(s => s.RegisterCompany(dto))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        var result = await _controller.RegisterCompany(dto);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task RegisterCompany_Returns500_WhenRegistrationException()
    {
        var dto = new CompanyRegisterDto();
        _companyServiceMock.Setup(s => s.RegisterCompany(dto))
            .ThrowsAsync(new RegistrationException("Reg error"));

        var result = await _controller.RegisterCompany(dto);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Reg error", errorValue);
    }

    [Fact]
    public async Task GetRecruitersInCompany_ReturnsOk_WhenSuccess()
    {
        var companyId = Guid.NewGuid();
        var recruiters = new List<Recruiter>();
        _companyServiceMock.Setup(s => s.GetRecruitersInCompany(companyId)).ReturnsAsync(recruiters);

        var result = await _controller.GetRecruitersInCompany(companyId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(recruiters, okResult.Value);
    }

    [Fact]
    public async Task GetRecruitersInCompany_ReturnsNotFound_WhenRecordNotFound()
    {
        var companyId = Guid.NewGuid();
        _companyServiceMock.Setup(s => s.GetRecruitersInCompany(companyId))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        var result = await _controller.GetRecruitersInCompany(companyId);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task GetRecruitersInCompany_Returns500_WhenFetchDataException()
    {
        var companyId = Guid.NewGuid();
        _companyServiceMock.Setup(s => s.GetRecruitersInCompany(companyId))
            .ThrowsAsync(new FetchDataException("Fetch error"));

        var result = await _controller.GetRecruitersInCompany(companyId);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Fetch error", errorValue);
    }

    [Fact]
    public async Task GetCompanyLocations_ReturnsOk_WhenSuccess()
    {
        var companyId = Guid.NewGuid();
        var locations = new List<Address>();
        _companyServiceMock.Setup(s => s.GetCompanyLocations(companyId)).ReturnsAsync(locations);

        var result = await _controller.GetCompanyLocations(companyId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(locations, okResult.Value);
    }

    [Fact]
    public async Task GetCompanyLocations_Returns500_WhenFetchDataException()
    {
        var companyId = Guid.NewGuid();
        _companyServiceMock.Setup(s => s.GetCompanyLocations(companyId))
            .ThrowsAsync(new FetchDataException("Fetch error"));

        var result = await _controller.GetCompanyLocations(companyId);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Fetch error", errorValue);
    }

    [Fact]
    public async Task UpdateCompanyDetails_ReturnsOk_WhenSuccess()
    {
        var companyId = Guid.NewGuid();
        var updateDto = new CompanyUpdateDto();

        _companyServiceMock.Setup(s => s.UpdateCompanyDetails(companyId, updateDto)).Returns(Task.CompletedTask);

        var result = await _controller.UpdateCompanyDetails(companyId, updateDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var msgObj = okResult.Value;
        Assert.NotNull(msgObj);
        var msgValue = msgObj.GetType().GetProperty("message")?.GetValue(msgObj)?.ToString();
        Assert.Equal("Company details updated successfully.", msgValue);
    }

    [Fact]
    public async Task UpdateCompanyDetails_ReturnsBadRequest_WhenNull()
    {
        var companyId = Guid.NewGuid();

        var result = await _controller.UpdateCompanyDetails(companyId, null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Update data is required.", badRequest.Value);
    }

    [Fact]
    public async Task UpdateCompanyDetails_ReturnsNotFound_WhenRecordNotFound()
    {
        var companyId = Guid.NewGuid();
        var updateDto = new CompanyUpdateDto();
        _companyServiceMock.Setup(s => s.UpdateCompanyDetails(companyId, updateDto))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        var result = await _controller.UpdateCompanyDetails(companyId, updateDto);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task UpdateCompanyDetails_Returns500_WhenUpdateException()
    {
        var companyId = Guid.NewGuid();
        var updateDto = new CompanyUpdateDto();
        _companyServiceMock.Setup(s => s.UpdateCompanyDetails(companyId, updateDto))
            .ThrowsAsync(new UpdateException("Update error"));

        var result = await _controller.UpdateCompanyDetails(companyId, updateDto);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Update error", errorValue);
    }

    [Fact]
    public async Task UpdateCompanyLocations_ReturnsOk_WhenSuccess()
    {
        var companyId = Guid.NewGuid();
        var locations = new List<AddressRegisterDto> { new AddressRegisterDto() };

        _companyServiceMock.Setup(s => s.UpdateCompanyLocations(companyId, locations)).Returns(Task.CompletedTask);

        var result = await _controller.UpdateCompanyLocations(companyId, locations);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var msgObj = okResult.Value;
        Assert.NotNull(msgObj);
        var msgValue = msgObj.GetType().GetProperty("message")?.GetValue(msgObj)?.ToString();
        Assert.Equal("Company locations updated successfully.", msgValue);
    }

    [Fact]
    public async Task UpdateCompanyLocations_ReturnsBadRequest_WhenNullOrEmpty()
    {
        var companyId = Guid.NewGuid();

        var result = await _controller.UpdateCompanyLocations(companyId, null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("At least one location is required.", badRequest.Value);

        var result2 = await _controller.UpdateCompanyLocations(companyId, new List<AddressRegisterDto>());

        var badRequest2 = Assert.IsType<BadRequestObjectResult>(result2);
        Assert.Equal("At least one location is required.", badRequest2.Value);
    }

    [Fact]
    public async Task UpdateCompanyLocations_ReturnsNotFound_WhenRecordNotFound()
    {
        var companyId = Guid.NewGuid();
        var locations = new List<AddressRegisterDto> { new AddressRegisterDto() };
        _companyServiceMock.Setup(s => s.UpdateCompanyLocations(companyId, locations))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        var result = await _controller.UpdateCompanyLocations(companyId, locations);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task UpdateCompanyLocations_Returns500_WhenUpdateException()
    {
        var companyId = Guid.NewGuid();
        var locations = new List<AddressRegisterDto> { new AddressRegisterDto() };
        _companyServiceMock.Setup(s => s.UpdateCompanyLocations(companyId, locations))
            .ThrowsAsync(new UpdateException("Update error"));

        var result = await _controller.UpdateCompanyLocations(companyId, locations);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Update error", errorValue);
    }

   

    [Fact]
    public async Task SoftDeleteCompany_ReturnsNotFound_WhenRecordNotFound()
    {
        var companyId = Guid.NewGuid();
        _companyServiceMock.Setup(s => s.SoftDeleteCompany(companyId))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        var result = await _controller.SoftDeleteCompany(companyId);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task SoftDeleteCompany_Returns500_WhenUpdateException()
    {
        var companyId = Guid.NewGuid();
        _companyServiceMock.Setup(s => s.SoftDeleteCompany(companyId))
            .ThrowsAsync(new UpdateException("Update error"));

        var result = await _controller.SoftDeleteCompany(companyId);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Update error", errorValue);
    }
}