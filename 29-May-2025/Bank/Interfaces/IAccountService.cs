using BankApi.Model.Dtos;
using BankApi.Models;
using System.Threading.Tasks;

namespace BankApi.Interfaces
{
    public interface IAccountService
    {
        Task<Account> GetAccountByAccNo(string accno);
        Task<Account> DeactivateAccount(string accno);
        Task<Account> Withdraw(AmountDto amountDto);
        Task<Account> Deposit(AmountDto amountDto);
        Task<decimal> GetBalanceByAccNo(string accno);
        Task<ICollection<Transaction>> GetTransactionsByAccNo(string accno);
    }
}
