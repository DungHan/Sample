using StackExchange.Redis;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sinyi.Caching.Redis
{
    public partial class RedisDatabase : IRedisDatabase
    {
        public Task<long> ListAddToLeftAsync<T>(string key, T item, When when = When.Always, CommandFlags flags = CommandFlags.None)
            where T : class
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException("key cannot be empty.", nameof(key));
            if (item == null) throw new ArgumentNullException(nameof(item), "item cannot be null.");

            #endregion

            // Serialize
            var serializedItem = JsonExtension.Serialize(item);

            // Push
            return Database.ListLeftPushAsync(key, serializedItem, when, flags);
        }

        public long ListAddToLeft<T>(string key, T item, When when = When.Always, CommandFlags flags = CommandFlags.None)
          where T : class
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException("key cannot be empty.", nameof(key));
            if (item == null) throw new ArgumentNullException(nameof(item), "item cannot be null.");

            #endregion

            // Serialize
            var serializedItem = JsonExtension.Serialize(item);

            // Push
            return Database.ListLeftPush(key, serializedItem, when, flags);
        }

        public Task<long> ListAddToLeftAsync<T>(string key, T[] items, CommandFlags flags = CommandFlags.None)
            where T : class
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException("key cannot be empty.", nameof(key));
            if (items == null) throw new ArgumentNullException(nameof(items), "items cannot be null.");

            #endregion

            // Serialize
            var serializedItems = items.Select(x => (RedisValue)JsonExtension.Serialize(x)).ToArray();

            // Push
            return Database.ListLeftPushAsync(key, serializedItems, flags);
        }

        public long ListAddToLeft<T>(string key, T[] items, CommandFlags flags = CommandFlags.None)
           where T : class
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException("key cannot be empty.", nameof(key));
            if (items == null) throw new ArgumentNullException(nameof(items), "items cannot be null.");

            #endregion

            // Serialize
            var serializedItems = items.Select(x => (RedisValue)JsonExtension.Serialize(x)).ToArray();

            // Push
            return Database.ListLeftPush(key, serializedItems, flags);
        }

        public async Task<T> ListGetFromRightAsync<T>(string key, CommandFlags flags = CommandFlags.None)
            where T : class
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException("key cannot be empty.", nameof(key));

            #endregion

            // Pop
            var item = await Database.ListRightPopAsync(key, flags).ConfigureAwait(false);
            if (item == RedisValue.Null) return null;

            // Return
            return item == RedisValue.Null ? null : JsonExtension.Deserialize<T>(item);
        }

        public T ListGetFromRight<T>(string key, CommandFlags flags = CommandFlags.None)
            where T : class
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException("key cannot be empty.", nameof(key));

            #endregion

            // Pop
            var item = Database.ListRightPop(key, flags);
            if (item == RedisValue.Null) return null;

            // Return
            return item == RedisValue.Null ? null : JsonExtension.Deserialize<T>(item);
        }
    }
}
