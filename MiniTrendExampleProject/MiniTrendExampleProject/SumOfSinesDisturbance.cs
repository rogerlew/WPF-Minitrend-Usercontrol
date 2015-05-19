using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniTrendExampleProject
{
    class SumOfSinesDisturbance
    {
        public struct WaveformComponent
        {
            public double amplitude;
            public double frequency;
            public double phase;

            public WaveformComponent(double amplitude, double frequency, double phase)
            {
                this.amplitude = amplitude;
                this.frequency = frequency;
                this.phase = phase;
            }
        }

        Random random;
        List<WaveformComponent> waveformComponents;
        double stdev;
        double bias;

        public SumOfSinesDisturbance(List<WaveformComponent> waveformComponents, double stdev=0.5, double bias=0.0)
        {
            this.waveformComponents = waveformComponents;
            this.stdev = stdev;
            this.bias = bias;
            random = new Random();
        }

        public double Sample(double t)
        {
            double foo = 0;

            foreach (var w in waveformComponents)
            {
                foo += w.amplitude * Math.Sin(t * 2.0 * Math.PI * w.phase * w.frequency);
            }
                
            foo += bias;
            foo += Normal(stdev);

            return foo;
        }

        double Normal(double stdDev)
        {
            // http://stackoverflow.com/questions/218060/random-gaussian-variables

            if (stdDev == 0.0)
            {
                return 0.0;
            }
            double u1 = random.NextDouble();
            double u2 = random.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                                   Math.Sin(2.0 * Math.PI * u2);
            return stdDev * randStdNormal;
        }
    }

}
