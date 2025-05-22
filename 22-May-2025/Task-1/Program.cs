using System;

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

class Program
{
    static List<Employee> employees = new List<Employee>()
        {
            new Employee(101,30, "John Doe",  50000),
            new Employee(102, 25,"Jane Smith",  60000),
            new Employee(103,35, "Sam Brown",  70000)
        };
    public delegate void MyDelegate(int num1, int num2);

    public void Add(int n1, int n2)
    {
        int sum = n1 + n2;
        Console.WriteLine($"The sum of {n1} and {n2} is {sum}");

    }
    public int Modulus(int a, int b)
    {
        return a % b;
    }
    public bool CheckEqual(int a)
    {
        return a%2==0;
    }
    public void Product(int n1, int n2)
    {
        int prod = n1 * n2;
        Console.WriteLine($"The product of {n1} and {n2} is {prod}");
    }
    public void FindEmployee()
    {
        int empId = 103;
        Predicate<Employee> predicate = e => e.Id == empId;
        Employee? emp = employees.Find(predicate);
        Console.WriteLine(emp.ToString() ?? "No Employees Found");
    }
    void SortEmployee()
    {
        Func<Employee, string> sortByName = e => e.Name;
        var sortedEmployees = employees.OrderBy(sortByName);
        foreach (var emp in sortedEmployees)
        {
            Console.WriteLine(emp.ToString());
        }
    }
    Program()
    {
        // MyDelegate del = new MyDelegate(Add);
        Action<int, int> del = Add;
        Func<int, int, int> func = Modulus;
        Predicate<int> predicate = CheckEqual;
        Console.WriteLine(func(10, 20));
        Console.WriteLine(predicate(10));
        del += Product;
        del += (int a, int b) => Console.WriteLine($"The Division of a and b is {a / b}");
        del(10, 20);
        FindEmployee();
        SortEmployee();
    }

    static void Main(string[] args)
    {
        new Program();
    }
}