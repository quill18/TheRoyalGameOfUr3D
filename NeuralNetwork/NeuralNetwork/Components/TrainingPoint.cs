using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.Components
{
    /// <summary>
    /// Point which is used to train a neural network
    /// </summary>
    public class TrainingPoint
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="input"></param>
        /// <param name="expectedOutput"></param>
        public TrainingPoint(double[] input, double[] expectedOutput)
        {
            Input = new double[input.Length];
            input.CopyTo(Input, 0);

            ExpectedOutput = new double[expectedOutput.Length];
            expectedOutput.CopyTo(ExpectedOutput, 0);
        }

        /// <summary>
        /// Input which generates expected output
        /// </summary>
        public double[] Input { get; private set; }
        /// <summary>
        /// Expected Output of Neural Network
        /// </summary>
        public double[] ExpectedOutput { get; private set; }
    }
}
