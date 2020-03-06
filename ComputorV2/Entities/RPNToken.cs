namespace ComputorV2
{
    public enum TokenType
    {
        BinOp,
        UnOp,
        OBracket,
        CBracket,
        DecimalNumber,
        Function,
        FunctionParameter,
        Variable,
        ComplexNumber
    }

    public struct RPNToken
    {
        public TokenType tokenType;
        public string str;

        public RPNToken(string str, TokenType tokenType)
        {
            this.str = str;
            this.tokenType = tokenType;
        }

        public override string ToString()
        {
            return str;
        }

        public static RPNToken OpeningBracketToken()
        {
            return new RPNToken
            {
                tokenType = TokenType.OBracket,
                str = "("
            };
        }

        public static RPNToken ClosingBracketToken()
        {
            return new RPNToken
            {
                tokenType = TokenType.CBracket,
                str = ")"
            };
        }

        //todo: sign of complex numbers??

        //public static RPNToken TokenizeBigNumber(BigNumber bn)
        //{
        //    TokenType definedTokenType;
        //    if (bn is BigDecimal)
        //        definedTokenType = TokenType.DecimalNumber;
        //    else if (bn is BigComplex)
        //        definedTokenType = TokenType.ComplexNumber;
        //    else throw new NotSupportedException(
        //        $"TokenizeBigNumber BigNumber of type {bn.GetType()} is not supported");
        //    return new RPNToken
        //    {
        //        tokenType = definedTokenType,
        //        str = bn.CleanString;
        //    }
        //}
    }
}