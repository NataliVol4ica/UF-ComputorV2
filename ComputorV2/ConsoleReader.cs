using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ComputorV2
{
    public static class ConsoleReader
    {

        private static Dictionary<CommandType, Action<string>> CommandExecutors;

        private static bool _isExitCommandEntered;
        private static Dictionary<string, Expression> _variables;
        private static readonly Regex _validVariableNameRegEx;

        public static List<string> Variables => _variables.Select(d => d.Key).ToList();

        static ConsoleReader()
        {
            _variables = new Dictionary<string, Expression>();
            _validVariableNameRegEx = new Regex(@"^[a-z]+$", RegexOptions.IgnoreCase);
            CommandExecutors = new Dictionary<CommandType, Action<string>> {
                {CommandType.Exit, ExecuteExitCommand },
                {CommandType.ShowVars, ExecuteVarsCommand },
                {CommandType.ShowHelp, ExecuteHelpCommand },
                {CommandType.Reset, ExecuteResetCommand },
                {CommandType.AssignVar, ExecuteAssignVarCommand},
                {CommandType.EvaluateExpression, ExecuteEvaluateExpressionCommand}
            };
        }
        
        public static void StartReading()
        {
            _isExitCommandEntered = false;
            do
            {
                try
                {
                    var inputString = Console.ReadLine();
                    var cmdType = ReaderCommandTools.GetCommandType(inputString);
                    CommandExecutors[cmdType](inputString);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            } while (!_isExitCommandEntered);
            Console.WriteLine("See ya!");
            Console.ReadLine();
        }

        public static List<RPNToken> GetVariableRPNTokens(string varName)
        {
            return _variables[varName].Tokens;
        }

        #region command executors
        private static void ExecuteExitCommand(string command)
        {
            _isExitCommandEntered = true;
        }
        private static void ExecuteVarsCommand(string command)
        {
            var varsText = String.Join("\n", _variables.Select(d => $"{d.Key} = {d.Value}"));
            Console.WriteLine(varsText);
        }
        private static void ExecuteHelpCommand(string command)
        {
            var helpText = ReaderCommandTools.GetHelp();
            Console.WriteLine(helpText);
        }
        private static void ExecuteResetCommand(string command)
        {
            _variables = new Dictionary<string, Expression>();
        }
        private static void ExecuteAssignVarCommand(string command)
        {
            var parts = command.Split('=');
            var cmdVarName = parts[0].Trim().ToLower();
            var cmdExpression = parts[1].Trim();
            if (!IsValidVarName(cmdVarName))
                throw new ArgumentException($"the variable name {command} is not valid");
            _variables[cmdVarName] = new Expression(cmdExpression);
            Console.WriteLine($"> {_variables[cmdVarName]}");
        }

        public static void AddManualVariable(string varName, Expression expression)
        {
            _variables[varName] = expression;
        }

        private static void ExecuteEvaluateExpressionCommand(string obj)
        {
            //validate left and right parts non-emptiness
            //same as var assignment but without assignment and left part is treated as the right one
            throw new NotImplementedException();
        }
        #endregion

        public static bool IsValidVarName (string name)
        {
            var varNAme = name.Trim();
            if (varNAme == "i" || varNAme == "I")
                return false;
            return _validVariableNameRegEx.IsMatch(varNAme);
        }
    }
}
