using System.Collections.Generic;

namespace AudioProxy.Extensions
{
    public static partial class Extensions
    {
        public static TValue AddOrSet<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.TryAdd(key, value))
            {
                dictionary[key] = value;
            }

            return value;
        }
    }
}
