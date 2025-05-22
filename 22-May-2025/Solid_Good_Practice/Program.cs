/* 
Single Responsibility Principle (SRP): A class should have only one reason to change, 
meaning it should do one thing only.

Open/Closed Principle (OCP): Software entities should be open for extension but closed for 
modification.

Liskov Substitution Principle (LSP): Subtypes must be substitutable for their base types 
without breaking the program.

Interface Segregation Principle (ISP): Clients shouldn't be forced to depend on interfaces 
they don't use.

Dependency Inversion Principle (DIP): High-level modules should depend on abstractions, 
not concrete implementations.
*/

public class Program
{
    public static void Main()
    {
        var marks = new List<SubjectMark>
        {
            new SubjectMark("Math", 90),
            new SubjectMark("Science", 85),
            new SubjectMark("English", 88),
            new SubjectMark("History", 75),
            new SubjectMark("Computer", 95)
        };

        var reportCard = new ReportCard("Alice", marks);

        var calculators = new List<IReportCardCalculator>
        {
            new TotalMarksCalculator(),
            new PercentageCalculator(),
            new GPACalculator()
        };

        var notifiers = new List<INotifier>
        {
            new EmailNotifier(),
            new SMSNotifier(),
            new WhatsAppNotifier()
        };

        var reportNotifier = new ReportCardNotifier(calculators, notifiers);
        reportNotifier.NotifyAll(reportCard);
    }
}