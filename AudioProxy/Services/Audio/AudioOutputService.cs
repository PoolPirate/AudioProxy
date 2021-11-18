using AudioProxy.Audio;
using AudioProxy.Models;
using AudioProxy.Options;
using Common.Services;
using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;

namespace AudioProxy.Services
{
    public sealed class AudioOutputService : Service, IAsyncDisposable
    {
        [Inject] private readonly AudioLoaderService AudioLoaderService;
        [Inject] private readonly KeyboardHookService KeyboardHookService;
        [Inject] private readonly DeviceService DeviceService;
        [Inject] private readonly AudioOptions AudioOptions;

        public WaveFormat OutputWaveFormat { get; private set; }
        private readonly List<AudioPlayer> Players;

        public AudioOutputService()
        {
            Players = new List<AudioPlayer>();
        }

        protected override ValueTask InitializeAsync()
        {
            OutputWaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(AudioOptions.SampleRate, AudioOptions.ChannelCount);

            foreach (var outputDevice in DeviceService.GetAllOutputDevices())
            {
                AddDevice(outputDevice);
            }

            return ValueTask.CompletedTask;
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
            var audioStream = await AudioLoaderService.GetOrLoadAudioAsync(soundId);

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
                    player.ClearAsync();
                }
            }
        }

        public void AddDevice(OutputDevice outputDevice)
        {
            var player = new AudioPlayer(outputDevice, OutputWaveFormat, AudioOptions.OutputDelay, AudioOptions.ResamplingQuality, KeyboardHookService);
            lock (Players)
            {
                Players.Add(player);
            }
            player.Start();
        }
        public async Task RemoveDeviceAsync(OutputDevice outputDevice)
        {
            AudioPlayer removedPlayer = null;

            lock (Players)
            {
                removedPlayer = Players.Where(x => x.Device == outputDevice).FirstOrDefault();
                Players.Remove(removedPlayer);
            }

            if (removedPlayer is null)
            {
                return;
            }

            await removedPlayer.DisposeAsync();
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var player in Players)
            {
                await player.DisposeAsync();
            }
        }
    }
}
