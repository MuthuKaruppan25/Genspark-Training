public class SeekerEducationUpdateDto
{
    public ICollection<EducationDetais>? educationDetais {get;set;}
}

public class EducationDetais
{
        public string courseName { get; set; } = string.Empty;
    public string collegeName { get; set; } = string.Empty;
    public double grade { get; set; }
}