using System;
using System.Collections.Generic;
using System.Linq;
class Employee
{
    private int _id, _age, _salary;
    private string? _name;

    public Employee(int id, int age, string? name, int salary)
    {
        _id = id;
        _age = age;
        _name = name;
        _salary = salary;
    }

    public int Id { get => _id; set => _id = value; }
    public int Age { get => _age; set => _age = value; }
    public string? Name { get => _name; set => _name = value; }
    public int Salary { get => _salary; set => _salary = value; }

    public override string ToString()
    {
        return $"ID: {_id}, Name: {_name}, Age: {_age}, Salary: {_salary}";
    }
}

class EmployeePromotion
{
    static void Main(string[] args)
    {
        List<Employee> employees = new List<Employee>();
        Console.WriteLine("Welcome to Employee Management System!");
        int choice;
        while (true)
        {
            Console.WriteLine("\n--- Employee Management ---");
            Console.WriteLine("1. Add Employee");
            Console.WriteLine("2. Display Employees");
            Console.WriteLine("3. Find Index by Name");
            Console.WriteLine("4. Optimize Memory");
            Console.WriteLine("5. Sort Employees by Name");
            Console.WriteLine("6. Exit");
            Console.Write("Enter your choice: ");
            choice = getUsersInput();

            switch (choice)
            {
                case 1:
                    AddEmployee(employees);
                    break;
                case 2:
                    DisplayEmployees(employees);
                    break;
                case 3:
                    FindIndexByName(employees);
                    break;
                case 4:
                    OptimizeMemory(employees);
                    break;
                case 5:
                    SortEmployeesByName(employees);
                    break;
                case 6:
                    Console.WriteLine("Exiting...");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please choose from 1 to 4.");
                    break;
            }
        } 
    }

    static void AddEmployee(List<Employee> employees)
    {
        int id = employees.Count + 1;
        Console.Write("Enter employee name: ");
        string? name = GetEmployeeName();

        Console.Write("Enter employee age: ");
        int age = getUsersInput();

        Console.Write("Enter employee salary: ");
        int salary = getUsersInput();

        Employee employee = new Employee(id, age, name, salary);
        employees.Add(employee);

        Console.WriteLine("Employee added successfully.");
    }

    static void DisplayEmployees(List<Employee> employees)
    {
        if (employees.Count == 0)
        {
            Console.WriteLine("No employees to display.");
            return;
        }

        Console.WriteLine("\nEmployee List:");
        int idx = 1;
        foreach (Employee emp in employees)
        {
            Console.WriteLine($"{idx++}. {emp.Name}");
        }
    }

    static void FindIndexByName(List<Employee> employees)
    {
        if (employees.Count == 0)
        {
            Console.WriteLine("Employee list is empty.");
            return;
        }

        Console.Write("Enter the employee name to search: ");
        string? nameToFind = GetEmployeeName();

        int index = employees.FindIndex(e => e.Name!.Equals(nameToFind, StringComparison.OrdinalIgnoreCase));
        if (index != -1)
        {
            Console.WriteLine($"Employee '{nameToFind}' found at index {++index}");
        }
        else
        {
            Console.WriteLine($"Employee '{nameToFind}' not found.");
        }
    }

    static void OptimizeMemory(List<Employee> employees)
    {
        if (employees.Count == 0)
        {
            Console.WriteLine("No employees to optimize.");
            return;
        }
        Console.WriteLine("Capacity before optimization: " + employees.Capacity);
        Console.WriteLine("Count before optimization: " + employees.Count);
        employees.TrimExcess();
        Console.WriteLine("Capacity After optimization: " + employees.Capacity);
        Console.WriteLine("Memory optimized successfully.");
    }
    static void SortEmployeesByName(List<Employee> employees)
    {
        if(employees.Count == 0)
        {
            Console.WriteLine("No employees to sort.");
            return;
        }
        var sortedEmployees = employees.OrderBy(employees => employees.Name).ToList();
        DisplayEmployees(sortedEmployees);
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
        while (!int.TryParse(Console.ReadLine(), out input) || input <= 0)
        {
            Console.Write("Invalid input. Please enter a valid number: ");
        }
        return input;
    }
}
