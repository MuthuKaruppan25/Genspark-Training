using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }
        // [Authorize(Roles = "Seeker")]
        // [HttpPost("upload")]
        // public async Task<IActionResult> UploadFile([FromForm] FileUploadDto fileUploadDto)
        // {
        //     if (fileUploadDto == null)
        //         return BadRequest("File upload data is required.");

        //     try
        //     {
        //         var response = await _fileService.UploadFileAsync(fileUploadDto);
        //         return Ok(response);
        //     }
        //     catch (FieldRequiredException ex)
        //     {
        //         return BadRequest(new { error = ex.Message });
        //     }
        //     catch (UploadException ex)
        //     {
        //         return StatusCode(500, new { error = ex.Message });
        //     }
        // }
        [Authorize(Roles = "Recruiter")]
        [HttpPost("download")]
        public async Task<IActionResult> DownloadFile([FromBody] FileGetRequestDto fileGetRequestDto)
        {
            if (fileGetRequestDto == null)
                return BadRequest("File request data is required.");

            try
            {
                var fileData = await _fileService.DownloadFileAsync(fileGetRequestDto);
                return File(fileData, "application/octet-stream", "resume");
            }
            catch (RecordNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (DownloadException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        // [Authorize(Roles = "Seeker")]
        // [HttpDelete]
        // public async Task<IActionResult> DeleteFile([FromBody] FileGetRequestDto fileGetRequestDto)
        // {
        //     if (fileGetRequestDto == null)
        //         return BadRequest("File request data is required.");

        //     try
        //     {
        //         await _fileService.DeleteFileAsync(fileGetRequestDto);
        //         return Ok(new { message = "File deleted successfully." });
        //     }
        //     catch (RecordNotFoundException ex)
        //     {
        //         return NotFound(new { error = ex.Message });
        //     }
        //     catch (UpdateException ex)
        //     {
        //         return StatusCode(500, new { error = ex.Message });
        //     }
        // }
    }
}