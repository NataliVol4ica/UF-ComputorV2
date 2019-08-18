using System;
using System.Collections.Generic;
using System.Text;

namespace ComputorV2
{
    public enum OpArity { Unary, Binary }
    public enum OpAssoc { None, Left, Right }

    public struct OperationInfo
    {
        public string op;
        public OpArity arity;
        public int priority;
        public OpAssoc assoc;
        public OperationInfo(string op, OpArity arity, int priority, OpAssoc assoc)
        {
            this.op = op;
            this.arity = arity;
            this.priority = priority;
            this.assoc = assoc;
        }

    }
}
