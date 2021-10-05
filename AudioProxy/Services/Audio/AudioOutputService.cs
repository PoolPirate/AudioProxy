using AudioProxy.Audio;
using AudioProxy.Models;
using AudioProxy.Options;
using Common.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AudioProxy.Services
{
    public sealed class AudioOutputService : Service, IDisposable
    {
        [Inject] private readonly AudioLoaderService AudioLoaderService;
        [Inject] private readonly KeyboardHookService KeyboardHookService;
        [Inject] private readonly DeviceService DeviceService;
        [Inject] private readonly AudioOptions AudioOptions;

        private readonly List<AudioPlayer> Players;

        public AudioOutputService()
        {
            Players = new List<AudioPlayer>();
        }

        protected override ValueTask InitializeAsync()
        {
            foreach (var outputDevice in DeviceService.GetAllOutputDevices())
            {
                AddDevice(outputDevice);
            }

            return default;
        }

        public void PlayInput(InputDevice inputDevice, byte[] buffer, int bytesRecorded)
        {
            lock (Players)
            {
                foreach (var player in Players)
                {
                    if (!player.Device.InputsMode.IsFulfilled(KeyboardHookService.GetPressedKeys()))
                    {
                        continue;
                    }

                    player.PlayInput(inputDevice, buffer, bytesRecorded);
                }
            }
        }
        public async Task PlaySoundAsync(Guid soundId)
        {
            var audioStream = AudioLoaderService.GetOrLoadAudioAsync(soundId);

            lock (Players)
            {
                foreach (var player in Players)
                {
                    player.PlaySound(audioStream);
                }
            }
        }
        public void StopAllSounds()
        {
            lock (Players)
            {
                foreach (var player in Players)
                {
                    player.Clear();
                }
            }
        }

        public void AddDevice(OutputDevice outputDevice)
        {
            var player = new AudioPlayer(outputDevice, AudioOptions.SampleRate, AudioOptions.ChannelCount, AudioOptions.OutputDelay, KeyboardHookService);
            lock (Players)
            {
                Players.Add(player);
            }
            player.Start();
        }
        public void RemoveDevice(OutputDevice outputDevice)
        {
            lock (Players)
            {
                var player = Players.Where(x => x.Device == outputDevice).FirstOrDefault();
                Players.Remove(player);
                player.Dispose();
            }
        }

        public void Dispose()
        {
            foreach (var player in Players)
            {
                player.Dispose();
            }
        }
    }
}
