

public class User
{
    public Guid guid { get; set; } = Guid.NewGuid();
    public string name { get; set; } = string.Empty;
    public int age { get; set; }
    public string education { get; set; } = string.Empty;
}