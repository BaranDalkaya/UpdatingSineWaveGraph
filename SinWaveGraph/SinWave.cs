using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SinWaveGraph
{
    public class SinWave
    {
        private float Amplitude { get; set; }
        private float Frequency { get; set; }
        public float CurrentValue { get; set; }
        public float TimeStep { get; set; }


        public SinWave()
        {
            this.Amplitude = 1;
            this.Frequency = 1;
            this.TimeStep = 0;
        }

        public SinWave(float amplitude, float frequency)
        {
            this.Amplitude = amplitude;
            this.Frequency = frequency;
        }

        public void SetNextValue()
        {
            float nextValue = Amplitude * (float)Math.Sin(2 * Math.PI * Frequency * TimeStep);
            TimeStep += 0.01f;
            CurrentValue = nextValue;
        }
    }
}
