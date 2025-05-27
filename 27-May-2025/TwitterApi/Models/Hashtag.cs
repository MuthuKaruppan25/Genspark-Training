namespace Twitterapi.Models;
public class Hashtag
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<Post>? Posts { get; set; }
    public ICollection<PostHashtag>? PostHashtags { get; set; }
}