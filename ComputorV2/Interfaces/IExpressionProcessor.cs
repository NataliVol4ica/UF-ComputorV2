using System.Collections.Generic;

namespace ComputorV2
{
    public interface IExpressionProcessor
    {
        Expression CreateExpression(List<RPNToken> tokens, bool isFunction = false);
        Expression CreateExpression(string str, bool isFunction = false, string functionParameterName = null, bool detailedMode = false);
    }
}