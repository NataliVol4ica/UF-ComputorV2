using ComputorV2.ExternalResources;

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