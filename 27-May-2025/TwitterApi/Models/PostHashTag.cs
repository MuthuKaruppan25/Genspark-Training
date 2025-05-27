namespace Twitterapi.Models;

public class PostHashtag
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public int HashtagId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Post? Post { get; set; }
    public Hashtag? Hashtag { get; set; }
}