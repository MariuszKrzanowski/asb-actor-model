// See https://aka.ms/new-console-template for more information

using System;
using System.Threading.Tasks;

public static class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Hello, World!");

        await Task.Yield();
    }
}

