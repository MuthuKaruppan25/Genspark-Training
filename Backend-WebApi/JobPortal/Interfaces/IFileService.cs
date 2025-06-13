
public interface IFileService
{

    Task<FileUploadResponseDto> UploadFileAsync(FileUploadDto fileUploadDto);
    Task<byte[]> DownloadFileAsync(FileGetRequestDto fileGetRequestDto);
    Task DeleteFileAsync(FileGetRequestDto fileGetRequestDto);
}