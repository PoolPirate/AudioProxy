using AudioProxy.Audio;
using AudioProxy.Models;
using AudioProxy.Options;
using Common.Services;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioProxy.Services
{
    public sealed class AudioInputService : Service, IDisposable
    {
        [Inject]
        private readonly DeviceService DeviceService;
        [Inject]
        private readonly AudioOutputService AudioOutputService;
        [Inject]
        private readonly AudioOptions AudioOptions;
        [Inject]
        private readonly KeyboardHookService KeyboardHookService;

        private readonly List<AudioRecorder> Recorders;

        public AudioInputService()
        {
            Recorders = new List<AudioRecorder>();
        }

        protected override ValueTask InitializeAsync()
        {
            foreach (var inputDevice in DeviceService.GetAllInputDevices())
            {
                AddDevice(inputDevice);
            }

            return default;
        }

        private void HandleRecorderData(AudioRecorder recorder, WaveInEventArgs e)
        {
            if (!recorder.Device.Mode.IsFulfilled(KeyboardHookService.GetPressedKeys()))
            {
                return;
            }

            AudioOutputService.PlayInput(recorder.Device, e.Buffer, e.BytesRecorded);
        }

        public void AddDevice(InputDevice inputDevice)
        {
            var recorder = new AudioRecorder(inputDevice, AudioOptions.SampleRate, AudioOptions.ChannelCount, AudioOptions.InputDelay);
            lock (Recorders)
            {
                Recorders.Add(recorder);
            }
            recorder.OnDataAvailable += HandleRecorderData;
            recorder.Start();
        }
        public void RemoveDevice(InputDevice inputDevice)
        {
            lock (Recorders)
            {
                var recorder = Recorders.Where(x => x.Device == inputDevice).FirstOrDefault();
                Recorders.Remove(recorder);
                recorder.Dispose();
            }
        }

        public void Dispose()
        {
            foreach (var recorder in Recorders)
            {
                recorder.Dispose();
            }
        }
    }
}
