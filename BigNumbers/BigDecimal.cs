﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

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

        private readonly object _dotPosMutex = new object();
        private readonly object _fracLenMutex = new object();
        private readonly object _isEvenMutex = new object();
        private readonly object _isIntegerMutex = new object();

        private int? _dotPos;
        private int? _fracLen;
        private bool? _isEven;
        private bool? _isInteger;

        #region Constructors

        public BigDecimal()
        {
        }

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
            var str = number.ToString();
            var sysDelim = System.Threading.Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            str = str.Replace(sysDelim, delimiter);
            if (str[0] == '-')
            {
                CleanString = str.Substring(1);
                Negate();
            }
            else
                CleanString = str;
        }

        #endregion Constructors

        #region Properties

        public int DotPos
        {
            get
            {
                if (!_dotPos.HasValue)
                {
                    lock (_dotPosMutex)
                    {
                        FindDotPos();
                    }
                }

                return _dotPos.Value;
            }
            private set => _dotPos = value;
        }

        public virtual bool IsInteger
        {
            get
            {
                if (!_isInteger.HasValue)
                {
                    lock (_isIntegerMutex)
                    {
                        if (FractionalLength == 0)
                            _isInteger = true;
                        else
                            _isInteger = false;
                    }
                }

                return _isInteger.Value;
            }
            private set => _isInteger = value;
        }

        public bool IsEven
        {
            get
            {
                if (!_isEven.HasValue)
                {
                    lock (_isEvenMutex)
                    {
                        if (!IsInteger)
                            _isEven = false;
                        else
                            _isEven = ConvertCharToDigit(CleanString[CleanString.Length - 1]) % 2 == 0;
                    }
                }

                return _isEven.Value;
            }
            private set => _isEven = value;
        }

        public int IntegerLength => DotPos;

        public int FractionalLength
        {
            get
            {
                if (!_fracLen.HasValue)
                    lock (_fracLenMutex)
                    {
                        _fracLen = CleanString.Length - DotPos;
                        if (_fracLen > 0)
                            _fracLen--;
                    }

                return _fracLen.Value;
            }
            private set => _fracLen = value;
        }

        public static int FracPrecision
        {
            get => _fracPrecision;
            set
            {
                if (value < 0)
                    _fracPrecision = 0;
                else
                    _fracPrecision = value;
            }
        }

        #endregion Properties

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

            var ret = new List<int>();
            int IntZeros, FracZeros;

            IntZeros = Math.Max(num.IntegerLength, desiredInt) - num.IntegerLength;
            FracZeros = Math.Max(num.FractionalLength, desiredFrac) - num.FractionalLength;
            ret.AddRange(Enumerable.Repeat(0, FracZeros));
            for (var i = num.CleanString.Length - 1; i >= 0; i--)
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
            var ret = new BigDecimal(this);
            if (ret.Sign < 0)
                ret.Negate();
            return ret;
        }

        public static BigDecimal Abs(BigDecimal bd)
        {
            return bd.Abs() as BigDecimal;
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
            var ret = new BigDecimal(this);
            ret.Negate();
            return ret;
        }

        public override BigNumber Copy()
        {
            var ret = new BigDecimal(this);
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
                return new BigComplex(this) + (BigComplex) op;
            if (!(op is BigDecimal))
                throw new ArgumentException("Cannot Add BigDecimal to " + op.GetType());

            var bfLeft = this;
            var bfRight = (BigDecimal) op;

            if (bfLeft.Sign != bfRight.Sign)
                return bfLeft.Substract(-bfRight);

            var desiredInt = Math.Max(bfLeft.IntegerLength, bfRight.IntegerLength);
            var desiredFrac = Math.Max(bfLeft.FractionalLength, bfRight.FractionalLength);
            var leftList = ConvertBigDecimalToIntList(bfLeft, desiredInt, desiredFrac);
            var rightList = ConvertBigDecimalToIntList(bfRight, desiredInt, desiredFrac);
            var resultList = leftList.SumWithList(rightList);
            NormalizeList(resultList);

            var bfAns = new BigDecimal(IntListToString(resultList, resultList.Count - desiredFrac));
            if (Sign < 0)
                bfAns.Negate();
            return bfAns;
        }

        public override BigNumber Substract(BigNumber op)
        {
            if (op is BigComplex)
                return new BigComplex(this) - (BigComplex) op;
            if (!(op is BigDecimal))
                throw new ArgumentException("Cannot Sub BigDecimal and " + op.GetType());

            var bfLeft = this;
            var bfRight = (BigDecimal) op;

            if (bfLeft.Sign > 0 && bfRight.Sign < 0)
                return bfLeft.Add(-bfRight);
            if (bfLeft.Sign < 0 && bfRight.Sign > 0)
                return -(BigDecimal) bfRight.Add(-bfLeft);
            if (bfLeft.Sign < 0 && bfRight.Sign < 0)
                return (-bfRight).Substract(-bfLeft);
            //both operands are > 0 here
            var sign = 1;

            if (bfLeft < bfRight)
            {
                sign = -sign;
                Swap(ref bfLeft, ref bfRight);
            }

            var desiredInt = Math.Max(bfLeft.IntegerLength, bfRight.IntegerLength);
            var desiredFrac = Math.Max(bfLeft.FractionalLength, bfRight.FractionalLength);
            var leftList = ConvertBigDecimalToIntList(bfLeft, desiredInt, desiredFrac);
            var rightList = ConvertBigDecimalToIntList(bfRight, desiredInt, desiredFrac);
            var resultList = leftList.SubByList(rightList);
            NormalizeList(resultList);

            var bfAns = new BigDecimal(IntListToString(resultList, resultList.Count - desiredFrac));
            if (sign < 0)
                bfAns.Negate();
            return bfAns;
        }

        public override BigNumber Multiply(BigNumber op)
        {
            if (op is BigComplex)
                return new BigComplex(this) * (BigComplex) op;
            if (!(op is BigDecimal))
                throw new ArgumentException("Cannot Mul BigDecimal and " + op.GetType());

            var bfLeft = this;
            var bfRight = (BigDecimal) op;

            if (bfLeft.IntegerLength + bfLeft.FractionalLength < bfRight.IntegerLength + bfRight.FractionalLength)
                Swap(ref bfLeft, ref bfRight);
            var newDot = bfLeft.FractionalLength + bfRight.FractionalLength;
            var leftList = ConvertBigDecimalToIntList(bfLeft);
            var rightList = ConvertBigDecimalToIntList(bfRight);
            var resultList = leftList.MulWithList(rightList);
            NormalizeList(resultList);

            var bfAns = new BigDecimal(IntListToString(resultList, resultList.Count - newDot));
            if (bfLeft.Sign * bfRight.Sign < 0)
                bfAns.Negate();
            return bfAns;
        }

        public override BigNumber Divide(BigNumber op)
        {
            if (op is BigComplex)
                return new BigComplex(this) / (BigComplex) op;
            if (!(op is BigDecimal))
                throw new ArgumentException("Cannot Div BigDecimal and " + op.GetType());

            if (op.CleanString == "0")
                throw new DivideByZeroException();
            var bfLeft = this;
            var bfRight = (BigDecimal) op;

            var multiplier = Math.Max(bfLeft.FractionalLength, bfRight.FractionalLength);
            var leftList = ConvertBigDecimalToIntList(bfLeft, 0, multiplier + FracPrecision);
            var rightList = ConvertBigDecimalToIntList(bfRight, 0, multiplier);
            leftList.RemoveTailingZeros();
            rightList.RemoveTailingZeros();

            var resultList = leftList.DivByList(rightList, NormalizeList, out var subList);
            var dotPos = resultList.Count - FracPrecision;

            var bfAns = new BigDecimal(IntListToString(resultList, dotPos));
            if (bfLeft.Sign * bfRight.Sign < 0)
                bfAns.Negate();
            return bfAns;
        }

        public override BigNumber Mod(BigNumber op)
        {
            if (op is BigComplex)
                return new BigComplex(this) % (BigComplex) op;
            if (!(op is BigDecimal))
                throw new ArgumentException("Cannot Mod BigDecimal and " + op.GetType());

            if (op.CleanString == "0")
                throw new ArgumentException("Cannot calculate BigDecimal % 0");
            var bdLeft = this;
            var bdRight = (BigDecimal) op;

            var temp = FracPrecision;
            FracPrecision = 0;
            var bfDiv = bdLeft / bdRight;
            FracPrecision = temp;
            var bfAns = bdLeft - bfDiv * bdRight;
            return bfAns;
        }

        #region Canonical operators overload

        public static BigDecimal operator -(BigDecimal num)
        {
            var ret = new BigDecimal(num);

            ret.Negate();
            return ret;
        }

        #region Ops with BigDecimal

        public static BigDecimal operator +(BigDecimal left, BigDecimal right)
        {
            if (left is null || right is null)
                return null;
            return (BigDecimal) left.Add(right);
        }

        public static BigDecimal operator -(BigDecimal left, BigDecimal right)
        {
            if (left is null || right is null)
                return null;
            return (BigDecimal) left.Substract(right);
        }

        public static BigDecimal operator *(BigDecimal left, BigDecimal right)
        {
            if (left is null || right is null)
                return null;
            return (BigDecimal) left.Multiply(right);
        }

        public static BigDecimal operator /(BigDecimal left, BigDecimal right)
        {
            if (left is null || right is null)
                return null;
            return (BigDecimal) left.Divide(right);
        }

        public static BigDecimal operator %(BigDecimal left, BigDecimal right)
        {
            if (left is null || right is null)
                return null;
            return (BigDecimal) left.Mod(right);
        }

        #endregion Ops with BigDecimal

        #region Ops with BigNumber

        public static BigNumber operator +(BigDecimal left, BigNumber right)
        {
            if (left is null || right is null)
                return null;
            return left.Add(right);
        }

        public static BigNumber operator -(BigDecimal left, BigNumber right)
        {
            if (left is null || right is null)
                return null;
            return left.Substract(right);
        }

        public static BigNumber operator *(BigDecimal left, BigNumber right)
        {
            if (left is null || right is null)
                return null;
            return left.Multiply(right);
        }

        public static BigNumber operator /(BigDecimal left, BigNumber right)
        {
            if (left is null || right is null)
                return null;
            return left.Divide(right);
        }

        public static BigNumber operator %(BigDecimal left, BigNumber right)
        {
            if (left is null || right is null)
                return null;
            return left.Mod(right);
        }

        #endregion Ops with BigNumber

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
            return !(left > right);
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
            if (left == right)
                return false;
            return true;
        }

        public override int this[int index]
        {
            get
            {
                if (index < 0 || index >= CleanString.Length - (DotPos == CleanString.Length ? 0 : 1))
                    return -1;
                return ConvertCharToDigit(CleanString[index - (index >= DotPos ? 1 : 0)]);
            }
        }

        #endregion Canonical operators overload

        public bool IsPositive() => Sign > 0;

        public bool IsNegative() => Sign < 0;

        public bool IsZero() => this == Zero;
        
        public static BigDecimal GenerateBigDecimalWithMinimalPrecision()
        {
            string numString = "0." + new string('0', FracPrecision - 1) + "1";
            return new BigDecimal(numString);
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
        
        private void FindDotPos()
        {
            if (_dotPos.HasValue)
                return;
            _dotPos = CleanString.IndexOf(delimiter);
            if (_dotPos < 0)
                _dotPos = CleanString.Length;
        }
      
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this == obj as BigDecimal;
        }

        #region Consts

        private static readonly BigDecimal _zero = new BigDecimal(0);

        public static BigDecimal Zero => _zero;

        private static readonly BigDecimal _one = new BigDecimal(1);

        public static BigDecimal One => _one;

        #endregion Consts

    }
}