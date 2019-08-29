using System;

namespace ComputorV2
{
    public class ConsoleReader : IConsoleReader
    {
        public string ReadLine()
        {
            return Console.ReadLine();
        }
    }
}
