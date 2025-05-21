using System;
using System.Collections.Generic;
using WholeApplication.Models;
using WholeApplication.Services;
using WholeApplication.Repositories;
using WholeApplication.Interfaces;

namespace WholeApplication
{
    class Program
    {
        static IEmployeeService employeeService = new EmployeeService(new EmployeeRepository());
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Employee Management System!");
            int choice;
            while (true)
            {
                Console.WriteLine("\n--- Employee Management ---");
                Console.WriteLine("1. Add Employee");
                Console.WriteLine("2. Search Employee by Name");
                Console.WriteLine("3. Search Employee by Id");
                Console.WriteLine("4. Search Employee by Age");
                Console.WriteLine("5. Search Employee by Salary");
                Console.WriteLine("6. Search Employees who are within than given Range of Age");
                Console.WriteLine("7. Search Employees who earns within the given Range of Salary");
                Console.WriteLine("8. Exit");
                choice = getUsersInput();
                switch (choice)
                {
                    case 1:
                        AddEmployee();
                        break;
                    case 2:
                        SearchByName();
                        break;
                    case 3:
                        SearchById();
                        break;
                    case 4:
                        SearchByAge();
                        break;
                    case 5:
                        SearchBySalary();
                        break;
                    case 6:
                        SearchByRangeAge();
                        break;
                    case 7:
                        SearchByRangeSalary();
                        break;
                    case 8:
                        Console.WriteLine("Exiting...");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please choose from 1 to 6.");
                        break;
                }
            }
        }

        static void AddEmployee()
        {
            Console.Write("Enter Name: ");
            string? name = GetEmployeeName();

            Console.Write("Enter Age: ");
            int age = getUsersInput();

            Console.Write("Enter Salary: ");
            double salary = GetDoubleInput();

            var employee = new Employee
            {
                Name = name!,
                Age = age,
                Salary = salary
            };

            int id = employeeService.AddEmployee(employee);
            if (id != -1)
                Console.WriteLine($"Employee added successfully with ID: {id}");
            else
                Console.WriteLine("Failed to add employee.");
        }

        static void SearchByName()
        {
            Console.Write("Enter name to search: ");
            string? name = GetEmployeeName();

            var result = employeeService.SearchEmployee(new SearchModel { Name = name });
            DisplayEmployees(result);
        }

        static void SearchById()
        {
            Console.Write("Enter ID to search: ");
            int id = getUsersInput();

            var result = employeeService.SearchEmployee(new SearchModel { Id = id });
            DisplayEmployees(result);
        }

        static void SearchByAge()
        {
            Console.Write("Enter minimum age: ");
            int minAge = getUsersInput();

            Console.Write("Enter maximum age: ");
            int maxAge = getUsersInput();

            var ageRange = new Range<int> { MinVal = minAge, MaxVal = maxAge };
            var result = employeeService.SearchEmployee(new SearchModel { Age = ageRange });
            DisplayEmployees(result);
        }

        static void SearchBySalary()
        {
            Console.Write("Enter minimum salary: ");
            double minSalary = GetDoubleInput();

            Console.Write("Enter maximum salary: ");
            double maxSalary = GetDoubleInput();

            var salaryRange = new Range<double> { MinVal = minSalary, MaxVal = maxSalary };
            var result = employeeService.SearchEmployee(new SearchModel { Salary = salaryRange });
            DisplayEmployees(result);
        }
 

        static void DisplayEmployees(List<Employee>? employees)
        {
            if (employees == null || employees.Count == 0)
            {
                Console.WriteLine("No employees found.");
                return;
            }

            Console.WriteLine("\n--- Employee List ---");
            foreach (var emp in employees)
            {
                Console.WriteLine($"ID: {emp.Id}, Name: {emp.Name}, Age: {emp.Age}, Salary: {emp.Salary}");
            }
        }
        static void SearchByRangeAge()
        {
            Console.Write("Enter minimum age: ");
            int minAge = getUsersInput();

            Console.Write("Enter maximum age: ");
            int maxAge = getUsersInput();

            var ageRange = new Range<int> { MinVal = minAge, MaxVal = maxAge };
            var result = employeeService.SearchEmployee(new SearchModel { Age = ageRange });
            DisplayEmployees(result);
        }
        static void SearchByRangeSalary()
        {
            Console.Write("Enter minimum salary: ");
            double minSalary = GetDoubleInput();

            Console.Write("Enter maximum salary: ");
            double maxSalary = GetDoubleInput();

            var salaryRange = new Range<double> { MinVal = minSalary, MaxVal = maxSalary };
            var result = employeeService.SearchEmployee(new SearchModel { Salary = salaryRange });
            DisplayEmployees(result);
        }
        static string? GetEmployeeName()
        {
            string? name = Console.ReadLine()?.Trim();
            while (string.IsNullOrWhiteSpace(name) || !IsValidName(name))
            {
                Console.WriteLine("Invalid name. Please enter a valid name:");
                name = Console.ReadLine()?.Trim();
            }
            return name;
        }

        static bool IsValidName(string name)
        {
            foreach (char c in name)
            {
                if (!char.IsLetter(c) && c != ' ')
                    return false;
            }
            return true;
        }

        static int getUsersInput()
        {
            int input;
            while (!int.TryParse(Console.ReadLine(), out input) || input < 0)
            {
                Console.Write("Invalid input. Please enter a valid number: ");
            }
            return input;
        }

        static double GetDoubleInput()
        {
            double input;
            while (!double.TryParse(Console.ReadLine(), out input) || input < 0)
            {
                Console.Write("Invalid input. Please enter a valid number: ");
            }
            return input;
        }
    }
}
