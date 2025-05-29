using BankApi.Interfaces;
using BankApi.Model.Dtos;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;

    public TransactionController(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateTransaction([FromBody] TransactionDto transactionDto)
    {
        

        try
        {
            var transaction = await _transactionService.CreateTransaction(transactionDto);
            return Ok(transaction);
        }
        catch (Exception ex)
        {
           
            return StatusCode(500, new { error = ex.Message });
        }
    }
}
