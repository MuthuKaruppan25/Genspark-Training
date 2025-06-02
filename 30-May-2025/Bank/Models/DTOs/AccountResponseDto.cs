namespace BankApi.Model.Dtos;
public class AccountResponseDto
{
    public string AccountNo { get; set; } = string.Empty;
    public string CustomerId { get; set; } = string.Empty;
    public string? GeneratedPassword { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public string BranchId { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string BranchLocation { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
