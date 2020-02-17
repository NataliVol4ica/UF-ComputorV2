using System;

namespace BigNumbers
{
    public static class PowCalculator
    {
        public static BigNumber CalculatePow(BigNumber numberToPow, BigDecimal pow)
        {
            if (!pow.IsInteger)
                throw new ArgumentException("Cannot Pow BigDecimal and non-integer number");
            if (pow.CleanString == "0")
                return new BigDecimal("1");
            bool isNegative = false;
            if (pow.Sign == -1)
            {
                isNegative = true;
                pow = -pow;
            }

            var result = DoPow(numberToPow, numberToPow, pow);
            if (isNegative)
                result = new BigDecimal(1) / result;
            return result;
        }

        private static BigNumber DoPow(BigNumber number, BigNumber initialNumber, BigDecimal pow)
        {
            if (pow.CleanString == "1")
                return initialNumber;
            if (pow.CleanString == "2")
                return initialNumber * initialNumber;
            if (pow.IsEven)
            {
                var evenResult = DoPow(number, initialNumber, pow / new BigDecimal(2));
                return evenResult * evenResult;
            }

            var newPow = (pow - new BigDecimal(1)) / new BigDecimal(2);
            var oddResult = DoPow(number, initialNumber, newPow);
            return oddResult * oddResult * initialNumber;
        }
    }
}