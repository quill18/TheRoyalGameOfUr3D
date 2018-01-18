using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using NeuralNetwork.Components;
using NeuralNetwork.NeuralMath;

namespace NeuralNetwork
{
    /// <summary>
    /// Basic feed forward neural network
    /// </summary>
    [Serializable]
    public class FeedForwardNetwork
    {
        /// <summary>
        /// The maximum tries allowed to optimize the weights and biases in the neural network
        /// </summary>
        private static readonly int MAX_TRIES = (int)1e6;

        /// <summary>
        /// The maximum value a weight is allowed to be 
        /// </summary>
        private static readonly double DEFAULT_UPPER_WEIGHT_LIMIT = 1d;

        /// <summary>
        /// The smallest value a weight is allowed to be 
        /// </summary>
        private static readonly double DEFAULT_LOWER_WEIGHT_LIMIT = -1d;
        /// <summary>
        /// The standard deviation used for mutations
        /// </summary>
        private static readonly double DEFAULT_MUTATE_STANDARD_DEVIATION = 0.1;

        /// <summary>
        /// The default minimum allowed R^2 value for batch training
        /// </summary>
        private static readonly double DEFAULT_MIN_R_SQUARE = 0.9;

        /// <summary>
        /// The default learning rate
        /// </summary>
        private static readonly double DEFAULT_LEARNING_RATE = 0.1;

        /// <summary>
        /// Basic feed forward neural network
        /// </summary>
        /// <param name="totalInputs">Number of elements the single dimensional input array will have</param>
        /// <param name="totalOutputs">Number of elements the single dimensional output array will have</param>
        /// <param name="totalHiddenLayers">The desired amount of hidden layers in the array</param>
        /// <param name="nodesPerLayer">The number of neurons each hidden layer will have</param>
        /// <param name="hasBias">True if the Network will contain a bias for each neuron</param>
        /// <param name="transferFunction">The transfer function for each neuron</param>
        public FeedForwardNetwork(int totalInputs, int totalOutputs, int totalHiddenLayers, int nodesPerLayer, bool hasBias, TransferFunction transferFunction)
        {
            HiddenLayers = new LayerOfNeurons[totalHiddenLayers];
            if (totalHiddenLayers == 1)
            {
                HiddenLayers[0] = new LayerOfNeurons(totalInputs, totalOutputs, hasBias, transferFunction);
            }
            else
            {
                HiddenLayers[0] = new LayerOfNeurons(totalInputs, totalOutputs, hasBias, transferFunction);
                for (int i = 1; i < HiddenLayers.Length; i++)
                {
                    HiddenLayers[i] = new LayerOfNeurons(totalOutputs, totalOutputs, hasBias, transferFunction);
                }
            }


        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="net"></param>
        public FeedForwardNetwork(FeedForwardNetwork net)
        {
            HiddenLayers = new LayerOfNeurons[net.HiddenLayers.Length];
            for (int i = 0; i < HiddenLayers.Length; i++)
            {
                HiddenLayers[i] = new LayerOfNeurons(net.HiddenLayers[i]);
            }
        }

        /// <summary>
        /// Used for unit testing
        /// </summary>
        /// <param name="hiddenLayers"></param>
        internal FeedForwardNetwork(LayerOfNeurons[] hiddenLayers)
        {
            HiddenLayers = new LayerOfNeurons[hiddenLayers.Length];
            // ensure no pointer sharing
            for (int i = 0; i < hiddenLayers.Length; i++)
            {
                HiddenLayers[i] = new LayerOfNeurons(hiddenLayers[i]);
            }
        }

        /// <summary>
        /// Determines the error for all points in the collection then updates the weights and biases using the mean square error
        /// <para>Used default values for the R^2 and learning rate</para>
        /// <para>Algorithm is based on Neural Network Design 2nd Edition Chapter 11's algorithm</para>
        /// </summary>
        /// <param name="trainingSet">All training points</param>
        public void BatchTrain(List<TrainingPoint> trainingSet)
        {
            BatchTrain(trainingSet, DEFAULT_MIN_R_SQUARE, DEFAULT_LEARNING_RATE);
        }




        /// <summary>
        /// Determines the error for all points in the collection then updates the weights and biases using the mean square error
        /// <para>Algorithm is based on Neural Network Design 2nd Edition Chapter 11's algorithm</para>
        /// </summary>
        /// <param name="trainingSet">All training points</param>
        /// <param name="minRSquaredValue">The minimum allowed R^2 value for any of the outputs
        /// <para>Note: Must be greater than 0, but less than or equal to 1</para> </param>
        /// <param name = "learningRate" > A fraction which determines how big of a step the change in the weights and biases will be along the gradient
        /// <para>Note: Must be greater than or equal to -1, but less than or equal to 1</para> 
        /// <para>Negative learning rate means the network is being "punished"</para></param>
        public void BatchTrain(List<TrainingPoint> trainingSet, double minRSquaredValue, double learningRate)
        {
            if(minRSquaredValue <= 0 || minRSquaredValue > 1)
            {
                throw new ArgumentOutOfRangeException("maxAllowedSquaredError");
            }
            else if (learningRate < -1 || learningRate > 1)
            {
                throw new ArgumentOutOfRangeException("learningRate");
            }

            // derivative error
            // all values should be zero
            double[][,] weightDerivativesSummation = new double[HiddenLayers.Length][,];
            double[][] biasDerivativesSummation = new double[HiddenLayers.Length][];

            double[] totalSumOfSquares = TotalSumOfSquares(trainingSet);
            
            int j = 0;

            while(j < MAX_TRIES)
            {
                double[] residualSumOfSquares = new double[totalSumOfSquares.Length];
                
                // sum all derivatives
                foreach (TrainingPoint trainingPoint in trainingSet)
                {
                    List<WeightUpdateData> weightUpdateData;
                    double[] actualOutput = Calculate(trainingPoint.Input, out weightUpdateData);
                    ComputeDerivative(weightUpdateData, trainingPoint.ExpectedOutput, actualOutput,
                        out double[][,] weightDerivatives, out double[][] biasDerivatives);

                    // sum all errors before dividing
                    residualSumOfSquares = Matrix.Add(residualSumOfSquares, SquareOfResiduals(trainingPoint.ExpectedOutput, actualOutput));
                    // sum all derivatives
                    for (int i = 0; i < HiddenLayers.Length; i++)
                    {
                        if (HiddenLayers[i].Biases != null)
                        {
                            if (biasDerivativesSummation[i] == null)
                            {
                                biasDerivativesSummation[i] = new double[HiddenLayers[i].Biases.Length];
                            }

                            biasDerivativesSummation[i] = Matrix.Add(biasDerivativesSummation[i], biasDerivatives[i]);
                        }

                        if (weightDerivativesSummation[i] == null)
                        {
                            weightDerivativesSummation[i] = new double[HiddenLayers[i].Weights.GetLength(0), HiddenLayers[i].Weights.GetLength(1)];
                        }

                        weightDerivativesSummation[i] = Matrix.Add(weightDerivativesSummation[i], weightDerivatives[i]);
                    }
                }
                // true if all elements in the rSquared array are above the minRSquaredValue
                bool aboveRSquared = true;
                double[] rSquared = new double[residualSumOfSquares.Length];
                
                for (int i = 0; i < residualSumOfSquares.Length; i++)
                {
                    rSquared[i] = 1 - (residualSumOfSquares[i] / totalSumOfSquares[i]);
                    if(minRSquaredValue > rSquared[i])
                    {
                        // no need to continue we have to loop again
                        aboveRSquared = false;
                        break;
                    }
                }

                if(aboveRSquared)
                {
                    // we're done here
                    return;
                }

                Train(learningRate / trainingSet.Count, weightDerivativesSummation, biasDerivativesSummation);
                j++;
            }

            throw new Exception("Max attempts to train the neural network reached");

        }


        /// <summary>
        /// Determines the errors for the training point and then updates the weights and biases a single time
        /// <para>Uses a default learning rate</para>
        /// <para>Algorithm is based on Neural Network Design 2nd Edition Chapter 11's algorithm</para>
        /// </summary>
        /// <param name="trainingPoint"></param>
        public void IncrementalTrain(TrainingPoint trainingPoint)
        {
            IncrementalTrain(trainingPoint, DEFAULT_LEARNING_RATE);
        }



        /// <summary>
        /// Determines the errors for the training point and then updates the weights and biases a single time
        /// <para>Algorithm is based on Neural Network Design 2nd Edition Chapter 11's algorithm</para>
        /// </summary>
        /// <param name="trainingPoint"></param>
        /// <param name = "learningRate" > A fraction which determines how big of a step the change in the weights and biases will be along the gradient
        /// <para>Note: Must be greater than or equal to -1, but less than or equal to 1</para> 
        /// <para>Negative learning rate means the network is being "punished"</para></param>
        public void IncrementalTrain(TrainingPoint trainingPoint, double learningRate)
        {
            if (learningRate < -1 || learningRate > 1)
            {
                throw new ArgumentOutOfRangeException("learningRate");
            }

            List<WeightUpdateData> weightUpdateData;
            double[] actualOutput = Calculate(trainingPoint.Input, out weightUpdateData);

            ComputeDerivative(weightUpdateData, trainingPoint.ExpectedOutput, actualOutput, out double[][,] weightDerivatives, out double[][] biasDerivatives);

            Train(learningRate, weightDerivatives, biasDerivatives);

        }

        /// <summary>
        /// Updates the weights and biases for the hidden layer
        /// </summary>
        /// <param name = "learningRate" > A fraction which determines how big of a step the change in the weights and biases will be along the gradient
        /// <para>Note: Must be greater than or equal to -1, but less than or equal to 1</para> 
        /// <para>Negative learning rate means the network is being "punished"</para></param>
        /// <param name="weightDerivatives">Assumes each index corresponds to hiddenLayers array</param>
        /// <param name="biasDerivatives">Assumes each index corresponds to hiddenLayers array</param>
        private void Train(double learningRate, double[][,] weightDerivatives, double[][] biasDerivatives)
        {
            if (learningRate < -1 || learningRate > 1)
            {
                throw new ArgumentOutOfRangeException("learningRate");
            }
            else if(weightDerivatives.Length != HiddenLayers.Length)
            {
                throw new ArgumentOutOfRangeException("weightDerivatives is not as big as hiddenLayers");
            }
            else if(biasDerivatives.Length != HiddenLayers.Length)
            {
                throw new ArgumentOutOfRangeException("biasDerivatives is not as big as hiddenLayers");
            }

            for (int i = 0; i < HiddenLayers.Length; i++)
            {
                if (HiddenLayers[i].Biases != null)
                {
                    HiddenLayers[i].Biases = Matrix.Subtract(HiddenLayers[i].Biases,
                        Matrix.ScalarMultiplication(learningRate, biasDerivatives[i])
                    );
                }

                HiddenLayers[i].Weights = Matrix.Subtract(HiddenLayers[i].Weights,
                     Matrix.ScalarMultiplication(learningRate, weightDerivatives[i])
                     );
            }
        }

        /// <summary>
        /// Performs the calculation for the neural network
        /// </summary>
        /// <param name="input"></param>
        /// <returns>The result of the neural network</returns>
        public double[] Calculate(double[] input)
        {
            // The data only useful during training
            List<WeightUpdateData> dontCare;
            return Calculate(input, out dontCare);
        }

        /// <summary>
        /// Performs the calculation for the neural network
        /// </summary>
        /// <param name="input"></param>
        /// <param name="weightUpdateData">Data needed to train the network where the indices match up with the indices of HiddenLayer</param>
        /// <returns></returns>
        private double[] Calculate(double[] input, out List<WeightUpdateData> weightUpdateData)
        {
            double[] outputArray = new double[input.Length];
            input.CopyTo(outputArray, 0);

            WeightUpdateData currentWeightUpdateData;
            weightUpdateData = new List<WeightUpdateData>();

            for (int i = 0; i < HiddenLayers.Length; i++)
            {
                outputArray = HiddenLayers[i].Calculate(outputArray, out currentWeightUpdateData);
                weightUpdateData.Add(currentWeightUpdateData);
            }

            return outputArray;
        }


        /// <summary>
        /// Randomly alters the weights and biases within this neural network
        /// <para>Uses default value for the lower and upper value limits and the standard deviation for changes to the weights and biases</para>
        /// </summary>
        public void Mutate()
        {
            Mutate(DEFAULT_LOWER_WEIGHT_LIMIT, DEFAULT_UPPER_WEIGHT_LIMIT, DEFAULT_MUTATE_STANDARD_DEVIATION);
        }

        /// <summary>
        /// Randomly alters the weights and biases within this neural network
        /// </summary>
        /// <param name="lowerValueLimit">The lower limit for the weights</param>
        /// <param name="upperValueLimit">The upper limit for the weights</param>
        /// <param name="stdDev">The standard deviation for changes to the weights and biases</param>
        public void Mutate(double lowerValueLimit, double upperValueLimit, double stdDev)
        {
            foreach (LayerOfNeurons layer in HiddenLayers)
            {
                layer.Mutate(lowerValueLimit, upperValueLimit, stdDev);
            }
        }

        /// <summary>
        /// Calculates the derative of the squared error with respect to the input of the transfer function for each layer of neurons
        /// <para>Algorithm is based on Neural Network Design 2nd Edition Chapter 11's algorithm</para>
        /// </summary>
        /// <param name="weightUpdateData">Indices matches up with the hiddenLayers</param>
        /// <param name="trainingPoint"></param>
        /// <param name="actualOutput"></param>
        /// <param name="expectedOutput"></param>
        /// <param name="weightDerivatives">Where the indexes match up with the hiddenLayer</param>
        /// <param name="biasDerivatives">Where the indexes match up with the hiddenLayer</param>
        private void ComputeDerivative(List<WeightUpdateData> weightUpdateData, double[] expectedOutput, double[] actualOutput,
            out double[][,] weightDerivatives, out double[][] biasDerivatives)
        {
            Stack<double[]> sensitivities = new Stack<double[]>();

            weightDerivatives = new double[HiddenLayers.Length][,];
            biasDerivatives = new double[HiddenLayers.Length][];

            sensitivities.Push( 
                Matrix.TwoToSingleDimensionalMatrix(
                Matrix.ScalarMultiplication(-2, 
                Matrix.Multiply(weightUpdateData[weightUpdateData.Count - 1].TransferFunctionJacobian, 
                Matrix.SingleToTwoDimensionalMatrixColumn(Matrix.Subtract(expectedOutput, actualOutput))))                
                ));
            // find sensitivities, or the derivative of the transfer function which repect to the error
            for (int i = 0; i < weightUpdateData.Count - 1; i++)
            {
                sensitivities.Push(
                Matrix.TwoToSingleDimensionalMatrix(
                Matrix.Multiply(
                    Matrix.Multiply(weightUpdateData[weightUpdateData.Count - 2 - i].TransferFunctionJacobian,
                    Matrix.TransposeMatrix(HiddenLayers[weightUpdateData.Count - 1 - i].Weights)),
                    Matrix.SingleToTwoDimensionalMatrixColumn(sensitivities.Peek()))
                ));
            }

            // calculate the weight and bias derivatives for each layer
            for (int i = 0; i < HiddenLayers.Length; i++)
            {
                biasDerivatives[i] = null;
                if (HiddenLayers[i].Biases != null)
                {
                    biasDerivatives[i] = sensitivities.Peek();
                }

                weightDerivatives[i] = Matrix.Multiply(
                                Matrix.SingleToTwoDimensionalMatrixColumn(sensitivities.Pop()),
                                Matrix.SingleToTwoDimensionalMatrixRow(weightUpdateData[i].Input)
                            );
            }
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Total_sum_of_squares
        /// </summary>
        /// <param name="trainingPoints"></param>
        /// <returns>Total sum of squares for the training points</returns>
        private double[] TotalSumOfSquares(List<TrainingPoint> trainingPoints)
        {
            double[] meanOfExpected = new double[HiddenLayers[HiddenLayers.Length - 1].Weights.GetLength(0)];

            // find the mean of the expected points
            foreach(TrainingPoint point in trainingPoints)
            {
                if (point.ExpectedOutput.Length != meanOfExpected.Length)
                {
                    throw new ArgumentException("All training point arrays be the same as the number of rows in the last hidden layer's weight rows");
                }

                for (int i = 0; i < meanOfExpected.Length; i++)
                {
                    meanOfExpected[i] += point.ExpectedOutput[i];
                }
            }

            for (int i = 0; i < meanOfExpected.Length; i++)
            {
                meanOfExpected[i] /= trainingPoints.Count;
            }

            double[] totalSumOfSquares = new double[meanOfExpected.Length];

            // calculate the total sum of squares
            foreach (TrainingPoint point in trainingPoints)
            {
                for (int i = 0; i < meanOfExpected.Length; i++)
                {
                    totalSumOfSquares[i] += Math.Pow(meanOfExpected[i] - point.ExpectedOutput[i], 2);
                }
            }

            return totalSumOfSquares;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <returns></returns>
        private double[] SquareOfResiduals(double[] expected, double[] actual)
        {
            if(expected.Length != actual.Length)
            {
                throw new ArgumentException("Expected and Actual must be the same size");
            }

            double[] squareOfResiduals = new double[expected.Length];
            for (int i = 0; i < squareOfResiduals.Length; i++)
            {
                squareOfResiduals[i] = Math.Pow(expected[i] - actual[i], 2);
            }

            return squareOfResiduals;
        }

        /// <summary>
        /// Saves this object to folderPath\fileName.bin
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="fileName"></param>
        public void Save(string folderPath, string fileName)
        {
            IFormatter formatter = new BinaryFormatter();

            string path = $"{ folderPath }\\{fileName}.bin";
            using (System.IO.FileStream file = System.IO.File.Create(path))
            {
                formatter.Serialize(file, this);
            }
        }

        /// <summary>
        /// Loads FeedForwardNetwork represented by folderPath\fileName.bin
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="fileName"></param>
        public static FeedForwardNetwork Load(string folderPath, string fileName)
        {
            IFormatter formatter = new BinaryFormatter();
            FeedForwardNetwork net;

            string path = $"{ folderPath }\\{fileName}.bin";
            
            using (System.IO.FileStream file = System.IO.File.OpenRead(path))
            {
                net = (FeedForwardNetwork)formatter.Deserialize(file);
            }

            return net;
        }

        /// <summary>
        /// Stores all the hidden layers for this neural network
        /// </summary>
        internal LayerOfNeurons[] HiddenLayers { get; private set; }

    }
}
