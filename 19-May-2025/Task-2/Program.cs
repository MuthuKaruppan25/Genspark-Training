using System;

class Program
{
    static void printLargest(int a, int b)
    {
        if (a > b)
        {
            Console.WriteLine($"The largest number is: {a}");
        }
        else if(b > a)
        {
            Console.WriteLine($"The largest number is: {b}");
        }
        else
        {
            Console.WriteLine("Both numbers are equal.");
        }
    }
    static void Main(string[] args)
    {
        Console.Write("Enter the first number: ");
        int a = Convert.ToInt32(Console.ReadLine());
        Console.Write("Enter the second number: ");
        int b = Convert.ToInt32(Console.ReadLine());
        printLargest(a, b);
    }
}