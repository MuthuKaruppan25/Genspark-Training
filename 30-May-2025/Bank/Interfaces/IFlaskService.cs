namespace BankApi.Interfaces;

public interface IFlaskService
{
    Task<string> GetAnswerAsync(string question);
    
    Task<string> RetrieveAnswerAsync(string question);
}
