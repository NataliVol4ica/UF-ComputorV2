using System;
using System.Collections.Generic;
using System.Text;

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
        Variable
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
    }
}
