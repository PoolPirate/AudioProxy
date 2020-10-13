using System;

namespace AudioProxy.Models
{
    public sealed class ProfileSound
    {
        public Guid SoundId { get; set; }
        public Keys[] Keys { get; set; } = Array.Empty<Keys>();

        public ProfileSound(Guid soundId, Keys[] keys)
        {
            SoundId = soundId;
            Keys = keys;
        }
        public ProfileSound()
        {
        }
    }
}
