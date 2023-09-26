using Autofac;
using MDP.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Hosting;
using Sinyi.Caching.Memory;
using System;

namespace Sinyi.Caching.Hosting
{
    public class MemoryKeyValueCacheFactory : ServiceFactory<MemoryKeyValueCache, MemoryKeyValueCache, MemoryKeyValueCacheFactory.Setting>
    {
        // Constructor
        public MemoryKeyValueCacheFactory()
        {
            // Default
             this.ServiceSingleton = true;
        }


        // Methods
        protected override MemoryKeyValueCache CreateService(IComponentContext componentContext, Setting setting)
        {
            #region Contracts

            if (componentContext == null) throw new ArgumentException(nameof(componentContext));
            if (setting == null) throw new ArgumentException(nameof(setting));

            #endregion

            // Create
            return new MemoryKeyValueCache
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