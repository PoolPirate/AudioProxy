using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AudioProxy.Models;
using Common.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AudioProxy.Options
{
    [SectionName("Profiles")]
    public sealed class ProfileOptions : Option, IList<Profile>
    {
        private List<Profile> Profiles { get; set; } = new List<Profile>();

        public Profile this[int index] { get => ((IList<Profile>) Profiles)[index]; set => ((IList<Profile>) Profiles)[index] = value; }
        public int Count => ((ICollection<Profile>) Profiles).Count;
        public bool IsReadOnly => ((ICollection<Profile>) Profiles).IsReadOnly;
        public void Add(Profile item) => ((ICollection<Profile>) Profiles).Add(item);
        public void Clear() => ((ICollection<Profile>) Profiles).Clear();
        public bool Contains(Profile item) => ((ICollection<Profile>) Profiles).Contains(item);
        public void CopyTo(Profile[] array, int arrayIndex) => ((ICollection<Profile>) Profiles).CopyTo(array, arrayIndex);
        public IEnumerator<Profile> GetEnumerator() => Profiles.GetEnumerator();
        public int IndexOf(Profile item) => ((IList<Profile>) Profiles).IndexOf(item);
        public void Insert(int index, Profile item) => ((IList<Profile>) Profiles).Insert(index, item);
        public bool Remove(Profile item) => ((ICollection<Profile>) Profiles).Remove(item);
        public void RemoveAt(int index) => ((IList<Profile>) Profiles).RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => Profiles.GetEnumerator();

        protected override ValueTask<ValidationResult> ValidateAsync(IServiceProvider provider)
        {
            var soundOptions = provider.GetRequiredService<SoundOptions>();
            var logger = provider.GetRequiredService<ILogger<ProfileOptions>>();

            foreach(var profile in Profiles)
            {
                foreach(var profileSound in profile.Sounds.ToArray())
                {
                    if (soundOptions.Any(x => x.Id == profileSound.SoundId))
                    {
                        continue;
                    }

                    profile.Sounds.Remove(profileSound);
                    logger.LogWarning("Removed a Profilesound that referenced a invalid/deleted Sound!");
                }
            }

            return new ValueTask<ValidationResult>(ValidationResult.Success);
        }
    }
}
