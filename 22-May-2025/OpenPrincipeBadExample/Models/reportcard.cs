public class ReportCard
{
    private string _studentName;
    private Dictionary<string, int> _marks;
    private IGradingStrategy _gradingStrategy;

    public ReportCard(string studentName, Dictionary<string, int> marks, IGradingStrategy gradingStrategy)
    {
        _studentName = studentName;
        _marks = marks;
        _gradingStrategy = gradingStrategy;
    }

    public void Print()
    {
        Console.WriteLine($"Report Card for {_studentName}");
        double result = _gradingStrategy.Calculate(_marks);
        // Depending on strategy, result may be GPA, percentage, or pass/fail status.
    }
}