//Bad Practice

using System;
using System.Collections.Generic;
using System.Linq;

public class ReportCardWrong
{
    private string _studentName;
    private Dictionary<string, int> _marks;
    private string _gradingMethod;

    public ReportCardWrong(string studentName, Dictionary<string, int> marks, string gradingMethod)
    {
        _studentName = studentName;
        _marks = marks;
        _gradingMethod = gradingMethod;
    }

    public void Print()
    {
        Console.WriteLine($"Report Card for {_studentName}");

        if (_gradingMethod == "GradePointsAverage")
        {
            double totalPoints = 0;
            foreach (var mark in _marks.Values)
            {
                totalPoints += GetGradePoint(mark);
            }
            double gpa = totalPoints / _marks.Count;
            Console.WriteLine($"Grade Points Average: {gpa:F2}");
        }
        else if (_gradingMethod == "Percentage")
        {
            double total = _marks.Values.Sum();
            double percentage = total / _marks.Count;
            Console.WriteLine($"Percentage: {percentage:F2}%");
            Console.WriteLine($"Letter Grade: {GetLetterGrade(percentage)}");
        }
        else if (_gradingMethod == "PassFail")
        {
            bool pass = true;
            foreach (var mark in _marks.Values)
            {
                if (mark < 35)
                {
                    pass = false;
                    break;
                }
            }
            Console.WriteLine($"Result: {(pass ? "Pass" : "Fail")}");
        }
        else
        {
            Console.WriteLine("Invalid grading method.");
        }
    }

    private double GetGradePoint(int mark)
    {
        if (mark >= 90) return 4.0;
        if (mark >= 80) return 3.0;
        if (mark >= 70) return 2.0;
        if (mark >= 60) return 1.0;
        return 0.0;
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


// Good Practice

public class Program
{
    public static void Main()
    {
        var marks = new Dictionary<string, int>
        {
            {"Math", 92},
            {"English", 81},
            {"Science", 78},
            {"History", 69},
            {"Computer", 88}
        };

        // Wrong approach
        Console.WriteLine("Wrong Practice:");
        var reportWrong = new ReportCardWrong("Alice", marks, "Percentage");
        reportWrong.Print();

        // Correct approach
        Console.WriteLine("\nCorrect Practice:");
        
        var reportGPA = new ReportCard("Bob", marks, new GradePointsAverage());
        reportGPA.Print();
        
        var reportPercentage = new ReportCard("Charlie", marks, new PercentageGrade());
        reportPercentage.Print();
        
        var reportPassFail = new ReportCard("Diana", marks, new PassFailEvaluation());
        reportPassFail.Print();
    }
}
