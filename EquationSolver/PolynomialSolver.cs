using System;
using System.Collections.Generic;
using BigNumbers;
using PolynomialExpressionSolver.Console;

namespace PolynomialExpressionSolver
{
    public class PolynomialSolver
    {
        private readonly IConsole _console;

        public PolynomialSolver(IConsole console)
        {
            _console = console ?? throw new ArgumentException();
        }

        public string SolveExpression(string expression)
        {
            var solution = new PolynomialSolution {Expression = expression};
            List<BigDecimal> polynomial = Polynomial.Parse(expression, solution);
            if (solution.IsValid)
            {
                Polynomial.ShortenCoef(polynomial);
                solution.ReducedForm = Polynomial.ToString(polynomial);
                solution.Degree = polynomial.Count - 1;
                if (solution.IsSolvable)
                    Polynomial.Solve(polynomial, solution);
            }

            solution.WriteSolution(_console);
            if (_console is BufferWriter writer)
                return writer.Output;
            return String.Empty;
        }
    }
}