using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using BigNumbers;
using PolynomialExpressionSolver.Exceptions;
using PolynomialExpressionSolver.HelperEntities;

namespace PolynomialExpressionSolver
{
    public static class Polynomial
    {
        private static readonly Regex TokenRegEx =
            new Regex(@"\s*(\d+((\.|,)\d+)?|[xX]|\+|-|\*|\^|=)\s*", RegexOptions.Compiled);

        private static BigDecimal _bigDecimalTwo = new BigDecimal(2);
        private static BigDecimal _bigDecimalFour = new BigDecimal(4);

        public static List<BigDecimal> Parse(string expression, PolynomialSolution solution)
        {
            List<BigDecimal> coefficients = default;
            try
            {
                var stringTokens = Tokenize(expression);
                var tokens = RecognizeLexems(stringTokens);
                CompileExpression(tokens, out coefficients);
            }
            catch (LexicalException ex)
            {
                solution.ValidationError = ex.Message;
            }

            return coefficients;
        }

        public static string ToString(List<double> poly)
        {
            var str = "";
            var isFirst = true;
            for (var i = 0; i < poly.Count; i++)
            {
                var coef = poly[i];
                if (coef != 0.0)
                {
                    if (isFirst)
                    {
                        if (coef < 0)
                        {
                            str += "-";
                            coef = -coef;
                        }

                        isFirst = false;
                    }
                    else
                    {
                        if (poly[i] < 0)
                        {
                            str += " - ";
                            coef = -coef;
                        }
                        else
                            str += " + ";
                    }

                    var wroteCoef = false;
                    if (i == 0 || i > 0 && coef != 1.0)
                    {
                        str += coef.ToString();
                        wroteCoef = true;
                    }

                    if (i == 0)
                        continue;
                    if (wroteCoef)
                        str += "*";
                    str += "X";
                    if (i == 1)
                        continue;
                    str += "^" + i;
                }
            }

            if (isFirst)
                str = "0";
            return str;
        }

        public static void ShortenCoef(List<BigDecimal> coefficients)
        {
            var cleanLen = coefficients.Count - 1;
            while (cleanLen > 0 && coefficients[cleanLen] == BigDecimal.Zero)
                cleanLen--;
            coefficients.RemoveRange(cleanLen + 1, coefficients.Count - cleanLen - 1);
        }

        public static void Solve(List<BigDecimal> coefficients, PolynomialSolution solution)
        {
            if (coefficients.Count == 1)
            {
                if (coefficients[0] == BigDecimal.Zero)
                    solution.SolutionType = SolutionType.All;
                else
                    solution.SolutionType = SolutionType.None;
            }
            else if (coefficients.Count == 2)
            {
                solution.SolutionType = SolutionType.Single;
                solution.Answers.Add($"{-coefficients[0] / coefficients[1]}");
            }
            else
            {
                var discr = coefficients[1] * coefficients[1] - _bigDecimalFour * coefficients[0] * coefficients[2];
                solution.Logs.Add($"D = {coefficients[1]}^2 - 4*{coefficients[0]}*{coefficients[2]} = {discr}");
                if (discr == BigDecimal.Zero)
                {
                    solution.Logs.Add("D = 0");
                    solution.Logs.Add($"X = -{coefficients[1]}/(2*{coefficients[2]})");
                    var x = -coefficients[1] / (_bigDecimalTwo * coefficients[2]);
                    solution.SolutionType = SolutionType.Single;
                    solution.Answers.Add($"{x}");
                }
                else if (discr > BigDecimal.Zero)
                {
                    solution.Logs.Add("D > 0");
                    solution.Logs.Add($"X = (-{coefficients[1]} +- sqrt({discr}))/(2*{coefficients[2]})");
                    var x1 = (-coefficients[1] + BigDecimal.Sqrt(discr)) / (_bigDecimalTwo * coefficients[2]);
                    var x2 = (-coefficients[1] - BigDecimal.Sqrt(discr)) / (_bigDecimalTwo * coefficients[2]);
                    solution.SolutionType = SolutionType.Double;
                    solution.Answers.Add(x1.ToString());
                    solution.Answers.Add(x2.ToString());
                }
                else
                {
                    solution.Logs.Add("D < 0");
                    solution.Logs.Add($"X = (-{coefficients[1]} +- sqrt({discr}))/(2*{coefficients[2]})");
                    BigDecimal a1 = -coefficients[1] / (_bigDecimalTwo * coefficients[2]);
                    BigDecimal a2 = BigDecimal.Abs(BigDecimal.Sqrt(-discr) / (_bigDecimalTwo * coefficients[2]));
                    var s1 = a1 != BigDecimal.Zero ? a1 + " " : "";
                    var s2 = a2 != BigDecimal.One ? " " + a2 : "";
                    solution.SolutionType = SolutionType.Double;
                    solution.Answers.Add($"{s1}+{s2}i");
                    solution.Answers.Add($"{s1}-{s2}i");
                }
            }
        }

        private static Queue<string> Tokenize(string expression)
        {
            var stringTokens = new Queue<string>();
            var lastMatchPos = 0;
            var lastMatchLen = 0;
            var errorMessage = "";
            var match = TokenRegEx.Match(expression);
            while (match.Success)
            {
                if (lastMatchPos + lastMatchLen < match.Index)
                {
                    errorMessage += string.Format("Error: Unknown lexem \"{0}\" at position {1}.\n",
                        expression.Substring(lastMatchLen + lastMatchPos, match.Index - lastMatchLen - lastMatchPos),
                        lastMatchLen + lastMatchPos + 1);
                }

                lastMatchPos = match.Index;
                lastMatchLen = match.Value.Length;
                stringTokens.Enqueue(match.Value.Trim());
                match = match.NextMatch();
            }

            if (lastMatchPos + lastMatchLen < expression.Length)
                errorMessage += string.Format("Error: Unknown lexem \"{0}\" at position {1}.\n",
                    expression.Substring(lastMatchLen + lastMatchPos, expression.Length - lastMatchLen - lastMatchPos),
                    lastMatchLen + lastMatchPos + 1);
            if (errorMessage.Length > 0)
                throw new LexicalException(errorMessage);
            return stringTokens;
        }

        private static List<PolyToken> RecognizeLexems(Queue<string> stringTokens)
        {
            var tokenQueue = new List<PolyToken>();
            TokenType tokenType;
            foreach (var token in stringTokens)
            {
                if (token == "+" || token == "-" || token == "*")
                    tokenType = TokenType.Operator;
                else if (token == "^")
                    tokenType = TokenType.Pow;
                else if (token == "=")
                    tokenType = TokenType.Equation;
                else if (token == "x" || token == "X")
                    tokenType = TokenType.Var;
                else
                    tokenType = TokenType.Number;
                tokenQueue.Add(new PolyToken(token, tokenType));
            }

            return tokenQueue;
        }

        private static void AddCoef(List<BigDecimal> multipliers, int pow, BigDecimal multiplier)
        {
            if (multipliers.Count <= pow)
            {
                multipliers.AddRange(
                    Enumerable
                        .Range(1, pow - multipliers.Count + 1)
                        .Select(i => new BigDecimal(0)));
            }

            multipliers[pow] += multiplier;
        }

        private static void CompileExpression(List<PolyToken> tokens, out List<BigDecimal> coefficients)
        {
            coefficients = new List<BigDecimal>();
            BigDecimal multiplier;
            int pow;
            double doublePow;
            int sign;
            var tokenIndex = 0;
            int numOfOperators;
            var isStart = true;
            var metEquation = false;
            while (tokenIndex < tokens.Count)
            {
                if (tokens[tokenIndex].tokenType == TokenType.Equation)
                {
                    if (tokenIndex == 0)
                        throw new SyntaxException("Error: Expression is missing its left part");
                    if (metEquation)
                        throw new SyntaxException("Error: Expression cannot have more than one equation");
                    metEquation = true;
                    isStart = true;
                    tokenIndex++;
                    if (tokenIndex == tokens.Count)
                        throw new SyntaxException("Error: Expression is missing it's right part");
                }

                sign = metEquation ? -1 : 1;
                numOfOperators = 0;
                while (tokenIndex < tokens.Count && tokens[tokenIndex].tokenType == TokenType.Operator)
                {
                    if (tokens[tokenIndex].str == "*")
                        throw new Exception("Error: invalid token \"*\"");
                    if (tokens[tokenIndex].str == "-")
                        sign = -sign;
                    tokenIndex++;
                    numOfOperators++;
                }

                if (tokenIndex == tokens.Count)
                    throw new SyntaxException("Error: Expression cannot be ended by operator");
                if (!isStart && numOfOperators == 0)
                    throw new SyntaxException("Error: Expression is missing operator");
                isStart = false;
                if (tokens[tokenIndex].tokenType == TokenType.Number)
                {
                    multiplier = new BigDecimal(tokens[tokenIndex++].str.Replace('.', ','));
                    if (tokenIndex == tokens.Count) //esli 4islo v konce
                    {
                        if (multiplier.Sign != sign)
                            multiplier.Negate();
                        AddCoef(coefficients, 0, multiplier);
                        break;
                    }

                    if (tokens[tokenIndex].str != "*") //esli tolko 4islo
                    {
                        if (multiplier.Sign != sign)
                            multiplier.Negate();
                        AddCoef(coefficients, 0, multiplier);
                        continue;
                    }

                    if (++tokenIndex == tokens.Count)
                        break;
                }
                else
                    multiplier = new BigDecimal(1);

                if (tokens[tokenIndex].tokenType == TokenType.Var) //esli dalshe idet x
                {
                    tokenIndex++;
                    if (tokenIndex == tokens.Count) //esli prosto x v konce
                    {
                        if (multiplier.Sign != sign)
                            multiplier.Negate();
                        AddCoef(coefficients, 1, multiplier);
                        break;
                    }

                    if (tokens[tokenIndex].str != "^") //esli tolko x
                    {
                        if (multiplier.Sign != sign)
                            multiplier.Negate();
                        AddCoef(coefficients, 1, multiplier);
                        continue;
                    }

                    if (++tokenIndex == tokens.Count)
                        break;
                    if (tokens[tokenIndex].str.Contains("."))
                        throw new SyntaxException(string.Format("Error: Pow has to be integer. {0} is not.",
                            tokens[tokenIndex].str));
                    Double.TryParse(tokens[tokenIndex].str.Replace('.', ','), out doublePow);
                    pow = (int) doublePow;
                    if (pow < 0)
                        throw new SyntaxException(string.Format("Error: Pow has to be >= 0. {0} is not.",
                            tokens[tokenIndex].str));

                    if (multiplier.Sign != sign)
                        multiplier.Negate();
                    AddCoef(coefficients, pow, multiplier);
                    tokenIndex++;
                }
                else
                    throw new SyntaxException("Error: Expression is missing X^N");
            }

            if (!metEquation)
                throw new SyntaxException("Error: Expression is missing '='");
        }
    }
}