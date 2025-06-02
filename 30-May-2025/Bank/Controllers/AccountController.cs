using BankApi.Interfaces;
using BankApi.Model.Dtos;
using BankApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly IAccountTransactionService _accountTransactionService;

    public AccountController(IAccountService accountService, IAccountTransactionService accountTransactionService)
    {
        _accountService = accountService;
        _accountTransactionService = accountTransactionService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateAccount([FromBody] AccountCreateDto accountCreateDto)
    {

        try
        {
            var account = await _accountTransactionService.CreateAccount(accountCreateDto);
            return Ok(account);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpGet("{accno}")]
    public async Task<IActionResult> GetAccountByAccNo(string accno)
    {
        try
        {
            var account = await _accountService.GetAccountByAccNo(accno);
            return Ok(account);
        }
        catch (Exception ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpPut("Deactivate/{accno}")]
    public async Task<IActionResult> DeactivateAccount(string accno)
    {
        try
        {
            var account = await _accountService.DeactivateAccount(accno);
            return Ok(account);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    [HttpPut("withdraw")]
    public async Task<IActionResult> Withdraw(AmountDto amountDto)
    {
        try
        {
            var account = await _accountService.Withdraw(amountDto);
            return Ok(account);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("deposit")]
    public async Task<IActionResult> Deposit(AmountDto amountDto)
    {
        try
        {
            var account = await _accountService.Deposit(amountDto);
            return Ok(account);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("balance/{accno}")]
    public async Task<IActionResult> GetBalanceByAccNo(string accno)
    {
        try
        {
            var balance = await _accountService.GetBalanceByAccNo(accno);
            return Ok(new { balance });
        }
        catch (Exception ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    [HttpGet("transactions/{accno}")]
    public async Task<IActionResult> GetTransactionsByAccNo(string accno)
    {
        try
        {
            var transactions = await _accountService.GetTransactionsByAccNo(accno);
            return Ok(transactions);
        }
        catch (Exception ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
