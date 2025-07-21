using Microsoft.AspNetCore.Mvc;
using Streaming_App.Models.DTOs;
using StreamingApp.Models;
using Streaming_App.Services;
using Streaming_App.Interfaces;
using Streaming_App.Helpers; // Assuming your service is in this namespace

namespace Streaming_App.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VideosController : ControllerBase
    {
        private readonly IVideoService _videoService;
        private readonly BlobStorageHelper _blobStorageHelper;

        public VideosController(IVideoService videoService, BlobStorageHelper blobStorageHelper)
        {
            _videoService = videoService;
            _blobStorageHelper = blobStorageHelper;
        }

        // POST: /api/videos/upload
        [HttpPost("upload")]
        public async Task<IActionResult> UploadVideo([FromForm] VideoUploadDto dto)
        {
            if (dto == null || dto.VideoFile == null)
                return BadRequest("Video file or metadata is missing.");

            try
            {
                var uploadedVideo = await _videoService.UploadVideo(dto);
                return CreatedAtAction(nameof(GetVideoById), new { id = uploadedVideo.Id }, uploadedVideo);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { message = ex.Message, details = ex.InnerException?.Message });
            }
        }

        // GET: /api/videos
        [HttpGet]
        public async Task<IActionResult> GetAllVideos([FromQuery] string? query = null)
        {
            try
            {
                var videos = await _videoService.GetAllVideos(query);
                return Ok(videos);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        // GET: /api/videos/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVideoById(Guid id)
        {
            try
            {
                var video = await _videoService.GetVideoById(id);
                if (video == null)
                    return NotFound($"Video with ID {id} not found.");

                return Ok(video);
            }
            catch (ApplicationException ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("stream/{id}")]
        public async Task<IActionResult> StreamVideo(Guid id)
        {
            var video = await _videoService.GetVideoById(id);
            if (video == null || string.IsNullOrEmpty(video.BlobUrl))
                return NotFound("Video not found.");

            var blobName = _blobStorageHelper.ExtractBlobNameFromUrl(video.BlobUrl);
            var blobResult = await _blobStorageHelper.GetVideoStreamAsync(blobName);

            if (blobResult == null)
                return NotFound("Video file not found in Blob Storage.");

            var contentType = blobResult.Details.ContentType ?? "video/mp4";
            var stream = blobResult.Content;

            return File(stream, contentType, enableRangeProcessing: true);
        }

    }
}
