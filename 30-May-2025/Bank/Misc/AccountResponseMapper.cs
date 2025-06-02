using BankApi.Model.Dtos;
using BankApi.Models;

public class AccountResponseMapper
{
    public AccountResponseDto MapToResponseDto(
        Account account,
        User user,
        Branch branch,
        string? generatedPassword = null
    )
    {
        return new AccountResponseDto
        {
            AccountNo = account.AccountNo,
            CustomerId = user.CustomerId,
            GeneratedPassword = generatedPassword,  
            AccountType = account.AccountType,
            Balance = account.Balance,
            BranchId = account.BranchId,           
            BranchName = branch.BranchName,
            BranchLocation = branch.Location,     
            CreatedAt = account.createdAt
        };
    }
}
