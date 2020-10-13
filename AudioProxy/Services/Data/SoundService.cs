using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioProxy.Models;
using AudioProxy.Options;
using Common.Services;

namespace AudioProxy.Services
{
    public sealed class SoundService : Service
    {
        [Inject] private readonly SoundOptions SoundOptions;
        [Inject] private readonly ProfileService ProfileService;

        public event Func<Task> OnSoundsChanged;

        public IEnumerable<Sound> GetAllSounds()
        {
            lock (SoundOptions)
            {
                return SoundOptions.ToArray();
            }
        }

        public Sound GetSoundById(Guid soundId)
        {
            lock (SoundOptions)
            {
                return SoundOptions.FirstOrDefault(x => x.Id == soundId);
            }
        }

        public void CreateSound(string soundName, SoundSource soundSource, string path)
        {
            var sound = new Sound(soundName, soundSource, path);
            lock (SoundOptions)
            {
                SoundOptions.Add(sound);
            }
            _ = OnSoundsChanged?.Invoke();
        }

        public void UpdateSound(Sound sound, Action<Sound> changeDelegate)
        {
            lock (SoundOptions)
            {
                changeDelegate.Invoke(sound);
            }
            _ = OnSoundsChanged?.Invoke();
        }

        public void DeleteSound(Sound sound)
        {
            lock (SoundOptions)
            {
                SoundOptions.Remove(sound);
            }
            ProfileService.DeleteSoundReferences(sound);
            _ = OnSoundsChanged?.Invoke();
        }
    }
}
