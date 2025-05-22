public class PercentageGrade : IGradingStrategy
{
    public double Calculate(Dictionary<string, int> marks)
    {
        double total = marks.Values.Sum();
        double percentage = total / marks.Count;
        Console.WriteLine($"Percentage: {percentage:F2}%");
        Console.WriteLine($"Letter Grade: {GetLetterGrade(percentage)}");
        return percentage;
    }

    private string GetLetterGrade(double percentage)
    {
        if (percentage >= 90) return "A";
        if (percentage >= 80) return "B";
        if (percentage >= 70) return "C";
        if (percentage >= 60) return "D";
        return "F";
    }
}