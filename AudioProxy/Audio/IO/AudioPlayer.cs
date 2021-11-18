using AudioProxy.Helpers;
using AudioProxy.Models;
using AudioProxy.Services;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace AudioProxy.Audio
{
    public sealed class AudioPlayer : IAsyncDisposable
    {
        private readonly WaveOutEvent WaveOut;
        private readonly ConcurrentDictionary<InputDevice, BufferedWaveProvider> InputBuffers;
        private readonly WaveFormat WaveFormat;
        private readonly KeyboardHookService KeyboardHookService;

        private readonly MixingSampleProvider InputMixer;
        private readonly MixingSampleProvider SoundMixer;
        private readonly MixingSampleProvider OutputMixer;

        private readonly ushort ResamplerQuality;

        public readonly OutputDevice Device;

        public AudioPlayer(OutputDevice device, WaveFormat waveFormat, int delay, ushort resamplerQuality, KeyboardHookService keyboardHookService)
        {
            KeyboardHookService = keyboardHookService;
            Device = device;
            WaveFormat = waveFormat;
            ResamplerQuality = resamplerQuality;

            WaveOut = new WaveOutEvent()
            {
                DeviceNumber = AudioDeviceHelper.GetOutputDeviceNumberByName(device.Name),
                DesiredLatency = delay
            };

            InputMixer = new MixingSampleProvider(WaveFormat)
            {
                ReadFully = true,
            };
            SoundMixer = new MixingSampleProvider(WaveFormat)
            {
                ReadFully = true,
            };

            SoundMixer.MixerInputEnded += HandleMixerInputEnded;

            OutputMixer = new MixingSampleProvider(WaveFormat)
            {
                ReadFully = true,
            };

            InputBuffers = new ConcurrentDictionary<InputDevice, BufferedWaveProvider>();
        }

        //Async void is used as DisposeAsync should never fail and we are not interested in the output 
        private async void HandleMixerInputEnded(object sender, SampleProviderEventArgs e)
        {
            if (e.SampleProvider is AudioStreamSampleProvider audioStreamSampleProvider)
            {
                await audioStreamSampleProvider.DisposeAsync();
            }
        }

        public void Start()
        {
            OutputMixer.AddMixerInput(InputMixer);
            OutputMixer.AddMixerInput(SoundMixer);

            WaveOut.Init(OutputMixer);
            WaveOut.Play();
        }

        public Task ClearAsync() 
            => ClearMixerAsync(SoundMixer);

        private async Task ClearMixerAsync(MixingSampleProvider mixer)
        {
            var inputs = SoundMixer.MixerInputs;
            SoundMixer.RemoveAllMixerInputs();

            foreach (var input in inputs)
            {
                if (input is AudioStreamSampleProvider audioStreamSampleProvider)
                {
                    await audioStreamSampleProvider.DisposeAsync();
                }
            }
        }

        public void RemoveBuffer(InputDevice inputDevice)
        {
            if (!InputBuffers.TryRemove(inputDevice, out var buffer))
            {
                return;
            }


            buffer.ClearBuffer();
        }

        public void PlaySound(MultiReaderWaveProvider audioStream)
        {
            var sampleProvider = new AudioStreamSampleProvider(audioStream, Device.SoundsMode, WaveFormat, ResamplerQuality, KeyboardHookService);
            SoundMixer.AddMixerInput(sampleProvider);
        }
        public void PlayInput(InputDevice inputDevice, byte[] buffer, int bytesRecorded)
        {
            var inputBuffer = InputBuffers.GetOrAdd(inputDevice, device =>
            {
                var inputBuffer = new BufferedWaveProvider(InputMixer.WaveFormat)
                {
                    DiscardOnBufferOverflow = true,
                    ReadFully = true
                };
                InputMixer.AddMixerInput(inputBuffer);
                return inputBuffer;
            });
            inputBuffer.AddSamples(buffer, 0, bytesRecorded);
        }

        public async ValueTask DisposeAsync()
        {
            await ClearMixerAsync(OutputMixer);
            await ClearMixerAsync(SoundMixer);
            await ClearMixerAsync(InputMixer);
            WaveOut.Dispose();
        }
    }
}
