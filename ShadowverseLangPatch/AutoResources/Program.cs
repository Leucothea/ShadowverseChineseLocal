using System;
using System.Collections.Generic;
using System.IO;
using System.Resources;
using System.Text;

namespace AutoResources
{
    class Program
    {
        static void Main(string[] args)
        {
            var write = new ResourceWriter($@"{args[0]}Resource1.resources");
            var jsonfolder = new DirectoryInfo($@"{args[0]}..\..\Completed\json_{args[1]}\");
            var masterfolder = new DirectoryInfo($@"{args[0]}..\..\Completed\master_{args[1]}\");
            var scenariofolder = new DirectoryInfo($@"{args[0]}..\..\Completed\scenario_{args[1]}\");
            foreach (var file in jsonfolder.GetFiles())
            {
                write.AddResource(Path.GetFileNameWithoutExtension(file.FullName), File.ReadAllText(file.FullName));
            }
            foreach (var file in masterfolder.GetFiles())
            {
                write.AddResource(Path.GetFileNameWithoutExtension(file.FullName), File.ReadAllText(file.FullName));
            }
            write.Generate();
            write.Close();
            var write2 = new ResourceWriter($@"{args[0]}Resource2.resources");
            foreach (var file in scenariofolder.GetFiles())
            {
                write2.AddResource(Path.GetFileNameWithoutExtension(file.FullName), File.ReadAllText(file.FullName));
            }
            write2.Generate();
            write2.Close();
            Console.WriteLine("AutoResources Done.");
        }
    }
}
