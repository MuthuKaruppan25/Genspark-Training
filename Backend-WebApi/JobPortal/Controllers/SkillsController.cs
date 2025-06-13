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
    public class SkillsController : ControllerBase
    {
        private readonly ISkillsService _skillsService;

        public SkillsController(ISkillsService skillsService)
        {
            _skillsService = skillsService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddSkill([FromBody] SkillRegisterDto skillRegisterDto)
        {
            if (skillRegisterDto == null)
                return BadRequest("Skill data is required.");

            try
            {
                var skill = await _skillsService.AddSkill(skillRegisterDto);
                return Ok(skill);
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
        [HttpPut("update/{skillId}")]
        public async Task<IActionResult> UpdateSkill(Guid skillId, [FromBody] SkillRegisterDto skillRegisterDto)
        {
            if (skillRegisterDto == null)
                return BadRequest("Skill data is required.");

            try
            {
                await _skillsService.UpdateSkill(skillId, skillRegisterDto);
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