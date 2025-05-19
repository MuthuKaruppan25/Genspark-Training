using System;

class Program
{
    static void Main(string[] args)
    {
        string name = GetUserName();
        GreetUser(name);
    }

    
    static string GetUserName()
    {
        Console.Write("Enter your name: ");
        string? input = Console.ReadLine();

        while (string.IsNullOrWhiteSpace(input))
        {
            Console.Write("Name cannot be empty. Please enter your name: ");
            input = Console.ReadLine();
        }

        return input;
    }

    static void GreetUser(string name)
    {
        Console.WriteLine("Hello, " + name + "!");
    }
}
