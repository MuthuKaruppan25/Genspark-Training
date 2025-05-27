namespace Twitterapi.Models;

public class Post
{
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public User? Author { get; set; }
    public ICollection<Likes>? Likes { get; set; }
    public ICollection<PostHashtag>? PostHashtags { get; set; }

}