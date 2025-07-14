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
    public class JobApplicantController : ControllerBase
    {
        private readonly IJobApplicantService _jobApplicantService;

        public JobApplicantController(IJobApplicantService jobApplicantService)
        {
            _jobApplicantService = jobApplicantService;
        }
        [Authorize(Roles = "Seeker")]
        [HttpPost("apply")]
        public async Task<IActionResult> CreateApplication(JobApplicantAddDto jobApplicantAddDto)
        {
            if (jobApplicantAddDto == null)
                return BadRequest("Application data is required.");

            try
            {
                var response = await _jobApplicantService.CreateApplication(jobApplicantAddDto, User);
                return Ok(response);
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
        public async Task<IActionResult> GetPagedApplications([FromBody] PageDataDto pageDataDto)
        {
            if (pageDataDto == null)
                return BadRequest("Pagination data is required.");

            try
            {
                var result = await _jobApplicantService.GetPagedApplications(pageDataDto.pageNumber, pageDataDto.pageSize);
                return Ok(result);
            }
            catch (FetchDataException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [Authorize(Roles = "Seeker")]
        [HttpDelete("{applicationId}")]
        public async Task<IActionResult> SoftDeleteApplication(Guid applicationId)
        {
            try
            {
                var result = await _jobApplicantService.SoftDeleteApplication(applicationId, User);
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

        [HttpGet("post/{postId}/applications")]
        public async Task<ActionResult<IEnumerable<JobApplication>>> GetApplicationsByPostId(Guid postId)
        {
            try
            {
                var applications = await _jobApplicantService.GetApplicationsByPostId(postId);

                if (!applications.Any())
                    return NotFound("No applications found for the given job post.");

                return Ok(applications);
            }
            catch (FetchDataException ex)
            {
                return StatusCode(500, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Unexpected error occurred: {ex.Message}");
            }
        }

        [Authorize(Roles = "Recruiter")]
        [HttpPut("update-status/{applicationId}")]
        public async Task<IActionResult> UpdateStatus(Guid applicationId, [FromBody] StatusUpdate statusUpdate)
        {
            if (statusUpdate == null || string.IsNullOrWhiteSpace(statusUpdate.status))
            {
                return BadRequest("Status is required.");
            }

            try
            {
                var updatedApplication = await _jobApplicantService.UpdateStatus(applicationId, statusUpdate);
                return Ok(updatedApplication);
            }
            catch (RecordNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (UpdateException ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An unexpected error occurred.", Details = ex.Message });
            }
        }
        [HttpGet("check-application-exists")]
        public async Task<IActionResult> CheckIfApplicationExists([FromQuery] Guid postId, [FromQuery] Guid seekerId)
        {
            try
            {
                var exists = await _jobApplicantService.CheckIfApplicationExists(postId, seekerId);
                return Ok(new { exists });
            }
            catch (FetchDataException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", details = ex.Message });
            }
        }

    }
}