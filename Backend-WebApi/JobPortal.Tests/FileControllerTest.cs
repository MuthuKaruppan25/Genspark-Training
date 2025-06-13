using Xunit;
using Moq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using JobPortal.Controllers;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models.DTOs;

public class FileControllerTests
{
    private readonly Mock<IFileService> _fileServiceMock = new();
    private readonly FileController _controller;

    public FileControllerTests()
    {
        _controller = new FileController(_fileServiceMock.Object);
    }

    [Fact]
    public async Task DownloadFile_ReturnsFile_WhenSuccess()
    {
        var requestDto = new FileGetRequestDto();
        var fileBytes = new byte[] { 1, 2, 3 };

        _fileServiceMock.Setup(s => s.DownloadFileAsync(requestDto)).ReturnsAsync(fileBytes);

        var result = await _controller.DownloadFile(requestDto);

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal(fileBytes, fileResult.FileContents);
        Assert.Equal("application/octet-stream", fileResult.ContentType);
        Assert.Equal("resume", fileResult.FileDownloadName);
    }

    [Fact]
    public async Task DownloadFile_ReturnsBadRequest_WhenNull()
    {
        var result = await _controller.DownloadFile(null);

        var badRequest = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("File request data is required.", badRequest.Value);
    }

    [Fact]
    public async Task DownloadFile_ReturnsNotFound_WhenRecordNotFound()
    {
        var requestDto = new FileGetRequestDto();
        _fileServiceMock.Setup(s => s.DownloadFileAsync(requestDto))
            .ThrowsAsync(new RecordNotFoundException("Not found"));

        var result = await _controller.DownloadFile(requestDto);

        var notFound = Assert.IsType<NotFoundObjectResult>(result);
        var errorObj = notFound.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Not found", errorValue);
    }

    [Fact]
    public async Task DownloadFile_Returns500_WhenDownloadException()
    {
        var requestDto = new FileGetRequestDto();
        _fileServiceMock.Setup(s => s.DownloadFileAsync(requestDto))
            .ThrowsAsync(new DownloadException("Download error"));

        var result = await _controller.DownloadFile(requestDto);

        var serverError = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, serverError.StatusCode);
        var errorObj = serverError.Value;
        Assert.NotNull(errorObj);
        var errorValue = errorObj.GetType().GetProperty("error")?.GetValue(errorObj)?.ToString();
        Assert.Equal("Download error", errorValue);
    }
}