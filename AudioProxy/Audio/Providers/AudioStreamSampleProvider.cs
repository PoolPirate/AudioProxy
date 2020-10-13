using System;
using NAudio.Wave;

namespace AudioProxy.Audio
{
    public sealed class AudioStreamSampleProvider : ISampleProvider, IDisposable
    {
        private readonly IAudioStream AudioStream;
        private int Position;
        public WaveFormat WaveFormat { get; set; }


        public AudioStreamSampleProvider(IAudioStream stream, WaveFormat waveFormat)
        {
            AudioStream = stream;
            WaveFormat = waveFormat;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int samplesRead = AudioStream.Read(buffer, offset, count, Position);
            Position += samplesRead;
            return samplesRead;
        }

        public void Dispose()
            => AudioStream.Dispose();
    }
}
