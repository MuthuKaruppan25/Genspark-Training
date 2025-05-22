public class EmailNotifier : INotifier
{
    public void Notify(string studentName, List<SubjectMark> marks, List<string> metrics)
    {
        Console.WriteLine($"\n📧 Email sent to {studentName} with report card:");
        foreach (var mark in marks)
            Console.WriteLine($"{mark.SubjectName}: {mark.Mark}");
        foreach (var metric in metrics)
            Console.WriteLine(metric);
    }
}