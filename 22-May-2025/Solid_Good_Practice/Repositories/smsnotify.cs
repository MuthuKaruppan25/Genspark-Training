public class SMSNotifier : INotifier
{
    public void Notify(string studentName, List<SubjectMark> marks, List<string> metrics)
    {
        Console.WriteLine($"\n📱 SMS sent to {studentName} with summary:");
        foreach (var metric in metrics)
            Console.WriteLine(metric);
    }
}