public class ReportCardNotifier
{
    private readonly List<IReportCardCalculator> _calculators;
    private readonly List<INotifier> _notifiers;

    public ReportCardNotifier(List<IReportCardCalculator> calculators, List<INotifier> notifiers)
    {
        _calculators = calculators;
        _notifiers = notifiers;
    }

    public void NotifyAll(ReportCard reportCard)
    {
        var metrics = new List<string>();
        foreach (var calculator in _calculators)
            metrics.Add($"{calculator.MetricName}: {calculator.Calculate(reportCard)}");

        foreach (var notifier in _notifiers)
            notifier.Notify(reportCard.StudentName, reportCard.Marks, metrics);
    }
}