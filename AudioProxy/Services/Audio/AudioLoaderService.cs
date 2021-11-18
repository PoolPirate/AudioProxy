using AudioProxy.Audio;
using AudioProxy.Models;
using Common.Services;
using Microsoft.Extensions.Logging;
using NAudio.Wave;
using System;
using System.IO;
using System.Threading.Tasks;
using YoutubeExplode.Videos;

namespace AudioProxy.Services
{
    public sealed class AudioLoaderService : Service
    {
        [Inject] 
        private readonly SoundService SoundService;
        [Inject]
        private readonly YoutubeAudioService YoutubeAudioService;

        public async Task<MultiReaderWaveProvider> GetOrLoadAudioAsync(Guid soundId)
        {
            var sound = SoundService.GetSoundById(soundId);

            if (sound is null)
            {
                Logger.LogError($"Tried to play a sound that does not seem to exist anymore! Id: {soundId}");
                throw new Exception("Sound not found!");
            }

            return await LoadMultiReaderAsync(sound);
        }

        public async Task<MultiReaderWaveProvider> LoadMultiReaderAsync(Sound sound)
        {
            switch (sound.Source)
            {
                case SoundSource.File:
                    var fileReader = new AudioFileReader(sound.Path);
                    return new MultiReaderWaveProvider(fileReader, fileReader.WaveFormat, 1);
                case SoundSource.Youtube:
                    var waveStream = await YoutubeAudioService.GetAudioStreamAsync(sound.Path);
                    return new MultiReaderWaveProvider(waveStream, waveStream.WaveFormat, 1);
                default:
                    Logger.LogError($"Invalid SoundSource! Id: {sound.Id}");
                    return null;
            }
        }

        public async Task<bool> IsValidSoundPath(SoundSource source, string path)
        {
            return source switch
            {
                SoundSource.File => File.Exists(path),
                SoundSource.Youtube => await YoutubeAudioService.IsValidVideoUrl(path),
                _ => false,
            };
        }
    }
}
