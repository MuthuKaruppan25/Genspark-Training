public class SubjectMark
{
    public string SubjectName { get; }
    public int Mark { get; }

    public SubjectMark(string subjectName, int mark)
    {
        SubjectName = subjectName;
        Mark = mark;
    }
}