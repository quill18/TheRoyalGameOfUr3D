using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.Components
{
    /// <summary>
    /// Contains the data required to update weights and biases in a neural network layer
    /// </summary>
    internal class WeightUpdateData
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input">The input into the layer of neurons</param>
        /// <param name="transferFunctionJacobian">Jacobian matrix for neural network at the transfer function</param>
        internal WeightUpdateData(double[] input, double[,] transferFunctionJacobian)
        {
            Input = new double[input.Length];
            input.CopyTo(Input, 0);

            TransferFunctionJacobian = new double[transferFunctionJacobian.GetLength(0), transferFunctionJacobian.GetLength(1)];
            for (int i = 0; i < TransferFunctionJacobian.GetLength(0); i++)
            {
                for (int j = 0; j < TransferFunctionJacobian.GetLength(1); j++)
                {
                    TransferFunctionJacobian[i, j] = transferFunctionJacobian[i, j];
                }
            }
        }

        /// <summary>
        /// The input into the layer of neurons
        /// </summary>
        internal double[] Input { get; private set; }

        /// <summary>
        /// Jacobian matrix for neural network at the transfer function
        /// </summary>
        internal double[,] TransferFunctionJacobian { get; private set; }
    }
}
