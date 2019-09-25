using System.Collections.Generic;

namespace ComputorV2
{
    public interface IVariableStorage
    {
        Expression this[string varName] { get; }

        List<string> AllVariablesNames { get; }

        bool ContainsVariable(string funcName);
        bool ContainsFunction(string funcName);

        string AddOrUpdateVariableValue(string varName, Expression expression);
        void EraseVariablesData();
        string GetVariablesString();
        bool IsValidFunctionDeclaration(string func, out string funcName, out string paramName, out string reason);
    }
}