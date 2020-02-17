using System.Collections.Generic;

namespace BigNumbers
{
    public abstract class BigNumber
    {
        public string CleanString { get; protected set; } = "0";
        public int Sign { get; protected set; } = 1;


        public static void Swap<T>(ref T left, ref T right)
        {
            T buf = left;
            left = right;
            right = buf;
        }

        public abstract void NormalizeList(List<int> digits);

        public abstract BigNumber Copy();

        public abstract BigNumber Add(BigNumber op);
        public abstract BigNumber Substract(BigNumber op);
        public abstract BigNumber Multiply(BigNumber op);
        public abstract BigNumber Divide(BigNumber op);
        public abstract BigNumber Mod(BigNumber op);

        public BigNumber Pow(BigDecimal op)
        {
            return PowCalculator.CalculatePow(this, op);
        }

        public abstract BigNumber Negative();
        public abstract BigNumber Abs();
        public abstract void Negate();
        public abstract int this[int index] { get; }

        public static BigNumber operator +(BigNumber left, BigNumber right)
        {
            if (left is null || right is null)
                return null;
            return left.Add(right);
        }

        public static BigNumber operator -(BigNumber left, BigNumber right)
        {
            if (left is null || right is null)
                return null;
            return left.Substract(right);
        }

        public static BigNumber operator *(BigNumber left, BigNumber right)
        {
            if (left is null || right is null)
                return null;
            return left.Multiply(right);
        }

        public static BigNumber operator /(BigNumber left, BigNumber right)
        {
            if (left is null || right is null)
                return null;
            return left.Divide(right);
        }

        public static BigNumber operator %(BigNumber left, BigNumber right)
        {
            if (left is null || right is null)
                return null;
            return left.Mod(right);
        }

        public static BigNumber operator -(BigNumber left)
        {
            if (left is null)
                return null;
            var ret = left.Copy();
            ret.Negate();
            return ret;
        }

        public static BigNumber operator +(BigNumber left)
        {
            if (left is null)
                return null;
            var ret = left.Copy();
            return ret;
        }


        public static BigNumber Abs(BigNumber number)
        {
            return number.Abs();
        }

        public override string ToString()
        {
            return (Sign > 0 ? CleanString : "-" + CleanString);
        }
    }
}