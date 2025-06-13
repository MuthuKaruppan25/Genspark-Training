using Xunit;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using JobPortal.Models.DTOs;
using Microsoft.AspNetCore.Http;

public class FileServiceTests
{
    private readonly Mock<IRepository<Guid, FileModel>> _fileRepoMock = new();
    private readonly Mock<IRepository<Guid, Seeker>> _seekerRepoMock = new();

    private readonly FileService _service;

    public FileServiceTests()
    {
        _service = new FileService(_fileRepoMock.Object, _seekerRepoMock.Object);
    }

    [Fact]
    public async Task UploadFileAsync_Success()
    {
        var seekerId = Guid.NewGuid();
        var jobPostId = Guid.NewGuid();
        var resumeMock = new Mock<IFormFile>();
        resumeMock.Setup(x => x.Length).Returns(100);
        resumeMock.Setup(x => x.FileName).Returns("resume.pdf");
        resumeMock.Setup(x => x.ContentType).Returns("application/pdf");
        resumeMock.Setup(x => x.CopyToAsync(It.IsAny<Stream>(), default))
            .Returns<Stream, System.Threading.CancellationToken>((stream, token) =>
            {
                var bytes = new byte[100];
                return stream.WriteAsync(bytes, 0, bytes.Length, token);
            });

        var seeker = new Seeker
        {
            guid = seekerId,
            jobApplications = new List<JobApplication>
            {
                new JobApplication { JobPostId = jobPostId, IsDeleted = false }
            }
        };

        _seekerRepoMock.Setup(x => x.Get(seekerId)).ReturnsAsync(seeker);
        _fileRepoMock.Setup(x => x.Add(It.IsAny<FileModel>())).ReturnsAsync((FileModel f) => f);

        var dto = new FileUploadDto
        {
            SeekerId = seekerId,
            JobPostId = jobPostId,
            Resume = resumeMock.Object
        };

        var result = await _service.UploadFileAsync(dto);

        Assert.Equal("resume.pdf", result.FileName);
        Assert.Equal("application/pdf", result.FileType);
        Assert.Equal(100, result.FileSize);
        Assert.True((DateTime.UtcNow - result.UploadDate).TotalSeconds < 10);
    }

    [Fact]
    public async Task UploadFileAsync_ThrowsFieldRequiredException_WhenResumeMissing()
    {
        var dto = new FileUploadDto { Resume = null! };

        await Assert.ThrowsAsync<FieldRequiredException>(() => _service.UploadFileAsync(dto));
    }

    [Fact]
    public async Task UploadFileAsync_ThrowsRecordNotFoundException_WhenSeekerNotFound()
    {
        var seekerId = Guid.NewGuid();
        var resumeMock = new Mock<IFormFile>();
        resumeMock.Setup(x => x.Length).Returns(1);
        var dto = new FileUploadDto
        {
            SeekerId = seekerId,
            JobPostId = Guid.NewGuid(),
            Resume = resumeMock.Object
        };
        _seekerRepoMock.Setup(x => x.Get(seekerId)).ReturnsAsync((Seeker)null);

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.UploadFileAsync(dto));
    }

    [Fact]
    public async Task UploadFileAsync_ThrowsValidationException_WhenNotApplied()
    {
        var seekerId = Guid.NewGuid();
        var jobPostId = Guid.NewGuid();
        var seeker = new Seeker { guid = seekerId, jobApplications = new List<JobApplication>() };
        var resumeMock = new Mock<IFormFile>();
        resumeMock.Setup(x => x.Length).Returns(1);

        var dto = new FileUploadDto
        {
            SeekerId = seekerId,
            JobPostId = jobPostId,
            Resume = resumeMock.Object
        };
        _seekerRepoMock.Setup(x => x.Get(seekerId)).ReturnsAsync(seeker);

        await Assert.ThrowsAsync<ValidationException>(() => _service.UploadFileAsync(dto));
    }

    [Fact]
    public async Task UploadFileAsync_ThrowsUploadException_OnGeneralError()
    {
        var seekerId = Guid.NewGuid();
        var jobPostId = Guid.NewGuid();
        var resumeMock = new Mock<IFormFile>();
        resumeMock.Setup(x => x.Length).Returns(100);
        resumeMock.Setup(x => x.FileName).Returns("resume.pdf");
        resumeMock.Setup(x => x.ContentType).Returns("application/pdf");
        resumeMock.Setup(x => x.CopyToAsync(It.IsAny<Stream>(), default)).Throws(new Exception("copy error"));

        var seeker = new Seeker
        {
            guid = seekerId,
            jobApplications = new List<JobApplication>
            {
                new JobApplication { JobPostId = jobPostId, IsDeleted = false }
            }
        };

        _seekerRepoMock.Setup(x => x.Get(seekerId)).ReturnsAsync(seeker);

        var dto = new FileUploadDto
        {
            SeekerId = seekerId,
            JobPostId = jobPostId,
            Resume = resumeMock.Object
        };

        await Assert.ThrowsAsync<UploadException>(() => _service.UploadFileAsync(dto));
    }

    [Fact]
    public async Task DownloadFileAsync_Success()
    {
        var seekerId = Guid.NewGuid();
        var jobPostId = Guid.NewGuid();
        var fileModel = new FileModel
        {
            guid = Guid.NewGuid(),
            SeekerId = seekerId,
            JobPostId = jobPostId,
            Data = new byte[] { 1, 2, 3 },
            IsDeleted = false
        };
        _fileRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<FileModel> { fileModel });

        var dto = new FileGetRequestDto { SeekerId = seekerId, JobPostId = jobPostId };

        var result = await _service.DownloadFileAsync(dto);

        Assert.Equal(new byte[] { 1, 2, 3 }, result);
    }

    [Fact]
    public async Task DownloadFileAsync_ThrowsRecordNotFoundException_WhenFileNotFound()
    {
        _fileRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<FileModel>());

        var dto = new FileGetRequestDto { SeekerId = Guid.NewGuid(), JobPostId = Guid.NewGuid() };

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.DownloadFileAsync(dto));
    }

    [Fact]
    public async Task DownloadFileAsync_ThrowsDownloadException_OnGeneralError()
    {
        _fileRepoMock.Setup(x => x.GetAll()).ThrowsAsync(new Exception("db error"));

        var dto = new FileGetRequestDto { SeekerId = Guid.NewGuid(), JobPostId = Guid.NewGuid() };

        await Assert.ThrowsAsync<DownloadException>(() => _service.DownloadFileAsync(dto));
    }

    [Fact]
    public async Task DeleteFileAsync_Success()
    {
        var seekerId = Guid.NewGuid();
        var jobPostId = Guid.NewGuid();
        var fileModel = new FileModel
        {
            guid = Guid.NewGuid(),
            SeekerId = seekerId,
            JobPostId = jobPostId,
            IsDeleted = false
        };
        _fileRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<FileModel> { fileModel });
        _fileRepoMock.Setup(x => x.Update(fileModel.guid, It.IsAny<FileModel>())).ReturnsAsync(fileModel);

        var dto = new FileGetRequestDto { SeekerId = seekerId, JobPostId = jobPostId };

        await _service.DeleteFileAsync(dto);

        _fileRepoMock.Verify(x => x.Update(fileModel.guid, It.Is<FileModel>(f => f.IsDeleted)), Times.Once);
    }

    [Fact]
    public async Task DeleteFileAsync_ThrowsRecordNotFoundException_WhenFileNotFound()
    {
        _fileRepoMock.Setup(x => x.GetAll()).ReturnsAsync(new List<FileModel>());

        var dto = new FileGetRequestDto { SeekerId = Guid.NewGuid(), JobPostId = Guid.NewGuid() };

        await Assert.ThrowsAsync<RecordNotFoundException>(() => _service.DeleteFileAsync(dto));
    }

    [Fact]
    public async Task DeleteFileAsync_ThrowsUpdateException_OnGeneralError()
    {
        _fileRepoMock.Setup(x => x.GetAll()).ThrowsAsync(new Exception("db error"));

        var dto = new FileGetRequestDto { SeekerId = Guid.NewGuid(), JobPostId = Guid.NewGuid() };

        await Assert.ThrowsAsync<UpdateException>(() => _service.DeleteFileAsync(dto));
    }
}