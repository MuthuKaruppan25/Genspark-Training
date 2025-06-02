
namespace BankApi.Model.Dtos;
public class AccountCreateDto
{
    public string Name { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PANNumber { get; set; } = string.Empty;
    public DateOnly DOB { get; set; }
    public int doorNo { get; set; }
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string AccountType { get; set; } = string.Empty;
    public decimal InitialDeposit { get; set; }
    public string BranchName { get; set; } = string.Empty;

}