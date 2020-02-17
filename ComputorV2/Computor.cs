﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ComputorV2.EquationSolverWithTools;
using ComputorV2.ExternalConnections;

namespace ComputorV2
{
    public class Computor
    {
        private readonly IConsoleProcessor _consoleProcessor;
        private readonly IVariableStorage _variableStorage;
        private readonly IExpressionProcessor _expressionProcessor;
        private readonly EquationSolver _equationSolver;

        private readonly Dictionary<CommandType, Action<string>> CommandExecutors;

        private bool _isExitCommandEntered;
        private bool _detailed;

        public Computor(IConsoleProcessor consoleProcessor = null,
            IVariableStorage varStorage = null,
            IExpressionProcessor expressionProcessor = null,
            EquationSolver equationSolver = null)
        {
            _consoleProcessor = consoleProcessor ?? new ConsoleProcessor();
            CommandExecutors = GetCommandExecutorsDictionary();
            _variableStorage = varStorage ?? new VariableStorage();
            _expressionProcessor = expressionProcessor ?? new ExpressionProcessor(_variableStorage);
            _equationSolver = equationSolver ?? new EquationSolver();
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
                    _consoleProcessor.WriteLine($"Error. {e.Message}");
                }
            } while (!_isExitCommandEntered);

            _consoleProcessor.WriteLine("See ya!");
            _consoleProcessor.ReadLine();
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
                throw new ArgumentNullException("Command string cannot be null");
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
                _consoleProcessor.WriteLine($"Error. {e.Message}");
            }
        }

        private void ExecuteDeclareFunctionCommand(string command)
        {
            if (String.IsNullOrWhiteSpace(command))
                throw new ArgumentNullException("Command string cannot be null");
            var parts = command.Split('=');
            var funcPart = parts[0].Trim().ToLower();
            var funcExpression = parts[1].Trim().ToLower();
            if (!_variableStorage.IsValidFunctionDeclaration(funcPart, out string funcName, out string paramName,
                out string reason))
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
                _consoleProcessor.WriteLine($"{e.Message}");
            }
        }

        private void ExecuteSolveEquationCommand(string command)
        {
            var cleanCmd = command.ToLower().Replace("?", "");
            var funcRegex = new Regex(@"\s*[a-z]+\s*\(\s*[a-z]+\s*\)\s*");
            var equationToSolve = funcRegex.Replace(cleanCmd, CapText);
            var varRegex = new Regex(@"\s*[a-z]+\s*");
            equationToSolve = varRegex.Replace(equationToSolve, CapText);
            _consoleProcessor.WriteLine($"The equation is : \"{equationToSolve}\"");
            var solutionLines = _equationSolver.SolveEquation(equationToSolve);
            _consoleProcessor.Write(solutionLines);
        }

        string CapText(Match m)
        {
            string match = m.ToString();
            var parts = match.Split('(', ')');
            return _variableStorage[parts[0].Trim()].ToString();
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
            catch (Exception e)
            {
                _consoleProcessor.WriteLine($"Error. {e.Message}");
            }
        }

        #endregion

        private Dictionary<CommandType, Action<string>> GetCommandExecutorsDictionary()
        {
            return new Dictionary<CommandType, Action<string>>
            {
                {CommandType.Exit, ExecuteExitCommand},
                {CommandType.Detailed, ExecuteDetailedCommand},
                {CommandType.ShowAlowedOperations, ExecuteAllowedCommand},
                {CommandType.ShowVars, ExecuteVarsCommand},
                {CommandType.ShowHelp, ExecuteHelpCommand},
                {CommandType.Reset, ExecuteResetCommand},
                {CommandType.AssignVar, ExecuteAssignVarCommand},
                {CommandType.EvaluateExpression, ExecuteEvaluateExpressionCommand},
                {CommandType.DeclareFunction, ExecuteDeclareFunctionCommand},
                {CommandType.SolveEquation, ExecuteSolveEquationCommand}
            };
        }
    }
}