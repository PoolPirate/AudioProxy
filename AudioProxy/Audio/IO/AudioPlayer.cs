using AudioProxy.Helpers;
using AudioProxy.Models;
using AudioProxy.Services;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Concurrent;

namespace AudioProxy.Audio
{
    public sealed class AudioPlayer : IDisposable
    {
        private readonly WaveOutEvent WaveOut;
        private readonly ConcurrentDictionary<InputDevice, BufferedWaveProvider> InputBuffers;
        private readonly WaveFormat WaveFormat;
        private readonly KeyboardHookService KeyboardHookService;

        private readonly MixingSampleProvider InputMixer;
        private readonly MixingSampleProvider SoundMixer;
        private readonly MixingSampleProvider OutputMixer;

        public readonly OutputDevice Device;

        public AudioPlayer(OutputDevice device, int sampleRate, int channelCount, int delay, KeyboardHookService keyboardHookService)
        {
            KeyboardHookService = keyboardHookService;
            Device = device;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount);
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
            OutputMixer = new MixingSampleProvider(WaveFormat)
            {
                ReadFully = true,
            };

            InputBuffers = new ConcurrentDictionary<InputDevice, BufferedWaveProvider>();
        }

        public void Start()
        {
            OutputMixer.AddMixerInput(InputMixer);
            OutputMixer.AddMixerInput(SoundMixer);

            WaveOut.Init(OutputMixer);
            WaveOut.Play();
        }
        public void Dispose()
        {
            OutputMixer.RemoveAllMixerInputs();
            SoundMixer.RemoveAllMixerInputs();
            InputMixer.RemoveAllMixerInputs();
            WaveOut.Dispose();
        }
        public void Clear()
            => SoundMixer.RemoveAllMixerInputs();

        public void RemoveBuffer(InputDevice inputDevice)
        {
            if (!InputBuffers.TryRemove(inputDevice, out var buffer))
            {
                return;
            }


            buffer.ClearBuffer();
        }

        public void PlaySound(IAudioStream audioStream)
        {
            var sampleProvider = new AudioStreamSampleProvider(audioStream, Device.SoundsMode, WaveFormat, KeyboardHookService);
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
    }
}
