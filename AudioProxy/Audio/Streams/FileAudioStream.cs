using NAudio.Wave;
using System;
using System.Threading.Tasks;

namespace AudioProxy.Audio
{
    public sealed class FileAudioStream : IAudioStream
    {
        private readonly AudioFileReader AudioReader;

        private Memory<byte> Buffer;

        private bool Disposed;
        private bool Finished;

        private long Position;

        public WaveFormat WaveFormat { get; }

        public FileAudioStream(string filePath, int bufferSeconds)
        {
            AudioReader = new AudioFileReader(filePath);
            WaveFormat = AudioReader.WaveFormat;
            int bufferSize = WaveFormat.SampleRate * WaveFormat.Channels * bufferSeconds;
            Buffer = new byte[bufferSize];

            Position = -Buffer.Length;
        }

        public int Read(byte[] buffer, long currentPosition, int offset, int count)
        {
            LoadNecessaryData(currentPosition, currentPosition + count - Buffer.Length);

            int startIndex = (int)(currentPosition - Position);
            int bytesToCopy = (int)Math.Min(count, Buffer.Length - startIndex);

            Buffer.Slice(startIndex, bytesToCopy).CopyTo(buffer.AsMemory()[offset..]);
            return bytesToCopy;
        }

        private void LoadNecessaryData(long startPosition, long endPosition)
        {
            if (startPosition < Position)
            {
                throw new IndexOutOfRangeException("Tried to read data that has been removed from the Buffer! Is a AudioPlayer running behind?");
            }
            if (endPosition <= Position) //No loading necessary
            {
                return;
            }

            lock (AudioReader)
            {
                if (Finished)
                {
                    return;
                }

                AdvanceBuffer();
            }
        }

        private void AdvanceBuffer()
        {
            Buffer[(Buffer.Length / 2)..].CopyTo(Buffer); //Copy second half of the Buffer to the front
            Position += Buffer.Length / 2;

            var emptiedSlice = Buffer[(Buffer.Length / 2)..].Span;
            int bytesRead = AudioReader.Read(emptiedSlice);

            if (bytesRead != Buffer.Length / 2)
            {
                Finished = true;
            }

            Buffer = Buffer.Slice(0, (Buffer.Length / 2) + bytesRead);
        }

        public ValueTask DisposeAsync()
        {
            lock (AudioReader)
            {
                if (Disposed)
                {
                    return ValueTask.CompletedTask;
                }
                Disposed = true;
            }

            return AudioReader.DisposeAsync();
        }
    }
}
