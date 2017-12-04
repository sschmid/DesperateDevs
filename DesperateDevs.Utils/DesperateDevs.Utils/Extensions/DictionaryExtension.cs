using System.Collections.Generic;
using System.Linq;

namespace DesperateDevs.Utils {

    public static class DictionaryExtension {

        public static Dictionary<TKey, TValue> Merge<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
            params Dictionary<TKey, TValue>[] dictionaries) {
            return dictionaries.Aggregate(dictionary, (current, dict) => current.Union(dict).ToDictionary(kv => kv.Key, kv => kv.Value));
        }
    }
}
