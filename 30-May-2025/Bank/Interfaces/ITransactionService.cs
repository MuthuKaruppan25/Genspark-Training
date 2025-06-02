
using System.Transactions;
using BankApi.Model.Dtos;

namespace BankApi.Interfaces;

public interface ITransactionService
{
    public Task<BankApi.Models.Transaction>  CreateTransaction(TransactionDto transactionDto);
}