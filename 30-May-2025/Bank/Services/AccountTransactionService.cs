using BankApi.Interfaces;
using BankApi.Model.Dtos;
using BankApi.Models;
using Microsoft.EntityFrameworkCore;

public class AccountTransactionService : IAccountTransactionService
{
    private readonly BankContext _context;
    UserMapper _userMapper;
    AccountMapper _accountMapper;
    AccountResponseMapper _accountResponseMapper;

    public AccountTransactionService(BankContext context)
    {
        _context = context;
        _userMapper = new UserMapper();
        _accountMapper = new AccountMapper();
        _accountResponseMapper = new AccountResponseMapper();
    }

    public async Task<AccountResponseDto> CreateAccount(AccountCreateDto accountCreateDto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var branch = await _context.Branches
                .FirstOrDefaultAsync(b => b.BranchName == accountCreateDto.BranchName);

            if (branch == null)
            {
                throw new Exception($"Branch '{accountCreateDto.BranchName}' does not exist.");
            }

            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.PANNumber == accountCreateDto.PANNumber);

            string customerId;
            string? generatedPassword = null;
            User user;

            if (existingUser != null)
            {
                customerId = existingUser.CustomerId;
                user = existingUser;
            }
            else
            {
                user = _userMapper.MapUserAddRequest(accountCreateDto);
                if (user == null)
                {
                    throw new Exception("Failed to map user.");
                }

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                customerId = user.CustomerId;
                generatedPassword = user.Password;
            }

            var account = _accountMapper.MapAccountCreateRequest(accountCreateDto, customerId, branch.IFSCCode);
            _context.Accounts.Add(account);
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            var responseDto = _accountResponseMapper.MapToResponseDto(account, user, branch, generatedPassword);

            return responseDto;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            throw new Exception($"Error creating account: {ex.Message}", ex);
        }
    }
}
