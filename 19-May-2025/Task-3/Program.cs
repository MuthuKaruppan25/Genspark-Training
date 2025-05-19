using System;

class Program
{
    public static void Main(string[] args) 
    {
        Console.Write("Enter the first number: "); 
        double a = Convert.ToDouble(Console.ReadLine());

        Console.Write("Enter the second number: ");
        double b = Convert.ToDouble(Console.ReadLine());

        string? op = GetOperation();

        bool success;
        double result = PerformOperation(a, b, op, out success);

        if (success)
        {
            Console.WriteLine($"The result of {a} {op} {b} = {result}");
        }
        else
        {
            Console.WriteLine("Error in operation.");
        }
    }

    static string? GetOperation()
    {
        Console.Write("Enter the operation (+, -, *, /): ");
        string? op = Console.ReadLine();
        if (op == "+" || op == "-" || op == "*" || op == "/")
        {
            return op;
        }
        else
        {
            Console.WriteLine("Invalid operation. Please enter one of +, -, *, /.");
            return GetOperation(); 
        }
    }

    static double PerformOperation(double a, double b, string? op, out bool success)
    {
        success = true;

        switch (op)
        {
            case "+":
                return a + b;
            case "-":
                return a - b;
            case "*":
                return a * b;
            case "/":
                if (b != 0)
                {
                    return a / b;
                }
                else
                {
                    Console.WriteLine("Cannot divide by zero.");
                    success = false;
                    return 0;
                }
            default:
                success = false;
                return 0;
        }
    }
}
