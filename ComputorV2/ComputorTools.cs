using System;
using System.Collections.Generic;
using System.Linq;

namespace ComputorV2
{
    public enum CommandType
    {
        Exit,
        Detailed,
        ShowAlowedOperations,
        ShowVars,
        ShowHelp,
        Reset,
        AssignVar,
        EvaluateExpression
    }

    public static class ComputorTools
    {
        static readonly Dictionary<string, CommandType> _commandStringTypes;
        static readonly Dictionary<string, string> _commandDescriptions;
        static readonly string _helpText;

        static ComputorTools()
        {
            _commandStringTypes = new Dictionary<string, CommandType>
            {
                {"exit", CommandType.Exit },
                {"vars", CommandType.ShowVars },
                {"detailed", CommandType.Detailed },
                {"reset", CommandType.Reset },
                {"help", CommandType.ShowHelp },
                {"allowed", CommandType.ShowAlowedOperations }
            };
            _commandDescriptions = new Dictionary<string, string>
            {
                {"exit", "Exit the program"},
                {"detailed", "For complex expression, in-between operations are shown" },
                {"vars", "View stored variables and their values" },
                {"reset", "Cleanup whole variable storage" },
                {"help", "View help"},
                {"allowed", "View list of allowed operations in-between types"}
            };
            _helpText = String.Join("\n", _commandDescriptions.Select(d => $"{d.Key}: {d.Value}"));
        }
        public static CommandType GetCommandType(string command)
        {
            var cmd = command.Trim().ToLower();
            var cmdType = GetSimpleCommandType(cmd);
            if (cmdType is null)
                return GetComplexCommandType(cmd);
            return (CommandType)cmdType;
        }

        public static string GetHelp()
        {
            return _helpText;
        }

        private static CommandType? GetSimpleCommandType(string command)
        {
            var cmd = command.Trim();
            if (_commandStringTypes.ContainsKey(cmd))
                return _commandStringTypes[cmd];
            return null;
        }

        private static CommandType GetComplexCommandType(string cmd)
        {
            var numOfEqualities = cmd.ToCharArray().Count(c => c == '=');

            if (numOfEqualities == 0)
                throw new ArgumentException($"Unknown command: '{cmd}'");
            if (numOfEqualities > 1)
                throw new ArgumentException($"Command cannot contain: '{numOfEqualities}' equal signs");
            if (cmd.Split('=')[1].Trim() == "?")
                return CommandType.EvaluateExpression;
            return CommandType.AssignVar;
        }

    }
}
