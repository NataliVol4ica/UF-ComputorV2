using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ComputorV2.ExternalResources;
using PolynomialExpressionSolver;
using PolynomialExpressionSolver.Console;

namespace ComputorV2
{
    public class Computor
    {
        private readonly IConsoleProcessor _consoleProcessor;
        private readonly PolynomialSolver _polynomialSolver;
        private readonly IExpressionProcessor _expressionProcessor;
        private readonly IVariableStorage _variableStorage;

        private readonly Dictionary<CommandType, Action<string>> CommandExecutors;
        private bool _detailed;

        private bool _isExitCommandEntered;

        public Computor(IConsoleProcessor consoleProcessor = null,
            IVariableStorage varStorage = null,
            IExpressionProcessor expressionProcessor = null,
            PolynomialSolver polynomialSolver = null)
        {
            _consoleProcessor = consoleProcessor ?? new ConsoleProcessor();
            CommandExecutors = GetCommandExecutorsDictionary();
            _variableStorage = varStorage ?? new VariableStorage();
            _expressionProcessor = expressionProcessor ?? new ExpressionProcessor(_variableStorage);
            _polynomialSolver = polynomialSolver ?? new PolynomialSolver(new BufferWriter());
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
                    if (string.IsNullOrEmpty(inputString))
                        continue;
                    var cmdType = ComputorTools.GetCommandType(inputString);
                    CommandExecutors[cmdType](inputString);
                }
                catch (Exception e)
                {
                    _consoleProcessor.WriteLine($"Error:  {e.Message}");
                }
            } while (!_isExitCommandEntered);

            _consoleProcessor.WriteLine("See ya!");
            _consoleProcessor.ReadLine();
        }

        private Dictionary<CommandType, Action<string>> GetCommandExecutorsDictionary()
        {
            return new Dictionary<CommandType, Action<string>>
            {
                {CommandType.Exit, ExecuteExitCommand},
                {CommandType.Detailed, ExecuteDetailedCommand},
                {CommandType.ShowAllowedOperations, ExecuteAllowedCommand},
                {CommandType.ShowVars, ExecuteVarsCommand},
                {CommandType.ShowHelp, ExecuteHelpCommand},
                {CommandType.Reset, ExecuteResetCommand},
                {CommandType.AssignVar, ExecuteAssignVarCommand},
                {CommandType.EvaluateExpression, ExecuteEvaluateExpressionCommand},
                {CommandType.DeclareFunction, ExecuteDeclareFunctionCommand},
                {CommandType.SolveEquation, ExecuteSolveEquationCommand}
            };
        }

        #region command executors

        private void ExecuteExitCommand(string command = null)
        {
            _isExitCommandEntered = true;
        }

        private void ExecuteDetailedCommand(string command = null)
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

        private void ExecuteVarsCommand(string command = null)
        {
            _consoleProcessor.WriteLine(_variableStorage.GetVariablesString());
        }

        private void ExecuteAllowedCommand(string command = null)
        {
            _consoleProcessor.WriteLine(ComputorTools.GetAllowedOperations());
        }

        private void ExecuteHelpCommand(string command = null)
        {
            var helpText = ComputorTools.GetHelp();
            _consoleProcessor.WriteLine(helpText);
        }

        private void ExecuteResetCommand(string command = null)
        {
            _variableStorage.EraseVariablesData();
        }

        private void ExecuteAssignVarCommand(string command)
        {
            if (String.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException(nameof(command));
            var parts = command.Split('=');
            var cmdVarName = parts[0].Trim().ToLower();
            var cmdExpression = parts[1].Trim().ToLower();
            if (!VariableStorage.IsValidVarName(cmdVarName))
                throw new ArgumentException($"the variable name {cmdVarName} is not valid");
            try
            {
                var newExpression = _expressionProcessor
                    .CreateExpression(cmdExpression, _detailed);
                var consoleOutput = _variableStorage
                    .AddOrUpdateVariableValue(cmdVarName, newExpression);
                _consoleProcessor.WriteLine($"> {consoleOutput}");
            }
            catch (Exception e)
            {
                _consoleProcessor.WriteLine($"Error: {e.Message}");
            }
        }

        private void ExecuteDeclareFunctionCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException("Command string cannot be null");
            var parts = command.Split('=');
            var funcPart = parts[0].Trim().ToLower();
            var funcExpression = parts[1].Trim().ToLower();
            if (!_variableStorage.IsValidFunctionDeclaration(funcPart, out var funcName, out var paramName,
                out var reason))
                throw new ArgumentException($"The function signature is not valid. {reason}");

            try
            {
                var functionExpression = _expressionProcessor
                    .CreateFunctionExpression(funcExpression, paramName);
                var consoleOutput = _variableStorage
                    .AddOrUpdateVariableValue(funcName, functionExpression);
                _consoleProcessor.WriteLine($"> {funcName}(X) = {consoleOutput}");
            }
            catch (Exception e)
            {
                _consoleProcessor.WriteLine($"Error: {e.Message}");
            }
        }

        private void ExecuteSolveEquationCommand(string command)
        {
            var cleanCmd = command.ToLower().Replace("?", "");
            var funcRegex = new Regex(@"\s*[a-z]+\s*\(\s*[a-z]+\s*\)\s*");
            var equationToSolve = funcRegex.Replace(cleanCmd, CapText);
            var varRegex = new Regex(@"\s*[a-z]+\s*");
            //todo: NULL REFERENCE
            equationToSolve = varRegex.Replace(equationToSolve, CapText);
            _consoleProcessor.WriteLine($"The equation is : \"{equationToSolve}\"");
            var solutionLines = _polynomialSolver.SolveExpression(equationToSolve);
            _consoleProcessor.Write(solutionLines);
        }

        private string CapText(Match m)
        {
            var match = m.ToString();
            var parts = match.Split('(', ')');
            var part0 = parts[0].Trim();
            if (part0.ToLower() == "i")
                throw new Exception("Cannot solve equation containing complex values");
            var storedVar = _variableStorage[part0];
            if (storedVar is null)
                throw new Exception($"Unknown variable: {part0}");
            return storedVar.ToString();
        }

        private void ExecuteEvaluateExpressionCommand(string command)
        {
            var parts = command.Split('=');
            var cmdExpression = parts[0].Trim().ToLower();
            try
            {
                var executedExpression = _expressionProcessor.CreateExpression(cmdExpression);
                _consoleProcessor.WriteLine($"{executedExpression}");
            }
            catch (InvalidOperationException)
            {
                _consoleProcessor.WriteLine($"Error: Invalid expression");
            }
            catch (Exception e)
            {
                _consoleProcessor.WriteLine($"Error:  {e.Message}");
            }
        }

        #endregion
    }
}