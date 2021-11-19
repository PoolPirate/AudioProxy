using NAudio.Wave;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AudioProxy.Audio
{
    public sealed class MultiReaderWaveProvider
    {
        private readonly Stream WaveStream;
        private readonly object AdvanceLock;

        private Memory<byte> Buffer;

        private bool Disposed;
        private bool Finished;

        private long Position;

        public WaveFormat WaveFormat { get; }

        public MultiReaderWaveProvider(Stream waveStream, WaveFormat format, int bufferSeconds)
        {
            AdvanceLock = new object();
            WaveStream = waveStream;
            WaveFormat = format;

            int bufferSize = WaveFormat.AverageBytesPerSecond * bufferSeconds;
            Buffer = new byte[bufferSize];

            Position = -Buffer.Length;
        }

        public int Read(byte[] buffer, long currentPosition, int offset, int count)
        {
            count = Math.Min(count, Buffer.Length);

            LoadNecessaryData(currentPosition, currentPosition + count);

            int startIndex = (int)(currentPosition - Position);
            int skippedBytes = 0;

            if (startIndex < 0)
            {
                skippedBytes = -startIndex;
                startIndex = 0;
                Debug.Print($"Warning: Player running behind. Skipping {skippedBytes} Bytes");
            }

            int bytesToCopy = (int)Math.Min(count, Buffer.Length - startIndex);

            Buffer.Slice(startIndex, bytesToCopy).CopyTo(buffer.AsMemory()[offset..]);
            return bytesToCopy + skippedBytes;
        }

        private void LoadNecessaryData(long startPosition, long endPosition)
        {
            if (startPosition == Position)
            {

            }

            lock (WaveStream)
            {
                if (startPosition < Position)
                {
                    throw new IndexOutOfRangeException("Tried to read data that has been removed from the Buffer! Is a AudioPlayer running behind?");
                }
                if (endPosition <= Position + Buffer.Length) //No loading necessary
                {
                    return;
                }
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
            int bytesRead = WaveStream.Read(emptiedSlice);

            if (bytesRead != Buffer.Length / 2)
            {
                Finished = true;
            }

            Buffer = Buffer.Slice(0, (Buffer.Length / 2) + bytesRead);
        }

        public ValueTask DisposeAsync()
        {
            lock (WaveStream)
            {
                if (Disposed)
                {
                    return ValueTask.CompletedTask;
                }
                Disposed = true;
            }

            return WaveStream.DisposeAsync();
        }
    }
}
