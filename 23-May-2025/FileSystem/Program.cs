using System;
using System.Collections.Generic;
using FileAccess.Interfaces;
using FileAccess.Models;
using FileAccess.Classes;

class Program
{
    static List<User> users = new List<User>();
    static void Main(string[] args)
    {
        while (true)
        {
            Console.WriteLine("\n--- Secure File Access System ---");
            Console.WriteLine("1. Add User");
            Console.WriteLine("2. Read File");
            Console.WriteLine("3. Exit");
            Console.Write("Enter your choice: ");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    AddUser();
                    break;

                case "2":
                    ReadFile();
                    break;

                case "3":
                    Console.WriteLine("Exiting...");
                    return;

                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
    }

    static void AddUser()
    {
        Console.Write("Enter username: ");
        string username = Console.ReadLine();

        Console.Write("Enter role (Admin/User/Guest): ");
        string role = Console.ReadLine();

        users.Add(new User{username = username,role= role});
        Console.WriteLine($"User '{username}' with role '{role}' added.");
    }

    static void ReadFile()
    {
        if (users.Count == 0)
        {
            Console.WriteLine("No users available. Please add a user first.");
            return;
        }

        Console.WriteLine("Select a user to perform file read:");
        for (int i = 0; i < users.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {users[i].username} ({users[i].role})");
        }

        Console.Write("Enter user number: ");
        if (int.TryParse(Console.ReadLine(), out int userIndex) && userIndex >= 1 && userIndex <= users.Count)
        {
            User currentUser = users[userIndex - 1];

            Console.Write("Enter filename to read: ");
            string filename = Console.ReadLine();

            IFile file = new ProxyFile(filename, currentUser);
            file.Read();
        }
        else
        {
            Console.WriteLine("Invalid user selection.");
        }
    }
}
