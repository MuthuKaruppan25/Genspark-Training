
namespace Twitterapi.Models;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public string? Website { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Post>? Posts { get; set; }
    public ICollection<Likes>? Likes { get; set; }
    public ICollection<UserFollow>? Followers { get; set; }
    public ICollection<UserFollow>? Following { get; set; }
}