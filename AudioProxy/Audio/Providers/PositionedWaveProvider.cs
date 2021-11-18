using NAudio.Wave;

namespace AudioProxy.Audio
{
    public sealed class PositionedWaveProvider : IWaveProvider
    {
        private readonly MultiReaderWaveProvider WaveReader;
        private int Position;
        public WaveFormat WaveFormat => WaveReader.WaveFormat;

        public PositionedWaveProvider(MultiReaderWaveProvider waveReader)
        {
            WaveReader = waveReader;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            int samplesRead = WaveReader.Read(buffer, Position, offset, count);
            Position += samplesRead;
            return samplesRead;
        }
    }
}
