using NAudio.Wave;
using System;

namespace AudioProxy.Audio
{
    public interface IAudioStream : IAsyncDisposable
    {
        public WaveFormat WaveFormat { get; }

        public int Read(byte[] buffer, long currentPosition, int offset, int count);
    }
}
