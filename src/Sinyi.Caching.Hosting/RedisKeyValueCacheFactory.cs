using Autofac;
using MDP.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sinyi.Caching.Redis;
using System;
using System.Collections.Generic;

namespace Sinyi.Caching.Hosting
{
    public class RedisKeyValueCacheFactory : ServiceFactory<RedisKeyValueCache, RedisKeyValueCache, RedisKeyValueCacheFactory.Setting>
    {
        // Constructor
        public RedisKeyValueCacheFactory()
        {
            // Default
            this.ServiceSingleton = true;
        }


        // Methods
        protected override RedisKeyValueCache CreateService(IComponentContext componentContext, Setting setting)
        {
            #region Contracts

            if (componentContext == null) throw new ArgumentException(nameof(componentContext));
            if (setting == null) throw new ArgumentException(nameof(setting));

            #endregion

            // Create
            return new RedisKeyValueCache
            (
                setting.RedisConfig,
                componentContext.Resolve<IHostEnvironment>().EnvironmentName,
                componentContext.Resolve<ILogger<RedisKeyValueCache>>()
            );
        }


        // Class
        public class Setting
        {
            // Properties
            public RedisConfig RedisConfig { get; set; }
        }
    }
}