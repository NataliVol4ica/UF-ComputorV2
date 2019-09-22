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
        private static readonly Regex validStringRegEx = new Regex(
            @"^\s*[+-]?\d+(\.\d+)?\s*$",
            RegexOptions.Compiled);
        private static readonly Regex cleanStringRegEx = new Regex(
            @"[1-9]+[0-9]*(\.[0-9]*[1-9]+)?|0\.[0-9]*[1-9]+", 
            RegexOptions.Compiled);

        private static volatile int _fracPrecision = 20;

        private int? _dotPos = null;
        private int? _fracLen = null;
        private bool? _isInteger = null;
        private bool? _isEven = null;

        private readonly Object dotPosMutex = new Object();
        private readonly Object fracLenMutex = new Object();
        private readonly Object isIntegerMutex = new Object();
        private readonly Object isEvenMutex = new Object();

        public BigDecimal() { }
        public BigDecimal(BigDecimal from)
        {
            CleanString = from.CleanString;
            if (from.Sign < 0)
                Negate();
            DotPos = from.DotPos;
            FractionalLength = from.FractionalLength;
        }
        public BigDecimal(string str)
        {
            if (string.IsNullOrEmpty(str) ||
                string.IsNullOrEmpty(validStringRegEx.Match(str).Value))
                throw new ArgumentException("Cannot create BigDecimal of \"" + str + "\"");
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
                if (!_dotPos.HasValue)
                {
                    lock (dotPosMutex)
                    {
                        FindDotPos();
                    }
                }
                return _dotPos.Value;
            }
            private set
            {
                _dotPos = value;
            }
        }
        public virtual bool IsInteger
        {
            get
            {
                if (!_isInteger.HasValue)
                {
                    lock (isIntegerMutex)
                    {
                        if (FractionalLength == 0)
                            _isInteger = true;
                        else
                            _isInteger = false;
                    }
                }
                return _isInteger.Value;
            }
            private set
            {
                _isInteger = value;
            }
        }
        public bool IsEven
        {
            get
            {
                if (!_isEven.HasValue)
                {
                    lock (isEvenMutex)
                    {
                        if (!IsInteger)
                            _isEven = false;
                        else
                            _isEven = ConvertCharToDigit(CleanString[CleanString.Length - 1]) % 2 == 0;
                    }
                }
                return _isEven.Value;
            }
            private set
            {
                _isEven = value;
            }
        }
        public int IntegerLength
        {
            get
            {
                return DotPos;
            }
        }
        public int FractionalLength
        {
            get
            {
                if (!_fracLen.HasValue)
                    lock (fracLenMutex)
                    {
                        _fracLen = CleanString.Length - DotPos;
                        if (_fracLen > 0)
                            _fracLen--;
                    }
                return _fracLen.Value;
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


        public static char ConvertDigitToChar(int digit)
        {
            if (digit >= 0 && digit < 10)
                return digit.ToString()[0];
            return '0';
        }
        public static int ConvertCharToDigit(char c)
        {
            if (Char.IsDigit(c))
                return Convert.ToInt32(c - '0');
            return -1;
        }

        public static List<int> ConvertBigDecimalToIntList(BigDecimal num, int desiredInt = 0, int desiredFrac = 0)
        {
            if (num is null)
                return null;

            List<int> ret = new List<int>();
            int IntZeros, FracZeros;

            IntZeros = Math.Max(num.IntegerLength, desiredInt) - num.IntegerLength;
            FracZeros = Math.Max(num.FractionalLength, desiredFrac) - num.FractionalLength;
            ret.AddRange(Enumerable.Repeat(0, FracZeros));
            for (int i = num.CleanString.Length - 1; i >= 0; i--)
                if (num.CleanString[i] != '.')
                    ret.Add(ConvertCharToDigit(num.CleanString[i]));
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
            if (dotPos <= 0)
            {
                digits.AddRange(Enumerable.Repeat(0, 1 - dotPos));
                dotPos = 1;
            }
            sb = new StringBuilder();
            reverseDot = digits.Count - dotPos;
            for (i = digits.Count - 1; i >= reverseDot; i--)
                sb.Append(ConvertDigitToChar(digits[i]));
            if (i >= 0)
            {
                sb.Append(delimiter);
                while (i >= 0)
                    sb.Append(ConvertDigitToChar(digits[i--]));
            }
            var result = sb.ToString();
            return result;
        }

        public override BigNumber Abs()
        {
            BigDecimal ret = new BigDecimal(this);
            if (ret.Sign < 0)
                ret.Negate();
            return ret;
        }

        public override void Negate()
        {
            if (String.Compare(CleanString, "0") != 0)
                Sign = -Sign;
            else
                Sign = 1;
        }
        public override BigNumber Negative()
        {
            BigDecimal ret = new BigDecimal(this);
            ret.Negate();
            return ret;
        }
        public override BigNumber Copy()
        {
            BigDecimal ret = new BigDecimal(this);
            return ret;
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
            if (op is BigComplex)
                return new BigComplex(this) + (BigComplex)op;
            if (!(op is BigDecimal))
                throw new ArgumentException("Cannot Add BigDecimal to " + op.GetType());

            BigDecimal bfLeft = this;
            BigDecimal bfRight = (BigDecimal)op;

            if (bfLeft.Sign != bfRight.Sign)
                return bfLeft.Substract(-bfRight);

            int desiredInt = Math.Max(bfLeft.IntegerLength, bfRight.IntegerLength);
            int desiredFrac = Math.Max(bfLeft.FractionalLength, bfRight.FractionalLength);
            var leftList = ConvertBigDecimalToIntList(bfLeft, desiredInt, desiredFrac);
            var rightList = ConvertBigDecimalToIntList(bfRight, desiredInt, desiredFrac);
            var resultList = leftList.SumWithList(rightList);
            NormalizeList(resultList);

            BigDecimal bfAns = new BigDecimal(IntListToString(resultList, resultList.Count - desiredFrac));
            if (Sign < 0)
                bfAns.Negate();
            return bfAns;
        }
        public override BigNumber Substract(BigNumber op)
        {
            if (op is BigComplex)
                return new BigComplex(this) + (BigComplex)op;
            if (!(op is BigDecimal))
                throw new ArgumentException("Cannot Sub BigDecimal and " + op.GetType());

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
            int desiredInt = Math.Max(bfLeft.IntegerLength, bfRight.IntegerLength);
            int desiredFrac = Math.Max(bfLeft.FractionalLength, bfRight.FractionalLength);
            var leftList = ConvertBigDecimalToIntList(bfLeft, desiredInt, desiredFrac);
            var rightList = ConvertBigDecimalToIntList(bfRight, desiredInt, desiredFrac);
            var resultList = leftList.SubByList(rightList);
            NormalizeList(resultList);

            BigDecimal bfAns = new BigDecimal(IntListToString(resultList, resultList.Count - desiredFrac));
            if (sign < 0)
                bfAns.Negate();
            return bfAns;
        }
        public override BigNumber Multiply(BigNumber op)
        {
            if (op is BigComplex)
                return new BigComplex(this) + (BigComplex)op;
            if (!(op is BigDecimal))
                throw new ArgumentException("Cannot Mul BigDecimal and " + op.GetType());

            BigDecimal bfLeft = this;
            BigDecimal bfRight = (BigDecimal)op;

            if (bfLeft.IntegerLength + bfLeft.FractionalLength < bfRight.IntegerLength + bfRight.FractionalLength)
                Swap(ref bfLeft, ref bfRight);
            int newDot = bfLeft.FractionalLength + bfRight.FractionalLength;
            var leftList = ConvertBigDecimalToIntList(bfLeft);
            var rightList = ConvertBigDecimalToIntList(bfRight);
            var resultList = leftList.MulWithList(rightList);
            NormalizeList(resultList);

            BigDecimal bfAns = new BigDecimal(IntListToString(resultList, resultList.Count - newDot));
            if (bfLeft.Sign * bfRight.Sign < 0)
                bfAns.Negate();
            return bfAns;
        }
        public override BigNumber Divide(BigNumber op)
        {
            if (op is BigComplex)
                return new BigComplex(this) + (BigComplex)op;
            if (!(op is BigDecimal))
                throw new ArgumentException("Cannot Div BigDecimal and " + op.GetType());

            if (op.CleanString == "0")
                throw new DivideByZeroException();
            BigDecimal bfLeft = this;
            BigDecimal bfRight = (BigDecimal)op;

            int multiplier = Math.Max(bfLeft.FractionalLength, bfRight.FractionalLength);
            var leftList = ConvertBigDecimalToIntList(bfLeft, 0, multiplier + FracPrecision);
            var rightList = ConvertBigDecimalToIntList(bfRight, 0, multiplier);
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
            if (op is BigComplex)
                return new BigComplex(this) + (BigComplex)op;
            if (!(op is BigDecimal))
                throw new ArgumentException("Cannot Mod BigDecimal and " + op.GetType());

            if (op.CleanString == "0")
                throw new ArgumentException("Cannot calculate BigDecimal % 0");
            BigDecimal bdLeft = this;
            BigDecimal bdRight = (BigDecimal)op;

            int temp = FracPrecision;
            FracPrecision = 0;
            BigDecimal bfDiv = bdLeft / bdRight;
            FracPrecision = temp;
            BigDecimal bfAns = bdLeft - bfDiv * bdRight;
            return bfAns;
        }
 
        public override int this[int index]
        {
            get
            {
                if (index < 0 || index >= (CleanString.Length - (DotPos == CleanString.Length ? 0 : 1)))
                    return -1;
                return ConvertCharToDigit(CleanString[index - (index >= DotPos ? 1 : 0)]);
            }
        }

        public static BigDecimal operator -(BigDecimal num)
        {
            BigDecimal ret = new BigDecimal(num);

            ret.Negate();
            return ret;
        }

        public static BigDecimal operator +(BigDecimal left, BigNumber right)
        {
            if (left is null || right is null)
                return null;
            return (BigDecimal)left.Add(right);
        }
        public static BigDecimal operator -(BigDecimal left, BigNumber right)
        {
            if (left is null || right is null)
                return null;
            return (BigDecimal)left.Substract(right);
        }
        public static BigDecimal operator *(BigDecimal left, BigNumber right)
        {
            if (left is null || right is null)
                return null;
            return (BigDecimal)left.Multiply(right);
        }
        public static BigDecimal operator /(BigDecimal left, BigNumber right)
        {
            if (left is null || right is null)
                return null;
            return (BigDecimal)left.Divide(right);
        }
        public static BigDecimal operator %(BigDecimal left, BigNumber right)
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
                if (left.IntegerLength > right.IntegerLength)
                    return true;
                if (left.IntegerLength < right.IntegerLength)
                    return false;
                if (string.Compare(left.CleanString, right.CleanString) > 0)
                    return true;
                return false;
            }
            if (right.Sign > 0)
                return false;
            if (left.IntegerLength > right.IntegerLength)
                return false;
            if (left.IntegerLength < right.IntegerLength)
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
            if (_dotPos.HasValue)
                return;
            _dotPos = CleanString.IndexOf(delimiter);
            if (_dotPos < 0)
                _dotPos = CleanString.Length;
        }

    }
}
