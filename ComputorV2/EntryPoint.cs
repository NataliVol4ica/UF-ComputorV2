using ComputorV2.ExternalConnections;

namespace ComputorV2
{
    internal class EntryPoint
    {
        private static void Main()
        {
            new Computor(new ConsoleProcessor()).StartReading();
        }
    }
}