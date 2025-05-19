using System;

class Program
{
    public static void Main(string[] args)
    {
        int size1 = getArraySize();
        int[] numbers1 = new int[size1];
        Console.WriteLine("Enter the numbers of array 1:");
        getNumbers(size1, numbers1);
        Console.WriteLine("The numbers you entered are:");
        printNumbers(numbers1);

        int size2 = getArraySize();
        int[] numbers2 = new int[size2];
        Console.WriteLine("Enter the numbers of array 2:");
        getNumbers(size2, numbers2);
        Console.WriteLine("The numbers you entered are:");
        printNumbers(numbers2);

        int[] mergedArray = new int[size1 + size2];
        mergeArrays(numbers1, numbers2, mergedArray);
        Console.WriteLine("The merged array is:");
        printNumbers(mergedArray);
    }

    static void mergeArrays(int[] arr1, int[] arr2, int[] mergedArray)
    {
        for (int i = 0; i < arr1.Length; i++)
        {
            mergedArray[i] = arr1[i];
        }
        for (int i = 0; i < arr2.Length; i++)
        {
            mergedArray[arr1.Length + i] = arr2[i];
        }
    }

    static void printNumbers(int[] numbers)
    {
        foreach (int number in numbers)
        {
            Console.Write(number + " ");
        }
        Console.WriteLine();
    }

    static void getNumbers(int size, int[] numbers)
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
            return getArraySize(); // recursive retry
        }
        return size;
    }
}
