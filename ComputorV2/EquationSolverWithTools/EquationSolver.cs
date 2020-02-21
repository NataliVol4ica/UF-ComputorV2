using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ComputorV2.EquationSolverWithTools
{
    public class EquationSolver
    {
        #region Variables

        private static readonly Regex tokenRegEx =
            new Regex(@"\s*(\d+((\.|,)\d+)?|[xX]|\+|-|\*|\^|=)\s*", RegexOptions.Compiled);

        #endregion

        public string SolveEquation(string equation)
        {
            var returnValue = "";
            var polynomial = Parse(equation);
            ShortenCoef(polynomial);
            returnValue += $"Reduced form: {ToString(polynomial)} = 0\n";
            returnValue += $"Degree: {polynomial.Count - 1}\n";
            if (polynomial.Count > 3)
                throw new Exception($"{returnValue}Degree has to be 0..2. {polynomial.Count - 1} is not.");
            returnValue += Solve(polynomial);
            return returnValue;
        }

        private List<double> Parse(string expression)
        {
            var stringTokens = Tokenize(expression);
            var tokens = RecognizeLexems(stringTokens);
            CompileExpression(tokens, out var coefficients);
            return coefficients;
        }

        private string ToString(List<double> poly)
        {
            var str = "";
            var isFirst = true;
            bool wroteCoef;
            double coef;
            for (var i = 0; i < poly.Count; i++)
            {
                coef = poly[i];
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

                    wroteCoef = false;
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

        private void ShortenCoef(List<double> coefficients)
        {
            var cleanLen = coefficients.Count - 1;
            while (cleanLen > 0 && coefficients[cleanLen] == 0.0)
                cleanLen--;
            coefficients.RemoveRange(cleanLen + 1, coefficients.Count - cleanLen - 1);
        }

        private string Solve(List<double> coefficients)
        {
            var returnValue = "";
            var solution = new List<string>();
            if (coefficients.Count == 1)
            {
                if (coefficients[0] == 0.0)
                    returnValue += "Solution:\nAll the real numbers.\n";
                else
                    returnValue += "There is no solution.\n";
            }
            else if (coefficients.Count == 2)
                returnValue += $"Solution:\nX = {-coefficients[0] / coefficients[1]}";
            else
            {
                var discr = coefficients[1] * coefficients[1] - 4 * coefficients[0] * coefficients[2];
                returnValue += $"D = {coefficients[1]}^2 - 4*{coefficients[0]}*{coefficients[2]} = {discr}";
                if (discr == 0)
                {
                    returnValue += "\n";
                    returnValue += $"X = -{coefficients[1]}/(2*{coefficients[2]})\n";
                    var x = -coefficients[1] / (2 * coefficients[2]);
                    returnValue += $"Solution:\nX = {x}\n";
                }
                else if (discr > 0)
                {
                    returnValue += " > 0\n";
                    returnValue += $"X = (-{coefficients[1]} +- sqrt({discr}))/(2*{coefficients[2]})\n";
                    var x1 = (-coefficients[1] + Math.Sqrt(discr)) / (2 * coefficients[2]);
                    var x2 = (-coefficients[1] - Math.Sqrt(discr)) / (2 * coefficients[2]);
                    returnValue += $"Solution:\nX1 = {x1}\nX2 = {x2}\n";
                }
                else
                {
                    returnValue += " < 0\n";
                    returnValue += $"X = (-{coefficients[1]} +- sqrt({discr}))/(2*{coefficients[2]})\n";
                    returnValue += "Solution:\n";
                    var a1 = -coefficients[1] / (2 * coefficients[2]);
                    var a2 = Math.Abs(Math.Sqrt(-discr) / (2 * coefficients[2]));
                    var s1 = a1 != 0 ? a1 + " " : "";
                    var s2 = a2 != 1 ? " " + a2 : "";
                    returnValue += $"X1 = {s1}+{s2}i\n";
                    returnValue += $"X2 = {s1}-{s2}i\n";
                }
            }

            return returnValue;
        }

        private Queue<string> Tokenize(string expression)
        {
            var stringTokens = new Queue<string>();
            var lastMatchPos = 0;
            var lastMatchLen = 0;
            var errorMessage = "";
            var match = tokenRegEx.Match(expression);
            while (match.Success)
            {
                if (lastMatchPos + lastMatchLen < match.Index)
                {
                    errorMessage += string.Format("Unknown lexem \"{0}\" at position {1}.\n",
                        expression.Substring(lastMatchLen + lastMatchPos, match.Index - lastMatchLen - lastMatchPos),
                        lastMatchLen + lastMatchPos + 1);
                }

                lastMatchPos = match.Index;
                lastMatchLen = match.Value.Length;
                stringTokens.Enqueue(match.Value.Trim());
                match = match.NextMatch();
            }

            if (lastMatchPos + lastMatchLen < expression.Length)
                errorMessage += string.Format("Unknown lexem \"{0}\" at position {1}.\n",
                    expression.Substring(lastMatchLen + lastMatchPos, expression.Length - lastMatchLen - lastMatchPos),
                    lastMatchLen + lastMatchPos + 1);
            if (errorMessage.Length > 0)
                throw new EquationSolverLexicalException(errorMessage);
            return stringTokens;
        }

        private List<PolyToken> RecognizeLexems(Queue<string> stringTokens)
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

        private void AddCoef(List<double> coefficients, int pow, double coef)
        {
            if (coefficients.Count <= pow)
                coefficients.AddRange(Enumerable.Repeat(0.0, pow - coefficients.Count + 1));
            coefficients[pow] += coef;
        }

        private void CompileExpression(List<PolyToken> tokens, out List<double> coefficients)
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
                        throw new EquationSolverSyntaxException("Expression is missing it's left part");
                    if (metEquation)
                        throw new EquationSolverSyntaxException("Expression cannot have more than one equation");
                    metEquation = true;
                    isStart = true;
                    tokenIndex++;
                    if (tokenIndex == tokens.Count)
                        throw new EquationSolverSyntaxException("Expression is missing it's right part");
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
                    throw new EquationSolverSyntaxException("Expression cannot be ended by operator");
                if (!isStart && numOfOperators == 0)
                    throw new EquationSolverSyntaxException("Expression is missing operator");
                isStart = false;
                if (tokens[tokenIndex].tokenType == TokenType.Number)
                {
                    double.TryParse(tokens[tokenIndex++].str.Replace('.', ','), out coeff);
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
                        throw new EquationSolverSyntaxException(string.Format("Pow has to be integer. {0} is not.",
                            tokens[tokenIndex].str));
                    double.TryParse(tokens[tokenIndex].str.Replace('.', ','), out doublePow);
                    pow = (int) doublePow;
                    if (pow < 0)
                        throw new EquationSolverSyntaxException(string.Format("Pow has to be >= 0. {0} is not.",
                            tokens[tokenIndex].str));
                    AddCoef(coefficients, pow, sign * coeff);
                    sign = 1;
                    tokenIndex++;
                }
                else
                    throw new EquationSolverSyntaxException("Expression is missing X^N");
            }

            if (!metEquation)
                throw new EquationSolverSyntaxException("Expression is missing \"=\"");
        }

        #region Enums and Structs

        protected enum TokenType
        {
            Number,
            Operator,
            Pow,
            Equation,
            Var
        }

        protected struct PolyToken
        {
            public PolyToken(string str, TokenType tokenType)
            {
                this.str = str;
                this.tokenType = tokenType;
            }

            public TokenType tokenType;
            public string str;

            public override string ToString()
            {
                return str;
            }
        }

        #endregion
    }
}