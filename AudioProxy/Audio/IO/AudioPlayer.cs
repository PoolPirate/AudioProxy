using System;
using System.Collections.Concurrent;
using System.Linq;
using AudioProxy.Helpers;
using AudioProxy.Models;
using AudioProxy.Services;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace AudioProxy.Audio
{
    public sealed class AudioPlayer : IDisposable
    {
        private readonly WaveOutEvent WaveOut;
        private readonly MixingSampleProvider Mixer;
        private readonly ConcurrentDictionary<InputDevice, BufferedWaveProvider> InputBuffers;
        private readonly WaveFormat WaveFormat;
        private readonly KeyboardHookService KeyboardHookService;

        public readonly OutputDevice Device;

        public AudioPlayer(OutputDevice device, int sampleRate, int channelCount, int delay, KeyboardHookService keyboardHookService)
        {
            Device = device;
            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount);
            WaveOut = new WaveOutEvent()
            {
                DeviceNumber = AudioDeviceHelper.GetOutputDeviceNumberByName(device.Name),
                DesiredLatency = delay
            };
            Mixer = new MixingSampleProvider(WaveFormat)
            {
                ReadFully = true,
            };

            InputBuffers = new ConcurrentDictionary<InputDevice, BufferedWaveProvider>();
            KeyboardHookService = keyboardHookService;
        }

        public void Start()
        {
            WaveOut.Init(Mixer);
            WaveOut.Play();
        }
        public void Dispose()
        {
            Mixer.RemoveAllMixerInputs();
            WaveOut.Dispose();
        }
        public void Clear()
        {
            foreach(var input in Mixer.MixerInputs.ToArray())
            {
                if(input.GetType() == typeof(BufferedWaveProvider)) 
                    //Ignore, they will be recreated on next mic input anyway
                {
                    continue;
                }

                Mixer.RemoveMixerInput(input);

                if (input is AudioStreamSampleProvider streamSampleProvider)
                {
                    streamSampleProvider.Dispose();
                }
            }
        }

        public void PlayAudio(IAudioStream audioStream)
        {
            var sampleProvider = new AudioStreamSampleProvider(audioStream, WaveFormat, Device.SoundsMode, KeyboardHookService);
            AddMixerInput(sampleProvider);
        }
        public void PlayAudio(InputDevice inputDevice, byte[] buffer, int bytesRecorded)
        {
            var inputBuffer = InputBuffers.GetOrAdd(inputDevice, device =>
            {
                var inputBuffer = new BufferedWaveProvider(Mixer.WaveFormat)
                {
                    DiscardOnBufferOverflow = true,
                    ReadFully = true
                };
                Mixer.AddMixerInput(inputBuffer);
                return inputBuffer;
            });
            inputBuffer.AddSamples(buffer, 0, bytesRecorded);
        }

        private void AddMixerInput(ISampleProvider input)
            => Mixer.AddMixerInput(ConvertToRightChannelCount(input));

        private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
            => input.WaveFormat.Channels == 1 && Mixer.WaveFormat.Channels == 2
                ? new MonoToStereoSampleProvider(input)
                : input.WaveFormat.Channels == 2 && Mixer.WaveFormat.Channels == 1
                    ? new StereoToMonoSampleProvider(input)
                    : input;
    }
}
