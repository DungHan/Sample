using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Sinyi.Caching.Redis
{
    public interface RedisCache
    {
        // Properties
        IRedisDatabase Db0 { get; }

        IRedisDatabase Db1 { get; }

        IRedisDatabase Db2 { get; }

        IRedisDatabase Db3 { get; }

        IRedisDatabase Db4 { get; }

        IRedisDatabase Db5 { get; }

        IRedisDatabase Db6 { get; }

        IRedisDatabase Db7 { get; }

        IRedisDatabase Db8 { get; }

        IRedisDatabase Db9 { get; }

        IRedisDatabase Db10 { get; }

        IRedisDatabase Db11 { get; }

        IRedisDatabase Db12 { get; }

        IRedisDatabase Db13 { get; }

        IRedisDatabase Db14 { get; }

        IRedisDatabase Db15 { get; }

        IRedisDatabase Db16 { get; }

        IRedisDatabase GetDatabase(int dbNumber, string keyPrefix = null);

        IRedisDatabase GetDbFromConfiguration();

    }
}