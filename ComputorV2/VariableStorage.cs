using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ComputorV2
{
    public class VariableStorage
    {
        private static readonly Regex _validVariableNameRegEx = new Regex(@"^[a-z]+$", RegexOptions.IgnoreCase);
        private Dictionary<string, Expression> _variables;
        public List<string> VariablesNames => _variables.Select(d => d.Key).ToList();
       
        public VariableStorage()
        {
            _variables = new Dictionary<string, Expression>();
        }
        public List<RPNToken> GetVariableRPNTokens(string varName)
        {
            return _variables[varName].Tokens;
        }

        public string GetVariablesString()
        {
            var varsText = String.Join("\n", _variables.Select(d => $"{d.Key} = {d.Value}"));
            return varsText;
        }
        public void RecreateVariablesData()
        {
            _variables = new Dictionary<string, Expression>();
        }
        public void AddOrUpdateVariableValue(string varName, Expression expression)
        {
            _variables[varName] = expression;
        }
        public static bool IsValidVarName(string name)
        {
            var varNAme = name.Trim();
            if (varNAme == "i" || varNAme == "I")
                return false;
            return _validVariableNameRegEx.IsMatch(varNAme);
        }
        public string this[string varName]
        {
            get
            {
                var lowVarName = varName.ToLower();
                if (_variables.ContainsKey(lowVarName))
                    return _variables[lowVarName].ToString();
                return null;
            }
        }
    }
}
