namespace PolynomialSolver.Console
{
    public class BufferWriter : IConsole
    {
        private string _output = string.Empty;

        public string Output => _output;

        public void Write(string s)
        {
            _output += s;
        }

        public void WriteLine(string s)
        {
            _output += s + '\n';
        }

        public int Read()
        {
            return System.Console.Read();
        }

        public string ReadLine()
        {
            return System.Console.ReadLine();
        }

        public void ResetOutput()
        {
            _output = string.Empty;
        }
    }
}