using System.Collections.Generic;
using BigNumbers;

namespace ComputorV2
{
    public interface IExpressionProcessor
    {
        BigNumber SimplifyTokensIntoBigNumber(List<RPNToken> tokens);
        Expression CreateExpression(string str, bool detailedMode = false);
        Expression CreateFunctionExpression(string funcExpression, string paramName);
    }
}