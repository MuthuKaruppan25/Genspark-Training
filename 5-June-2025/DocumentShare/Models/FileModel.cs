
namespace DocumentShare.Models;
public class FileModel
{
    public Guid guid { get; set; } = Guid.NewGuid();
    public string FileName { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty;
    public byte[] FileData { get; set; } = Array.Empty<byte>();
    public string Uploader { get; set; } = string.Empty;
    public DateTime UploadDate { get; set; } = DateTime.UtcNow;
}