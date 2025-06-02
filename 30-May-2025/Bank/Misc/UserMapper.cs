using BankApi.Model.Dtos;
using BankApi.Models;
using System;
using System.Security.Cryptography;

public class UserMapper
{
    private static readonly Random _random = new();

    public User? MapUserAddRequest(AccountCreateDto dto)
    {
        string generatedCustomerId = GenerateCustomerId();
        string generatedPassword = GeneratePassword();

        return new User
        {
            CustomerId = generatedCustomerId,
            Name = dto.Name,
            Password = generatedPassword,
            PhoneNumber = dto.PhoneNumber,
            PANNumber = dto.PANNumber,
            DOB = dto.DOB,
            doorNo = dto.doorNo,
            Street = dto.Street,
            City = dto.City,
            State = dto.State
        };
    }

    public static string GenerateCustomerId()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var idChars = new char[8];
        var random = new Random();
        for (int i = 0; i < 8; i++)
        {
            idChars[i] = chars[random.Next(chars.Length)];
        }
        return new string(idChars);
    }

    public string GeneratePassword(int length = 10)
    {
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string digits = "0123456789";
        const string special = "!@#$%^&*";
        string allChars = lower + upper + digits + special;

        var password = new char[length];
        using var rng = RandomNumberGenerator.Create();

        password[0] = lower[_random.Next(lower.Length)];
        password[1] = upper[_random.Next(upper.Length)];
        password[2] = digits[_random.Next(digits.Length)];
        password[3] = special[_random.Next(special.Length)];

        for (int i = 4; i < length; i++)
        {
            byte[] randomByte = new byte[1];
            rng.GetBytes(randomByte);
            password[i] = allChars[randomByte[0] % allChars.Length];
        }


        for (int i = password.Length - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (password[i], password[j]) = (password[j], password[i]);
        }

        return new string(password);
    }
}
