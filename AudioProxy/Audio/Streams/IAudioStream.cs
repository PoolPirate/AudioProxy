using System;

namespace AudioProxy.Audio
{
    public interface IAudioStream : IDisposable
    {
        public int Read(float[] buffer, int offset, int count, int position);
        public int Advance(int count, int position);
    }
}
