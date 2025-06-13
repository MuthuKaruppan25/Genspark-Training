using Xunit;
using Moq;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JobPortal.Controllers;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;

public class IndustryTypeControllerTests
{
    private readonly Mock<IIndustryTypeService> _industryTypeServiceMock = new();
    private readonly IndustryTypeController _controller;

    public IndustryTypeControllerTests()
    {
        _controller = new IndustryTypeController(_industryTypeServiceMock.Object);
    }

    [Fact]
    public async Task AddIndustryType_ReturnsOk_WhenSuccess()
    {
        var dto = new IndustryTypeRegister();
        var response = new IndustryType();

        _industryTypeServiceMock.Setup(s => s.AddIndustryType(dto)).ReturnsAsync(response);

        var result = await _controller.AddIndustryType(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(response, okResult.Value);
    }

    [Fact]
    public async Task AddIndustryType_ReturnsBadRequest_WhenNull()
    {
        var result = await _controller.AddIndustryType(null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Industry type data is required.", badRequest.Value);
    }

    [Fact]
    public async Task AddIndustryType_ReturnsBadRequest_WhenFieldRequired()
    {
        var dto = new IndustryTypeRegister();
        _industryTypeServiceMock.Setup(s => s.AddIndustryType(dto))
            .ThrowsAsync(new FieldRequiredException("Field required"));

        var result = await _controller.AddIndustryType(dto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var errorObj = badRequest.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Field required", errorValue);
    }

    [Fact]
    public async Task AddIndustryType_ReturnsConflict_WhenDuplicateEntry()
    {
        var dto = new IndustryTypeRegister();
        _industryTypeServiceMock.Setup(s => s.AddIndustryType(dto))
            .ThrowsAsync(new DuplicateEntryException("Duplicate"));

        var result = await _controller.AddIndustryType(dto);

        var conflict = Assert.IsType<ConflictObjectResult>(result);
        var errorObj = conflict.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Duplicate", errorValue);
    }

    [Fact]
    public async Task AddIndustryType_Returns500_WhenRegistrationException()
    {
        var dto = new IndustryTypeRegister();
        _industryTypeServiceMock.Setup(s => s.AddIndustryType(dto))
            .ThrowsAsync(new RegistrationException("Reg error"));

        var result = await _controller.AddIndustryType(dto);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Reg error", errorValue);
    }

    [Fact]
    public async Task UpdateIndustryType_ReturnsNoContent_WhenSuccess()
    {
        var id = Guid.NewGuid();
        var dto = new IndustryTypeRegister();

        _industryTypeServiceMock.Setup(s => s.UpdateIndustryType(id, dto)).Returns(Task.CompletedTask);

        var result = await _controller.UpdateIndustryType(id, dto);

        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateIndustryType_ReturnsBadRequest_WhenNull()
    {
        var id = Guid.NewGuid();

        var result = await _controller.UpdateIndustryType(id, null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("Industry type data is required.", badRequest.Value);
    }

    [Fact]
    public async Task UpdateIndustryType_ReturnsBadRequest_WhenFieldRequired()
    {
        var id = Guid.NewGuid();
        var dto = new IndustryTypeRegister();
        _industryTypeServiceMock.Setup(s => s.UpdateIndustryType(id, dto))
            .ThrowsAsync(new FieldRequiredException("Field required"));

        var result = await _controller.UpdateIndustryType(id, dto);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        var errorObj = badRequest.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Field required", errorValue);
    }

    [Fact]
    public async Task UpdateIndustryType_ReturnsNotFound_WhenRecordNotFound()
    {
        var id = Guid.NewGuid();
        var dto = new IndustryTypeRegister();
        _industryTypeServiceMock.Setup(s => s.UpdateIndustryType(id, dto))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        var result = await _controller.UpdateIndustryType(id, dto);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task UpdateIndustryType_Returns500_WhenRegistrationException()
    {
        var id = Guid.NewGuid();
        var dto = new IndustryTypeRegister();
        _industryTypeServiceMock.Setup(s => s.UpdateIndustryType(id, dto))
            .ThrowsAsync(new RegistrationException("Reg error"));

        var result = await _controller.UpdateIndustryType(id, dto);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Reg error", errorValue);
    }
}