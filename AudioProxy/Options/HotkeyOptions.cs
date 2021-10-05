using AudioProxy.Models;
using Common.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace AudioProxy.Options
{
    [SectionName("Hotkeys")]
    public sealed class HotkeyOptions : Option, IDictionary<Hotkey, Keys[]>
    {
        public Dictionary<Hotkey, Keys[]> Hotkeys { get; set; } = new Dictionary<Hotkey, Keys[]>();

        public ICollection<Hotkey> Keys => ((IDictionary<Hotkey, Keys[]>)Hotkeys).Keys;
        public ICollection<Keys[]> Values => ((IDictionary<Hotkey, Keys[]>)Hotkeys).Values;
        public int Count => ((ICollection<KeyValuePair<Hotkey, Keys[]>>)Hotkeys).Count;
        public bool IsReadOnly => ((ICollection<KeyValuePair<Hotkey, Keys[]>>)Hotkeys).IsReadOnly;
        public Keys[] this[Hotkey key] { get => ((IDictionary<Hotkey, Keys[]>)Hotkeys)[key]; set => ((IDictionary<Hotkey, Keys[]>)Hotkeys)[key] = value; }
        public void Add(Hotkey key, Keys[] value) => ((IDictionary<Hotkey, Keys[]>)Hotkeys).Add(key, value);
        public bool ContainsKey(Hotkey key) => ((IDictionary<Hotkey, Keys[]>)Hotkeys).ContainsKey(key);
        public bool Remove(Hotkey key) => ((IDictionary<Hotkey, Keys[]>)Hotkeys).Remove(key);
        public bool TryGetValue(Hotkey key, [MaybeNullWhen(false)] out Keys[] value) => ((IDictionary<Hotkey, Keys[]>)Hotkeys).TryGetValue(key, out value);
        public void Add(KeyValuePair<Hotkey, Keys[]> item) => ((ICollection<KeyValuePair<Hotkey, Keys[]>>)Hotkeys).Add(item);
        public void Clear() => ((ICollection<KeyValuePair<Hotkey, Keys[]>>)Hotkeys).Clear();
        public bool Contains(KeyValuePair<Hotkey, Keys[]> item) => ((ICollection<KeyValuePair<Hotkey, Keys[]>>)Hotkeys).Contains(item);
        public void CopyTo(KeyValuePair<Hotkey, Keys[]>[] array, int arrayIndex) => ((ICollection<KeyValuePair<Hotkey, Keys[]>>)Hotkeys).CopyTo(array, arrayIndex);
        public bool Remove(KeyValuePair<Hotkey, Keys[]> item) => ((ICollection<KeyValuePair<Hotkey, Keys[]>>)Hotkeys).Remove(item);
        public IEnumerator<KeyValuePair<Hotkey, Keys[]>> GetEnumerator() => ((IEnumerable<KeyValuePair<Hotkey, Keys[]>>)Hotkeys).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)Hotkeys).GetEnumerator();
    }
}
