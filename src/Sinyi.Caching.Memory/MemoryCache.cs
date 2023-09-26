using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Sinyi.Caching.Memory
{
    public interface MemoryCache
    {
        // Methods
        TItem Set<TItem>(string key, TItem value, TimeSpan absoluteExpirationRelativeToNow, bool compression = false);

        bool Remove(string key);

        bool Remove(IEnumerable<string> keyList);

        bool RemoveAll(string pattern);

        TItem Get<TItem>(string key, bool decompression = false);

        bool TryGetValue<TItem>(string key, out TItem value, bool decompression = false);

        TItem GetOrCreate<TItem>(string key, Func<TItem> factory, TimeSpan absoluteExpirationRelativeToNow, bool decompressionOrcompression = false);

        bool Contains(string key);
    }
}