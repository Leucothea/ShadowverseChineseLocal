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
            var write = new ResourceWriter("Resource1.resources");
            var jsonfolder = new DirectoryInfo($@"..\..\Completed\json_{args[0]}\");
            var masterfolder = new DirectoryInfo($@"..\..\Completed\master_{args[0]}\");
            var scenariofolder = new DirectoryInfo($@"..\..\Completed\scenario_{args[0]}\");
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
            var write2 = new ResourceWriter("Resource2.resources");
            foreach (var file in scenariofolder.GetFiles())
            {
                write2.AddResource(Path.GetFileNameWithoutExtension(file.FullName), File.ReadAllText(file.FullName));
            }
            write2.Generate();
            write2.Close();
        }
    }
}
