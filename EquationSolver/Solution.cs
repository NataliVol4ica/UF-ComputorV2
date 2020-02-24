using System;
using System.Collections.Generic;
using PolynomialSolver.Console;

namespace PolynomialSolver
{
    public class Solution
    {
        public Solution()
        {
            Degree = -1;
            ValidationError = "";
            Answers = new List<string>();
            Logs = new List<string>();
        }

        public string Expression { get; set; }

        public string ValidationError { get; set; }
        public bool IsValid => String.IsNullOrEmpty(ValidationError);

        public string ReducedForm { get; set; }
        public int Degree { get; set; }
        public bool IsSolvable => String.IsNullOrEmpty(ErrorMessage) && IsValid;

        public string ErrorMessage
        {
            get
            {
                if (Degree > 2 || Degree < 0)
                    return $"Expected degree: 0..2. Actual degree: {Degree}";
                return "";
            }
        }

        public SolutionType SolutionType { get; set; }

        public List<string> Answers { get; set; }
        public List<string> Logs { get; set; }

        public void WriteSolution(IConsole console)
        {
            console.WriteLine($"Expression: {Expression}");

            console.WriteLine($"Reduced form: {ReducedForm} = 0");
            console.WriteLine($"Polynomial Degree: {Degree}");
            if (IsSolvable)
            {
                PrintLogs(console);
                SolutionPrinter.Print(this, console);
            }
            else
            {
                console.WriteLine(ErrorMessage);
            }
        }

        private void PrintLogs(IConsole console)
        {
            Logs.ForEach(console.WriteLine);
        }
    }
}