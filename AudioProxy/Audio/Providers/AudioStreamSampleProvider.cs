using AudioProxy.Models;
using AudioProxy.Services;
using NAudio.Wave;
using System;
using System.Threading.Tasks;

namespace AudioProxy.Audio
{
    public sealed class AudioStreamSampleProvider : ISampleProvider, IAsyncDisposable
    {
        private readonly IAudioStream AudioStream;
        private readonly DeviceMode ActivationMode;
        private readonly KeyboardHookService KeyboardHookService;

        public readonly ISampleProvider SampleProvider;

        public WaveFormat WaveFormat { get; set; }

        public AudioStreamSampleProvider(IAudioStream stream, DeviceMode activationMode, WaveFormat waveFormat, KeyboardHookService keyboardHookService)
        {
            AudioStream = stream;
            WaveFormat = waveFormat;
            ActivationMode = activationMode;
            KeyboardHookService = keyboardHookService;

            IWaveProvider waveProvider = new PositionedWaveProvider(stream);

            if (waveFormat.SampleRate != stream.WaveFormat.SampleRate || 
                waveFormat.BitsPerSample != stream.WaveFormat.BitsPerSample || 
                waveFormat.Channels != stream.WaveFormat.Channels)
            {
                waveProvider = new MediaFoundationResampler(waveProvider, waveFormat);
            }

            SampleProvider = waveProvider.ToSampleProvider();
        }

        public int Read(float[] buffer, int offset, int count)
        {
            bool isActivated = ActivationMode.IsFulfilled(KeyboardHookService.GetPressedKeys());
            int samplesRead = SampleProvider.Read(buffer, offset, count);

            if (!isActivated)
            {
                Array.Fill(buffer, 0f, offset, samplesRead);
            }

            return samplesRead;
        }

        public ValueTask DisposeAsync()
        {
            return AudioStream.DisposeAsync();
        }
    }
}
