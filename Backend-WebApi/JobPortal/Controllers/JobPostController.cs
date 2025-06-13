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
    public class JobPostController : ControllerBase
    {
        private readonly IJobPostService _jobPostService;

        public JobPostController(IJobPostService jobPostService)
        {
            _jobPostService = jobPostService;
        }
        [Authorize(Roles = "Recruiter")]
        [HttpPost("add")]
        public async Task<IActionResult> AddJobPost([FromBody] JobPostDto jobPostDto)
        {
            if (jobPostDto == null)
                return BadRequest("Job post data is required.");

            try
            {
                var response = await _jobPostService.AddJobPost(jobPostDto);
                return Ok(response);
            }
            catch (RecordNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (FieldRequiredException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (RegistrationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize]
        [HttpPost("paged")]
        public async Task<IActionResult> GetPagedJobPosts([FromBody] PageDataDto pageDataDto)
        {
            if (pageDataDto == null)
                return BadRequest("Pagination data is required.");

            try
            {
                var result = await _jobPostService.GetPagedJobPosts(pageDataDto);
                return Ok(result);
            }
            catch (FetchDataException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize]
        [HttpPost("matching-profile")]
        public async Task<IActionResult> GetJobPostsMatchingProfile(Guid SeekerId, [FromBody] PageDataDto pageDataDto)
        {
            if (pageDataDto == null)
                return BadRequest("Username and pagination data are required.");

            try
            {
                var result = await _jobPostService.GetJobPostsMatchingProfile(
                    SeekerId, pageDataDto.pageNumber, pageDataDto.pageSize);
                return Ok(result);
            }
            catch (FetchDataException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("by-company/{companyName}")]
        public async Task<IActionResult> GetJobPostsByCompanyName([FromQuery] string companyName)
        {
            try
            {
                var result = await _jobPostService.GetJobPostsByCompanyNameAsync(companyName);
                return Ok(result);
            }
            catch (FetchDataException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("{postId}")]
        public async Task<IActionResult> GetJobPostById(Guid postId)
        {
            try
            {
                var result = await _jobPostService.GetJobPostByIdAsync(postId);
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
        [Authorize(Roles = "Recruiter")]
        [HttpPost("{postId}/applicants")]
        public async Task<IActionResult> GetJobPostWithPagedApplicants(Guid postId, [FromBody] PageDataDto pageDataDto)
        {
            if (pageDataDto == null)
                return BadRequest("Pagination data is required.");

            try
            {
                var result = await _jobPostService.GetJobPostWithPagedApplicants(postId, pageDataDto, User);
                return Ok(result);
            }
            catch (FetchDataException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
            catch (RecordNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }
        [Authorize(Roles = "Recruiter")]
        [HttpDelete("{postId}")]
        public async Task<IActionResult> SoftDeleteJobPost(Guid postId)
        {
            try
            {
                var result = await _jobPostService.SoftDeleteJobPost(postId, User);
                return Ok(new { success = result });
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
        [Authorize(Roles = "Recruiter")]
        [HttpPut("{postId}")]
        public async Task<IActionResult> UpdateJobPost(Guid postId, [FromBody] JobPostUpdateDto updatedPostDto)
        {
            try
            {
                var result = await _jobPostService.UpdateJobPost(postId, updatedPostDto, User);
                return Ok(result);
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
        [Authorize(Roles = "Recruiter")]
        [HttpPut("{postId}/responsibilities")]
        public async Task<IActionResult> UpdateResponsibilities(Guid postId, [FromBody] List<ResponsibilitiesAddDto> updatedResponsibilities)
        {
            try
            {
                await _jobPostService.UpdateResponsibilities(postId, updatedResponsibilities, User);
                return Ok(new { message = "Responsibilities updated successfully." });
            }
            catch (UpdateException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize(Roles = "Recruiter")]
        [HttpPut("{postId}/requirements")]
        public async Task<IActionResult> UpdateRequirements(Guid postId, [FromBody] List<RequirementsAddDto> updatedRequirements)
        {
            try
            {
                await _jobPostService.UpdateRequirements(postId, updatedRequirements, User);
                return Ok(new { message = "Requirements updated successfully." });
            }
            catch (UpdateException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize(Roles = "Recruiter")]
        [HttpPut("{postId}/skills")]
        public async Task<IActionResult> UpdatePostSkills(Guid postId, [FromBody] List<SkillRegisterDto> updatedSkills)
        {
            try
            {
                await _jobPostService.UpdatePostSkills(postId, updatedSkills, User);
                return Ok(new { message = "Skills updated successfully." });
            }
            catch (UpdateException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
  
        [HttpGet("filter/title")]
        public async Task<IActionResult> FilterByTitle([FromQuery] string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                return BadRequest("Title is required for filtering.");

            var result = await _jobPostService.FilterJobPostsByTitle(title);
            return Ok(result);
        }

   
        [HttpGet("filter/location")]
        public async Task<IActionResult> FilterByLocation([FromQuery] string location)
        {
            if (string.IsNullOrWhiteSpace(location))
                return BadRequest("Location is required for filtering.");

            var result = await _jobPostService.FilterJobPostsByLocation(location);
            return Ok(result);
        }

      
        [HttpGet("filter/salary")]
        public async Task<IActionResult> FilterBySalary([FromQuery] decimal? minSalary = null,[FromQuery] decimal? maxSalary = null)
        {
            if (minSalary<=0 && maxSalary<=0)
                return BadRequest("Salary must be greater than zero.");

            var result = await _jobPostService.FilterJobPostsBySalary(minSalary,maxSalary);
            return Ok(result);
        }
    }
}