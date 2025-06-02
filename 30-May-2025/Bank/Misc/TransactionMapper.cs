

using System.Transactions;
using BankApi.Model.Dtos;
using BankApi.Models;
public class TransactionMapper
{
    public BankApi.Models.Transaction MapTransactionAddRequest(TransactionDto transactionDto)
    {
        BankApi.Models.Transaction transaction = new();
        transaction.TransactionDate = DateTime.UtcNow;
        transaction.Amount = transactionDto.Amount;
        transaction.Description = transactionDto.Description;
        transaction.FromAccountId = transactionDto.FromAccountId;
        transaction.ToAccountId = transactionDto.ToAccountId;

        return transaction;
    }
}