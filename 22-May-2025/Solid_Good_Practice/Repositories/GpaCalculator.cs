public class GPACalculator : IReportCardCalculator
{
    public string MetricName => "GPA";

    public string Calculate(ReportCard reportCard)
    {
        double sum = 0;
        foreach (var mark in reportCard.Marks)
            sum += (mark.Mark / 10.0);
        double gpa = sum / reportCard.Marks.Count;
        return gpa.ToString("0.00");
    }
}