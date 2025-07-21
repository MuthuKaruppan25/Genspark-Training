using Streaming_App.Helpers;
using Streaming_App.Models.DTOs;
using StreamingApp.Contexts;
using StreamingApp.Models;
using Microsoft.EntityFrameworkCore;
using Streaming_App.Interfaces;

namespace Streaming_App.Services;
public class VideoService : IVideoService
{
    private readonly VideoDbContext _context;
    private readonly BlobStorageHelper _blobStorageHelper;

    public VideoService(VideoDbContext context, BlobStorageHelper blobStorageHelper)
    {
        _context = context;
        _blobStorageHelper = blobStorageHelper;
    }

    public async Task<VideoModel> UploadVideo(VideoUploadDto videoUploadDto)
    {
        if (videoUploadDto == null || videoUploadDto.VideoFile == null)
            throw new ArgumentNullException(nameof(videoUploadDto), "Video file or metadata is missing.");

        try
        {
            // Upload the video file to Blob Storage
            var blobUrl = await _blobStorageHelper.UploadVideoAsync(videoUploadDto.VideoFile);

            // Create a new VideoModel entry
            var video = new VideoModel
            {
                Title = videoUploadDto.Title,
                Description = videoUploadDto.Description,
                UploadDate = DateTime.UtcNow,
                BlobUrl = blobUrl
            };

            _context.Videos.Add(video);
            await _context.SaveChangesAsync();

            return video;
        }
        catch (Exception ex)
        {

            throw new ApplicationException("An error occurred while uploading the video.", ex);
        }
    }

public async Task<IEnumerable<VideoModel>> GetAllVideos(string? query = null)
{
    try
    {
        var videosQuery = _context.Videos.AsQueryable();

        if (!string.IsNullOrWhiteSpace(query))
        {
                Console.WriteLine(query);
            query = query.ToLower();
            videosQuery = videosQuery.Where(v =>
                v.Title.ToLower().Contains(query) ||
                v.Description.ToLower().Contains(query));
        }

        return await videosQuery
            .OrderByDescending(v => v.UploadDate)
            .ToListAsync();
    }
    catch (Exception ex)
    {
        throw new ApplicationException("Failed to retrieve video list.", ex);
    }
}

    public async Task<VideoModel?> GetVideoById(Guid id)
    {
        try
        {
            return await _context.Videos.FindAsync(id);
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Failed to fetch video with ID {id}.", ex);
        }
    }
}
