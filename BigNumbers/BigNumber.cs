using System;
using System.Collections.Generic;

namespace BigNumbers
{
    public abstract class BigNumber
    {
        public string CleanString { get; protected set; } = "0";
        public int Sign { get; protected set; } = 1;
        public abstract int this[int index] { get; }


        public static void Swap<T>(ref T left, ref T right)
        {
            var buf = left;
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

        public static BigDecimal Sqrt(BigNumber num)
        {
            if (!(num is BigDecimal))
                throw new ArgumentException("Sqrt can only be calculated for BigDecimal");
            BigDecimal number = (BigDecimal) num;
            if (number.IsNegative())
                throw new ArgumentException("Cannot get decimal sqrt of a negative number. Use ComplexSqrt method instead.");
            if (number.ToString() == "0")
                return new BigDecimal(0);
            var bigDecimalTwo = new BigDecimal(2);
            var a = new BigDecimal(1);
            bool pDec = false;
            for (; ; )
            {
                var b = (number / a + a) / bigDecimalTwo;
                if (a == b || a < b && pDec)
                    break;
                pDec = a > b;
                a = b;
            }
            return a;
        }

        public static BigComplex ComplexSqrt(BigNumber num)
        {
            if (!(num is BigDecimal))
                throw new ArgumentException("ComplexSqrt can only be calculated for BigDecimal");
            BigDecimal number = (BigDecimal)num;
            bool isNegative = number.IsNegative();
            if (isNegative)
                number.Negate();
            BigDecimal decimalResult = Sqrt(number);
            string appendix = isNegative ? "i" : "";
            return new BigComplex($"{decimalResult}{appendix}");
        }

        public override string ToString()
        {
            return Sign > 0 ? CleanString : "-" + CleanString;
        }
    }
}