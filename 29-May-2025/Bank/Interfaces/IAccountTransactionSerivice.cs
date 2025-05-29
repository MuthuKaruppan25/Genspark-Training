
namespace BankApi.Interfaces;

using BankApi.Model.Dtos;
using BankApi.Models;

public interface IAccountTransactionService
{
    public Task<AccountResponseDto> CreateAccount(AccountCreateDto accountCreateDto);
}