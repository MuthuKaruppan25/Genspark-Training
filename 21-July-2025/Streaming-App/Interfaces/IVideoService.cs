
using Streaming_App.Models.DTOs;
using StreamingApp.Models;

namespace Streaming_App.Interfaces;

public interface IVideoService
{
    Task<VideoModel> UploadVideo(VideoUploadDto videoUploadDto);
    Task<IEnumerable<VideoModel>> GetAllVideos(string? query = null);
    Task<VideoModel?> GetVideoById(Guid id);
}