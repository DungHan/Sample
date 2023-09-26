using Autofac;
using MDP.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using System;

namespace Sinyi.Caching.Memory.Hosting
{
    public class MemoryCacheClientFactory : ServiceFactory<MemoryCacheClient, MemoryCacheClient, MemoryCacheClientFactory.Setting>
    {
        // Constructor
        public MemoryCacheClientFactory()
        {
            // Default
             this.ServiceSingleton = true;
        }


        // Methods
        protected override MemoryCacheClient CreateService(IComponentContext componentContext, Setting setting)
        {
            #region Contracts

            if (componentContext == null) throw new ArgumentException(nameof(componentContext));
            if (setting == null) throw new ArgumentException(nameof(setting));

            #endregion

            // Create
            return new MemoryCacheClient
            (
                componentContext.Resolve<IMemoryCache>()
            );
        }


        // Class
        public class Setting
        {
            // Properties
        }
    }
}