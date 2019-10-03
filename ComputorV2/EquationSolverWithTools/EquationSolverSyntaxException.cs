using System;

namespace ComputorV2.EquationSolverWithTools
{
    public class EquationSolverSyntaxException : Exception
    {
        public EquationSolverSyntaxException() { }
        public EquationSolverSyntaxException(string message)
            : base(message) { }
        public EquationSolverSyntaxException(string message, Exception inner)
            : base(message, inner) { }
    }
}
