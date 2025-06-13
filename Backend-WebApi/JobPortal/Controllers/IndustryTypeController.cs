using JobPortal.Exceptions;
using JobPortal.Interfaces;
using JobPortal.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace JobPortal.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class IndustryTypeController : ControllerBase
    {
        private readonly IIndustryTypeService _industryTypeService;

        public IndustryTypeController(IIndustryTypeService industryTypeService)
        {
            _industryTypeService = industryTypeService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddIndustryType([FromBody] IndustryTypeRegister industryTypeRegister)
        {
            if (industryTypeRegister == null)
                return BadRequest("Industry type data is required.");

            try
            {
                var industryType = await _industryTypeService.AddIndustryType(industryTypeRegister);
                return Ok(industryType);
            }
            catch (FieldRequiredException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (DuplicateEntryException ex)
            {
                return Conflict(new { error = ex.Message });
            }
            catch (RegistrationException ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
        [HttpPut("{industryTypeId}")]
        public async Task<IActionResult> UpdateIndustryType(Guid industryTypeId, [FromBody] IndustryTypeRegister industryTypeRegister)
        {
            if (industryTypeRegister == null)
                return BadRequest("Industry type data is required.");

            try
            {
                await _industryTypeService.UpdateIndustryType(industryTypeId, industryTypeRegister);
                return NoContent();
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
    }
}