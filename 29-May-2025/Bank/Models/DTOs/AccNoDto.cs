
namespace BankApi.Model.Dtos;
public class AccnoDto
{
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? FromAccountId { get; set; }

}