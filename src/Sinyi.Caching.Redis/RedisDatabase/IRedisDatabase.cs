using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinyi.Caching.Redis
{
    public partial interface IRedisDatabase
    {
        // Properties
        IDatabase Database { get; }

        IServer RedisServer { get; }


        // Methods
        TItem Set<TItem>(string key, TItem value, TimeSpan absoluteExpirationRelativeToNow, bool compression = false);

        Task<TItem> SetAsync<TItem>(string key, TItem value, TimeSpan absoluteExpirationRelativeToNow, bool compression = false);

        bool Remove(string key);

        Task<bool> RemoveAsync(string key);

        bool Remove(IEnumerable<string> keyList);

        Task<bool> RemoveAsync(IEnumerable<string> keyList);

        bool RemoveAll(string pattern);

        Task<bool> RemoveAllAsync(string key);

        TItem Get<TItem>(string key, bool decompression = false);

        Task<TItem> GetAsync<TItem>(string key, bool decompression = false);

        bool TryGetValue<TItem>(string key, out TItem value, bool decompression = false);

        TItem GetOrCreate<TItem>(string key, Func<TItem> factory, TimeSpan absoluteExpirationRelativeToNow, bool decompressionOrcompression = false);

        Task<TItem> GetOrCreateAsync<TItem>(string key, Func<Task<TItem>> factory, TimeSpan absoluteExpirationRelativeToNow, bool decompressionOrcompression = false);

        bool Contains(string key);

        Task<bool> ContainsAsync(string key);
    }
}
