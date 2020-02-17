namespace ComputorV2.ExternalConnections
{
    public interface IConsoleProcessor
    {
        string ReadLine();
        void Write(string str);
        void WriteLine(string str);
    }
}