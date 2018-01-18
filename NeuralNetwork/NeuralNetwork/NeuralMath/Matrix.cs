using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.NeuralMath
{
    internal class Matrix
    {

        /// <summary>
        /// single dimensional array add
        /// </summary>
        /// <param name="leftArray"></param>
        /// <param name="rightArray"></param>
        internal static double[] Add(double[] leftArray, double[] rightArray)
        {
            return TwoToSingleDimensionalMatrix(
                Add(SingleToTwoDimensionalMatrixRow(leftArray), 
                SingleToTwoDimensionalMatrixRow(rightArray)));
        }

        /// <summary>
        /// two dimensional array add
        /// </summary>
        /// <param name="leftArray"></param>
        /// <param name="rightArray"></param>
        /// <returns></returns>
        internal static double[,] Add(double[,] leftArray, double[,] rightArray)
        {
            for(int i = 0; i < leftArray.Rank; i++)
            {
                if (leftArray.GetLength(i) != rightArray.GetLength(i))
                {
                    throw new ArgumentOutOfRangeException("Left and right array must have the same dimensions!");
                }
            }

            double[,] outputArray = new double[leftArray.GetLength(0), leftArray.GetLength(1)];

            for (int i = 0; i < leftArray.GetLength(0); i++)
            {
                for (int j = 0; j < leftArray.GetLength(1); j++)
                {
                    outputArray[i, j] = leftArray[i, j] + rightArray[i, j];
                }
            }

            return outputArray;
        }

        /// <summary>
        /// single dimensional array subtraction assumes the array lengths are the same
        /// </summary>
        /// <param name="leftArray"></param>
        /// <param name="rightArray"></param>
        /// <returns></returns>
        internal static double[] Subtract(double[] leftArray, double[] rightArray)
        {
            return TwoToSingleDimensionalMatrix(
                Subtract(SingleToTwoDimensionalMatrixRow(leftArray),
                SingleToTwoDimensionalMatrixRow(rightArray)));
        }


        /// <summary>
        /// two dimensional array subtraction
        /// C = A - B where A is the left, B is the right and C is returned
        /// assumes the array row and column lengths are the same
        /// </summary>
        /// <param name="leftArray"></param>
        /// <param name="rightArray"></param>
        /// <returns></returns>
        internal static double[,] Subtract(double[,] leftArray, double[,] rightArray)
        {
            for (int i = 0; i < leftArray.Rank; i++)
            {
                if (leftArray.GetLength(i) != rightArray.GetLength(i))
                {
                    throw new ArgumentOutOfRangeException("Left and right array must have the same dimensions!");
                }
            }

            double[,] outputArray = new double[leftArray.GetLength(0), leftArray.GetLength(1)];

            for (int i = 0; i < leftArray.GetLength(0); i++)
            {
                for (int j = 0; j < leftArray.GetLength(1); j++)
                {
                    outputArray[i, j] = leftArray[i, j] - rightArray[i, j];
                }
            }

            return outputArray;
        }

        /// <summary>
        /// Multiplies matrix by a scalar assumes length of arrays are identical
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        internal static double[] ScalarMultiplication(double scalar, double[] array)
        {
            return TwoToSingleDimensionalMatrix(
                ScalarMultiplication(scalar,
                SingleToTwoDimensionalMatrixRow(array)));
        }

        /// <summary>
        /// Multiplies matrix by a scalar
        /// </summary>
        /// <param name="scalar"></param>
        /// <param name="array"></param>
        /// <returns></returns>
        internal static double[,] ScalarMultiplication(double scalar, double[,] array)
        {
            double[,] outputArray = new double[array.GetLength(0), array.GetLength(1)];

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    outputArray[i, j] = array[i, j] * scalar;
                }
            }

            return outputArray;
        }

        /// <summary>
        /// multiplies two matrices  
        /// </summary>
        /// <param name="leftArray">The the form [row, column]</param>
        /// <param name="rightArray">The the form [row, column]</param>
        /// <returns></returns>
        internal static double[,] Multiply(double[,] leftArray, double[,] rightArray)
        {
            if (leftArray.GetLength(1) != rightArray.GetLength(0))
            {
                throw new ArgumentOutOfRangeException("leftArray's columns must be equal to rightArray's rows");
            }

            double[,] outputArray = new double[leftArray.GetLength(0), rightArray.GetLength(1)];

            for (int r = 0; r < leftArray.GetLength(0); r++)
            {
                for (int i = 0; i < rightArray.GetLength(1); i++)
                {
                    outputArray[r, i] = 0;
                    for (int j = 0; j < rightArray.GetLength(0); j++)
                    {
                        outputArray[r, i] += leftArray[r, j] * rightArray[j, i];
                    }
                }
            }

            return outputArray;
        }

        /// <summary>
        /// Randomly changes elements of the passes array within the upper and lower value limits
        /// </summary>
        /// <param name="array"></param>
        /// <param name="lowerValueLimit"></param>
        /// <param name="upperValueLimit"></param>
        /// <param name="stdev">Standard deviation, used to create a bell curve centered on the current element values in the array so that the weights have a higher chance of not changing drastically </param>
        public static void MutateMatrix(double[] array, double lowerValueLimit, double upperValueLimit, double stdev)
        {
            double[,] twoDimensionalMatrix = SingleToTwoDimensionalMatrixRow(array);
            MutateMatrix(twoDimensionalMatrix, lowerValueLimit, upperValueLimit, stdev);
            TwoToSingleDimensionalMatrix(twoDimensionalMatrix).CopyTo(array, 0);
        }


        /// <summary>
        /// Randomly changes elements of the passes array within the upper and lower value limits
        /// </summary>
        /// <param name="array"></param>
        /// <param name="stdev"></param>
        public static void MutateMatrix(double[,] array, double lowerValueLimit, double upperValueLimit, double stdev)
        {
            if (stdev <= 0)
            {
                throw new System.ArgumentException("standard deviation cannot be <= 0", "stdev");
            }
            else if(lowerValueLimit >= upperValueLimit)
            {
                throw new ArgumentException("lowerValueLimit must be lower than the upper value limit", "upperValueLimit");
            }

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    // make sure that the element isn't already out of range
                    if (lowerValueLimit > array[i, j])
                    {
                        array[i, j] = lowerValueLimit;
                    }
                    else if (upperValueLimit < array[i, j])
                    {
                        array[i, j] = upperValueLimit;
                    }

                    // will check to see if above the lower or upper limit
                    double tempNum = Stats.ZScore.RandomGaussianDistribution(array[i, j], stdev);
                    Math.Round(tempNum, 2);
                    if (lowerValueLimit > tempNum)
                    {
                        tempNum = (lowerValueLimit - tempNum) + lowerValueLimit;
                    }
                    else if (upperValueLimit < tempNum)
                    {
                        tempNum = (upperValueLimit - tempNum) + upperValueLimit;
                    }
                    array[i, j] = tempNum;
                }
            }


        }

        /// <summary>
        /// Transposes the passed array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[,] TransposeMatrix(double[,] array)
        {
            double[,] outputArray = new double[array.GetLength(1), array.GetLength(0)];

            for (int i = 0; i < array.GetLength(0); i++)
            {
                for (int j = 0; j < array.GetLength(1); j++)
                {
                    outputArray[j, i] = array[i, j];
                }
            }
            return outputArray;
        }

        /// <summary>
        /// Converts a single to a two dimensional single row array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[,] SingleToTwoDimensionalMatrixRow(double[] array)
        {
            double[,] outputArray = new double[1, array.Length];

            for(int i = 0; i < array.Length; i++)
            {
                outputArray[0, i] = array[i];
            }

            return outputArray;
        }

        /// <summary>
        /// Converts a single to a two dimensional single column array
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[,] SingleToTwoDimensionalMatrixColumn(double[] array)
        {
            double[,] outputArray = new double[array.Length, 1];

            for (int i = 0; i < array.Length; i++)
            {
                outputArray[i, 0] = array[i];
            }

            return outputArray;
        }

        /// <summary>
        /// Converts a two to a single dimensional array
        /// <para>NOTE: The two dimensional array must be either a single row or a single column as [row, column]</para>
        /// </summary>
        /// <param name="array"></param>
        /// <returns></returns>
        public static double[] TwoToSingleDimensionalMatrix(double[,] array)
        {
            if(array.GetLength(0) != 1 && array.GetLength(1) != 1)
            {
                throw new ArgumentException("Passed array must contain a single row or column", "array");
            }

            double[] outputArray;

            if(array.GetLength(0) != 1)
            {
                outputArray = new double[array.GetLength(0)];
                for (int i = 0; i < array.GetLength(0); i++)
                {
                    outputArray[i] = array[i, 0];
                }
            }
            else
            {
                outputArray = new double[array.GetLength(1)];
                for (int i = 0; i < array.GetLength(1); i++)
                {
                    outputArray[i] = array[0, i];
                }
            }

            return outputArray;

        }
    }
}
