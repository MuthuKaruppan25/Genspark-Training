public class TotalMarksCalculator : IReportCardCalculator
{
    public string MetricName => "Total Marks";

    public string Calculate(ReportCard reportCard)
    {
        int total = 0;
        foreach (var mark in reportCard.Marks)
            total += mark.Mark;
        return total.ToString();
    }
}
