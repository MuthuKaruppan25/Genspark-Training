using System;

class Program
{
    static void Main(string[] args)
    {
        int size = getArraySize();
        int[] arr = new int[size];
        getNumbers(size,arr);

        Console.WriteLine("\nOriginal Array:");
        printArray(arr);

        rotateLeftByOne(arr);

        Console.WriteLine("\nArray after left rotation by one:");
        printArray(arr);
    }

    static int getArraySize()
    {
        Console.Write("Enter the size of the array: ");
        int size;
        bool isValid = int.TryParse(Console.ReadLine(), out size);
        if (!isValid || size <= 0)
        {
            Console.WriteLine("Invalid input. Please enter a positive number.");
            return getArraySize();
        }
        return size;
    }

    static void getNumbers(int size,int[] numbers)
    {
        
        for (int i = 0; i < size; i++)
        {
            Console.Write($"Enter element {i + 1}: ");
            bool isValid = int.TryParse(Console.ReadLine(), out numbers[i]);
            if (!isValid)
            {
                Console.WriteLine("Invalid input. Try again.");
                i--;
            }
        }
        
    }

    static void printArray(int[] arr)
    {
        foreach (int num in arr)
        {
            Console.Write(num + " ");
        }
        Console.WriteLine();
    }

    static void rotateLeftByOne(int[] arr)
    {
        if (arr.Length == 0) return;

        int first = arr[0];
        for (int i = 0; i < arr.Length - 1; i++)
        {
            arr[i] = arr[i + 1];
        }
        arr[arr.Length - 1] = first;
    }
}
