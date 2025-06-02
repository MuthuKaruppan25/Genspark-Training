
using System.Transactions;
using BankApi.Interfaces;
using BankApi.Model.Dtos;
using Microsoft.EntityFrameworkCore;
public class TransactionService : ITransactionService
{
    private readonly BankContext _bankContext;

    TransactionMapper _transactionMapper;

    public TransactionService(BankContext bankContext)
    {
        _bankContext = bankContext;
        _transactionMapper = new TransactionMapper();
    }
    public async Task<BankApi.Models.Transaction> CreateTransaction(TransactionDto transactionDto)
    {
        using (var dbTransaction = await _bankContext.Database.BeginTransactionAsync())
        {
            try
            {
                
                var fromAccount = await _bankContext.Accounts
                    .SingleOrDefaultAsync(a => a.AccountNo == transactionDto.FromAccountId);

      
                var toAccount = await _bankContext.Accounts
                    .SingleOrDefaultAsync(a => a.AccountNo == transactionDto.ToAccountId);

                if (fromAccount == null || toAccount == null)
                    throw new Exception("Invalid account details.");

                if (fromAccount.Balance < transactionDto.Amount)
                    throw new Exception("Insufficient funds.");

                
                fromAccount.Balance -= transactionDto.Amount;

                
                toAccount.Balance += transactionDto.Amount;

                
                _bankContext.Accounts.Update(fromAccount);
                _bankContext.Accounts.Update(toAccount);

                var tran = _transactionMapper.MapTransactionAddRequest(transactionDto);
                await _bankContext.Transactions.AddAsync(tran);

               
                await _bankContext.SaveChangesAsync();

                await dbTransaction.CommitAsync();

                return tran;
            }
            
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                throw new Exception("Transaction failed: " + ex.Message);
            }
        }
    }


}