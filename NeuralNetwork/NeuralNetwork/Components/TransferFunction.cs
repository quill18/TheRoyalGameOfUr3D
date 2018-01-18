using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.Components
{
    [Serializable]
    public class TransferFunction
    {
        /// <summary>
        /// y = (1 + e ^ -x) ^ -1
        /// </summary>
        public static readonly TransferFunction LogSigmoid = 
            new TransferFunction(
            delegate(double input) 
            {
                return 1d / (1d + Math.Exp(-1d * input));
            }, 
            delegate (double input) 
            {
                return Math.Exp(-1d * input) / Math.Pow(1d + Math.Exp(-1d * input), 2);
            }
            );
        /// <summary>
        /// y = x
        /// </summary>
        public static readonly TransferFunction PureLine = 
            new TransferFunction(
            delegate (double input)
            {
                return input;
            },
            delegate (double input)
            {
                return 1;
            }
            );


        private TransferFunction(FunctionDelegate function, FunctionDelegate derivative)
        {
            Derivative = derivative;
            Function = function;
        }

        /// <summary>
        /// Returns the value of the derivative of the transfer function at input
        /// </summary>
        internal FunctionDelegate Derivative { get; private set; }

        /// <summary>
        /// Returns the value of the transfer function at input
        /// </summary>
        internal FunctionDelegate Function { get; private set; }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="input">The input of the transfer function</param>
        /// <returns></returns>
        internal delegate double FunctionDelegate(double input);
    }
}
