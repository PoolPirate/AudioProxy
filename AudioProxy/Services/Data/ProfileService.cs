using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AudioProxy.Models;
using AudioProxy.Options;
using Common.Services;

namespace AudioProxy.Services
{
    public sealed class ProfileService : Service
    {
        [Inject] private readonly ProfileOptions ProfileOptions;

        public Profile CurrentProfile => ProfileOptions.ElementAtOrDefault(CurrentProfileIndex);
        private int CurrentProfileIndex;

        public event Func<Task> OnSwitchedProfile;
        public event Func<Task> OnProfilesChanged;

        public IEnumerable<Profile> GetAllProfiles()
        {
            lock (ProfileOptions)
            {
                return ProfileOptions.ToArray();
            }
        }

        public void CreateProfile(string profileName)
        {
            lock (ProfileOptions)
            {
                var profile = new Profile(profileName);
                ProfileOptions.Add(profile);
            }
            _ = OnProfilesChanged?.Invoke();
        }

        public void UpdateProfile(Profile profile, Action<Profile> changeDelegate)
        {
            lock (ProfileOptions)
            {
                changeDelegate.Invoke(profile);
            }
            _ = OnProfilesChanged?.Invoke();
        }

        public void DeleteProfile(Profile profile)
        {
            lock (ProfileOptions)
            {
                int index = ProfileOptions.IndexOf(profile);
                ProfileOptions.RemoveAt(index);

                if (index == -1)
                {
                    return;
                }

                if (index < CurrentProfileIndex)
                {
                    SelectLastProfile();
                }
                else if (index == CurrentProfileIndex)
                {
                    CurrentProfileIndex = -1;
                }


            }
            _ = OnProfilesChanged?.Invoke();
        }
        public void DeleteSoundReferences(Sound sound)
        {
            lock (ProfileOptions)
            {
                foreach (var profile in ProfileOptions)
                {
                    profile.Sounds.RemoveAll(x => x.SoundId == sound.Id);
                    _ = OnProfilesChanged?.Invoke();
                }
            }
        }

        public void SelectProfile(Guid profileId)
        {
            lock (ProfileOptions)
            {
                CurrentProfileIndex = ProfileOptions.ToList()
                    .FindIndex(x => x.Id == profileId);
            }
            _ = OnSwitchedProfile?.Invoke();
        }
        public void SelectNextProfile()
        {
            Interlocked.Increment(ref CurrentProfileIndex);

            if (CurrentProfileIndex > ProfileOptions.Count - 1)
            {
                Interlocked.Exchange(ref CurrentProfileIndex, ProfileOptions.Count - 1);
                return;
            }
            _ = OnSwitchedProfile?.Invoke();
        }
        public void SelectLastProfile()
        {
            Interlocked.Decrement(ref CurrentProfileIndex);

            if (CurrentProfileIndex < 0)
            {
                Interlocked.Exchange(ref CurrentProfileIndex, 0);
                return;
            }
            _ = OnSwitchedProfile?.Invoke();
        }

        public bool TryGetPressedProfileSounds(Keys[] pressedKeys, out ProfileSound[] profileSounds)
        {
            var pressedProfileSounds = new List<ProfileSound>();

            lock (ProfileOptions)
            {
                foreach(var profile in ProfileOptions)
                {
                    foreach(var profileSound in profile.Sounds)
                    {
                        if (pressedKeys.Length != profileSound.Keys.Length ||
                            !pressedKeys.All(x => profileSound.Keys.Contains(x)))
                        {
                            continue;
                        }

                        pressedProfileSounds.Add(profileSound);
                    }
                }
            }

            profileSounds = pressedProfileSounds.ToArray();
            return pressedProfileSounds.Count != 0;
        }
    }
}
