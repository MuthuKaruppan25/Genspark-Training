
namespace Twitterapi.Models;

public class Likes
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int PostId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public User? User { get; set; }
    public Post? Post { get; set; }
}