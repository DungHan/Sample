using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using StackExchange.Redis.KeyspaceIsolation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sinyi.Caching.Redis
{
    public partial class RedisDatabase : IRedisDatabase
    {
        // Fields
        private readonly RedisCacheConnectionManager _redisCacheConnectionManager = null;

        private readonly int _dbNumber = 0;

        private readonly string _keyPrefix = null;

        private readonly ILogger _logger = null;

        private readonly IMemoryCache _memorycache = null;

        private readonly Lazy<IDatabase> _lazyDatabase = null;

        private readonly Lazy<IServer> _lazyServer = null;


        // Consturctor
        public RedisDatabase(RedisCacheConnectionManager redisCacheConnectionManager,
                             int dbNumber,
                             string keyPrefix,
                             ILogger logger,
                             IMemoryCache memorycache)
        {
            #region Contracts

            if (redisCacheConnectionManager == null) throw new ArgumentException(nameof(redisCacheConnectionManager));
            if (string.IsNullOrEmpty(keyPrefix) == true) throw new ArgumentException(nameof(keyPrefix));
            if (logger == null) throw new ArgumentException(nameof(logger));
            if (memorycache == null) throw new ArgumentException(nameof(memorycache));

            #endregion

            // Default
            _redisCacheConnectionManager = redisCacheConnectionManager;
            _dbNumber = dbNumber;
            _keyPrefix = keyPrefix;
            _logger = logger;
            _memorycache = memorycache;
            _lazyDatabase = new Lazy<IDatabase>(() => GetRedisDatabase());
            _lazyServer = new Lazy<IServer>(() => _redisCacheConnectionManager.GetConnection().GetServers().FirstOrDefault());
        }


        public IDatabase Database => _lazyDatabase.Value;

        public IServer RedisServer => _lazyServer.Value;


        // Methods
        public TItem Set<TItem>(string key, TItem value, TimeSpan absoluteExpirationRelativeToNow, bool compression = false)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException(nameof(key));

            #endregion

            // Convert
            var item = value.Serialize(compression);

            // Set
            this.Database.StringSet(key, item, absoluteExpirationRelativeToNow);

            // Return
            return value;
        }

        public async Task<TItem> SetAsync<TItem>(string key, TItem value, TimeSpan absoluteExpirationRelativeToNow, bool compression = false)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException(nameof(key));

            #endregion

            // Convert
            var item = value.Serialize(compression);

            // Set
            await this.Database.StringSetAsync(key, item, absoluteExpirationRelativeToNow);

            // Return
            return value;
        }

        public bool Remove(string key)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException(nameof(key));

            #endregion

            // Remove
            var result = this.Database.KeyDelete(key);

            // Log
            _logger.LogInformation($"Remove cache {result} by key = {key}");

            // Remove
            return result;
        }

        public async Task<bool> RemoveAsync(string key)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException(nameof(key));

            #endregion

            // Remove
            var result = await this.Database.KeyDeleteAsync(key);

            // Log
            _logger.LogInformation($"Remove cache {result} by key = {key}");

            // Remove
            return result;
        }

        public bool Remove(IEnumerable<string> keyList)
        {
            #region Contracts

            if (keyList == null) throw new ArgumentException(nameof(keyList));

            #endregion

            // Fields
            var redisKeys = this.ConvertToRedisKeys(keyList);

            // Remove
            return this.Database.KeyDelete(redisKeys) > 0;
        }

        public async Task<bool> RemoveAsync(IEnumerable<string> keyList)
        {
            #region Contracts

            if (keyList == null) throw new ArgumentException(nameof(keyList));

            #endregion

            // Fields
            var redisKeys = this.ConvertToRedisKeys(keyList);

            // Remove
            return await this.Database.KeyDeleteAsync(redisKeys) > 0;
        }

        public bool RemoveAll(string pattern)
        {
            #region Contracts

            if (string.IsNullOrEmpty(pattern) == true) throw new ArgumentException(nameof(pattern));

            #endregion

            // Remove
            var result = this.Database.KeyDelete(this.GetKeys(pattern).Result) > 0;

            // Log
            _logger.LogInformation($"Remove cache {result} by pattern = {pattern}");

            // Return
            return result;
        }

        public async Task<bool> RemoveAllAsync(string pattern)
        {
            #region Contracts

            if (string.IsNullOrEmpty(pattern) == true) throw new ArgumentException(nameof(pattern));

            #endregion

            // Fields
            var keys = await this.GetKeys(pattern);

            // Remove
            var result = await this.Database.KeyDeleteAsync(keys) > 0;

            // Log
            _logger.LogInformation($"Remove cache {result} by pattern = {pattern}");

            // Return
            return result;
        }

        public TItem Get<TItem>(string key, bool decompression = false)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException(nameof(key));

            #endregion

            // Get
            var item = this.Database.StringGetAsync(key).GetAwaiter().GetResult();
            if (item.HasValue == false) return default(TItem);

            // Convert
            var result = JsonExtension.Deserialize<TItem>(item, decompression);
            if (result == null) return default(TItem);

            // Return
            return result;
        }

        public async Task<TItem> GetAsync<TItem>(string key, bool decompression = false)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException(nameof(key));

            #endregion

            // Get
            var item = await this.Database.StringGetAsync(key);
            if (item.HasValue == false) return default(TItem);

            // Convert
            var result = JsonExtension.Deserialize<TItem>(item, decompression);
            if (result == null) return default(TItem);

            // Return
            return result;
        }

        public bool TryGetValue<TItem>(string key, out TItem value, bool decompression = false)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException(nameof(key));

            #endregion

            // Requirement
            value = default(TItem);
            if (this.Contains(key) == false) return false;

            // Get
            value = this.Get<TItem>(key, decompression);

            // Return
            return true;
        }

        public TItem GetOrCreate<TItem>(string key, Func<TItem> factory, TimeSpan absoluteExpirationRelativeToNow, bool decompressionOrcompression = false)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException(nameof(key));
            if (factory == null) throw new ArgumentException(nameof(factory));

            #endregion

            // Get
            var cacheResult = this.Get<TItem>(key, decompressionOrcompression);
            if (cacheResult != null) return cacheResult;

            // Create
            cacheResult = factory();
            if (cacheResult == null) return default(TItem);

            // Set to cache
            this.Set(key, cacheResult, absoluteExpirationRelativeToNow, decompressionOrcompression);    // Set 內會處理 prefix，所以這裡不處理

            // Return
            return cacheResult;
        }

        public async Task<TItem> GetOrCreateAsync<TItem>(string key, Func<Task<TItem>> factory, TimeSpan absoluteExpirationRelativeToNow, bool decompressionOrcompression = false)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException(nameof(key));
            if (factory == null) throw new ArgumentException(nameof(factory));

            #endregion

            // Get
            var cacheResult = await this.GetAsync<TItem>(key, decompressionOrcompression);
            if (cacheResult != null) return cacheResult;

            // Create
            cacheResult = await factory();
            if (cacheResult == null) return default(TItem);

            // Set to cache
            await this.SetAsync(key, cacheResult, absoluteExpirationRelativeToNow, decompressionOrcompression);    // Set 內會處理 prefix，所以這裡不處理

            // Return
            return cacheResult;
        }

        public bool Contains(string key)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException(nameof(key));

            #endregion

            // Return
            return this.Database.KeyExistsAsync(key).GetAwaiter().GetResult();
        }

        public async Task<bool> ContainsAsync(string key)
        {
            #region Contracts

            if (string.IsNullOrEmpty(key) == true) throw new ArgumentException(nameof(key));

            #endregion

            // Return
            return await this.Database.KeyExistsAsync(key);
        }

        private async Task<RedisKey[]> GetKeys(string pattern)
        {
            // Fields
            var result = new List<RedisKey>();

            // Get
            var redisKeys = RedisServer.KeysAsync(pattern: $"{_keyPrefix}:{pattern}*", pageSize: 10000);

            // Convert
            await foreach (var redisKey in redisKeys)
            {
                // Remove prefix
                var noprefixKey = redisKey.ToString().Remove(0, _keyPrefix.Length + 1);

                result.Add(new RedisKey(noprefixKey));
            }

            // Return
            return result.ToArray();
        }

        private RedisKey[] ConvertToRedisKeys(IEnumerable<string> keyList)
        {
            // Return
            return keyList.Select(key => new RedisKey(key)).ToArray();
        }

        private IDatabase GetRedisDatabase()
        {
            // Get
            var db = _redisCacheConnectionManager.GetConnection().GetDatabase(_dbNumber);
            if (string.IsNullOrEmpty(_keyPrefix) != true) db = db.WithKeyPrefix($"{_keyPrefix}:");

            // Return
            return db;
        }
    }
}
