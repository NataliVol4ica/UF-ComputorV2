using ComputorV2.ExternalConnections;

namespace ComputorV2
{
    class EntryPoint
    {
        static void Main()
        {
            new Computor(new ConsoleProcessor()).StartReading();
        }
    }
}