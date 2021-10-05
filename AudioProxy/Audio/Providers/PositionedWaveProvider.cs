using NAudio.Wave;

namespace AudioProxy.Audio
{
    public sealed class PositionedWaveProvider : IWaveProvider
    {
        private readonly IAudioStream SourceStream;
        private int Position;
        public WaveFormat WaveFormat => SourceStream.WaveFormat;

        public PositionedWaveProvider(IAudioStream sourceStream)
        {
            SourceStream = sourceStream;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            int samplesRead = SourceStream.Read(buffer, Position, offset, count);
            Position += samplesRead;
            return samplesRead;
        }
    }
}
