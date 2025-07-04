
namespace Twitterapi.Models;

public class UserFollow
{
    public int Id { get; set; }
    public int FollowerId { get; set; } 
    public int FollowingId { get; set; } 
    public User? Follower { get; set; }
    public User? Following { get; set; }
}