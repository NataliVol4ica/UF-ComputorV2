namespace PolynomialSolver.Console
{
    internal class RealConsoleWrapper : IConsole
    {
        public void Write(string s)
        {
            System.Console.Write(s);
        }

        public void WriteLine(string s)
        {
            System.Console.WriteLine(s);
        }

        public int Read()
        {
            return System.Console.Read();
        }

        public string ReadLine()
        {
            return System.Console.ReadLine();
        }
    }
}