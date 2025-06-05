namespace DocumentShare.Models;

public class User
{
    public string Username { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
    public byte[] HashKey { get; set; } = Array.Empty<byte>();
    public string Role { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;

}