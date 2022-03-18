using System.Collections.Generic;

namespace DesperateDevs.Extensions
{
    public static class DictionaryExtension
    {
        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(
            this Dictionary<TKey, TValue> dictionary,
            params Dictionary<TKey, TValue>[] dictionaries
        )
        {
            foreach (var dict in dictionaries)
            foreach (var kvp in dict)
                dictionary[kvp.Key] = kvp.Value;

            return dictionary;
        }
    }
}
