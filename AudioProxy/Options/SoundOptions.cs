using System.Collections;
using System.Collections.Generic;
using AudioProxy.Models;
using Common.Configuration;

namespace AudioProxy.Options
{
    [SectionName("Sounds")]
    public sealed class SoundOptions : Option, IList<Sound>
    {
        private List<Sound> Sounds { get; set; } = new List<Sound>();

        public Sound this[int index] { get => ((IList<Sound>) Sounds)[index]; set => ((IList<Sound>) Sounds)[index] = value; }
        public int Count => ((ICollection<Sound>) Sounds).Count;
        public bool IsReadOnly => ((ICollection<Sound>) Sounds).IsReadOnly;
        public void Add(Sound item) => ((ICollection<Sound>) Sounds).Add(item);
        public void Clear() => ((ICollection<Sound>) Sounds).Clear();
        public bool Contains(Sound item) => ((ICollection<Sound>) Sounds).Contains(item);
        public void CopyTo(Sound[] array, int arrayIndex) => ((ICollection<Sound>) Sounds).CopyTo(array, arrayIndex);
        public IEnumerator<Sound> GetEnumerator() => Sounds.GetEnumerator();
        public int IndexOf(Sound item) => ((IList<Sound>) Sounds).IndexOf(item);
        public void Insert(int index, Sound item) => ((IList<Sound>) Sounds).Insert(index, item);
        public bool Remove(Sound item) => ((ICollection<Sound>) Sounds).Remove(item);
        public void RemoveAt(int index) => ((IList<Sound>) Sounds).RemoveAt(index);
        IEnumerator IEnumerable.GetEnumerator() => Sounds.GetEnumerator();
    }
}
