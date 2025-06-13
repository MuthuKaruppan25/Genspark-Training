using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace JobPortal.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class SeekerController : ControllerBase
    {
        private readonly ISeekerService _seekerService;

        public SeekerController(ISeekerService seekerService)
        {
            _seekerService = seekerService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterSeeker([FromBody] SeekerRegisterDto seekerRegisterDto)
        {
            if (seekerRegisterDto == null)
                return BadRequest("Seeker registration data is required.");

            try
            {
                var response = await _seekerService.RegisterSeeker(seekerRegisterDto);
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
        [HttpPost("paged")]
        public async Task<IActionResult> GetPagedSeekers([FromBody] PageDataDto pageDataDto)
        {
            if (pageDataDto == null)
                return BadRequest("Pagination data is required.");

            try
            {
                var result = await _seekerService.GetPagedSeekers(pageDataDto.pageNumber, pageDataDto.pageSize);
                return Ok(result);
            }
            catch (NoRecordsFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (FetchDataException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("applications/{username}")]
        public async Task<IActionResult> GetSeekerWithApplications(string username)
        {
            try
            {
                var result = await _seekerService.GetSeekerWithApplications(username);
                return Ok(result);
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
        [Authorize]
        [HttpGet("/skills/{username}")]
        public async Task<IActionResult> GetSeekerSkills(string username)
        {
            try
            {
                var result = await _seekerService.GetSeekerSkills(username);
                return Ok(result);
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

        [Authorize(Roles = "Seeker")]
        [HttpPut("{username}")]
        public async Task<IActionResult> UpdateSeekerDetails(string username, [FromBody] SeekerUpdateDto updateDto)
        {
            if (updateDto == null)
                return BadRequest("Update data is required.");

            try
            {
                await _seekerService.UpdateSeekerDetails(username, updateDto, User);
                return Ok(new { message = "Seeker details updated successfully." });
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
        [Authorize(Roles = "Seeker")]
        [HttpPut("skills/{username}")]
        public async Task<IActionResult> UpdateSeekerSkills(string username, [FromBody] List<SkillRegisterDto> skillNames)
        {
            if (skillNames == null || skillNames.Count == 0)
                return BadRequest("At least one skill is required.");

            try
            {
                await _seekerService.UpdateSeekerSkills(username, skillNames, User);
                return Ok(new { message = "Seeker skills updated successfully." });
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
        [Authorize]
        [HttpGet("{username}")]
        public async Task<IActionResult> GetSeekerByUsername(string username)
        {
            try
            {
                var result = await _seekerService.GetSeekerByUsername(username);
                return Ok(result);
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
        [Authorize(Roles = "Seeker")]
        [HttpDelete("{username}")]
        public async Task<IActionResult> SoftDeleteSeeker(string username)
        {
            try
            {
                await _seekerService.SoftDeleteSeekerAsync(username, User);
                return Ok(new { message = "Seeker deleted successfully." });
            }
            catch (RecordNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (UpdateException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("search/by-name")]
        public async Task<IActionResult> SearchSeekersByName([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name parameter is required.");

            try
            {
                var result = await _seekerService.SearchSeekersByName(name);
                return Ok(result);
            }
            catch (NoRecordsFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
        [Authorize]
        [HttpPost("search/by-skills")]
        public async Task<IActionResult> SearchSeekersBySkills([FromBody] List<SkillRegisterDto> skills)
        {
            if (skills == null || !skills.Any())
                return BadRequest("At least one skill is required.");

            try
            {
                var result = await _seekerService.SearchSeekersBySkills(skills);
                return Ok(result);
            }
            catch (FieldRequiredException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (RecordNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (NoRecordsFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("search/by-education")]
        public async Task<IActionResult> SearchSeekersByEducation([FromQuery] string education)
        {
            if (string.IsNullOrWhiteSpace(education))
                return BadRequest("Education parameter is required.");

            try
            {
                var result = await _seekerService.SearchSeekersByEducation(education);
                return Ok(result);
            }
            catch (NoRecordsFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }



    }
}