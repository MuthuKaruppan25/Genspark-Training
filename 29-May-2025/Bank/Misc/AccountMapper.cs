using BankApi.Model.Dtos;
using BankApi.Models;
using System;

public class AccountMapper
{
    private static readonly Random _random = new();

    public Account MapAccountCreateRequest(
        AccountCreateDto dto,
        string customerId,
        string branchIfsc
    )
    {
        return new Account
        {
            AccountNo = GenerateAccountNumber(),
            CustomerId = customerId,
            BranchId = branchIfsc,
            AccountType = dto.AccountType,
            Balance = dto.InitialDeposit,
            createdAt = DateTime.UtcNow
        };
    }

    public string GenerateAccountNumber()
    {
    
        var accountNumber = new char[12];
        for (int i = 0; i < 12; i++)
        {
            accountNumber[i] = (char)('0' + _random.Next(10)); 
        }
        return new string(accountNumber);
    }
}
