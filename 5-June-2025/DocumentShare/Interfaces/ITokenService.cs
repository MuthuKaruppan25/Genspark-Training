using DocumentShare.Models;

namespace DocumentShare.Interfaces;

public interface ITokenService
{
    public Task<string> GenerateToken(User user);
}