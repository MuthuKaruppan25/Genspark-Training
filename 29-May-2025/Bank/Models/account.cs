
namespace BankApi.Models;

public class Account
{
    public string AccountNo { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public DateTime createdAt { get; set; } = DateTime.Now;
    public decimal Balance { get; set; } = 1000;
    public string CustomerId { get; set; } = string.Empty;
    public string BranchId { get; set; }
    public Branch? Branch { get; set; }
    public User? user;
    public string Status { get; set; } = "Active";
    public ICollection<Transaction>? Transactions { get; set; }


}