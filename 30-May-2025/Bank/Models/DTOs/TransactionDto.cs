namespace BankApi.Model.Dtos;

public class TransactionDto
{
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string FromAccountId { get; set; }= string.Empty;
    public string ToAccountId { get; set; }= string.Empty;
}