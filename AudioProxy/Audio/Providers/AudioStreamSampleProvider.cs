using AudioProxy.Models;
using AudioProxy.Services;
using NAudio.Wave;
using System;
using System.Threading.Tasks;

namespace AudioProxy.Audio
{
    public sealed class AudioStreamSampleProvider : ISampleProvider, IAsyncDisposable
    {
        private readonly MultiReaderWaveProvider WaveReader;
        private readonly DeviceMode ActivationMode;
        private readonly KeyboardHookService KeyboardHookService;

        private readonly MediaFoundationResampler Resampler;
        public readonly ISampleProvider SampleProvider;

        public WaveFormat WaveFormat { get; set; }

        public AudioStreamSampleProvider(MultiReaderWaveProvider waveReader, DeviceMode activationMode, WaveFormat waveFormat, int resamplerQuality, KeyboardHookService keyboardHookService)
        {
            WaveReader = waveReader;
            WaveFormat = waveFormat;
            ActivationMode = activationMode;
            KeyboardHookService = keyboardHookService;

            IWaveProvider waveProvider = new PositionedWaveProvider(WaveReader);

            if (waveFormat.SampleRate != waveProvider.WaveFormat.SampleRate || 
                waveFormat.BitsPerSample != waveProvider.WaveFormat.BitsPerSample || 
                waveFormat.Channels != waveProvider.WaveFormat.Channels)
            {
                Resampler = new MediaFoundationResampler(waveProvider, waveFormat)
                {
                    ResamplerQuality = resamplerQuality,
                };
                waveProvider = Resampler;
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
            if (Resampler is not null)
            {
                Resampler.Dispose();
            }

            return WaveReader.DisposeAsync();
        }
    }
}
