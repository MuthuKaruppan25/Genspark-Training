using System;
using System.Collections.Generic;
using System.Linq;
class Employee : IComparable<Employee>
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
    public int CompareTo(Employee? emp)
    {
        if (emp == null)
            return 1;
        return emp.Salary.CompareTo(this.Salary);

    }
}

class EmployeeDetails
{
    public static void Main(string[] args)
    {
        Dictionary<int, Employee> employees = new Dictionary<int, Employee>();
        Console.WriteLine("Welcome to Employee Management System!");
        int choice;
        while (true)
        {
            Console.WriteLine("\n--- Employee Management ---");
            Console.WriteLine("1. Add Employee");
            Console.WriteLine("2. Display Employees");
            Console.WriteLine("3. Sort Employees By Salary");
            Console.WriteLine("4. Find Employee By Id");
            Console.WriteLine("5. Find Employee By Name");
            Console.WriteLine("6. Find Employees who are older than the input age");
            Console.WriteLine("7. Exit");
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
                    SortEmployeesBySalary(employees);
                    break;
                case 4:
                    FindEmployeeById(employees);
                    break;
                case 5:
                    FindEmployeeByName(employees);
                    break;
                case 6:
                    FindEmployeesOlderThanInputAge(employees);
                    break;
                case 7:
                    Console.WriteLine("Exiting...");
                    return;
                default:
                    Console.WriteLine("Invalid choice. Please choose from 1 to 4.");
                    break;
            }
        }
    }
    static void AddEmployee(Dictionary<int, Employee> employees)
    {

        int id = employees.Any() ? employees.Keys.Max() + 1 : 1;
        Console.Write("Enter employee name: ");
        string? name = GetEmployeeName();

        Console.Write("Enter employee age: ");
        int age = getUsersInput();

        Console.Write("Enter employee salary: ");
        int salary = getUsersInput();

        Employee employee = new Employee(id, age, name, salary);
        employees.Add(id, employee);
        Console.WriteLine("Employee added successfully!");
    }
    static void DisplayEmployees(Dictionary<int, Employee> employees)
    {
        Console.WriteLine("Employees:");
        foreach (var employee in employees)
        {
            Console.WriteLine($" {employee.Key}. Name : {employee.Value.Name}, Age : {employee.Value.Age}, Salary : {employee.Value.Salary}");
        }
    }
    static void SortEmployeesBySalary(Dictionary<int, Employee> employees)
    {
        if (employees.Count == 0)
        {
            Console.WriteLine("No Employees to sort.");
        }
        var sortedEmployeesBySalary = employees.Values.ToList();
        sortedEmployeesBySalary.Sort();
        Console.WriteLine("Employees sorted by Salary:");
        DisplayEmployeesByList(sortedEmployeesBySalary);
    }
    static void DisplayEmployeesByList(List<Employee> employees)
    {
        foreach (var employee in employees)
        {
            Console.WriteLine($"Name : {employee.Name}, Age : {employee.Age}, Salary : {employee.Salary}");
        }
    }
    static void FindEmployeeById(Dictionary<int, Employee> employees)
    {
        Console.WriteLine("Enter the user id to find:");
        int id = getUsersInput();

        var result = employees.Where(e => e.Key == id).FirstOrDefault();

        if (result.Value != null)
        {
            Console.WriteLine($"Employee found: {result.Value}");
        }
        else
        {
            Console.WriteLine("Employee not found.");
        }
    }

    static void FindEmployeeByName(Dictionary<int, Employee> employees)
    {
        Console.WriteLine("Enter the name of the employee to find:");
        string? name = GetEmployeeName();
        var employee = employees.Where(employee => employee.Value.Name!.ToLower() == name!.ToLower()).FirstOrDefault();
        if (employee.Value == null)
        {
            Console.WriteLine("Employee not found.");
        }
        else
        {
            Console.WriteLine($"Employee found: {employee.Value}");
        }
    }
    static void FindEmployeesOlderThanInputAge(Dictionary<int, Employee> employees)
    {

        Console.WriteLine("Enter the id of the employee to compare: ");
        int id = getUsersInput();
        if (employees.TryGetValue(id, out var emp))
        {
            Console.WriteLine($"Employee found: {emp}");
        }
        else
        {
            Console.WriteLine("Employee not found.");
            return;
        }
        int age = emp!.Age;
        var employee = employees.Where(employee => employee.Value.Age > age);

        if (employee.Count() == 0)
        {
            Console.WriteLine("No Employee is older than the given employee");
        }
        else
        {
            Console.WriteLine("Employees Older than Given Employee: ");
            foreach (var e in employee)
            {
                Console.WriteLine($"Employee: {e.Value}");
            }
        }
    }

    static string? GetEmployeeName()
    {
        string? name = Console.ReadLine()?.Trim();
        while (string.IsNullOrEmpty(name) || !IsValidName(name))
        {
            Console.WriteLine("Invalid name. Please enter a valid name:");
            return GetEmployeeName();
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
            return getUsersInput();
        }
        return input;
    }


}