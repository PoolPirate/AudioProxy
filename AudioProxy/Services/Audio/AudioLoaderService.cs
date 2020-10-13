using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using AudioProxy.Audio;
using AudioProxy.Models;
using Common.Services;
using Microsoft.Extensions.Logging;

namespace AudioProxy.Services
{
    public sealed class AudioLoaderService : Service
    {
        [Inject] private readonly SoundService SoundService;

        public IAudioStream GetOrLoadAudioAsync(Guid soundId)
        {
            var sound = SoundService.GetSoundById(soundId);

            if (sound == null)
            {
                Logger.LogError($"Tried to play a sound that does not seem to exist anymore! Id: {soundId}");
                return null;
            }

            return LoadAudioStream(sound);
        }

        public IAudioStream LoadAudioStream(Sound sound)
        {
            switch (sound.Source)
            {
                case SoundSource.File:
                    return new FileAudioStream(sound.Path);
                default:
                    Logger.LogError($"Invalid SoundSource! Id: {sound.Id}");
                    return null;
            }
        }

        public bool IsValidAudioFile(string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }
            return true;
        }
    }
}
