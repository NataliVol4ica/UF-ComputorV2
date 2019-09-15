using System;
using System.Collections.Generic;

namespace BigNumbers
{
    public abstract class BigNumber
    {
        public string CleanString { get; protected set; } = "0";
        public int Sign { get; private set; } = 1;

       
        public static void Swap<T>(ref T left, ref T right)
        {
            T buf = left;
            left = right;
            right = buf;
        }
       
        public abstract void NormalizeList(List<int> digits);        
        public abstract BigNumber Add(BigNumber op);
        public abstract BigNumber Substract(BigNumber op);        
        public abstract BigNumber Multiply(BigNumber op);
        public abstract BigNumber Divide(BigNumber op);
        public abstract BigNumber Mod(BigNumber op);
        public abstract BigNumber Pow(BigNumber op);
        public abstract BigNumber Abs();

        public void Negate()
        {
            if (String.Compare(CleanString, "0") != 0)
                Sign = -Sign;
            else
                Sign = 1;
        }
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
