using System;
using System.IO;
using System.Threading.Tasks;

internal class TrivialAsyncDemo
{
    private static async Task Main()
    {
        string s = await File.ReadAllTextAsync(@"C:\Users\maxfire\.gitconfig");
        Console.Write(s);
    }
}
