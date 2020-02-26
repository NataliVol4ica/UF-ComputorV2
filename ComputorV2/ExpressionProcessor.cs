using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using BigNumbers;

namespace ComputorV2
{
    public class ExpressionProcessor : IExpressionProcessor
    {
        public ExpressionProcessor(IVariableStorage variableStorage)
        {
            _variableStorage = variableStorage;
        }

        public Expression CreateExpression(string str, bool detailedMode = false)
        {
            var stringTokens = Tokenize(str.ToLower());
            var tokens = RecognizeLexems(stringTokens);
            var bigNumberResult = Simplify(tokens, detailedMode);
            var newTokenList = new List<RPNToken>
            {
                new RPNToken
                {
                    str = bigNumberResult.ToString(),
                    tokenType = TokenType.DecimalNumber
                }
            };
            return new Expression(newTokenList, false, str);
        }

        public BigNumber SimplifyTokensIntoBigNumber(List<RPNToken> tokens)
        {
            return Simplify(tokens);
        }

        public Expression CreateFunctionExpression(string funcExpression, string paramName)
        {
            var stringTokens = Tokenize(funcExpression.ToLower());
            var tokens = RecognizeLexems(stringTokens, paramName);
            try
            {
                var tokensForTrySolve = tokens
                    .Select(t =>
                        t.tokenType == TokenType.FunctionParameter
                            ? new RPNToken {str = "1", tokenType = TokenType.DecimalNumber}
                            : t)
                    .ToList();
                var bigNumberResult = Simplify(tokensForTrySolve);
            }
            catch (Exception)
            {
                throw new ArgumentException("Error: Cannot create function because its expression is invalid");
            }

            return new Expression(tokens, true, funcExpression);
        }

        // Step 1
        private List<string> Tokenize(string str)
        {
            if (String.IsNullOrWhiteSpace(str))
                throw new ArgumentException("Error: Cannot tokenize empty string");
            var stringTokens = new List<string>();
            var lastMatchPos = 0;
            var lastMatchLen = 0;
            var match = expressionRegex.Match(str);
            while (match.Success)
            {
                lastMatchPos = match.Index;
                lastMatchLen = match.Value.Length;
                stringTokens.Add(match.Value.Trim());
                match = match.NextMatch();
            }

            if (lastMatchPos + lastMatchLen < str.Length)
                throw new ArgumentException("Error: The expression is invalid");
            return stringTokens;
        }

        // Step 2
        private List<RPNToken> RecognizeLexems(List<string> stringTokens,
            string funcParameter = null)
        {
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
                         prev.Value == TokenType.DecimalNumber ||
                         prev.Value == TokenType.ComplexNumber)
                    ) //todo: if new number type is created then append this 'if' with new type
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
                    token = "X";
                }
                else if (_bigDecimalRegex.IsMatch(token))
                    tokenType = TokenType.DecimalNumber;
                else if (_bigComplexRegex.IsMatch(token))
                    tokenType = TokenType.ComplexNumber;
                else if (_variableStorage.ContainsVariable(token))
                    tokenType = TokenType.Variable;
                else if (_variableStorage.ContainsFunction(token))
                    tokenType = TokenType.Function;
                else
                    throw new ArgumentException($"Error: Invalid token: '{token}'");

                prev = tokenType;
                if (tokenType == TokenType.Variable)
                {
                    tokenList.Add(new RPNToken("(", TokenType.OBracket));
                    tokenList.AddRange(_variableStorage[token].Tokens);
                    tokenList.Add(new RPNToken(")", TokenType.CBracket));
                }
                else
                    tokenList.Add(new RPNToken(token, tokenType));
            }

            return tokenList;
        }

        // Step 3
        private BigNumber Simplify(List<RPNToken> tokens, bool detailedMode = false)
        {
            if (tokens is null || tokens.Count == 0)
                return null;
            RPNToken currentToken;
            RPNToken tempToken;
            OperationInfo curOpInfo;
            var bufferStack = new Stack<RPNToken>();
            var outputStack = new Stack<BigNumber>();
            var inputQueue = new Queue<RPNToken>(tokens);

            var queueLen = tokens.Count;

            while (inputQueue.Count > 0)
            {
                currentToken = inputQueue.Dequeue();
                if (currentToken.tokenType == TokenType.DecimalNumber)
                    outputStack.Push(new BigDecimal(currentToken.str));
                else if (currentToken.tokenType == TokenType.ComplexNumber)
                    outputStack.Push(new BigComplex(currentToken.str));
                else if (currentToken.tokenType == TokenType.Function)
                    bufferStack.Push(currentToken);
                else if (currentToken.tokenType == TokenType.BinOp ||
                         currentToken.tokenType == TokenType.UnOp)
                {
                    curOpInfo = GetOperationInfo(currentToken);
                    while (bufferStack.Count() > 0)
                    {
                        var cmpToken = bufferStack.Peek();
                        if (cmpToken.tokenType == TokenType.Function ||
                            IsOperation(cmpToken) &&
                            CompareOperationPriorities(GetOperationInfo(cmpToken), curOpInfo) > 0)
                            CalculateToken(outputStack, bufferStack.Pop());
                        else
                            break;
                    }

                    bufferStack.Push(currentToken);
                }
                else if (currentToken.tokenType == TokenType.OBracket)
                    bufferStack.Push(currentToken);
                else if (currentToken.tokenType == TokenType.CBracket)
                {
                    while ((tempToken = bufferStack.Pop()).tokenType != TokenType.OBracket)
                        CalculateToken(outputStack, tempToken, detailedMode);
                    if (bufferStack.Count() > 0 && bufferStack.Peek().tokenType == TokenType.Function)
                        CalculateToken(outputStack, bufferStack.Pop(), detailedMode);
                }
                else
                    throw new NotImplementedException($"Error: Unimplemented token '{currentToken.str}'");

                if (detailedMode)
                    Console.WriteLine($"Step {queueLen - inputQueue.Count}. Current token is {currentToken} \n " +
                                      $"Input {String.Join(", ", inputQueue)} \n " +
                                      $"Buffer {String.Join(", ", bufferStack)} \n " +
                                      $"Result {String.Join(", ", outputStack)}");
            }

            while (bufferStack.Count() > 0)
                CalculateToken(outputStack, bufferStack.Pop(), detailedMode);
            if (outputStack.Count() > 1)
                throw new ArgumentException(
                    "Error: Cannot calculate this expression. Remaining RPN buffer contains extra numbers.");
            if (outputStack.Count() == 0)
                return new BigDecimal("0");
            return outputStack.Pop();
        }

        // Tools
        private OperationInfo GetOperationInfo(RPNToken token)
        {
            var arity = token.tokenType == TokenType.BinOp ? OpArity.Binary : OpArity.Unary;
            var curOps = opInfoMap[token.str];
            var curOp = curOps.Count() == 1 ? curOps.Single() : curOps.Single(o => o.arity == arity);
            return curOp;
        }

        private bool IsOperation(RPNToken t)
        {
            if (t.tokenType == TokenType.BinOp || t.tokenType == TokenType.UnOp)
                return true;
            return false;
        }

        private int CompareOperationPriorities(OperationInfo left, OperationInfo right)
        {
            if (right.assoc == OpAssoc.Left && right.priority <= left.priority ||
                right.assoc == OpAssoc.Right && right.priority < left.priority)
                return 1;
            return -1;
        }

        private void CalculateToken(Stack<BigNumber> result, RPNToken op, bool detailedMode = false)
        {
            if (op.tokenType == TokenType.Function)
                result.Push(CalculateFunc(result.Pop(), op.str));
            else if (op.tokenType == TokenType.BinOp)
                result.Push(CalculateBinaryOp(result.Pop(), result.Pop(), op.str, detailedMode));
            else if (op.tokenType == TokenType.UnOp)
                result.Push(CalculateUnaryOp(result.Pop(), op.str));
            else
                throw new ArgumentException($"Error: Unexpected token '{op.str}'");
        }

        public List<RPNToken> GetRPNTokensForBigNumber(BigNumber bn)
        {
            var stringTokens = Tokenize(bn.ToString());
            var rpnTokens = RecognizeLexems(stringTokens);
            return rpnTokens;
        }

        #region Regex static data

        private static readonly Regex expressionRegex;
        private static readonly string regexFormat = @"\G\s*({0}|\+|-|\*|/|%|\^|\(|\))\s*";

        private static readonly Dictionary<string, string> _numbersRegexes = new Dictionary<string, string>
        {
            {"VariableNameRegex", @"[a-z]+"},
            {"BigComplexRegex", @"(\d+(\.\d+)?)?i"},
            {"BigDecimalRegex", @"\d+(\.\d+)?"}
        };

        private static readonly Regex _bigDecimalRegex;
        private static readonly Regex _bigComplexRegex;

        private static readonly List<string> funcs = new List<string> {"abs"};
        private static readonly ILookup<string, OperationInfo> opInfoMap;
        private readonly IVariableStorage _variableStorage;

        static ExpressionProcessor()
        {
            _bigDecimalRegex = new Regex($"^{_numbersRegexes["BigDecimalRegex"]}$", RegexOptions.Compiled);
            _bigComplexRegex = new Regex($"^{_numbersRegexes["BigComplexRegex"]}$", RegexOptions.Compiled);

            var numericRegexString = String.Join("|", _numbersRegexes.Select(d => d.Value));
            var funcRegexString = String.Join("|", funcs);
            var innerRegex = $"{numericRegexString}|{funcRegexString}";

            expressionRegex = new Regex(String.Format(regexFormat, innerRegex), RegexOptions.Compiled);

            Debug.WriteLine($"The Expression's regex is: \n{expressionRegex}");

            opInfoMap = new[]
            {
                new OperationInfo("-", OpArity.Binary, 1, OpAssoc.Left),
                new OperationInfo("-", OpArity.Unary, 3, OpAssoc.Right),
                new OperationInfo("+", OpArity.Binary, 1, OpAssoc.Left),
                new OperationInfo("+", OpArity.Unary, 3, OpAssoc.Right),
                new OperationInfo("*", OpArity.Binary, 2, OpAssoc.Left),
                new OperationInfo("/", OpArity.Binary, 2, OpAssoc.Left),
                new OperationInfo("%", OpArity.Binary, 2, OpAssoc.Left),
                new OperationInfo("^", OpArity.Binary, 2, OpAssoc.Right)
            }.ToLookup(op => op.op);
        }

        #endregion

        #region Calculations

        private BigNumber CalculateBinaryOp(BigNumber right, BigNumber left, string op, bool detailedMode = false)
        {
            if (op.Equals("+"))
            {
                var result = left + right;
                if (detailedMode)
                    Console.WriteLine($"   Calculating {left} + {right} = {result}");
                return result;
            }

            if (op.Equals("-"))
            {
                var result = left - right;
                if (detailedMode)
                    Console.WriteLine($"   Calculating {left} - {right} = {result}");
                return result;
            }

            if (op.Equals("*"))
            {
                var result = left * right;
                if (detailedMode)
                    Console.WriteLine($"   Calculating {left} * {right} = {result}");
                return result;
            }

            if (op.Equals("/"))
            {
                var result = left / right;
                if (detailedMode)
                    Console.WriteLine($"   Calculating {left} / {right} = {result}");
                return result;
            }

            if (op.Equals("%"))
            {
                var result = left % right;
                if (detailedMode)
                    Console.WriteLine($"   Calculating {left} % {right} = {result}");
                return result;
            }

            if (op.Equals("^"))
            {
                var result = left.Pow((BigDecimal) right);
                if (detailedMode)
                    Console.WriteLine($"   Calculating {left} ^ {right} = {result}");
                return result;
            }

            throw new NotImplementedException("RPNParser met unimplemented operator \"" + op + "\"");
        }

        private BigNumber CalculateUnaryOp(BigNumber operand, string op)
        {
            BigNumber ret;
            switch (op)
            {
                case "+":
                    ret = operand.Copy();
                    break;
                case "-":
                    ret = operand.Copy();
                    ret.Negate();
                    break;
                default:
                    throw new ArgumentException($"Unsupported operation {op}");
            }

            Console.WriteLine($"   Calculating {op}({operand}) = {ret}");
            return ret;
        }

        private BigNumber CalculateFunc(BigNumber arg, string func)
        {
            BigNumber returnValue;
            switch (func)
            {
                case "abs":
                    returnValue = BigNumber.Abs(arg);
                    Console.WriteLine($"   Calculating |{arg}| = {returnValue}");
                    break;
                default:
                    if (!_variableStorage.ContainsFunction(func))
                        throw new ArgumentException($"Variable storage does not contain function '{func}'");
                    var argumentRpnTokens = GetRPNTokensForBigNumber(arg);
                    returnValue = _variableStorage[func].EvaluateFunctionWithTokens(this, argumentRpnTokens);
                    Console.WriteLine($"   Calculating {func}({arg}) = {returnValue}");
                    break;
            }

            return returnValue;
        }

        #endregion
    }
}