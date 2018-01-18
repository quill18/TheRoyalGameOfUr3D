using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeuralNetwork.NeuralMath;

namespace NeuralNetwork.Components
{
    [Serializable]
    internal class LayerOfNeurons
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="totalInputs">Number of inputs for this layer of neurons</param>
        /// <param name="totalOutputs">Number of outputs for this layers</param>
        /// <param name="hasBias"></param>
        /// <param name="transferFunction"></param>
        internal LayerOfNeurons(int totalInputs, int totalOutputs, bool hasBias, TransferFunction transferFunction)
        {
            TransferFunction = transferFunction;
            Weights = new double[totalOutputs, totalInputs];
            for (int i = 0; i < Weights.GetLength(0); i++)
            {
                for (int j = 0; j < Weights.GetLength(1); j++)
                {
                    Weights[i, j] = Stats.ZScore.RandomGaussianDistribution(0, 0.1);
                }
            }

            if (hasBias)
            {
                Biases = new double[totalOutputs];

                for (int i = 0; i < Biases.Length; i++)
                {
                    Biases[i] = Stats.ZScore.RandomGaussianDistribution(0, 0.1);
                }
            }
            else
            {
                Biases = null;
            }
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="oldLayer"></param>
        internal LayerOfNeurons(LayerOfNeurons oldLayer)
        {
            Weights = new double[oldLayer.Weights.GetLength(0), oldLayer.Weights.GetLength(1)];
            for (int i = 0; i < oldLayer.Weights.GetLength(0); i++)
            {
                for (int j = 0; j < oldLayer.Weights.GetLength(1); j++)
                {
                    Weights[i, j] = oldLayer.Weights[i, j];
                }
            }

            if (oldLayer.Biases == null)
            {
                Biases = null;
            }
            else if (oldLayer.Weights.GetLength(0) != oldLayer.Biases.Length)
            {
                throw new ArgumentException("The number of rows in the weights must be equal to the biases length");
            }
            else
            {

                Biases = new double[oldLayer.Biases.Length];
                for (int i = 0; i < Biases.Length; i++)
                {
                    Biases[i] = oldLayer.Biases[i];
                }
            }

            TransferFunction = oldLayer.TransferFunction;
        }

        /// <summary>
        /// Used for unit testing
        /// </summary>
        /// <param name="weights"></param>
        /// <param name="biases"></param>
        /// <param name="transferFunction"></param>
        internal LayerOfNeurons( double[,] weights, double[] biases, TransferFunction transferFunction)
        {
            TransferFunction = transferFunction;
            Weights = new double[weights.GetLength(0), weights.GetLength(1)];
            for (int i = 0; i < Weights.GetLength(0); i++)
            {
                for (int j = 0; j < Weights.GetLength(1); j++)
                {
                    Weights[i, j] = weights[i, j];
                }
            }

            if (biases != null)
            {
                Biases = new double[biases.Length];

                for (int i = 0; i < Biases.Length; i++)
                {
                    Biases[i] = biases[i];
                }
            }
            else
            {
                Biases = null;
            }
        }

        /// <summary>
        /// Performs the calculation for the neural network
        /// </summary>
        /// <param name="input"></param>
        /// <param name="transferFunctionJacobian">Data required to update the weights of this layer during training</param>
        /// <returns></returns>
        internal double[] Calculate(double[] input, out WeightUpdateData weightData)
        {
            double[] outputArray = Matrix.TwoToSingleDimensionalMatrix(NeuralMath.Matrix.Multiply(Weights, Matrix.SingleToTwoDimensionalMatrixColumn(input)));

            if(Biases != null)
            {
                outputArray = NeuralMath.Matrix.Add(outputArray, Biases);
            }

            double[,] transferFunctionJacobian = new double[outputArray.Length, outputArray.Length];
            for (int i = 0; i < outputArray.Length; i++)
            {
                transferFunctionJacobian[i, i] = TransferFunction.Derivative(outputArray[i]);
                outputArray[i] = TransferFunction.Function(outputArray[i]);
            }

            weightData = new WeightUpdateData(input, transferFunctionJacobian);

            return outputArray;
        }

        /// <summary>
        /// Randomly Changes the weights and biases in this layer
        /// </summary>
        /// <param name="lowerValueLimit">The lower limit for the weights</param>
        /// <param name="upperValueLimit">The upper limit for the weights</param>
        /// <param name="stdDev">The standard deviation for changes to the weights and biases</param>
        internal void Mutate(double lowerValueLimit, double upperValueLimit, double stdDev)
        {
            if(Biases != null)
            {
                Matrix.MutateMatrix(Biases, lowerValueLimit, upperValueLimit, stdDev);
            }

            Matrix.MutateMatrix(Weights, lowerValueLimit, upperValueLimit, stdDev);
        }

        /// <summary>
        /// The weights of this net
        /// </summary>
        internal double[,] Weights { get; set; }

        /// <summary>
        /// The biases for each node
        /// </summary>
        internal double[] Biases { get; set; }

        internal TransferFunction TransferFunction { get; private set; }
    }
}
