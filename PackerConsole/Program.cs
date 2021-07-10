using System;
using System.IO;
using System.Linq;
using com.mobiquity.packer;

namespace PackerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var filePath = args.Any() ? args[0] : Path.Combine(AppContext.BaseDirectory, "example_input.txt");
            var output = Packer.pack(filePath);
            Console.WriteLine(output);
        }
    }
}