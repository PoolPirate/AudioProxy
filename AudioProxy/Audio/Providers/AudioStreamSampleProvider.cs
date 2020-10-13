using System;
using AudioProxy.Models;
using AudioProxy.Services;
using NAudio.Wave;

namespace AudioProxy.Audio
{
    public sealed class AudioStreamSampleProvider : ISampleProvider, IDisposable
    {
        private readonly IAudioStream AudioStream;
        private readonly DeviceMode ActivationMode;
        private readonly KeyboardHookService KeyboardHookService;
        private int Position;
        public WaveFormat WaveFormat { get; set; }


        public AudioStreamSampleProvider(IAudioStream stream, WaveFormat waveFormat, DeviceMode activationMode, KeyboardHookService keyboardHookService)
        {
            AudioStream = stream;
            WaveFormat = waveFormat;
            ActivationMode = activationMode;
            KeyboardHookService = keyboardHookService;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            bool isActivated = ActivationMode.IsFulfilled(KeyboardHookService.GetPressedKeys());

            int samplesRead = isActivated
                ? AudioStream.Read(buffer, offset, count, Position)
                : AudioStream.Advance(count, Position);

            if (!isActivated)
            {
                Array.Fill(buffer, 0, offset, count);
            }

            Position += samplesRead;
            return samplesRead;
        }

        public void Dispose()
            => AudioStream.Dispose();
    }
}
