using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RadioCS
{
    internal class WaveProviderDisposableWrapper : IDisposable
    {
        public BufferedWaveProvider WaveProvider { get; }

        public WaveProviderDisposableWrapper(BufferedWaveProvider waveProvider)
        {
            WaveProvider = waveProvider;
        }

        public void Dispose()
        {
            WaveProvider.ClearBuffer();
        }
    }

}
