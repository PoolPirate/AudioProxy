using AudioProxy.Helpers;
using AudioProxy.Models;
using NAudio.Wave;
using System;

namespace AudioProxy.Audio
{
    public sealed class AudioRecorder : IDisposable
    {
        public readonly InputDevice Device;
        private readonly WaveInEvent WaveIn;

        public event Action<AudioRecorder, WaveInEventArgs> OnDataAvailable;

        public AudioRecorder(InputDevice device, int sampleRate, int channelCount, int delay)
        {
            Device = device;
            WaveIn = new WaveInEvent()
            {
                DeviceNumber = AudioDeviceHelper.GetInputDeviceNumberByName(device.Name),
                WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount),
                BufferMilliseconds = delay,
            };

            WaveIn.DataAvailable += HandleDataAvailable;
        }

        public void Start()
            => WaveIn.StartRecording();
        public void Dispose()
        {
            WaveIn.DataAvailable -= HandleDataAvailable;
            WaveIn.Dispose();
        }


        private void HandleDataAvailable(object sender, WaveInEventArgs e)
            => OnDataAvailable?.Invoke(this, e);
    }
}
