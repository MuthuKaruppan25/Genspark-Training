namespace SecondWebApi.Interfaces;

public interface ITokenService
{
    public Task<string> GenerateToken(User user);
}