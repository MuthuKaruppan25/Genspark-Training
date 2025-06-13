using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using JobPortal.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace JobPortal.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class RecruiterController : ControllerBase
    {
        private readonly IRecruiterService _recruiterService;

        public RecruiterController(IRecruiterService recruiterService)
        {
            _recruiterService = recruiterService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterRecruiter([FromBody] RecruiterRegisterDto recruiterRegisterDto)
        {
            if (recruiterRegisterDto == null)
                return BadRequest("Recruiter registration data is required.");

            try
            {
                var response = await _recruiterService.RegisterCompany(recruiterRegisterDto);
                return Ok(response);
            }
            catch (DuplicateEntryException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (FieldRequiredException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (RecordNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (RegistrationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("{recruiterId}")]
        public async Task<IActionResult> GetRecruiterById(Guid recruiterId)
        {
            try
            {
                var recruiter = await _recruiterService.GetRecruiterById(recruiterId);
                return Ok(recruiter);
            }
            catch (RecordNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (FetchDataException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize(Roles ="Recruiter")]
        [HttpGet("{recruiterId}/jobposts")]
        public async Task<IActionResult> GetRecruiterJobPosts(Guid recruiterId)
        {
            try
            {
                var posts = await _recruiterService.GetRecruiterJobPosts(recruiterId);
                return Ok(posts);
            }
            catch (FetchDataException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("by-username/{username}")]
        public async Task<IActionResult> GetRecruiterByUsername(string username)
        {
            try
            {
                var recruiter = await _recruiterService.GetRecruiterByUsername(username);
                return Ok(recruiter);
            }
            catch (RecordNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (FetchDataException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize(Roles ="Recruiter")]
        [HttpPut("{recruiterId}")]
        public async Task<IActionResult> UpdateRecruiterDetails(Guid recruiterId, [FromBody] RecruiterUpdateDto updateDto)
        {
            if (updateDto == null)
                return BadRequest("Update data is required.");

            try
            {
                await _recruiterService.UpdateRecruiterDetails(recruiterId, updateDto);
                return Ok(new { message = "Recruiter details updated successfully." });
            }
            catch (RecordNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UpdateException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize(Roles ="Recruiter")]
        [HttpDelete("{recruiterId}")]
        public async Task<IActionResult> SoftDeleteRecruiter(Guid recruiterId)
        {
            try
            {
                await _recruiterService.SoftDeleteRecruiter(recruiterId,User);
                return Ok(new { message = "Recruiter deleted successfully." });
            }
            catch (RecordNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (UpdateException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}