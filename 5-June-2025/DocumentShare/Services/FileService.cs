using System.Security.Claims;
using DocumentShare.Contexts;
using DocumentShare.Exceptions;
using DocumentShare.Interfaces;
using DocumentShare.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

public class FileService : IFileService
{
    private readonly IRepository<Guid, FileModel> _fileRepository;
    private readonly IHubContext<NotificationHub> _hubContext;

    public FileService(IRepository<Guid, FileModel> fileRepository, IHubContext<NotificationHub> hubContext)
    {
        _fileRepository = fileRepository;
        _hubContext = hubContext;
    }


    public async Task<string> SaveFileAsync(IFormFile file, ClaimsPrincipal user)
    {
        if (file == null || file.Length == 0)
            throw new FileNotFoundException("File is empty or null");

        try
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);

            if (memoryStream.Length == 0)
                throw new FileConversionException("File could not be read into memory");

            var username =
                user?.Identity?.Name
                ?? user?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                ?? "unknown";

            var fileModel = new FileModel
            {
                FileName = file.FileName,
                FileType = file.ContentType,
                FileData = memoryStream.ToArray(),
                Uploader = username
            };

            var savedfile = await _fileRepository.Add(fileModel);
            if (savedfile == null)
                throw new FileUploadException("Failed to store file in the database");

            await _hubContext.Clients.All.SendAsync("ReceiveMessage", fileModel.Uploader, fileModel.FileName);

            return savedfile.guid.ToString();
        }
        catch (Exception ex) when (!(ex is FileUploadException || ex is FileConversionException))
        {
            throw new Exception("Unexpected error occurred during file upload", ex);
        }
    }


    public async Task<byte[]?> GetFileAsync(string fileName)
    {
        try
        {
            var fileRecords = await _fileRepository.GetAll();

            var fileRecord = fileRecords.FirstOrDefault(f => f.FileName == fileName);
            if (fileRecord == null)
                throw new FileNotFoundException("File not found in database");

            return fileRecord.FileData;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error retrieving file: {ex.Message}", ex);
        }

    }


}
