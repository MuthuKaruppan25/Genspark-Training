namespace JobPortal.Models;

public class Requirements
{
    public Guid guid { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public Guid PostId { get; set; }
    public JobPost? jobPost { get; set; }

}