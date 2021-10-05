using AudioProxy.Audio;
using AudioProxy.Models;
using Common.Services;
using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace AudioProxy.Services
{
    public sealed class AudioLoaderService : Service
    {
        [Inject] private readonly SoundService SoundService;

        public IAudioStream GetOrLoadAudioAsync(Guid soundId)
        {
            var sound = SoundService.GetSoundById(soundId);

            if (sound is null)
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
                    return new FileAudioStream(sound.Path, 2);
                //case SoundSource.Youtube:
                //    return new RawAudioStream(sound.Path);
                default:
                    Logger.LogError($"Invalid SoundSource! Id: {sound.Id}");
                    return null;
            }
        }

        public bool IsValidAudioFile(string path)
            => File.Exists(path);
    }
}
