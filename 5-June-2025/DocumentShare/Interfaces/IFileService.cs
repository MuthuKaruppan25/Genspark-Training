

using System.Security.Claims;

public interface IFileService
{
    Task<string> SaveFileAsync(IFormFile file,ClaimsPrincipal user);
    Task<byte[]?> GetFileAsync(string fileName);
}