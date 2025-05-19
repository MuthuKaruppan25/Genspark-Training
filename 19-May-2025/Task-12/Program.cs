using System;
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter the string to encrypt:");
        string? input = getStrInput();
        Console.WriteLine("This is the String you entered: " + input);
        string encryptedString = EncryptString(input!);
        Console.WriteLine("Encrypted String: " + encryptedString);
        string decryptedString = DecryptString(encryptedString);
        Console.WriteLine("Decrypted String: " + decryptedString);
    }
    static string EncryptString(string input)
    {
        char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        char[] charArray = input.ToCharArray();
        for(int i=0;i<charArray.Length;i++)
        {
            int idx = Array.IndexOf(alphabet, charArray[i]);
            charArray[i] = alphabet[(idx + 3) % alphabet.Length];

        }
        return new string(charArray);
    }
    static string DecryptString(string input)
    {
        char[] alphabet = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        char[] charArray = input.ToCharArray();
        for(int i=0;i<charArray.Length;i++)
        {
            int idx = Array.IndexOf(alphabet, charArray[i]);
            charArray[i] = alphabet[(idx - 3 + alphabet.Length) % alphabet.Length];
        }
        return new string(charArray);
    }

    static string? getStrInput()
    {
        string? input = Console.ReadLine()!.Trim();
        
        if (string.IsNullOrEmpty(input) || !isValidInput(input!))
        {
            Console.WriteLine("Invalid input. Please enter a valid string.");
            return getStrInput();
        }
        return input.ToLower();
    }
    static bool isValidInput(string input)
    {
        foreach (char c in input)
        {
            if (!char.IsLetter(c))
            {
                return false;
            }
        }
        return true;
    }
}