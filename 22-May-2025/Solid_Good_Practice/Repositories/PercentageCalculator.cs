public class PercentageCalculator : IReportCardCalculator
{
    public string MetricName => "Percentage";

    public string Calculate(ReportCard reportCard)
    {
        int total = 0;
        foreach (var mark in reportCard.Marks)
            total += mark.Mark;
        double percent = total / (reportCard.Marks.Count * 100.0) * 100;
        return percent.ToString("0.00") + "%";
    }
}