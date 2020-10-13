using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using NAudio.Wave;

namespace AudioProxy.Audio
{
    public sealed class FileAudioStream : IAudioStream, IDisposable
    {
        public readonly WaveFormat WaveFormat;

        private readonly AudioFileReader AudioReader;
        private bool Disposed;
        private bool Finished;

        private int BufferPosition;
        private int BufferEnd = int.MaxValue;
        private float[] Buffer;
        private readonly int BufferSize;

        public FileAudioStream(string filePath)
        {
            AudioReader = new AudioFileReader(filePath);
            //ToDo: Add Resampler using Target Format

            WaveFormat = AudioReader.WaveFormat;
            BufferSize = WaveFormat.SampleRate * WaveFormat.Channels * 2;
            Buffer = new float[BufferSize];

            int samplesRead = AudioReader.Read(Buffer, 0, BufferSize);

            if (samplesRead < BufferSize)
            {
                FinishAtPosition(samplesRead);
            }
        }

        public int Read(float[] buffer, int offset, int count, int position)
        {
            int samplesCopied = Advance(count, position);
            Array.Copy(Buffer, position - BufferPosition, buffer, offset, samplesCopied);

            if (samplesCopied < count)
            {
                Dispose();
            }

            return samplesCopied;
        }
        public int Advance(int count, int position)
        {
            if (position < BufferPosition)
            {
                throw new InvalidOperationException("Tried to read Audio Samples that already left the FileAudioStream! Is a AudioPlayer lacking behind?");
            }

            int endPosition = position + count;
            lock (AudioReader)
            {
                if (!Finished && !IsCached(endPosition))
                {
                    MoveForward();
                }
            }

            return Math.Min(count, BufferEnd - position);
        }


        private void MoveForward()
        {
            int segmentLenght = BufferSize / 2;
            Buffer.AsMemory(segmentLenght).CopyTo(Buffer); //Move Second Half of Buffer to the front
            int samplesRead = AudioReader.Read(Buffer, segmentLenght, segmentLenght);

            BufferPosition += segmentLenght;

            if (samplesRead < segmentLenght)
            {
                FinishAtPosition(BufferPosition + segmentLenght + samplesRead);
            }
        }
        private void FinishAtPosition(int position)
        {
            BufferEnd = position;
            Finished = true;
        }

        private bool IsCached(int position)
            => position <= BufferPosition + Buffer.Length;

        public void Dispose()
        {
            lock (AudioReader)
            {
                if (Disposed)
                {
                    return;
                }
                Disposed = true;
            }

            AudioReader.Dispose();
        }
    }
}
