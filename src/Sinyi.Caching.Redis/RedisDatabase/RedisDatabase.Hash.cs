using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sinyi.Caching.Redis
{
    public partial class RedisDatabase : IRedisDatabase
    {
        public Task<bool> HashDeleteAsync(string key, string hashField, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException("key cannot be empty.", nameof(key));
            if (string.IsNullOrEmpty(hashField) == true) throw new ArgumentException("hashField cannot be empty.", nameof(hashField));

            #endregion

            // Delete
            return Database.HashDeleteAsync(key, hashField, commandFlags);
        }

        public async Task<Dictionary<string, T>> HashGetAllAsync<T>(string key)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException("key cannot be empty.", nameof(key));

            #endregion

            // Get
            var result = await Database.HashGetAllAsync(key);
            if (result == null) return null;

            // Deserialize
            var dic = result.ToDictionary(r => r.Name.ToString(), r => JsonExtension.Deserialize<T>(r.Value));

            // Return
            return dic;
        }

        public Dictionary<string, T> HashGetAll<T>(string key)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException("key cannot be empty.", nameof(key));

            #endregion

            // Get
            var result = Database.HashGetAll(key);
            if (result == null) return null;

            // Deserialize
            var dic = result.ToDictionary(r => r.Name.ToString(), r => JsonExtension.Deserialize<T>(r.Value));

            // Return
            return dic;
        }

        public bool HashDelete(string key, string hashField, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException("key cannot be empty.", nameof(key));
            if (string.IsNullOrEmpty(hashField) == true) throw new ArgumentException("hashField cannot be empty.", nameof(hashField));

            #endregion

            // Delete
            return Database.HashDelete(key, hashField, commandFlags);
        }

        public Task<long> HashDeleteAsync(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));

            #endregion

            // Delete
            return Database.HashDeleteAsync(hashKey, keys.Select(x => (RedisValue)x).ToArray(), commandFlags);
        }

        public long HashDelete(string hashKey, IEnumerable<string> keys, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));

            #endregion

            // Delete
            return Database.HashDelete(hashKey, keys.Select(x => (RedisValue)x).ToArray(), commandFlags);
        }

        public Task<bool> HashExistsAsync(string key, string hashField, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException("key cannot be empty.", nameof(key));
            if (string.IsNullOrEmpty(hashField) == true) throw new ArgumentException("hashField cannot be empty.", nameof(hashField));

            #endregion

            // Exists
            return Database.HashExistsAsync(key, hashField, commandFlags);
        }

        public bool HashExists(string key, string hashField, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException("key cannot be empty.", nameof(key));
            if (string.IsNullOrEmpty(hashField) == true) throw new ArgumentException("hashField cannot be empty.", nameof(hashField));

            #endregion

            // Exists
            return Database.HashExists(key, hashField, commandFlags);
        }

        public async Task<T> HashGetAsync<T>(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));
            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException("key cannot be empty.", nameof(key));
           
            #endregion

            // Get
            var redisValue = await Database.HashGetAsync(hashKey, key, commandFlags).ConfigureAwait(false);

            // Deserialize
            return redisValue.HasValue ? JsonExtension.Deserialize<T>(redisValue) : default;
        }

        public T HashGet<T>(string hashKey, string key, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));
            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException("key cannot be empty.", nameof(key));

            #endregion

            // Get
            var redisValue = Database.HashGet(hashKey, key, commandFlags);

            // Return
            return redisValue.HasValue ? JsonExtension.Deserialize<T>(redisValue) : default;
        }

        public async Task<Dictionary<string, T>> HashGetAsync<T>(string hashKey, IList<string> keys, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));
            if (keys == null) throw new ArgumentException("keys cannot be empty.", nameof(keys));

            #endregion

            // Get
            var tasks = keys.Select(key => HashGetAsync<T>(hashKey, key, commandFlags));
            await Task.WhenAll(tasks).ConfigureAwait(false);

            // Convert
            var index = 0;
            var result = tasks.ToDictionary(task => keys[index++], task => task.Result);
               
            // Return
            return result;
        }

        public Dictionary<string, T> HashGet<T>(string hashKey, IList<string> keys, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));
            if (keys == null) throw new ArgumentException("keys cannot be empty.", nameof(keys));

            #endregion

            // Get
            var tasks = keys.Select(key => HashGetAsync<T>(hashKey, key, commandFlags));
            Task.WhenAll(tasks).ConfigureAwait(false).GetAwaiter();

            // Convert
            var index = 0;
            var result = tasks.ToDictionary(task => keys[index++], task => task.Result);

            // Return
            return result;
        }

        public async Task<Dictionary<string, T>> HashGetAllAsync<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));

            #endregion

            // Get
            var redisValues = await Database.HashGetAllAsync(hashKey, commandFlags).ConfigureAwait(false);
            var result = redisValues.ToDictionary(r => r.Name.ToString(), r => JsonExtension.Deserialize<T>(r.Value), StringComparer.Ordinal);

            // Return
            return result;
        }

        public Dictionary<string, T> HashGetAll<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));

            #endregion

            // Get
            var redisValues = Database.HashGetAll(hashKey, commandFlags);
            var result = redisValues.ToDictionary(r => r.Name.ToString(), r => JsonExtension.Deserialize<T>(r.Value), StringComparer.Ordinal);

            // Return
            return result;
        }

        public Task<long> HashIncerementByAsync(string hashKey, string key, long value, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));
            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException("key cannot be empty.", nameof(key));

            #endregion

            // Return
            return Database.HashIncrementAsync(hashKey, key, value, commandFlags);
        }

        public long HashIncerement(string hashKey, string key, long value, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));
            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException("key cannot be empty.", nameof(key));

            #endregion

            // Return
            return Database.HashIncrement(hashKey, key, value, commandFlags);
        }

        public Task<double> HashIncerementByAsync(string hashKey, string key, double value, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));
            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException("key cannot be empty.", nameof(key));

            #endregion

            // Return
            return Database.HashIncrementAsync(hashKey, key, value, commandFlags);
        }

        public double HashIncerement(string hashKey, string key, double value, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));
            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException("key cannot be empty.", nameof(key));

            #endregion

            // Return
            return Database.HashIncrement(hashKey, key, value, commandFlags);
        }

        public async Task<IEnumerable<string>> HashKeysAsync(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));

            #endregion

            // Keys
            var redisValues = await Database.HashKeysAsync(hashKey, commandFlags).ConfigureAwait(false);

            // Return
            return redisValues.Select(x => x.ToString());
        }

        public IEnumerable<string> HashKeys(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));

            #endregion

            // Keys
            var redisValues = Database.HashKeys(hashKey, commandFlags);

            // Return
            return redisValues.Select(x => x.ToString());
        }

        public Task<long> HashLengthAsync(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));

            #endregion

            // Return
            return Database.HashLengthAsync(hashKey, commandFlags);
        }

        public long HashLength(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));

            #endregion

            // Return
            return Database.HashLength(hashKey, commandFlags);
        }

        public Task<bool> HashSetAsync<T>(string hashKey, string key, T value, When when = When.Always, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));
            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException("key cannot be empty.", nameof(key));

            #endregion
            
            // Return
            return Database.HashSetAsync(hashKey, key, JsonExtension.Serialize(value), when, commandFlags);
        }

        public bool HashSet<T>(string hashKey, string key, T value, When when = When.Always, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));
            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException("key cannot be empty.", nameof(key));

            #endregion

            // Return
            return Database.HashSet(hashKey, key, JsonExtension.Serialize(value), when, commandFlags);
        }

        public Task HashSetAsync<T>(string hashKey, IDictionary<string, T> values, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));

            #endregion
            
            // Set
            var entries = values.Select(kv => new HashEntry(kv.Key, JsonExtension.Serialize(kv.Value)));
            return Database.HashSetAsync(hashKey, entries.ToArray(), commandFlags);
        }

        public void HashSet<T>(string hashKey, IDictionary<string, T> values, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));

            #endregion

            // Set
            var entries = values.Select(kv => new HashEntry(kv.Key, JsonExtension.Serialize(kv.Value)));
            Database.HashSet(hashKey, entries.ToArray(), commandFlags);
        }

        public async Task<IEnumerable<T>> HashValuesAsync<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));

            #endregion

            // Return
            return (await Database.HashValuesAsync(hashKey, commandFlags).ConfigureAwait(false)).Select(x => JsonExtension.Deserialize<T>(x));
        }

        public IEnumerable<T> HashValues<T>(string hashKey, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));

            #endregion

            // Return
            return (Database.HashValues(hashKey, commandFlags)).Select(x => JsonExtension.Deserialize<T>(x));
        }

        public Dictionary<string, T> HashScan<T>(string hashKey, string pattern, int pageSize = 10, CommandFlags commandFlags = CommandFlags.None)
        {
            #region Contracts

            if (string.IsNullOrEmpty(hashKey) == true) throw new ArgumentException("hashKey cannot be empty.", nameof(hashKey));
            if (string.IsNullOrEmpty(pattern) == true) throw new ArgumentException("pattern cannot be empty.", nameof(pattern));

            #endregion

            // Return
            return Database.HashScan(hashKey, pattern, pageSize, commandFlags).ToDictionary(x => x.Name.ToString(), x => JsonExtension.Deserialize<T>(x.Value), StringComparer.Ordinal);
        }
    }
}
