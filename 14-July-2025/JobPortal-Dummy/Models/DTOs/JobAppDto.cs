public class JobApplicationDto
{
    [TextValidator]
    public string JobTitle { get; set; } = string.Empty;
    [TextValidator]
    public string JobDescription { get; set; } = string.Empty;

    public string EmploymentType { get; set; } = string.Empty;



    public string companyName { get; set; } = string.Empty;

    public int Minsalary { get; set; }
    public int Maxsalary { get; set; }
    public DateTime AppliedOn { get; set; }
    public string Status { get; set; } = string.Empty;
}