using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace BigNumbers
{
    public class BigComplex : BigNumber
    {
        private static readonly Regex validStringRegEx = new Regex(
            @"^(\s*([+-]?\d+(\.\d+)?\s*[+-]\s*)?[+-]?(\d+(\.\d+)?)?\s*[iI]\s*)|(\s*[+-]?\d+(\.\d+)?\s*)$", 
            RegexOptions.Compiled);

        #region Contructors
        static BigComplex()
        {
        }
        public BigComplex(string str)
        {
            var lowString = str?.ToLower();
            if (string.IsNullOrEmpty(lowString) ||
               string.IsNullOrEmpty(validStringRegEx.Match(lowString).Value))
                throw new ArgumentException("Cannot create BigComplex of \"" + str + "\"");
            CleanAndSaveNumericString(lowString.ToLower());
        }
        public BigComplex(BigComplex bc) : this(bc.ToString())
        {
        }
        public BigComplex(BigDecimal bc) : this(bc.ToString())
        {
        }
        public BigComplex(BigDecimal real, BigDecimal imaginary)
        {
            if (real is null || imaginary is null)
                throw new ArgumentNullException("Cannot create BigComplex of null part");
            this.Real = real;
            this.Imaginary = imaginary;
        }

        #region Constructor tools
        private void CleanAndSaveNumericString(string str)
        {
            var noWsString = Regex.Replace(str, @"\s+", "");
            var parts = SplitComplexString(noWsString);
            if (parts.Count == 1)
            {
                if (!parts[0].Contains("i"))
                {

                    Real = new BigDecimal(CreateImaginaryMultiplierString(parts[0]));
                    Imaginary = new BigDecimal("0");
                }
                else
                {
                    Real = new BigDecimal("0");
                    Imaginary = new BigDecimal(CreateImaginaryMultiplierString(parts[0]));
                }
            }
            else
            {
                Real = new BigDecimal(parts[0]);
                Imaginary = new BigDecimal(CreateImaginaryMultiplierString(parts[1]));
            }
            CleanString = this.ToString();
        }

        private string CreateImaginaryMultiplierString(string initialStr)
        {
            var noWsString = Regex.Replace(initialStr, @"i", "");
            if (noWsString == "" || noWsString == "+")
                noWsString = "1";
            else if (noWsString == "-")
                noWsString = "-1";
            return noWsString;
        }
        private List<string> SplitComplexString(string inputString)
        {
            var result = new List<string>();
            var lastPos = 0;
            string substr;
            for (int i = 0; i < inputString.Length; i++)
                if (inputString[i] == '+' || inputString[i] == '-')
                {
                    substr = inputString.Substring(lastPos, i - lastPos);
                    if (!String.IsNullOrWhiteSpace(substr))
                        result.Add(substr);
                    lastPos = i;
                }
            substr = inputString.Substring(lastPos, inputString.Length - lastPos);
            if (!String.IsNullOrWhiteSpace(substr))
                result.Add(substr);
            return result;
        }
        #endregion
        #endregion

        #region Operations parent override

        public override BigNumber Add(BigNumber op)
        {
            var right = TryConvertBigNumberToBigComplex(op);
            var left = this;
            var realResult = left.Real + right.Real;
            var imaginaryResult = left.Imaginary + right.Imaginary;
            var result = new BigComplex(realResult, imaginaryResult);
            var simplifiedResult = TryConvertBigNumberToBigComplex(result);
            return simplifiedResult;
        }
        public override BigNumber Substract(BigNumber op)
        {
            var right = TryConvertBigNumberToBigComplex(op);
            var left = this;
            var realResult = left.Real- right.Real;
            var imaginaryResult = left.Imaginary - right.Imaginary;
            var result = new BigComplex(realResult, imaginaryResult);
            var simplifiedResult = TryConvertBigNumberToBigComplex(result);
            return simplifiedResult;
        }
        public override BigNumber Multiply(BigNumber op)
        {
            var right = TryConvertBigNumberToBigComplex(op);
            var left = this;
            var a = left.Real;
            var b = left.Imaginary;
            var c = right.Real;
            var d = right.Imaginary;
            var realResult = a * c - b * d;
            var imaginaryResult = a * d + b * c;
            var result = new BigComplex(realResult, imaginaryResult);
            var simplifiedResult = TryConvertBigNumberToBigComplex(result);
            return simplifiedResult;
        }
        public override BigNumber Divide(BigNumber op)
        {
            var right = TryConvertBigNumberToBigComplex(op);
            var left = this;
            var a1 = left.Real;
            var b1 = left.Imaginary;
            var a2 = right.Real;
            var b2 = right.Imaginary;
            var realResult = (a1 * a2 + b1 * b2) / (a2 * a2 + b2 * b2);
            var imaginaryResult = (a2 * b1 - a1 * b2) / (a2 * a2 + b2 * b2);
            var result = new BigComplex(realResult, imaginaryResult);
            var simplifiedResult = TryConvertBigNumberToBigComplex(result);
            return simplifiedResult;
        }
        public override BigNumber Mod(BigNumber op)
        {
            throw new NotImplementedException();
        }


        #region Tools

        private BigNumber TryToCreateBigDecimal()
        {
            if (Imaginary.ToString() == "0")
                return new BigDecimal(this.ToString());
            return this;
        }
        private BigComplex TryConvertBigNumberToBigComplex(BigNumber number)
        {
            BigComplex result;
            if (number is BigDecimal)
                result = new BigComplex((BigDecimal)number);
            else if (number is BigComplex)
                result = (BigComplex)number;
            else
                throw new ArgumentException("Cannot Add BigComplex to " + number.GetType());
            return result;
        }

        #endregion

        #endregion

        #region Function parent override
        public override BigNumber Abs()
        {
            throw new NotImplementedException("Cannot take abs of a complex number");
        }

        #endregion

        #region Other parent overriden tools
        public override BigNumber Negative()
        {
            BigComplex ret = new BigComplex(this);
            ret.Negate();
            return ret;
        }
        public override BigNumber Copy()
        {
            BigComplex ret = new BigComplex(this);
            return ret;
        }
        public override string ToString()
        {
            if (Real.CleanString == "0" && Imaginary.CleanString == "0")
                    return "0";
            var realString = Real.ToString() != "0" ? Real.ToString() : "";
            string imaginaryString;
            if (Imaginary.CleanString == "0")
                imaginaryString = "";
            else
            {
                imaginaryString = Imaginary.CleanString == "1" ? "" : Imaginary.CleanString;
                if (Imaginary.Sign < 0)
                    imaginaryString = $"-{imaginaryString}";
                else
                {
                    if (realString.Length > 0)
                        imaginaryString = $"+{imaginaryString}";
                }
                imaginaryString += "i";
            }
            return $"{realString}{imaginaryString}";
        }
        public override void Negate()
        {
            Real = -Real;
            Imaginary = -Imaginary;
            CleanString = this.ToString();
        }
        public override int this[int index] => throw new NotImplementedException();
        public override void NormalizeList(List<int> digits)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Operators overload

        public static BigComplex operator -(BigComplex num)
        {
            BigComplex ret = new BigComplex(num);
            ret.Negate();
            return ret;
        }

        public static BigComplex operator +(BigComplex left, BigComplex right)
        {
            if (left is null || right is null)
                return null;
            return (BigComplex)left.Add(right);
        }
        public static BigComplex operator -(BigComplex left, BigComplex right)
        {
            if (left is null || right is null)
                return null;
            return (BigComplex)left.Substract(right);
        }
        public static BigComplex operator *(BigComplex left, BigComplex right)
        {
            if (left is null || right is null)
                return null;
            return (BigComplex)left.Multiply(right);
        }
        public static BigComplex operator /(BigComplex left, BigComplex right)
        {
            if (left is null || right is null)
                return null;
            return (BigComplex)left.Divide(right);
        }
        public static BigComplex operator %(BigComplex left, BigComplex right)
        {
            if (left is null || right is null)
                return null;
            return (BigComplex)left.Mod(right);
        }
        #endregion
        
        public BigDecimal Imaginary { get; private set; }
        public BigDecimal Real { get; private set; }
    }
}
