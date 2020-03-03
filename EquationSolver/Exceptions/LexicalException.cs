using System;

namespace PolynomialExpressionSolver.Exceptions
{
    public class LexicalException : Exception
    {
        public LexicalException()
        {
        }

        public LexicalException(string message)
            : base(message)
        {
        }

        public LexicalException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}