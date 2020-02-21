using System;
using System.Collections.Generic;
using BigNumbers;

namespace ComputorV2
{
    public class Expression
    {
        public Expression(List<RPNToken> tokens, bool isFunction, string initialString = null)
        {
            Tokens = tokens;
            IsFunction = isFunction;
            if (initialString is null)
                InitialString = ToString();
            else
                InitialString = initialString;
        }

        public string InitialString { get; private set; }
        public List<RPNToken> Tokens { get; private set; }
        public bool IsFunction { get; private set; }

        public override string ToString()
        {
            var str = String.Join(" ", Tokens);
            return str;
        }

        public BigNumber EvaluateFunctionWithTokens(ExpressionProcessor ep, List<RPNToken> parameterTokens)
        {
            if (!IsFunction)
                throw new ArgumentException("Cannot calculate function value of a variable");
            var resultTokens = new List<RPNToken>();
            var tokensToInsert = new List<RPNToken>();
            tokensToInsert.Add(RPNToken.OpeningBracketToken());
            tokensToInsert.AddRange(parameterTokens);
            tokensToInsert.Add(RPNToken.ClosingBracketToken());
            for (var i = 0; i < Tokens.Count; i++)
            {
                if (Tokens[i].tokenType == TokenType.FunctionParameter)
                    resultTokens.AddRange(tokensToInsert);
                else
                    resultTokens.Add(Tokens[i]);
            }

            return ep.SimplifyTokensIntoBigNumber(resultTokens);
        }
    }
}