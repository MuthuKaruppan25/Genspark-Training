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
    }
}