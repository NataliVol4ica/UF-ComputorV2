using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputorV2
{
    public enum CommandType
    {
        Exit,
        Detailed,
        ShowAllowedOperations,
        ShowVars,
        ShowHelp,
        Reset,
        AssignVar,
        DeclareFunction,
        SolveEquation,
        EvaluateExpression
    }

    public static class ComputorTools
    {
        private static readonly Dictionary<string, CommandType> CommandStringTypes;
        private static readonly string HelpText;
        private static readonly string AllowedOperations;

        static ComputorTools()
        {
            CommandStringTypes = new Dictionary<string, CommandType>
            {
                {"exit", CommandType.Exit},
                {"vars", CommandType.ShowVars},
                {"detailed", CommandType.Detailed},
                {"reset", CommandType.Reset},
                {"help", CommandType.ShowHelp},
                {"allowed", CommandType.ShowAllowedOperations}
            };
            var commandDescriptions = new Dictionary<string, string>
            {
                {"exit", "Exit program"},
                {"detailed", "For complex expression, in-between operations are shown"},
                {"vars", "View stored variables and their values"},
                {"reset", "Cleanup whole variable storage"},
                {"help", "View help"},
                {"allowed", "View list of allowed operations in-between types"}
            };
            HelpText = String.Join("\n", commandDescriptions.Select(d => $"{d.Key}: {d.Value}"));
            AllowedOperations = " >> Rational << \n\n"
                                 + " + -abs rational \n"
                                 + " rational all rational \n"
                                 + " rational +-* complex \n"
                                 + " rational* matrix \n\n"
                                 + " >> Complex << \n\n"
                                 + " +-abs complex \n"
                                 + " complex ^ int \n"
                                 + " complex +-* rational"
                                 + " sqrt(rational) \n"
                                 + " complexsqrt(rational) \n"
                                 + " \n complex +-* complex \n\n"
                                 + " >> Matrix << \n\n"
                                 + " +- matrix \n"
                                 + " matrix */ rational \n"
                                 + " matrix + - matrix of same size \n"
                                 + " matrix A[LxM] * matrix B[MxN]"
                                 + " T(matrix) - transponation \n"
                                 + " R(matrix) - reverse \n"
                                 + " abs(matrix) - opredelitel \n\n"
                                 + " >> Func << \n\n"
                                 + " func cannot be in the right part of equation if it has no known variable or value as parameter \n"
                                 + " func(x) -> x: expression containing rational, complex, funcs\n"
                                 + " func(x) = exp -> expr must only contain rationals and x\n"
                                 + " \n Pows must be integers \n";
        }

        public static CommandType GetCommandType(string command)
        {
            if (String.IsNullOrWhiteSpace(command))
                throw new ArgumentException($"Empty command: '{command}'");
            var cmd = command.Trim().ToLower();
            var cmdType = GetSimpleCommandType(cmd);
            if (cmdType is null)
                return GetComplexCommandType(cmd);
            return (CommandType) cmdType;
        }

        public static string GetHelp()
        {
            return HelpText;
        }

        public static string GetAllowedOperations()
        {
            return AllowedOperations;
        }

        private static CommandType? GetSimpleCommandType(string command)
        {
            var cmd = command.Trim();
            if (CommandStringTypes.ContainsKey(cmd))
                return CommandStringTypes[cmd];
            return null;
        }

        private static CommandType GetComplexCommandType(string cmd)
        {
            var numOfEqualities = cmd.ToCharArray().Count(c => c == '=');

            if (numOfEqualities == 0)
                throw new ArgumentException($"Unknown command: '{cmd}'");
            if (numOfEqualities > 1)
                throw new ArgumentException($"Command cannot contain: '{numOfEqualities}' equal signs");
            var cmdParts = cmd.Split('=');
            var isEvaluateCommand = cmdParts[1].Trim() == "?";
            var isSolveEquation = !isEvaluateCommand && cmdParts[1].Contains("?");
            var isFunction = cmdParts[0].Contains('(');
            if (isSolveEquation)
            {
                if (!isFunction)
                    throw new ArgumentException("Cannot solve equation because function is missing");
                return CommandType.SolveEquation;
            }

            if (isEvaluateCommand)
                return CommandType.EvaluateExpression;
            if (isFunction)
                return CommandType.DeclareFunction;
            return CommandType.AssignVar;
        }
    }
}