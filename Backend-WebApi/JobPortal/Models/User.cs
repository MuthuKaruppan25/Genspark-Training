namespace JobPortal.Models;

public class User
{
    public Guid guid { get; set; } = Guid.NewGuid();
    public string Username { get; set; } = string.Empty;
    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
    public byte[] HashKey { get; set; } = Array.Empty<byte>();
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiryTime { get; set; } 
    public string Role { get; set; } = string.Empty;
    public bool IsDeleted { get; set; } = false;
    public Recruiter? recruiter { get; set; }
    public Seeker? seeker { get; set; }
}