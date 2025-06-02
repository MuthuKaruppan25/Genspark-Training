using BankApi.Interfaces;
using BankApi.Model.Dtos;
using BankApi.Models;

public class AccountService : IAccountService
{
    private readonly IRepository<string, Account> _repository;

    public AccountService(IRepository<string, Account> repository)
    {
        _repository = repository;
    }

    public async Task<Account> GetAccountByAccNo(string accno)
    {
        try
        {
            var account = await _repository.Get(accno);

            if (account.Status.Equals("DeActivated"))
                throw new Exception("Account is deactivated.");

            return account;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get account {accno}: {ex.Message}");
        }
    }

    public async Task<Account> DeactivateAccount(string accno)
    {
        try
        {
            var account = await _repository.Get(accno);
            account.Status = "DeActivated";
            await _repository.Update(accno, account);
            return account;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to deactivate account {accno}: {ex.Message}");
        }
    }

    public async Task<Account> Withdraw(AmountDto amountDto)
    {
        try
        {
            var account = await _repository.Get(amountDto.accno);

            if (account.Status.Equals("DeActivated"))
                throw new Exception("Account is deactivated.");

            if (account.Balance < amountDto.amount)
                throw new Exception("Insufficient balance.");

            account.Balance -= amountDto.amount;
            await _repository.Update(amountDto.accno, account);
            return account;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to withdraw from account {amountDto.accno}: {ex.Message}");
        }
    }

    public async Task<Account> Deposit(AmountDto amountDto)
    {
        try
        {
            var account = await _repository.Get(amountDto.accno);

            if (account.Status.Equals("DeActivated"))
                throw new Exception("Account is deactivated.");

            account.Balance += amountDto.amount;
            await _repository.Update(amountDto.accno, account);
            return account;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to deposit to account {amountDto.accno}: {ex.Message}");
        }
    }

    public async Task<decimal> GetBalanceByAccNo(string accno)
    {
        try
        {
            var account = await _repository.Get(accno);

            if (account.Status.Equals("DeActivated"))
                throw new Exception("Account is deactivated.");

            return account.Balance;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get balance for account {accno}: {ex.Message}");
        }
    }

    public async Task<ICollection<Transaction>> GetTransactionsByAccNo(string accno)
    {
        try
        {
            var account = await _repository.Get(accno);

            if (account.Status.Equals("DeActivated"))
                throw new Exception("Account is deactivated.");

            if (account.Transactions == null || !account.Transactions.Any())
                throw new Exception("No transactions found for this account.");

            return account.Transactions;
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to get transactions for account {accno}: {ex.Message}");
        }
    }
}
