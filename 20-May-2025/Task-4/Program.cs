using System;
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
    static void Main(string[] args)
    {
        Dictionary<int, Employee> employees = new Dictionary<int, Employee>();
        Console.WriteLine("Welcome to Employee Management System!");
        int choice;
        while (true)
        {
            Console.WriteLine("\n--- Employee Management ---");
            Console.WriteLine("1. Add Employee");
            Console.WriteLine("2. Display Employees");
            Console.WriteLine("3. Modify the details of the employee by Id");
            Console.WriteLine("4. Find Employee By Id");
            Console.WriteLine("5. Delete the employee by Id");
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
                    ModifyEmployeeById(employees);
                    break;
                case 4:
                    FindEmployeeById(employees);
                    break;
                case 5:
                    DeleteEmployeeById(employees);
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

    static void ModifyEmployeeById(Dictionary<int, Employee> employees)
    {
        Console.WriteLine("Enter the id of the employee to update: ");
        int id = getUsersInput();
       
        if (!employees.TryGetValue(id,out var employee))
        {
            Console.WriteLine("Employee not found.");
            return;
        }
        Console.WriteLine($"Current details: {employee}");

        Console.WriteLine("Update values of the Employee based on the choice (use comma between the choices to update multiple values):");
        Console.WriteLine("1. Name");
        Console.WriteLine("2. Age");
        Console.WriteLine("3. Salary");
        string? name = employee!.Name;
        int age = employee!.Age;
        int salary = employee!.Salary;
        string[] choices = getChoice();
        foreach (string choice in choices)
        {
            switch (choice.Trim())
            {
                case "1":
                    Console.Write("Enter new employee name: ");
                    name = GetEmployeeName();
                    break;
                case "2":
                    Console.Write("Enter new employee age: ");
                    age = getUsersInput();
                    break;
                case "3":
                    Console.Write("Enter new employee salary: ");
                    salary = getUsersInput();
                    break;
                default:
                    Console.WriteLine("Invalid! Enter valid Choice");
                    ModifyEmployeeById(employees);
                    return;
            }
            Console.WriteLine();
        }
        Employee updatedEmployee = new Employee(id, age, name, salary);
        employees[id] = updatedEmployee;
        Console.WriteLine("Employee details updated successfully!");
    }
    static void FindEmployeeById(Dictionary<int, Employee> employees)
    {
        Console.WriteLine("Enter the user id to find:");
        int id = getUsersInput();
        if (employees.TryGetValue(id,out var employee))
        {
            Console.WriteLine($"Employee found: {employee}");
        }
        else
        {
            Console.WriteLine("Employee not found.");
        }
    }

    static void DeleteEmployeeById(Dictionary<int, Employee> employees)
    {
        Console.WriteLine("Enter the id of the emploeyee to Delete: ");
        int id = getUsersInput();
        if (employees.Remove(id))
        {
            Console.WriteLine("Employee is removed successfully");
        }
        else
        {
            Console.WriteLine("Employee not Found");
        }
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
    static string[] getChoice()
    {
        Console.WriteLine("Enter the choice(s) of the value to update: ");
        string? choice = Console.ReadLine();
        string[] choices = choice?.Split(',') ?? Array.Empty<string>();
        return choices;
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