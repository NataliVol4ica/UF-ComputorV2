using BigNumbers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace ComputorV2
{
    public static class ExpressionProcessor
    {
        #region Regex static data
        private static readonly Regex expressionRegex;
        private static readonly string regexFormat = @"\G\s*({0}|\+|-|\*|/|%|\(|\))\s*";

        private static readonly Dictionary<string, string> _numbersRegexes = new Dictionary<string, string>
        {
            { "VariableNameRegex", @"[a-z]+" },
            { "BigDecimalRegex", @"\d+(\.\d+)?" }
        };

        private static readonly Regex _bigDecimalRegex;

        private static readonly List<string> funcs = new List<string> { "abs" };
        private static readonly ILookup<string, OperationInfo> opInfoMap;

        static ExpressionProcessor()
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

        public static Expression CreateExpression(string str,
            ConsoleReader consoleReaderRef,
            bool isFunction = false, 
            string functionParameterName = null)
        {
            var stringTokens = Tokenize(str);
            var tokens = RecognizeLexems(stringTokens, consoleReaderRef, functionParameterName);
            var simplifiedTokens = Simplify(tokens);
            var newTokenList = new List<RPNToken>
            {
                new RPNToken
                {
                    str = simplifiedTokens.ToString(),
                    tokenType = TokenType.DecimalNumber
                }
            };
            return new Expression(newTokenList, isFunction, str);
        }
        public static Expression CreateExpression(List<RPNToken> tokens, bool isFunction = false)
        {
            return new Expression(tokens, isFunction);
        }

        // Step 1
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
        // Step 2
        public static List<RPNToken> RecognizeLexems(List<string> stringTokens,
            ConsoleReader consoleReaderRef,
            string funcParameter = null)
        {
            var variables = consoleReaderRef.Variables;

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
                    tokenList.AddRange(consoleReaderRef.GetVariableRPNTokens(token));
                    tokenList.Add(new RPNToken(")", TokenType.CBracket));
                }
                else
                    tokenList.Add(new RPNToken(token, tokenType));
            }
            return tokenList;
        }
        // Step 3
        public static BigNumber Simplify(List<RPNToken> tokens)
        {
            RPNToken currentToken;
            RPNToken tempToken;
            OperationInfo curOpInfo;
            var buffer = new Stack<RPNToken>();
            var result = new Stack<BigNumber>();
            var expressionQueue = new Queue<RPNToken>(tokens);

            while (expressionQueue.Count > 0)
            {
                currentToken = expressionQueue.Dequeue();
                if (currentToken.tokenType == TokenType.DecimalNumber)
                    result.Push(new BigDecimal(currentToken.str));
                else if (currentToken.tokenType == TokenType.Function)
                    buffer.Push(currentToken);
                else if (currentToken.tokenType == TokenType.BinOp ||
                    currentToken.tokenType == TokenType.UnOp)
                {
                    curOpInfo = GetOperationInfo(currentToken);
                    while (buffer.Count() > 0)
                    {
                        RPNToken cmpToken = buffer.Peek();
                        if (cmpToken.tokenType == TokenType.Function ||
                             (IsOperation(cmpToken) && CompareOperationPriorities(GetOperationInfo(cmpToken), curOpInfo) > 0))
                            CalculateToken(result, buffer.Pop());
                        else
                            break;

                    }
                    buffer.Push(currentToken);
                }
                else if (currentToken.tokenType == TokenType.OBracket)
                    buffer.Push(currentToken);
                else if (currentToken.tokenType == TokenType.CBracket)
                {
                    while ((tempToken = buffer.Pop()).tokenType != TokenType.OBracket)
                        CalculateToken(result, tempToken);
                    if (buffer.Count() > 0 && buffer.Peek().tokenType == TokenType.Function)
                        CalculateToken(result, buffer.Pop());
                }
                else
                    throw new NotImplementedException($"Unimplemented token '{currentToken.str}'");
            }
            while (buffer.Count() > 0)
                CalculateToken(result, buffer.Pop());
            if (result.Count() > 1)
                throw new ArgumentException("Cannot calculate this expression. Remaining RPN buffer contains extra numbers.");
            else if (result.Count() == 0)
                return new BigDecimal("0");
            return result.Pop();

        }

        // Tools
        private static OperationInfo GetOperationInfo(RPNToken token)
        {
            OpArity arity = token.tokenType == TokenType.BinOp ?
                                    OpArity.Binary :
                                    OpArity.Unary;
            var curOps = opInfoMap[token.str];
            var curOp = curOps.Count() == 1 ?
                                    curOps.Single() :
                                    curOps.Single(o => o.arity == arity);
            return curOp;
        }
        private static bool IsOperation(RPNToken t)
        {
            if (t.tokenType == TokenType.BinOp || t.tokenType == TokenType.UnOp)
                return true;
            return false;
        }
        private static int CompareOperationPriorities(OperationInfo left, OperationInfo right)
        {
            if ((right.assoc == OpAssoc.Left && right.priority <= left.priority) ||
                (right.assoc == OpAssoc.Right && right.priority < left.priority))
                return 1;
            return -1;
        }
        private static void CalculateToken(Stack<BigNumber> result, RPNToken op)
        {
            if (op.tokenType == TokenType.Function)
                result.Push(CalcFunc(result.Pop(), op.str));
            else if (op.tokenType == TokenType.BinOp)
                result.Push(CalculateBinaryOp(result.Pop(), result.Pop(), op.str));
            else if (op.tokenType == TokenType.UnOp)
                result.Push(CalcUnaryOp(result.Pop(), op.str));
        }

        #region Calculations
        private static BigNumber CalculateBinaryOp(BigNumber right, BigNumber left, string op)
        {
            if (op.Equals("+"))
                return left + right;
            if (op.Equals("-"))
                return left - right;
            if (op.Equals("*"))
                return left * right;
            if (op.Equals("/"))
                return left / right;
            if (op.Equals("%"))
                return left % right;
            throw new NotImplementedException("RPNParser met unimplemented operator \"" + op + "\"");
        }

        private static BigNumber CalcUnaryOp(BigNumber operand, string op)
        {
            switch (op)
            {
                case "+":
                    break;
                case "-":
                    operand.Negate();
                    break;
            }
            return operand;
        }

        private static BigNumber CalcFunc(BigNumber arg, string func)
        {
            switch (func)
            {
                case "abs":
                    arg = BigNumber.Abs(arg);
                    break;
            }
            return arg;
        }
        #endregion
    }
}
