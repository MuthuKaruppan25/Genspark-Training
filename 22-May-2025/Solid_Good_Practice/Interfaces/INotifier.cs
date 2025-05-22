public interface INotifier
{
    void Notify(string studentName, List<SubjectMark> marks, List<string> metrics);
}
