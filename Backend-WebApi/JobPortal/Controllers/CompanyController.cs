using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using JobPortal.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace JobPortal.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class CompanyController : ControllerBase
    {
        private readonly ICompanyService _companyService;

        public CompanyController(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpPost("register")]
        [SwaggerResponseExample(200, typeof(CompanyRegisterResponseDto))]
        public async Task<IActionResult> RegisterCompany([FromBody] CompanyRegisterDto companyRegisterDto)
        {
            if (companyRegisterDto == null)
                return BadRequest("Company registration data is required.");

            try
            {
                var response = await _companyService.RegisterCompany(companyRegisterDto);
                return Ok(response);
            }
            catch (FieldRequiredException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (DuplicateEntryException ex)
            {
                return Conflict(new { error = ex.Message });
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

        [HttpGet("{companyId}/recruiters")]
        public async Task<IActionResult> GetRecruitersInCompany(Guid companyId)
        {
            try
            {
                var recruiters = await _companyService.GetRecruitersInCompany(companyId);
                return Ok(recruiters);
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

        [HttpGet("{companyId}/locations")]
        public async Task<IActionResult> GetCompanyLocations(Guid companyId)
        {
            try
            {
                var locations = await _companyService.GetCompanyLocations(companyId);
                return Ok(locations);
            }
            catch (FetchDataException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }

        [HttpPut("{companyId}")]
        public async Task<IActionResult> UpdateCompanyDetails(Guid companyId, [FromBody] CompanyUpdateDto updateDto)
        {
            if (updateDto == null)
                return BadRequest("Update data is required.");

            try
            {
                await _companyService.UpdateCompanyDetails(companyId, updateDto);
                return Ok(new { message = "Company details updated successfully." });
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

        [HttpPut("{companyId}/locations")]
        public async Task<IActionResult> UpdateCompanyLocations(Guid companyId, [FromBody] List<AddressRegisterDto> locationDtos)
        {
            if (locationDtos == null || locationDtos.Count == 0)
                return BadRequest("At least one location is required.");

            try
            {
                await _companyService.UpdateCompanyLocations(companyId, locationDtos);
                return Ok(new { message = "Company locations updated successfully." });
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
        [HttpDelete("{companyId}")]
        public async Task<IActionResult> SoftDeleteCompany(Guid companyId)
        {
            try
            {
                var result = await _companyService.SoftDeleteCompany(companyId);
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