using DocumentShare.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DocumentShare.Misc;
using SecondWebApi.Misc;
namespace DocumentShare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }


        [HttpPost("upload")]
        [Authorize(Roles = "HRAdmin")]
        [ServiceFilter(typeof(FileExceptionFilter))] 
        [RequestSizeLimit(10 * 1024 * 1024)] 
        public async Task<IActionResult> UploadFile(IFormFile file)
        {


            var fileId = await _fileService.SaveFileAsync(file, User);
            return Ok(new { message = "File uploaded successfully", fileId });
        }


        [HttpGet("download/{fileName}")]
        [Authorize] 
        [ServiceFilter(typeof(CustomExceptionFilter))]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            var fileBytes = await _fileService.GetFileAsync(fileName);
            var mimeType = "application/octet-stream";
            return File(fileBytes, mimeType, fileName);
        }
    }
}
