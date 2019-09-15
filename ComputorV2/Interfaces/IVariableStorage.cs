using System.Collections.Generic;

namespace ComputorV2
{
    public interface IVariableStorage
    {
        Expression this[string varName] { get; }

        List<string> VariablesNames { get; }

        string AddOrUpdateVariableValue(string varName, Expression expression);
        void EraseVariablesData();
        string GetVariablesString();
    }
}