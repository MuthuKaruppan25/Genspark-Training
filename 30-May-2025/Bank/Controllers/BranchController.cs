using BankApi.Models;
using Microsoft.AspNetCore.Mvc;
using BankApi.Services;
using BankApi.Interfaces;

namespace BankApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BranchController : ControllerBase
    {
        private readonly IBranchService _branchService;

        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }


        [HttpPost]
        public async Task<IActionResult> AddBranch([FromBody] Branch branch)
        {


            try
            {
                var addedBranch = await _branchService.AddBranch(branch);
                return CreatedAtAction(nameof(GetBranchById), new { id = addedBranch.IFSCCode }, addedBranch);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error adding branch: {ex.Message}");
            }
        }

  
        [HttpGet]
        public async Task<IActionResult> GetAllBranches()
        {
            try
            {
                var branches = await _branchService.GetAllBranches();
                return Ok(branches);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching branches: {ex.Message}");
            }
        }

        // GET: api/Branch/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBranchById(int id)
        {
            try
            {
                var branch = await _branchService.GetBranchById(id);
                if (branch == null)
                    return NotFound($"Branch with ID {id} not found.");
                return Ok(branch);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error fetching branch: {ex.Message}");
            }
        }
    }
}
