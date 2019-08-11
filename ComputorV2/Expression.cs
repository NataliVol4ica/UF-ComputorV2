using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace ComputorV2
{
    public class Expression
    {
        #region Regex static data
        private static readonly Regex expressionRegex;
        protected static readonly string regexFormat = @"\G\s*({0}|\+|-|\*|/|%|\(|\))\s*";

        private static readonly Dictionary<string, string> _numbersRegexes = new Dictionary<string, string>
        {
            { "VariableNameRegex", @"[a-z]+" },
            { "BigDecimalRegex", @"\d+(\.\d+)?" }
        };

        private static readonly Regex _bigDecimalRegex;

        private static readonly List<string> funcs = new List<string> { "abs" };
        private static readonly ILookup<string, OperationInfo> opInfoMap;

        static Expression()
        {
            _bigDecimalRegex = new Regex(_numbersRegexes["BigDecimalRegex"], RegexOptions.Compiled);

            var numericRegexString = String.Join("|", _numbersRegexes.Select(d => d.Value));
            var funcRegexString = String.Join("|", funcs);
            string innerRegex = $"{numericRegexString}|{funcRegexString}";

            expressionRegex = new Regex(String.Format(regexFormat, innerRegex), RegexOptions.Compiled);

            Debug.WriteLine($"The Expression's regex is: \n{expressionRegex.ToString()}");

            opInfoMap = new[]
            {
                new OperationInfo("-", OpArity.Binary, 1, OpAssoc.Left),
                new OperationInfo("-", OpArity.Unary,  3, OpAssoc.Left),
                new OperationInfo("+", OpArity.Binary, 1, OpAssoc.Left),
                new OperationInfo("+", OpArity.Unary,  3, OpAssoc.Left),
                new OperationInfo("*", OpArity.Binary, 2, OpAssoc.Left),
                new OperationInfo("/", OpArity.Binary, 2, OpAssoc.Left),
                new OperationInfo("%", OpArity.Binary, 2, OpAssoc.Left)
                //example of OpAssoc.Right operator is
                //new OperationInfo("^", OpArity.Binary, 2, OpAssoc.Right)
            }.ToLookup(op => op.op);
        }
        #endregion

        public string InitialString { get; private set; }
        public List<RPNToken> Tokens { get; private set; }
        public bool IsFunction { get; private set; }

        public Expression(string str, bool isFunction = false, string functionParameterName = null)
        {
            InitialString = str;
            IsFunction = isFunction;
            var stringTokens = Tokenize(str);
            Tokens = RecognizeLexems(stringTokens, functionParameterName);
        }
        public Expression(List<RPNToken> tokens, bool isFunction = false)
        {
            IsFunction = isFunction;
            Tokens = tokens;
            InitialString = this.ToString();
        }

        public static List<string> Tokenize(string str)
        {
            if (str is null)
                throw new ArgumentException("Cannot tokenize null string");
            var stringTokens = new List<string>();
            int lastMatchPos = 0;
            int lastMatchLen = 0;
            Match match = expressionRegex.Match(str);
            while (match.Success)
            {
                lastMatchPos = match.Index;
                lastMatchLen = match.Value.Length;
                stringTokens.Add(match.Value.Trim());
                match = match.NextMatch();
            }
            if (lastMatchPos + lastMatchLen < str.Length)
                throw new ArgumentException("The expression is invalid");
            return stringTokens;
        }

        public static List<RPNToken> RecognizeLexems(List<string> stringTokens, string funcParameter = null)
        {
            var variables = ConsoleReader.Variables;

            var tokenList = new List<RPNToken>();
            TokenType tokenType;
            TokenType? prev = null;
            foreach (var dirtyToken in stringTokens)
            {
                if (String.IsNullOrEmpty(dirtyToken))
                    continue;
                var token = dirtyToken.ToLower();
                if (opInfoMap[token].Count() > 0)
                {
                    if (!(prev is null) &&
                        (prev.Value == TokenType.CBracket ||
                        prev.Value == TokenType.Variable ||
                        prev.Value == TokenType.FunctionParameter ||
                        prev.Value == TokenType.DecimalNumber)) //todo: if new number type is created then append this 'if' with new type
                        tokenType = TokenType.BinOp;
                    else
                        tokenType = TokenType.UnOp;
                }
                else if (token == "(")
                    tokenType = TokenType.OBracket;
                else if (token == ")")
                    tokenType = TokenType.CBracket;
                else if (funcs.Contains(token))
                    tokenType = TokenType.Function;
                else if (!(funcParameter is null) && token == funcParameter)
                {
                    tokenType = TokenType.FunctionParameter;
                    token = "x";
                }
                else if (_bigDecimalRegex.IsMatch(token))
                    tokenType = TokenType.DecimalNumber;
                else if (variables.Contains(token))
                    tokenType = TokenType.Variable;
                else
                    throw new ArgumentException($"Invalid token: '{token}'");
                prev = tokenType;
                if (tokenType == TokenType.Variable)
                {
                    tokenList.Add(new RPNToken("(", TokenType.OBracket));
                    tokenList.AddRange(ConsoleReader.GetVariableRPNTokens(token));
                    tokenList.Add(new RPNToken(")", TokenType.CBracket));
                }
                else
                    tokenList.Add(new RPNToken(token, tokenType));
            }
            return tokenList;
        }

        public override string ToString()
        {
            var str = String.Join(" ", Tokens);
            return str;
        }
    }
}
