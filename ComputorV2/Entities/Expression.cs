using BigNumbers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

namespace ComputorV2
{
    public class Expression
    {
        public string InitialString { get; private set; }
        public List<RPNToken> Tokens { get; private set; }
        public bool IsFunction { get; private set; }

        public Expression(List<RPNToken> tokens, bool isFunction, string initialString = null)
        {
            Tokens = tokens;
            IsFunction = isFunction;
            if (initialString is null)
                InitialString = this.ToString();
            else
                InitialString = initialString;
        }      
        public override string ToString()
        {
            var str = String.Join(" ", Tokens);
            return str;
        }

        public BigNumber EvaluateFunctionWithTokens(ExpressionProcessor ep, List<RPNToken> parameterTokens)
        {
            if (!IsFunction)
                throw new ArgumentException($"Cannot calculate function value of a variable");
            var resultTokens = new List<RPNToken>();
            var tokensToInsert = new List<RPNToken>();
            tokensToInsert.Add(RPNToken.OpeningBracketToken());
            tokensToInsert.AddRange(parameterTokens);
            tokensToInsert.Add(RPNToken.ClosingBracketToken());
            for (int i = 0; i < Tokens.Count; i++)
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
