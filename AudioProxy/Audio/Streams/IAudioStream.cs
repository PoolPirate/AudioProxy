using System;

namespace AudioProxy.Audio
{
    public interface IAudioStream : IDisposable
    {
        public int Read(float[] buffer, int offset, int count, int position);
    }
}
