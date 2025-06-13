using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using JobPortal.Models.DTOs;
using log4net;
using Microsoft.EntityFrameworkCore;

public class FileService : IFileService
{
    private readonly IRepository<Guid, FileModel> _fileRepository;
    private readonly IRepository<Guid, Seeker> _seekerRepository;

    private static readonly ILog _errorLogger = LogManager.GetLogger("ErrorFileAppender");
    private static readonly ILog _dataLogger = LogManager.GetLogger("DataChangeFileAppender");

    public FileService(IRepository<Guid, FileModel> fileRepository, IRepository<Guid, Seeker> seekerRepository)
    {
        _fileRepository = fileRepository;
        _seekerRepository = seekerRepository;
    }

    public async Task<FileUploadResponseDto> UploadFileAsync(FileUploadDto fileUploadDto)
    {
        try
        {
            if (fileUploadDto.Resume == null || fileUploadDto.Resume.Length == 0)
                throw new FieldRequiredException("Resume file is required.");

            var seeker = await _seekerRepository.Get(fileUploadDto.SeekerId);
            if (seeker == null)
                throw new RecordNotFoundException("Seeker not found.");

            var hasApplied = seeker.jobApplications?.Any(app => app.JobPostId == fileUploadDto.JobPostId && !app.IsDeleted) ?? false;
            if (!hasApplied)
                throw new ValidationException("Seeker must apply to the job before uploading a resume for it.");

            using var memoryStream = new MemoryStream();
            await fileUploadDto.Resume.CopyToAsync(memoryStream);
            var fileBytes = memoryStream.ToArray();

            var fileModel = new FileModel
            {
                Name = fileUploadDto.Resume.FileName,
                Type = fileUploadDto.Resume.ContentType,
                Data = fileBytes,
                Size = fileUploadDto.Resume.Length,
                JobPostId = fileUploadDto.JobPostId,
                SeekerId = fileUploadDto.SeekerId
            };

            await _fileRepository.Add(fileModel);

            _dataLogger.Info($"Resume uploaded: SeekerId = {fileUploadDto.SeekerId}, JobPostId = {fileUploadDto.JobPostId}");

            return new FileUploadResponseDto
            {
                FileName = fileModel.Name,
                FileType = fileModel.Type,
                FileSize = fileModel.Size,
                UploadDate = fileModel.CreatedAt
            };
        }
        catch (FieldRequiredException ex)
        {
            _errorLogger.Error("Missing resume during file upload", ex);
            throw;
        }
        catch (ValidationException ex)
        {
            _errorLogger.Error("Validation failed during file upload", ex);
            throw;
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Error("Seeker not found during file upload", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Failed to upload resume due to unexpected error", ex);
            throw new UploadException("Failed to upload resume.", ex);
        }
    }

    public async Task<byte[]> DownloadFileAsync(FileGetRequestDto fileGetRequestDto)
    {
        try
        {
            var allFiles = await _fileRepository.GetAll();

            var file = allFiles.FirstOrDefault(f =>
                f.SeekerId == fileGetRequestDto.SeekerId &&
                f.JobPostId == fileGetRequestDto.JobPostId &&
                !f.IsDeleted);

            if (file == null)
                throw new RecordNotFoundException("Resume not found for the specified seeker and job post.");

            _dataLogger.Info($"Resume downloaded: SeekerId = {fileGetRequestDto.SeekerId}, JobPostId = {fileGetRequestDto.JobPostId}");

            return file.Data;
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Error("Resume not found during download", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Failed to download resume due to unexpected error", ex);
            throw new DownloadException("Failed to download resume.", ex);
        }
    }

    public async Task DeleteFileAsync(FileGetRequestDto fileGetRequestDto)
    {
        try
        {
            var allFiles = await _fileRepository.GetAll();

            var file = allFiles.FirstOrDefault(f =>
                f.SeekerId == fileGetRequestDto.SeekerId &&
                f.JobPostId == fileGetRequestDto.JobPostId &&
                !f.IsDeleted);

            if (file == null)
                throw new RecordNotFoundException("Resume not found or already deleted.");

            file.IsDeleted = true;
            await _fileRepository.Update(file.guid, file);

            _dataLogger.Info($"Resume deleted: SeekerId = {fileGetRequestDto.SeekerId}, JobPostId = {fileGetRequestDto.JobPostId}");
        }
        catch (RecordNotFoundException ex)
        {
            _errorLogger.Error("Resume not found or already deleted during delete operation", ex);
            throw;
        }
        catch (Exception ex)
        {
            _errorLogger.Error("Failed to delete resume due to unexpected error", ex);
            throw new UpdateException("Failed to delete resume.", ex);
        }
    }
}
