using System;
using System.Collections.Generic;
using System.Linq;

namespace BigNumbers
{
    internal static class ListOperationExtensions
    {
        public delegate void Normalizer(List<int> list);

        public static List<int> SumWithList(this List<int> leftList, List<int> rightList)
        {
            if (leftList.Count <= 0 || rightList.Count <= 0)
                return new List<int>();
            var maxlen = Math.Max(leftList.Count, rightList.Count);
            leftList.AddRange(Enumerable.Repeat(0, maxlen - leftList.Count));
            rightList.AddRange(Enumerable.Repeat(0, maxlen - rightList.Count));
            var resultList = new List<int>(leftList.Count);
            for (var i = 0; i < maxlen; i++)
                resultList.Add(leftList[i] + rightList[i]);
            return resultList;
        }

        public static List<int> SubByList(this List<int> leftList, List<int> rightList)
        {
            if (leftList.CompareWithList(rightList) < 0)
                throw new ArgumentException("leftList cannot be bigger than rightList!");
            rightList.AddRange(Enumerable.Repeat(0, leftList.Count - rightList.Count));
            var resultList = new List<int>();
            for (var i = 0; i < leftList.Count; i++)
                resultList.Add(leftList[i] - rightList[i]);
            return resultList;
        }

        public static List<int> MulWithList(this List<int> leftList, List<int> rightList)
        {
            var resultList = new List<int>();
            var tempList = new List<int>();
            resultList = leftList.MulWithDigit(rightList[0]);
            for (var i = 1; i < rightList.Count; i++)
            {
                if (rightList[i] == 0)
                    continue;
                tempList = leftList.MulWithDigit(rightList[i], i);
                resultList = resultList.SumWithList(tempList);
            }

            return resultList;
        }

        public static List<int> DivByList(this List<int> leftList, List<int> rightList,
            Normalizer NormFunc, out List<int> remainder)
        {
            var resultList = new List<int>();
            remainder = new List<int>();
            if (leftList.CompareWithList(rightList) >= 0)
            {
                var unnormed = true;
                int sum, dif;
                var indexToAdd = leftList.Count - rightList.Count - 1;
                remainder.AddRange(leftList.GetRange(indexToAdd + 1, rightList.Count));
                do
                {
                    if (unnormed)
                        while (remainder.Count > 0 && remainder.Last() == 0)
                            remainder.RemoveAt(remainder.Count - 1);
                    unnormed = true;
                    sum = 0;
                    while (remainder.CompareWithList(rightList) >= 0)
                    {
                        dif = remainder.Count - rightList.Count;
                        rightList.AddRange(Enumerable.Repeat(0, dif));
                        remainder = remainder.SubByList(rightList);
                        NormFunc(remainder);
                        if (dif > 0)
                            rightList.RemoveAt(rightList.Count - 1);
                        while (remainder.Count > 0 && remainder.Last() == 0)
                            remainder.RemoveAt(remainder.Count - 1);
                        sum++;
                        unnormed = false;
                    }

                    resultList.Add(sum);
                    if (indexToAdd >= 0)
                        remainder.Insert(0, leftList[indexToAdd]);
                    indexToAdd--;
                } while (indexToAdd >= -1);
            }

            if (resultList.Count == 0)
                resultList.Add(0);
            resultList.Reverse();
            return resultList;
        }

        public static List<int> MulWithDigit(this List<int> leftList, int digit, int padding = 0)
        {
            if (digit == 0)
                return new List<int> {0};
            var resultList = new List<int>(leftList.Count);
            resultList.AddRange(Enumerable.Repeat(0, padding));
            for (var i = 0; i < leftList.Count; i++)
                resultList.Add(leftList[i] * digit);
            return resultList;
        }

        public static int CompareWithList(this List<int> left, List<int> right)
        {
            if (left.Count != right.Count)
                return left.Count - right.Count;
            var i = left.Count - 1;
            while (i >= 0 && left[i] == right[i])
                i--;
            if (i == -1)
                return 0;
            return left[i] - right[i];
        }

        public static void RemoveTailingZeros(this List<int> list)
        {
            while (list.Last() == 0 && list.Count > 1)
                list.RemoveAt(list.Count - 1);
        }
    }
}