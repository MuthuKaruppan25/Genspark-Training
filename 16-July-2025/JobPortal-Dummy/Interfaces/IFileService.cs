
public interface IFileService
{

    Task<FileUploadResponseDto> UploadFileAsync(FileUploadDto fileUploadDto);
    // Task<string> DownloadFileAsync(FileGetRequestDto fileGetRequestDto);

    Task<(Stream stream, string fileName, string contentType)> DownloadFileStreamAsync(FileGetRequestDto requestDto);

    Task DeleteFileAsync(FileGetRequestDto fileGetRequestDto);
}