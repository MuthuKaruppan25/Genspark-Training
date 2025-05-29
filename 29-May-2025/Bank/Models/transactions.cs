namespace BankApi.Models;

public class Transaction
{
    public int TransactionId { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? FromAccountId { get; set; }
    public Account? FromAccount { get; set; }
    public string? ToAccountId { get; set; }
    public Account? ToAccount { get; set; }
}
