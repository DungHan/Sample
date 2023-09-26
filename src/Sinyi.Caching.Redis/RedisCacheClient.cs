using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;

namespace Sinyi.Caching.Redis
{
    public class RedisCacheClient : RedisCache
    {
        // Fields
        private readonly RedisConfig _config = null;

        private readonly ILogger _logger = null;

        private readonly IMemoryCache _memorycache = null;

        private readonly string _environmentName = null;

        private readonly RedisCacheConnectionManager _redisCacheConnectionManager = null;


        // Constructor
        public RedisCacheClient(RedisConfig config, 
                                ILogger logger,
                                IMemoryCache memorycache,
                                string environmentName)
        {
            #region Contracts

            if (config == null) throw new ArgumentException(nameof(config));
            if (logger == null) throw new ArgumentException(nameof(logger));
            if (memorycache == null) throw new ArgumentException(nameof(memorycache));
            if (string.IsNullOrEmpty(environmentName) == true) throw new ArgumentException(nameof(environmentName));

            #endregion

            // Default
            _config = config;
            _environmentName = environmentName;
            _logger = logger;
            _memorycache = memorycache;
            _redisCacheConnectionManager = new RedisCacheConnectionManager(config);
        }


        // Properties
        public IRedisDatabase Db0 => GetDatabase(0);

        public IRedisDatabase Db1 => GetDatabase(1);

        public IRedisDatabase Db2 => GetDatabase(2);

        public IRedisDatabase Db3 => GetDatabase(3);

        public IRedisDatabase Db4 => GetDatabase(4);

        public IRedisDatabase Db5 => GetDatabase(5);

        public IRedisDatabase Db6 => GetDatabase(6);

        public IRedisDatabase Db7 => GetDatabase(7);

        public IRedisDatabase Db8 => GetDatabase(8);

        public IRedisDatabase Db9 => GetDatabase(9);

        public IRedisDatabase Db10 => GetDatabase(10);

        public IRedisDatabase Db11 => GetDatabase(11);

        public IRedisDatabase Db12 => GetDatabase(12);

        public IRedisDatabase Db13 => GetDatabase(13);

        public IRedisDatabase Db14 => GetDatabase(14);

        public IRedisDatabase Db15 => GetDatabase(15);

        public IRedisDatabase Db16 => GetDatabase(16);


        // Methods
        public IRedisDatabase GetDatabase(int dbNumber, string keyPrefix = null)
        {
            // Requirement
            if (string.IsNullOrEmpty(keyPrefix))
                keyPrefix = _config.KeyPrefix;

            // Return
            return new RedisDatabase(_redisCacheConnectionManager,
                                     dbNumber,
                                     keyPrefix,
                                     _logger,
                                     _memorycache);
        }

        public IRedisDatabase GetDbFromConfiguration()
        {
            // Return
            return this.GetDatabase(_config.DbIndex, _config.KeyPrefix);
        }
    }
}
