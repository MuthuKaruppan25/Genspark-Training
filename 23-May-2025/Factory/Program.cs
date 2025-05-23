using System;
using Factory.Interfaces;
using Factory.Repositories;
using Factory.Services;
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Select logging method (text, log, console):");
        string inputType = Console.ReadLine()?.Trim().ToLower();

        LoggerFactory factory = inputType switch
        {
            "text" => new TextLoggerFactory(),
            "log" => new LogLoggerFactory(),
            "console" => new ConsoleLoggerFactory(),
            _ => throw new InvalidOperationException("Invalid logger type")
        };

        

        Console.WriteLine("Enter a message to log (or 'exit' to quit):");
        string message = Console.ReadLine();

        while (message.ToLower() != "exit")
        {
            factory.WriteLog(message);
            Console.WriteLine("Message logged. Enter another message (or 'exit' to quit):");
            message = Console.ReadLine();
        }

        Console.WriteLine("Reading logs...");
        Console.WriteLine(factory.ReadLog());
    }
}
