// Controllers/FAQController.cs
using BankApi.Interfaces;
using BankApi.Model.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class FAQController : ControllerBase
{
    private readonly IFlaskService _flaskService;

    public FAQController(IFlaskService flaskService)
    {
        _flaskService = flaskService;
    }

    [HttpPost("ask")]
    public async Task<IActionResult> AskQuestion([FromBody] QuestionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return BadRequest("Question cannot be empty.");
        }

        var answer = await _flaskService.GetAnswerAsync(request.Question);
        return Ok(new { Answer = answer });
    }

    [HttpPost("Retrive")]
    public async Task<IActionResult> RetriveAnswer([FromBody] QuestionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Question))
        {
            return BadRequest("Question cannot be empty.");
        }

        var answer = await _flaskService.RetrieveAnswerAsync(request.Question);
        return Ok(new { Answer = answer });
    }


}
