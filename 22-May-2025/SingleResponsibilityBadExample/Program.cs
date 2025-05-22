// Bad Example

public class ReportCard
{
    public string StudentName { get; set; }
    public int Maths { get; set; }
    public int English { get; set; }
    public int Science { get; set; }
    public int History { get; set; }
    public int Computer { get; set; }

    public void PrintReport()
    {
        Console.WriteLine($"Report Card for {StudentName}");
        Console.WriteLine($"Maths: {Maths}, English: {English}, Science: {Science}, History: {History}, Computer: {Computer}");
        Console.WriteLine($"Average: {(Maths + English + Science + History + Computer) / 5.0}");
    }

    public void SaveToFile()
    {
        string content = $"{StudentName},{Maths},{English},{Science},{History},{Computer}";
        File.WriteAllText($"{StudentName}_report.txt", content);
    }
}


//Good Practice

public class ReportCardCalculator
{
    public double CalculateAverage(StudentMarks marks)
    {
        return marks.Subjects.Values.Average();
    }

    public string CalculateGrade(double average)
    {
        if (average >= 90) return "A";
        else if (average >= 80) return "B";
        else if (average >= 70) return "C";
        else if (average >= 60) return "D";
        else return "F";
    }
}

public class ReportCardPrinter
{
    public void Print(StudentMarks marks, double average, string grade)
    {
        Console.WriteLine($"Report Card for {marks.StudentName}");
        foreach (var subject in marks.Subjects)
        {
            Console.WriteLine($"{subject.Key}: {subject.Value}");
        }
        Console.WriteLine($"Average: {average}");
        Console.WriteLine($"Grade: {grade}");
    }
}

public class ReportCardSaver
{
    public void SaveToFile(StudentMarks marks, double average, string grade)
    {
        var content = new StringBuilder();
        content.AppendLine($"Report Card for {marks.StudentName}");
        foreach (var subject in marks.Subjects)
        {
            content.AppendLine($"{subject.Key}: {subject.Value}");
        }
        content.AppendLine($"Average: {average}");
        content.AppendLine($"Grade: {grade}");

        File.WriteAllText($"{marks.StudentName}_ReportCard.txt", content.ToString());
    }
}
