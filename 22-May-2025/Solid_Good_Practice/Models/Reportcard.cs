public class ReportCard
{
    public string StudentName { get; }
    public List<SubjectMark> Marks { get; }

    public ReportCard(string studentName, List<SubjectMark> marks)
    {
        StudentName = studentName;
        Marks = marks;
    }
}
