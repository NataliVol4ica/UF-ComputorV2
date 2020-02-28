using System.Collections.Generic;
using PolynomialExpressionSolver.Console;

namespace PolynomialExpressionSolver
{
    public static class SolutionPrinter
    {
        private static readonly Dictionary<SolutionType, Printer> Printers = new Dictionary<SolutionType, Printer>
        {
            {SolutionType.None, PrintSolution_None},
            {SolutionType.Single, PrintSolution_Single},
            {SolutionType.Double, PrintSolution_Double},
            {SolutionType.All, PrintSolution_All}
        };

        private static void PrintSolution_None(PolynomialSolution solution, IConsole console)
        {
            console.WriteLine("Solution:");
            console.WriteLine(" None");
        }

        private static void PrintSolution_Single(PolynomialSolution solution, IConsole console)
        {
            console.WriteLine("Solution:");
            console.WriteLine($"X = {solution.Answers[0]}");
        }

        private static void PrintSolution_Double(PolynomialSolution solution, IConsole console)
        {
            console.WriteLine("Solutions:");
            console.WriteLine($"X1 = {solution.Answers[0]}");
            console.WriteLine($"X2 = {solution.Answers[1]}");
        }

        private static void PrintSolution_All(PolynomialSolution solution, IConsole console)
        {
            console.WriteLine("Solution:");
            console.WriteLine("All real numbers");
        }

        public static void Print(PolynomialSolution s, IConsole console)
        {
            Printers[s.SolutionType](s, console);
        }

        private delegate void Printer(PolynomialSolution solution, IConsole console);
    }
}