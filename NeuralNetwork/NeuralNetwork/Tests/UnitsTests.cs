using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NeuralNetwork.NeuralMath;
using NeuralNetwork.Components;

/// <summary>
/// Needs
/// Microsoft.VisualStudio.QualityTools.UnitTestFramework.dll 
/// C:\Program Files (x86)\(This is different depending on the version you have)\Common7\IDE\PublicAssemblies\" directory (for VS2010 professional or above; .NET Framework 4.0)
/// https://stackoverflow.com/questions/13602508/where-to-find-microsoft-visualstudio-testtools-unittesting-missing-dll
/// </summary>
namespace NeuralNetwork.Tests
{
    [TestClass]
    public class UnitsTests
    {
        [TestMethod]
        public void MatrixMath()
        {
            double[,] A = {  { 5, 6, 1 },
                            { 5, 7, 8 },
                            { 9, 1, 3 }    };
            double[,] B = {  { 5, 6, 2 },
                            { 8, 9, 1 },
                            { 3, 5, 6 } };

            double[,] Expected = {{ 10, 12, 3 },
                                { 13, 16, 9 },
                                { 12, 6, 9 } };

            Assert.IsTrue(ArraysAreEqual(Matrix.Add(A, B), Expected), "ThreeXThree Add");

            Expected = new double[,]{ { 0, 0, -1 },
                                    { -3, -2, 7 },
                                    { 6, -4, -3 }    };
            Assert.IsTrue(ArraysAreEqual(Matrix.Subtract(A, B), Expected), "ThreeXThree Subtract");

            Expected = new double[,]{ { 76, 89, 22 },
                                    { 105, 133, 65 },
                                    { 62, 78, 37 }    };
            Assert.IsTrue(ArraysAreEqual(Matrix.Multiply(A, B), Expected), "ThreeXThree Multiply");

            B =new double[,] {  { 5, 6 },
                            { 8, 9},
                            { 3, 5} };

            Expected = new double[,]{ { 76, 89 },
                                    { 105, 133 },
                                    { 62, 78 }    };
            Assert.IsTrue(ArraysAreEqual(Matrix.Multiply(A, B), Expected), "ThreeXThree and ThreeXTwo Multiply");

        }

        private bool ArraysAreEqual(double[,] A, double[,] B)
        {
            for (int i = 0; i < A.Rank; i++)
            {
                if (A.GetLength(i) != B.GetLength(i))
                {
                    throw new ArgumentOutOfRangeException("A and B array must have the same dimensions!");
                }
            }

            for(int i = 0; i < A.GetLength(0); i++)
            {
                for(int j = 0; j < B.GetLength(1); j++)
                {
                    Assert.AreEqual(A[i, j], B[i, j], 0.1);
                }
            }

            return true;
        }

        private bool ArraysAreEqual(double[] A, double[] B)
        {
            for (int i = 0; i < A.Rank; i++)
            {
                if (A.GetLength(i) != B.GetLength(i))
                {
                    throw new ArgumentOutOfRangeException("A and B array must have the same dimensions!");
                }
            }

            for (int i = 0; i < A.Length; i++)
            {
                Assert.AreEqual(A[i], B[i], 0.01);
            }

            return true;
        }

        /// <summary>
        /// Simple Neural Network Calculation test
        /// </summary>
        [TestMethod]
        public void NeuralNetworkCalculate()
        {
            FeedForwardNetwork net = new FeedForwardNetwork(new LayerOfNeurons[] 
            {
                new LayerOfNeurons(new double[,]{ { 3, 2 } }, new double[]{ 1.2 }, TransferFunction.LogSigmoid)
            });

            double[] expected = { (1d / (1d + Math.Exp(1.8))) };

            Assert.IsTrue(ArraysAreEqual(expected, net.Calculate(new double[] { -5, 6 })));



        }
        /// <summary>
        /// Make sure no pointer sharing happens
        /// </summary>
        [TestMethod]
        public void NeuralNetworkGeneticAlgorithm()
        {
            FeedForwardNetwork net = new FeedForwardNetwork(4, 6, 7, 9, true, TransferFunction.LogSigmoid);
            FeedForwardNetwork newNet = new FeedForwardNetwork(net);
            newNet.Mutate();

            for (int i = 0; i < net.HiddenLayers.Length; i++)
            {
                for (int j = 0; j < net.HiddenLayers[i].Biases.Length; j++)
                {
                    Assert.AreNotEqual(net.HiddenLayers[i].Biases[j], newNet.HiddenLayers[i].Biases[j], 0.01);
                }

                for (int j = 0; j < net.HiddenLayers[i].Weights.GetLength(0); j++)
                {
                    for (int k = 0; k < net.HiddenLayers[i].Weights.GetLength(1); k++)
                    {
                        Assert.AreNotEqual(net.HiddenLayers[i].Weights[j, k], newNet.HiddenLayers[i].Weights[j, k], 0.01);
                    }
                }
            }
            net.IncrementalTrain(new TrainingPoint(new double[] { 1, 1, 0, 0 }, new double[] { 1 , 1, 1, 1, 1, 1}));
        }

        /// <summary>
        /// 
        /// </summary>
        [TestMethod]
        public void SaveLoadNeuralNetwork()
        {
            FeedForwardNetwork net = new FeedForwardNetwork(4, 6, 7, 9, true, TransferFunction.LogSigmoid);

            string folderPath = AppDomain.CurrentDomain.BaseDirectory,
                fileName = "helloWorld",
                path = $"{ folderPath }\\{fileName}.bin";

            net.Save(folderPath, fileName);

            FeedForwardNetwork newNet = FeedForwardNetwork.Load(folderPath, fileName);

            for (int i = 0; i < net.HiddenLayers.Length; i++)
            {
                for (int j = 0; j < net.HiddenLayers[i].Biases.Length; j++)
                {
                    Assert.AreEqual(net.HiddenLayers[i].Biases[j], newNet.HiddenLayers[i].Biases[j]);
                }

                for (int j = 0; j < net.HiddenLayers[i].Weights.GetLength(0); j++)
                {
                    for (int k = 0; k < net.HiddenLayers[i].Weights.GetLength(1); k++)
                    {
                        Assert.AreEqual(net.HiddenLayers[i].Weights[j, k], newNet.HiddenLayers[i].Weights[j, k]);
                    }
                }
            }


            System.IO.File.Delete(path);
        }


        /// <summary>
        /// Tests the training methods of the neural network
        /// </summary>
        [TestMethod]
        public void NeuralNetworkIncrementalTrain()
        {
            FeedForwardNetwork net = new FeedForwardNetwork(new LayerOfNeurons[]
            {
                new LayerOfNeurons(new double[,]{ { -0.27 }, { -0.41 } },
                    new double[]{ -0.48 , -0.13 },
                    TransferFunction.LogSigmoid),
                new LayerOfNeurons(new double[,]{ { 0.09, -0.17 } },
                    new double[]{ 0.48 },
                    TransferFunction.PureLine)
            });

            double[] expected = new double[] { 0.446 };
            Assert.IsTrue(ArraysAreEqual(expected, net.Calculate(new double[] { 1 })));

            net.IncrementalTrain(new TrainingPoint(new double[] { 1 }, new double[] { 1 + Math.Sin(Math.PI / 4) }));

            Assert.IsTrue(ArraysAreEqual(net.HiddenLayers[0].Biases, new double[] { -0.475, -0.140 }));
            Assert.IsTrue(ArraysAreEqual(net.HiddenLayers[1].Biases, new double[] { 0.732 }));

            Assert.IsTrue(ArraysAreEqual(net.HiddenLayers[0].Weights, new double[,] { { -0.265 }, { -0.420 } }));
            Assert.IsTrue(ArraysAreEqual(net.HiddenLayers[1].Weights, new double[,] { { 0.171, -0.0772 } }));

            // test without biases
            net = new FeedForwardNetwork(new LayerOfNeurons[]
            {
                    new LayerOfNeurons(new double[,]{ { -0.27, 0.05 } },
                        null,
                        TransferFunction.LogSigmoid)
            });
            // make sure we don't crash
            net.IncrementalTrain(new TrainingPoint(new double[] { 1, 1 }, new double[] { 1 + Math.Sin(Math.PI / 4) }));
        }

        [TestMethod]
        public void NeuralNetworkBatchTrain()
        {
            FeedForwardNetwork net = new FeedForwardNetwork(new LayerOfNeurons[]
            {
                new LayerOfNeurons(new double[,]{ { -0.27, 0.05 } },
                    new double[]{ -0.48 },
                    TransferFunction.LogSigmoid)
            });

            List<TrainingPoint> pointCollection = new List<TrainingPoint>();
            // AND gate
            pointCollection.Add(
                new TrainingPoint(
                    new double[] { 1, 1 },
                new double[] { 1 }));

            pointCollection.Add(
                new TrainingPoint(
                    new double[] { 0, 0 },
                new double[] { 0 }));

            pointCollection.Add(
                new TrainingPoint(
                    new double[] { 1, 0 },
                new double[] { 0 }));

            pointCollection.Add(
                new TrainingPoint(
                    new double[] { 0, 1 },
                new double[] { 0 }));

            net.BatchTrain(pointCollection, 0.9999, 0.1);

            Assert.IsTrue(ArraysAreEqual(pointCollection[0].ExpectedOutput, net.Calculate( pointCollection[0].Input)));


        }
    }
}
