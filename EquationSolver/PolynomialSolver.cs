using System;
using PolynomialSolver.Console;

namespace PolynomialSolver
{
    public class PolynomialSolver
    {
        private readonly IConsole _console;

        public PolynomialSolver(IConsole console)
        {
            _console = console ?? throw new ArgumentException();
        }

        public int Solve(string[] args)
        {
            try
            {
                var expression = ParseArgs(args);
                SolveExpression(expression);
                return 0;
            }
            catch (Exception ex)
            {
                _console.WriteLine(ex.Message);
                return 1;
            }
        }

        private string ParseArgs(string[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("Please enter a single argument.");
            }

            return args[0];
        }

        private void SolveExpression(string expression)
        {
            var solution = new Solution {Expression = expression};
            var polynomial = Polynomial.Parse(expression, solution);
            if (solution.IsValid)
            {
                Polynomial.ShortenCoef(polynomial);
                solution.ReducedForm = Polynomial.ToString(polynomial);
                solution.Degree = polynomial.Count - 1;
                if (solution.IsSolvable)
                    Polynomial.Solve(polynomial, solution);
            }

            solution.WriteSolution(_console);
        }
    }
}