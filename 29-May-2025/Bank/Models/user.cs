namespace BankApi.Models;

public class User
{
    public string CustomerId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string PANNumber { get; set; } = string.Empty;
    public DateOnly DOB { get; set; }
    public int doorNo { get; set; }
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public ICollection<Account>? Accounts { get; set; }

}