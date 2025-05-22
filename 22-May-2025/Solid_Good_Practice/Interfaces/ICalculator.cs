public interface IReportCardCalculator
{
    string MetricName { get; }
    string Calculate(ReportCard reportCard);
}
