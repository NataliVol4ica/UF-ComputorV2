namespace ComputorV2.ExternalResources
{
    public interface IConsoleProcessor
    {
        string ReadLine();
        void Write(string str);
        void WriteLine(string str);
    }
}