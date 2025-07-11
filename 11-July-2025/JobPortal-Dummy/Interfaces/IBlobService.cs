public interface IBlobService
{
    Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType);
}