using ComputorV2.ExternalConnections;
using System;
using System.Collections.Generic;

namespace ComputorV2
{
    public class Computor
    {
        // Dependency Injections
        private readonly IConsoleProcessor _consoleProcessor;
        private readonly VariableStorage _variableStorage;

        private readonly Dictionary<CommandType, Action<string>> CommandExecutors;

        private bool _isExitCommandEntered;
        public bool _detailed = false;

        public Computor(IConsoleProcessor consoleProcessor = null, VariableStorage varStorage = null)
        {
            _consoleProcessor = consoleProcessor ?? new ConsoleProcessor();
            CommandExecutors = GetCommandExecutorsDictionary();
            _variableStorage = varStorage ?? new VariableStorage();
        }
        public void StartReading()
        {
            _isExitCommandEntered = false;
            do
            {
                try
                {
                    _consoleProcessor.Write("> ");
                    var inputString = _consoleProcessor.ReadLine();
                    if (String.IsNullOrEmpty(inputString))
                        continue;
                    var cmdType = ComputorTools.GetCommandType(inputString);
                    CommandExecutors[cmdType](inputString);
                }
                catch (Exception e)
                {
                    _consoleProcessor.WriteLine(e.Message);
                }
            } while (!_isExitCommandEntered);
            _consoleProcessor.WriteLine("See ya!");
            _consoleProcessor.ReadLine();
        }

        public List<RPNToken> GetVariableRPNTokens(string varName)
        {
            return _variableStorage[varName].Tokens;
        }

        #region command executors
        public void ExecuteExitCommand(string command = null)
        {
            _isExitCommandEntered = true;
        }
        public void ExecuteDetailedCommand(string command = null)
        {
            _consoleProcessor.WriteLine("Display detailed expression evaluation process? [y/n]");
            var input = _consoleProcessor.ReadLine().ToLower();
            if (input == "y" || input == "yes")
                _detailed = true;
            else if (input == "n" || input == "no")
                _detailed = false;
            else
                _consoleProcessor.WriteLine($"Invalid answer. The detailed will remain {_detailed}");
        }
        public void ExecuteVarsCommand(string command = null)
        {
            _consoleProcessor.WriteLine(_variableStorage.GetVariablesString());
        }
        public void ExecuteAllowedCommand(string command = null)
        {
            _consoleProcessor.WriteLine(ComputorTools.GetAllowedOperations());
        }
        public void ExecuteHelpCommand(string command = null)
        {
            var helpText = ComputorTools.GetHelp();
            _consoleProcessor.WriteLine(helpText);
        }
        public void ExecuteResetCommand(string command = null)
        {
            _variableStorage.EraseVariablesData();
        }
        public void ExecuteAssignVarCommand(string command)
        {
            if (String.IsNullOrEmpty(command))
                return;
            var parts = command.Split('=');
            var cmdVarName = parts[0].Trim().ToLower();
            var cmdExpression = parts[1].Trim().ToLower();
            if (!VariableStorage.IsValidVarName(cmdVarName))
                throw new ArgumentException($"the variable name {cmdVarName} is not valid");
            try
            {
                var newExpression = ExpressionProcessor.CreateExpression(str: cmdExpression, computorRef: this, detailedMode: _detailed);
                var consoleOutput = _variableStorage.AddOrUpdateVariableValue(cmdVarName, newExpression);
                _consoleProcessor.WriteLine($"> {consoleOutput}");
            }
            catch (Exception e)
            {
                _consoleProcessor.WriteLine($"Error. {e.Message}");
            }
        }

        public void ExecuteEvaluateExpressionCommand(string command)
        {
            var parts = command.Split('=');
            var cmdExpression = parts[0].Trim().ToLower();
            try
            {
                var executedExpression = ExpressionProcessor.CreateExpression(cmdExpression, this);
                _consoleProcessor.WriteLine($"{executedExpression}");
            }
            catch (Exception e)
            {
                _consoleProcessor.WriteLine($"Error. {e.Message}");
            }
        }
        #endregion

        public List<string> GetVariablesNameList()
        {
            return _variableStorage.VariablesNames;
        }

        private Dictionary<CommandType, Action<string>> GetCommandExecutorsDictionary()
        {
            return new Dictionary<CommandType, Action<string>>{
                { CommandType.Exit, ExecuteExitCommand },
                { CommandType.Detailed, ExecuteDetailedCommand },
                { CommandType.ShowAlowedOperations, ExecuteAllowedCommand },
                { CommandType.ShowVars, ExecuteVarsCommand },
                { CommandType.ShowHelp, ExecuteHelpCommand },
                { CommandType.Reset, ExecuteResetCommand },
                { CommandType.AssignVar, ExecuteAssignVarCommand},
                { CommandType.EvaluateExpression, ExecuteEvaluateExpressionCommand}
            };
        }       
    }
}
