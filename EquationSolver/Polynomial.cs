using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using PolynomialExpressionSolver.Exceptions;
using PolynomialExpressionSolver.HelperEntities;

namespace PolynomialExpressionSolver
{
    public static class Polynomial
    {
        private static readonly Regex TokenRegEx =
            new Regex(@"\s*(\d+((\.|,)\d+)?|[xX]|\+|-|\*|\^|=)\s*", RegexOptions.Compiled);

        public static List<double> Parse(string expression, Solution solution)
        {
            List<double> coefficients = default;
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

        public static void ShortenCoef(List<double> coefficients)
        {
            var cleanLen = coefficients.Count - 1;
            while (cleanLen > 0 && coefficients[cleanLen] == 0.0)
                cleanLen--;
            coefficients.RemoveRange(cleanLen + 1, coefficients.Count - cleanLen - 1);
        }

        public static void Solve(List<double> coefficients, Solution solution)
        {
            if (coefficients.Count == 1)
            {
                if (coefficients[0] == 0.0)
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
                var discr = coefficients[1] * coefficients[1] - 4 * coefficients[0] * coefficients[2];
                solution.Logs.Add($"D = {coefficients[1]}^2 - 4*{coefficients[0]}*{coefficients[2]} = {discr}");
                if (discr == 0.0)
                {
                    solution.Logs.Add("D = 0");
                    solution.Logs.Add($"X = -{coefficients[1]}/(2*{coefficients[2]})");
                    var x = -coefficients[1] / (2 * coefficients[2]);
                    solution.SolutionType = SolutionType.Single;
                    solution.Answers.Add($"{x}");
                }
                else if (discr > 0)
                {
                    solution.Logs.Add("D > 0");
                    solution.Logs.Add($"X = (-{coefficients[1]} +- sqrt({discr}))/(2*{coefficients[2]})");
                    var x1 = (-coefficients[1] + Math.Sqrt(discr)) / (2 * coefficients[2]);
                    var x2 = (-coefficients[1] - Math.Sqrt(discr)) / (2 * coefficients[2]);
                    solution.SolutionType = SolutionType.Double;
                    solution.Answers.Add(x1.ToString());
                    solution.Answers.Add(x2.ToString());
                }
                else
                {
                    solution.Logs.Add("D < 0");
                    solution.Logs.Add($"X = (-{coefficients[1]} +- sqrt({discr}))/(2*{coefficients[2]})");
                    var a1 = -coefficients[1] / (2 * coefficients[2]);
                    var a2 = Math.Abs(Math.Sqrt(-discr) / (2 * coefficients[2]));
                    var s1 = a1 != 0 ? a1 + " " : "";
                    var s2 = a2 != 1 ? " " + a2 : "";
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
                    errorMessage += String.Format("Unknown lexem \"{0}\" at position {1}.\n",
                        expression.Substring(lastMatchLen + lastMatchPos, match.Index - lastMatchLen - lastMatchPos),
                        lastMatchLen + lastMatchPos + 1);
                }

                lastMatchPos = match.Index;
                lastMatchLen = match.Value.Length;
                stringTokens.Enqueue(match.Value.Trim());
                match = match.NextMatch();
            }

            if (lastMatchPos + lastMatchLen < expression.Length)
                errorMessage += String.Format("Unknown lexem \"{0}\" at position {1}.\n",
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

        private static void AddCoef(List<double> coefficients, int pow, double coef)
        {
            if (coefficients.Count <= pow)
                coefficients.AddRange(Enumerable.Repeat(0.0, pow - coefficients.Count + 1));
            coefficients[pow] += coef;
        }

        private static void CompileExpression(List<PolyToken> tokens, out List<double> coefficients)
        {
            coefficients = new List<double>();
            double coeff;
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
                        throw new SyntaxException("Expression is missing it's left part");
                    if (metEquation)
                        throw new SyntaxException("Expression cannot have more than one equation");
                    metEquation = true;
                    isStart = true;
                    tokenIndex++;
                    if (tokenIndex == tokens.Count)
                        throw new SyntaxException("Expression is missing it's right part");
                }

                sign = metEquation ? -1 : 1;
                numOfOperators = 0;
                while (tokenIndex < tokens.Count && tokens[tokenIndex].tokenType == TokenType.Operator)
                {
                    if (tokens[tokenIndex].str == "*")
                        throw new Exception("invalid token \"*\"");
                    if (tokens[tokenIndex].str == "-")
                        sign = -sign;
                    tokenIndex++;
                    numOfOperators++;
                }

                if (tokenIndex == tokens.Count)
                    throw new SyntaxException("Expression cannot be ended by operator");
                if (!isStart && numOfOperators == 0)
                    throw new SyntaxException("Expression is missing operator");
                isStart = false;
                if (tokens[tokenIndex].tokenType == TokenType.Number)
                {
                    Double.TryParse(tokens[tokenIndex++].str.Replace('.', ','), out coeff);
                    if (tokenIndex == tokens.Count) //esli 4islo v konce
                    {
                        AddCoef(coefficients, 0, sign * coeff);
                        sign = 1;
                        break;
                    }

                    if (tokens[tokenIndex].str != "*") //esli tolko 4islo
                    {
                        AddCoef(coefficients, 0, sign * coeff);
                        sign = 1;
                        continue;
                    }

                    if (++tokenIndex == tokens.Count)
                        break;
                }
                else
                    coeff = 1;

                if (tokens[tokenIndex].tokenType == TokenType.Var) //esli dalshe idet x
                {
                    tokenIndex++;
                    if (tokenIndex == tokens.Count) //esli prosto x v konce
                    {
                        AddCoef(coefficients, 1, sign * coeff);
                        sign = 1;
                        break;
                    }

                    if (tokens[tokenIndex].str != "^") //esli tolko x
                    {
                        AddCoef(coefficients, 1, sign * coeff);
                        sign = 1;
                        continue;
                    }

                    if (++tokenIndex == tokens.Count)
                        break;
                    if (tokens[tokenIndex].str.Contains("."))
                        throw new SyntaxException(String.Format("Pow has to be integer. {0} is not.",
                            tokens[tokenIndex].str));
                    Double.TryParse(tokens[tokenIndex].str.Replace('.', ','), out doublePow);
                    pow = (int) doublePow;
                    if (pow < 0)
                        throw new SyntaxException(String.Format("Pow has to be >= 0. {0} is not.",
                            tokens[tokenIndex].str));
                    AddCoef(coefficients, pow, sign * coeff);
                    sign = 1;
                    tokenIndex++;
                }
                else
                    throw new SyntaxException("Expression is missing X^N");
            }

            if (!metEquation)
                throw new SyntaxException("Expression is missing '='");
        }
    }
}