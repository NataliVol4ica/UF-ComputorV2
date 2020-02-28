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
            List<BigDecimal> polynomial = PolynomialProcessor.Parse(expression, solution);
            if (solution.IsValid)
            {
                PolynomialProcessor.ShortenCoef(polynomial);
                solution.ReducedForm = PolynomialProcessor.ToString(polynomial);
                solution.Degree = polynomial.Count - 1;
                if (solution.IsSolvable)
                    PolynomialProcessor.Solve(polynomial, solution);
            }

            solution.WriteSolution(_console);
            if (_console is BufferWriter writer)
                return writer.Output;
            return String.Empty;
        }
    }
}