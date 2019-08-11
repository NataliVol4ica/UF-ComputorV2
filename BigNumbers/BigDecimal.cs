using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace BigNumbers
{
    public class BigDecimal : BigNumber
    {

        private static readonly string delimiter = ".";
        private static readonly Regex validStringRegEx = new
Regex(@"^\s*[+-]?\d+(\.\d+)?\s*$", RegexOptions.Compiled);
        private static readonly Regex cleanStringRegEx =
new Regex(@"[1-9]+[0-9]*(\.[0-9]*[1-9]+)?|0\.[0-9]*[1-9]+", RegexOptions.Compiled);

        private static volatile int _fracPrecision = 20;

        private volatile int _dotPos = 0;
        private volatile int _fracLen = -1;

        private readonly Object dotPosMutex = new Object();
        private readonly Object fracLenMutex = new Object();


        public BigDecimal() { }
        public BigDecimal(BigDecimal from)
        {
            CleanString = from.CleanString;
            if (from.Sign < 0)
                Negate();
            DotPos = from.DotPos;
            Fractional = from.Fractional;
        }
        public BigDecimal(string str)
        {
            if (string.IsNullOrEmpty(str) ||
                string.IsNullOrEmpty(validStringRegEx.Match(str).Value))
                throw new ArgumentException("Invalid argument \"" + str + "\"");
            CleanAndSaveNumericString(str);
        }
        public BigDecimal(decimal number)
        {
            string str = number.ToString();
            string sysDelim = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            str = str.Replace(sysDelim, delimiter);
            if (str[0] == '-')
            {
                CleanString = str.Substring(1);
                Negate();
            }
            else
                CleanString = str;
        }

        public int DotPos
        {
            get
            {
                if (_dotPos == 0)
                {
                    lock (dotPosMutex)
                    {
                        FindDotPos();
                    }
                }
                return _dotPos;
            }
            private set
            {
                _dotPos = value;
            }
        }
        public int Integer
        {
            get
            {
                return DotPos;
            }
        }
        public int Fractional
        {
            get
            {
                if (_fracLen < 0)
                    lock (fracLenMutex)
                    {
                        _fracLen = CleanString.Length - DotPos;
                        if (_fracLen > 0)
                            _fracLen--;
                    }
                return _fracLen;
            }
            private set
            {
                _fracLen = value;
            }
        }
        public static int FracPrecision
        {
            get
            {
                return _fracPrecision;
            }
            set
            {
                if (value < 0)
                    _fracPrecision = 0;
                else
                    _fracPrecision = value;
            }
        }


        public static char ToChar(int digit)
        {
            if (digit >= 0 && digit < 10)
                return digit.ToString()[0];
            return '0';
        }
        public static int ToDigit(char c)
        {
            if (Char.IsDigit(c))
                return Convert.ToInt32(c - '0');
            return -1;
        }

        public static List<int> BigDecimalToIntList(BigDecimal num, int desiredInt = 0, int desiredFrac = 0)
        {
            if (num is null)
                return null;

            List<int> ret = new List<int>();
            int IntZeros, FracZeros;

            IntZeros = Math.Max(num.Integer, desiredInt) - num.Integer;
            FracZeros = Math.Max(num.Fractional, desiredFrac) - num.Fractional;
            ret.AddRange(Enumerable.Repeat(0, FracZeros));
            for (int i = num.CleanString.Length - 1; i >= 0; i--)
                if (num.CleanString[i] != '.')
                    ret.Add(ToDigit(num.CleanString[i]));
            ret.AddRange(Enumerable.Repeat(0, IntZeros));
            return ret;
        }
        public static string IntListToString(List<int> digits, int dotPos = 0)
        {
            int i;
            int reverseDot;
            StringBuilder sb;

            if (digits is null || digits.Count == 0)
                return "";
            if (dotPos < 0)
                dotPos = 1;
            sb = new StringBuilder();
            reverseDot = digits.Count - dotPos;
            for (i = digits.Count - 1; i >= reverseDot; i--)
                sb.Append(ToChar(digits[i]));
            if (i >= 0)
            {
                sb.Append(delimiter);
                while (i >= 0)
                    sb.Append(ToChar(digits[i--]));
            }
            return sb.ToString();
        }
        public static BigDecimal Abs(BigDecimal bf)
        {
            BigDecimal ret = new BigDecimal(bf);
            if (bf.Sign < 0)
                bf.Negate();
            return bf;
        }

        public override void NormalizeList(List<int> digits)
        {
            int i;

            if (digits is null || digits.Count == 0)
                return;
            for (i = 0; i < digits.Count - 1; i++)
            {
                if (digits[i] < 0)
                {
                    digits[i] += 10;
                    digits[i + 1]--;
                }
                else
                {
                    digits[i + 1] += digits[i] / 10;
                    digits[i] %= 10;
                }
            }
            while (digits[i] > 9)
            {
                digits.Add(digits[i] / 10);
                digits[i] %= 10;
                i++;
            }
        }

        public override BigNumber Add(BigNumber op)
        {
            if (!(op is BigDecimal))
                throw new ArgumentException("Cannot Add BigDecimal to " + op.GetType());

            BigDecimal bfLeft = this;
            BigDecimal bfRight = (BigDecimal)op;

            if (bfLeft.Sign != bfRight.Sign)
                return bfLeft.Substract(-bfRight);

            int desiredInt = Math.Max(bfLeft.Integer, bfRight.Integer);
            int desiredFrac = Math.Max(bfLeft.Fractional, bfRight.Fractional);
            var leftList = BigDecimalToIntList(bfLeft, desiredInt, desiredFrac);
            var rightList = BigDecimalToIntList(bfRight, desiredInt, desiredFrac);
            var resultList = leftList.SumWithList(rightList);
            NormalizeList(resultList);

            BigDecimal bfAns = new BigDecimal(IntListToString(resultList, resultList.Count - desiredFrac));
            if (Sign < 0)
                bfAns.Negate();
            return bfAns;
        }
        public override BigNumber Substract(BigNumber op)
        {
            if (!(op is BigDecimal))
                throw new ArgumentException("Cannot Add BigDecimal and " + op.GetType());

            BigDecimal bfLeft = this;
            BigDecimal bfRight = (BigDecimal)op;

            if (bfLeft.Sign > 0 && bfRight.Sign < 0)
                return bfLeft.Add(-bfRight);
            if (bfLeft.Sign < 0 && bfRight.Sign > 0)
                return -(BigDecimal)bfRight.Add(-bfLeft);
            if (bfLeft.Sign < 0 && bfRight.Sign < 0)
                return (-bfRight).Substract(-bfLeft);
            //both operands are > 0 here
            int sign = 1;

            if (bfLeft < bfRight)
            {
                sign = -sign;
                Swap(ref bfLeft, ref bfRight);
            }
            int desiredInt = Math.Max(bfLeft.Integer, bfRight.Integer);
            int desiredFrac = Math.Max(bfLeft.Fractional, bfRight.Fractional);
            var leftList = BigDecimalToIntList(bfLeft, desiredInt, desiredFrac);
            var rightList = BigDecimalToIntList(bfRight, desiredInt, desiredFrac);
            var resultList = leftList.SubByList(rightList);
            NormalizeList(resultList);

            BigDecimal bfAns = new BigDecimal(IntListToString(resultList, resultList.Count - desiredFrac));
            if (sign < 0)
                bfAns.Negate();
            return bfAns;
        }
        public override BigNumber Multiply(BigNumber op)
        {
            if (!(op is BigDecimal))
                throw new ArgumentException("Cannot Add BigDecimal and " + op.GetType());

            BigDecimal bfLeft = this;
            BigDecimal bfRight = (BigDecimal)op;

            if (bfLeft.Integer + bfLeft.Fractional < bfRight.Integer + bfRight.Fractional)
                Swap(ref bfLeft, ref bfRight);
            int newDot = bfLeft.Fractional + bfRight.Fractional;
            var leftList = BigDecimalToIntList(bfLeft);
            var rightList = BigDecimalToIntList(bfRight);
            var resultList = leftList.MulWithList(rightList);
            NormalizeList(resultList);

            BigDecimal bfAns = new BigDecimal(IntListToString(resultList, resultList.Count - newDot));
            if (bfLeft.Sign * bfRight.Sign < 0)
                bfAns.Negate();
            return bfAns;
        }
        public override BigNumber Divide(BigNumber op)
        {
            if (!(op is BigDecimal))
                throw new ArgumentException("Cannot Add BigDecimal and " + op.GetType());

            if (op.CleanString == "0")
                throw new DivideByZeroException();
            BigDecimal bfLeft = this;
            BigDecimal bfRight = (BigDecimal)op;

            int multiplier = Math.Max(bfLeft.Fractional, bfRight.Fractional);
            var leftList = BigDecimalToIntList(bfLeft, 0, multiplier + FracPrecision);
            var rightList = BigDecimalToIntList(bfRight, 0, multiplier);
            leftList.RemoveTailingZeros();
            rightList.RemoveTailingZeros();

            List<int> resultList = leftList.DivByList(rightList, NormalizeList, out List<int> subList);
            int dotPos = resultList.Count - FracPrecision;

            BigDecimal bfAns = new BigDecimal(IntListToString(resultList, dotPos));
            if (bfLeft.Sign * bfRight.Sign < 0)
                bfAns.Negate();
            return bfAns;
        }
        public override BigNumber Mod(BigNumber op)
        {
            if (!(op is BigDecimal))
                throw new ArgumentException("Cannot Add BigDecimal and " + op.GetType());

            if (op.CleanString == "0")
                throw new ArgumentException("Cannot calculate BigDecimal % 0");
            BigDecimal bfLeft = this;
            BigDecimal bfRight = (BigDecimal)op;

            int temp = FracPrecision;
            FracPrecision = 0;
            BigDecimal bfDiv = bfLeft / bfRight;
            FracPrecision = temp;
            BigDecimal bfAns = bfLeft - bfDiv * bfRight;
            return bfAns;
        }

        public override int this[int index]
        {
            get
            {
                if (index < 0 || index >= (CleanString.Length - (DotPos == CleanString.Length ? 0 : 1)))
                    return -1;
                return ToDigit(CleanString[index - (index >= DotPos ? 1 : 0)]);
            }
        }

        public static BigDecimal operator -(BigDecimal num)
        {
            BigDecimal ret = new BigDecimal(num);

            ret.Negate();
            return ret;
        }

        public static BigDecimal operator +(BigDecimal left, BigDecimal right)
        {
            if (left is null || right is null)
                return null;
            return (BigDecimal)left.Add(right);
        }
        public static BigDecimal operator -(BigDecimal left, BigDecimal right)
        {
            if (left is null || right is null)
                return null;
            return (BigDecimal)left.Substract(right);
        }
        public static BigDecimal operator *(BigDecimal left, BigDecimal right)
        {
            if (left is null || right is null)
                return null;
            return (BigDecimal)left.Multiply(right);
        }
        public static BigDecimal operator /(BigDecimal left, BigDecimal right)
        {
            if (left is null || right is null)
                return null;
            return (BigDecimal)left.Divide(right);
        }
        public static BigDecimal operator %(BigDecimal left, BigDecimal right)
        {
            if (left is null || right is null)
                return null;
            return (BigDecimal)left.Mod(right);
        }

        public static bool operator >(BigDecimal left, BigDecimal right)
        {
            if (left.Sign > 0)
            {
                if (right.Sign < 0)
                    return true;
                if (left.Integer > right.Integer)
                    return true;
                if (left.Integer < right.Integer)
                    return false;
                if (string.Compare(left.CleanString, right.CleanString) > 0)
                    return true;
                return false;
            }
            if (right.Sign > 0)
                return false;
            if (left.Integer > right.Integer)
                return false;
            if (left.Integer < right.Integer)
                return true;
            if (string.Compare(left.CleanString, right.CleanString) > 0)
                return false;
            return true;
        }
        public static bool operator <(BigDecimal left, BigDecimal right)
        {
            return (!(left > right));
        }
        public static bool operator ==(BigDecimal left, BigDecimal right)
        {
            if (string.Compare(left.CleanString, right.CleanString) != 0
                || left.Sign != right.Sign)
                return false;
            return true;
        }
        public static bool operator !=(BigDecimal left, BigDecimal right)
        {
            if (string.Compare(left.ToString(), right.ToString()) != 0)
                return false;
            return true;
        }

        protected void CleanAndSaveNumericString(string rawString)
        {
            string substr;

            substr = cleanStringRegEx.Match(rawString).Value;
            if (substr == "")
            {
                CleanString = "0";
                return;
            }
            CleanString = substr;
            if (rawString.Contains("-"))
                Negate();
        }

        public BigDecimal Copy()
        {
            return new BigDecimal(this);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return (this == obj as BigDecimal);
        }


        private void FindDotPos()
        {
            if (_dotPos > 0)
                return;
            _dotPos = CleanString.IndexOf(delimiter);
            if (_dotPos < 0)
                _dotPos = CleanString.Length;
        }

    }
}
