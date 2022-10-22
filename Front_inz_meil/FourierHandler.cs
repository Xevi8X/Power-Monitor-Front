using MathNet.Numerics;
using MathNet.Numerics.IntegralTransforms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Front_inz_meil
{
    public static class FourierHandler
    {
        public static (double[] hz, double[] mag) Calc(List<MainForm.Measure> measures)
        {
            double meanMicros = Enumerable.Range(1, measures.Count - 1).Select(n => measures[n].microsecounds - measures[n - 1].microsecounds).Average();
            double samplesPerSecond = 1000000 / meanMicros;
            int numberOfSamples = measures.Count;
            Complex32[] samples = measures.Select(m => new Complex32((float)m.voltage, 0.0f)).ToArray();

            Fourier.Forward(samples, FourierOptions.Matlab);

            double hzPerSample = samplesPerSecond / numberOfSamples;
            int noOfRes = numberOfSamples / 2 + 1;
            double[] hz = new double[noOfRes];
            double[] mag = new double[noOfRes];

            for (int i = 0; i < noOfRes; i++)
            {
                hz[i] = i * hzPerSample;
                mag[i] = (2.0 / numberOfSamples) *
                    Math.Sqrt(Math.Pow(samples[i].Real, 2) + Math.Pow(samples[i].Imaginary, 2));
            }
            return (hz, mag);
        }
    }
}
