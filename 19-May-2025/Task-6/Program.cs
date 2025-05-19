using System;

class Program
{
    static void Main(string[] args)
    {
        int size = getArraySize();
        int[] numbers = new int[size];
        Console.WriteLine("Enter the numbers:");
        getNumbers(size, numbers);
        Console.WriteLine("The numbers you entered are:");
        printNumbers(numbers);
        Dictionary<int, int> frequency = new Dictionary<int, int>();
        frequency = countFrequency(numbers);
        DisplayFrequency(frequency);
    }
    static void DisplayFrequency(Dictionary<int, int> frequency)
    {
        Console.WriteLine("Number Frequency:");
        foreach (var kvp in frequency)
        {
            Console.WriteLine($"Number: {kvp.Key}, Frequency: {kvp.Value}");
        }
    }
    static Dictionary<int,int> countFrequency(int[] numbers)
    {
        Dictionary<int, int> frequency = new Dictionary<int, int>();
        foreach(int num in numbers)
        {
            if(frequency.ContainsKey(num))
            {
                frequency[num]++;
            }
            else
            {
                frequency[num] = 1;
            }
        }
        return frequency;
    }
    static void printNumbers(int[] numbers)
    {
        foreach (int number in numbers)
        {
            Console.Write(number + " ");
        }
        Console.WriteLine();
    }

    static void getNumbers(int size,int[] numbers)
    {
        
        for (int i = 0; i < size; i++)
        {
            Console.Write($"Enter number {i + 1}: ");
            bool isValid = int.TryParse(Console.ReadLine(), out numbers[i]);
            if (!isValid)
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
                i--; 
            }
        }
        
    }
    static int getArraySize()
    {
        Console.Write("Enter the size of the array: ");
        int size;
        bool isValid = int.TryParse(Console.ReadLine(), out size);
        if (!isValid || size <= 0)
        {
            Console.WriteLine("Invalid input. Please enter a positive integer.");
            return getArraySize();
        }
        return size;
    }
}