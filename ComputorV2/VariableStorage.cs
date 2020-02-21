using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ComputorV2
{
    public class VariableStorage : IVariableStorage
    {
        private static readonly Regex _validVariableNameRegEx =
            new Regex(@"^[a-z]+$", RegexOptions.IgnoreCase);

        private Dictionary<string, Expression> _variables;

        public VariableStorage()
        {
            _variables = new Dictionary<string, Expression>();
        }

        public List<string> AllVariablesNames => _variables
            .Select(d => d.Key)
            .ToList();

        public bool ContainsVariable(string funcName)
        {
            var containsVar = _variables.TryGetValue(funcName, out var expr);
            if (!containsVar)
                return false;
            return !expr.IsFunction;
        }

        public bool ContainsFunction(string funcName)
        {
            var containsVar = _variables.TryGetValue(funcName, out var expr);
            if (!containsVar)
                return false;
            return expr.IsFunction;
        }

        public string GetVariablesString()
        {
            var varsText = String.Join("\n", _variables.Select(
                d => d.Value.IsFunction ? $"{d.Key}(X) = {d.Value}" : $"{d.Key} = {d.Value}"));
            return varsText;
        }

        public void EraseVariablesData()
        {
            _variables = new Dictionary<string, Expression>();
        }

        public string AddOrUpdateVariableValue(string varName, Expression expression)
        {
            _variables[varName] = expression;
            return _variables[varName].ToString();
        }

        public Expression this[string varName]
        {
            get
            {
                var lowVarName = varName.ToLower();
                if (_variables.ContainsKey(lowVarName))
                    return _variables[lowVarName];
                return null;
            }
        }

        public bool IsValidFunctionDeclaration(string func, out string funcName, out string paramName,
            out string reason)
        {
            funcName = "";
            paramName = "";
            reason = "";
            var cleanString = Regex.Replace(func, @"\s+", "");
            if (cleanString.Count(c => c == '(') != 1)
            {
                reason = $"{func} contains invalid number of '('";
                return false;
            }

            if (cleanString.Count(c => c == ')') != 1)
            {
                reason = $"{func} contains invalid number of ')'";
                return false;
            }

            var parts = cleanString.Split('(', ')');
            if (!IsValidVarName(parts[0]))
            {
                reason = $"{func} has invalid function name format";
                return false;
            }

            if (!IsValidVarName(parts[1]))
            {
                reason = $"{func} has invalid parameter name format";
                return false;
            }

            if (_variables.TryGetValue(parts[1].ToLower(), out _))
            {
                reason = $"{func} has invalid parameter name (it is a name of existing variable)." +
                         " Choose another parameter name or use 'reset' command.";
                return false;
            }

            if (parts[1].ToLower().Equals("i"))
            {
                reason = $"{func} has invalid parameter name '{parts[1]}'";
                return false;
            }

            funcName = parts[0];
            paramName = parts[1];
            return true;
        }

        public static bool IsValidVarName(string name)
        {
            var varNAme = name.Trim();
            if (varNAme == "i" || varNAme == "I")
                return false;
            return _validVariableNameRegEx.IsMatch(varNAme);
        }
    }
}