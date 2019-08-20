using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ComputorV2
{
    public class ConsoleReader
    {

        private readonly Dictionary<CommandType, Action<string>> CommandExecutors;

        private bool _isExitCommandEntered;
        private Dictionary<string, Expression> _variables;
        private static readonly Regex _validVariableNameRegEx;

        public List<string> Variables => _variables.Select(d => d.Key).ToList();
        public bool _detailed = false;

        static ConsoleReader()
        {
            _validVariableNameRegEx = new Regex(@"^[a-z]+$", RegexOptions.IgnoreCase);
        }
        public ConsoleReader()
        {
            _variables = new Dictionary<string, Expression>();
            CommandExecutors = new Dictionary<CommandType, Action<string>> {
                {CommandType.Exit, ExecuteExitCommand },
                {CommandType.Detailed, ExecuteDetailedCommand },
                {CommandType.ShowVars, ExecuteVarsCommand },
                {CommandType.ShowHelp, ExecuteHelpCommand },
                {CommandType.Reset, ExecuteResetCommand },
                {CommandType.AssignVar, ExecuteAssignVarCommand},
                {CommandType.EvaluateExpression, ExecuteEvaluateExpressionCommand}
            };
        }
        public ConsoleReader(Dictionary<string, Expression> variables)
        {
            _variables = variables;
            CommandExecutors = new Dictionary<CommandType, Action<string>> {
                {CommandType.Exit, ExecuteExitCommand },
                {CommandType.Detailed, ExecuteDetailedCommand },
                {CommandType.ShowVars, ExecuteVarsCommand },
                {CommandType.ShowHelp, ExecuteHelpCommand },
                {CommandType.Reset, ExecuteResetCommand },
                {CommandType.AssignVar, ExecuteAssignVarCommand},
                {CommandType.EvaluateExpression, ExecuteEvaluateExpressionCommand}
            };
        }
        public void StartReading()
        {
            _isExitCommandEntered = false;
            do
            {
                try
                {
                    Console.Write("> ");
                    var inputString = Console.ReadLine();
                    if (String.IsNullOrEmpty(inputString))
                        continue;
                    var cmdType = ConsoleReaderTools.GetCommandType(inputString);
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

        public List<RPNToken> GetVariableRPNTokens(string varName)
        {
            return _variables[varName].Tokens;
        }

        #region command executors
        public void ExecuteExitCommand(string command = null)
        {
            _isExitCommandEntered = true;
        }
        public void ExecuteDetailedCommand(string command = null)
        {
            Console.WriteLine("Display detailed expression evaluation process? [y/n]");
            var input = Console.ReadLine().ToLower();
            if (input == "y" || input == "yes")
                _detailed = true;
            else if (input == "n" || input == "no")
                _detailed = false;
            else
                Console.WriteLine($"Invalid answer. The detailed will remain {_detailed}");
        }
        public void ExecuteVarsCommand(string command = null)
        {
            var varsText = String.Join("\n", _variables.Select(d => $"{d.Key} = {d.Value}"));
            Console.WriteLine(varsText);
        }
        public static void ExecuteHelpCommand(string command = null)
        {
            var helpText = ConsoleReaderTools.GetHelp();
            Console.WriteLine(helpText);
        }
        public void ExecuteResetCommand(string command = null)
        {
            _variables = new Dictionary<string, Expression>();
        }
        public void ExecuteAssignVarCommand(string command)
        {
            if (String.IsNullOrEmpty(command))
                return;
            var parts = command.Split('=');
            var cmdVarName = parts[0].Trim().ToLower();
            var cmdExpression = parts[1].Trim().ToLower();
            if (!IsValidVarName(cmdVarName))
                throw new ArgumentException($"the variable name {cmdVarName} is not valid");
            try
            {
                _variables[cmdVarName] = ExpressionProcessor.CreateExpression( str : cmdExpression, consoleReaderRef : this, detailedMode: _detailed);
                Console.WriteLine($"> {_variables[cmdVarName]}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error. {e.Message}");
            }
        }

        public void ExecuteEvaluateExpressionCommand(string command)
        {
            var parts = command.Split('=');
            var cmdExpression = parts[0].Trim().ToLower();
            try
            {
                var executedExpression = ExpressionProcessor.CreateExpression(cmdExpression, this);
                Console.WriteLine($"{executedExpression}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error. {e.Message}");
            }
        }
        #endregion

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
