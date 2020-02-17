using System;

namespace ComputorV2.EquationSolverWithTools
{
    public class EquationSolverLexicalException : Exception
    {
        public EquationSolverLexicalException()
        {
        }

        public EquationSolverLexicalException(string message)
            : base(message)
        {
        }

        public EquationSolverLexicalException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}