public class CompanyUpdateDto
{
    [TextValidator]
    public string CompanyName { get; set; } = string.Empty;
    [TextValidator]
    public string Description { get; set; } = string.Empty;
    [UrlValidator]
    public string WebsiteUrl { get; set; } = string.Empty;
}