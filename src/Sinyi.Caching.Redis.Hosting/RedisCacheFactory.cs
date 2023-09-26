using Autofac;
using MDP.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;

namespace Sinyi.Caching.Redis.Hosting
{
    public class RedisCacheFactory : ServiceFactory<RedisCacheClient, RedisCacheClient, RedisCacheFactory.Setting>
    {
        // Constructor
        public RedisCacheFactory()
        {
            // Default
            this.ServiceSingleton = true;
        }


        // Methods
        protected override RedisCacheClient CreateService(IComponentContext componentContext, Setting setting)
        {
            #region Contracts

            if (componentContext == null) throw new ArgumentException(nameof(componentContext));
            if (setting == null) throw new ArgumentException(nameof(setting));

            #endregion

            // Fields
            var redisConfig = this.GetEnvironmentVariable(setting);

            // Create
            return new RedisCacheClient
            (
                redisConfig,
                componentContext.Resolve<ILogger<RedisCacheClient>>(),
                componentContext.Resolve<IMemoryCache>(),
                componentContext.Resolve<IHostEnvironment>().EnvironmentName
            );
        }

        private RedisConfig GetEnvironmentVariable(Setting setting)
        {
            // GetEnvironmentVariable
            var connectionStringFromEnvironmentVariable = Environment.GetEnvironmentVariable(setting.EnvironmentVariable.ConnectionString);
            var keyPrefixFromEnvironmentVariable = Environment.GetEnvironmentVariable(setting.EnvironmentVariable.KeyPrefix);
            
            // Set
            if (string.IsNullOrEmpty(connectionStringFromEnvironmentVariable) != true) setting.RedisConfig.ConnectionString = connectionStringFromEnvironmentVariable;
            if (string.IsNullOrEmpty(keyPrefixFromEnvironmentVariable) != true) setting.RedisConfig.KeyPrefix = keyPrefixFromEnvironmentVariable;

            // Return
            return setting.RedisConfig;
        }


        // Class
        public class Setting
        {
            // Properties
            public RedisConfig RedisConfig { get; set; } = new RedisConfig();

            public EnvironmentVariable EnvironmentVariable { get; set; } = new EnvironmentVariable();
        }
    }
}