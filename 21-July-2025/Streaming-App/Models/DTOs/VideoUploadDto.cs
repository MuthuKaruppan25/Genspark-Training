namespace Streaming_App.Models.DTOs;

public class VideoUploadDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public IFormFile VideoFile { get; set; } = default!;
}