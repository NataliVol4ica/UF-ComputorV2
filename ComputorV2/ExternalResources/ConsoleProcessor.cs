using System;

namespace ComputorV2.ExternalConnections
{
    public class ConsoleProcessor : IConsoleProcessor
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }

        public void Write(string str)
        {
            Console.Write(str);
        }

        public void WriteLine(string str)
        {
            Console.WriteLine(str);
        }
    }
}
