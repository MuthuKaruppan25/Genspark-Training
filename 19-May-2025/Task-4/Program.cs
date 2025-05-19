using System;

class Program
{
    public static void Main(string[] args)
    {
        int attempts = 0;
        const int maxAttempts = 3;

        while (attempts < maxAttempts)
        {
            Console.Write("Enter username: ");
            string? username = Console.ReadLine();

            Console.Write("Enter password: ");
            string? password = Console.ReadLine();

            if (Login(username, password))
            {
                Console.WriteLine("Login successful!");
                return; 
            }
            else
            {
                attempts++;
                Console.WriteLine("Invalid username or password.");
            }
        }

        Console.WriteLine("Invalid attempts for 3 times. Exiting....");
    }

    static bool Login(string? username, string? password)
    {
        return username == "Admin" && password == "pass";
    }
}
