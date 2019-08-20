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
    }
}
