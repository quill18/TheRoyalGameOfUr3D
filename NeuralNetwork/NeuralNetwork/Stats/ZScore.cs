using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork.Stats
{
    internal class ZScore
    {

        /// <summary>
        /// returns a random number based on a gaussian distribution
        /// </summary>
        internal static double RandomGaussianDistribution(double mean, double stdev)
        {

            Random rand = new Random();

            double zScore = ZLessThanX(rand.NextDouble());

            return mean + stdev * (zScore - 0.5);
        }


        /// <summary>
        /// The actual probability density function, for a standard normal distribution
        /// </summary>
        private static double StandardNormalPdf(double x)
        {
            var exponent = -1 * (0.5 * Math.Pow(x, 2));
            var numerator = Math.Pow(Math.E, exponent);
            var denominator = Math.Sqrt(2 * Math.PI);
            return numerator / denominator;
        }

        public static double ProbabilityLessThanX(double x)
        {
            var integral = Integral(StandardNormalPdf, 0, x);
            return integral + 0.5;
        }


        private delegate double Function(double x);
        /// <summary>
        /// integral of a function between a and b using simpson's rule
        /// </summary>
        private static double Integral(Function f, double a, double b)
        {
            double multiplier = (b - a) / 8;

            double sum = multiplier * (f(a) + (3 * f(((2 * a) + b) / 3)) + (3 * f((a + (2 * b)) / 3)) + f(b));

            return sum;
        }

        /// <summary>
        /// P is the key and Z is the value
        /// </summary>
        private static Dictionary<double, double> _PtoZ = new Dictionary<double, double>();
        /// <summary>
        /// gets z sore based on probability of a gaussian distribution
        /// </summary>
        public static double ZLessThanX(double p)
        {
            double value;
            if (_PtoZ.TryGetValue(p, out value))
            {
                return value;
            }

            if (p > 1 || p < 0)
            {
                return 0;
            }
            double err = 1,
             maxErr = 0.00000001,
             testZ = 0,
             testP = 0.5,
             step = 3;
            while (maxErr < err)
            {
                testP = ProbabilityLessThanX(testZ);
                if (testP >= p)
                {
                    testZ -= step;
                }
                else
                {
                    testZ += step;
                }
                if (testZ > 3)
                {
                    return 1;
                }
                else if (testZ < -3)
                {
                    return 0;
                }
                step = (double)step / (double)2;
                err = Math.Abs(testP - p);
            }
            _PtoZ.Add(p, testZ);
            return testZ;
        }
    }
}
