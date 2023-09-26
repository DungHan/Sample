using Autofac;
using MDP.Hosting;
using Microsoft.Extensions.Logging;
using Sinyi.Caching;
using Sinyi.Caching.Redis;
using Sinyi.Customers.Accesses;
using Sinyi.MessageQueue.MQTTnet;
using System;

namespace Sinyi.Customers.Hosting
{
    public class RedisCacheCustomerRepositoryFactory : ServiceFactory<CustomerRepository, RedisCacheCustomerRepository, RedisCacheCustomerRepositoryFactory.Setting>
    {
        // Methods
        protected override RedisCacheCustomerRepository CreateService(IComponentContext componentContext, Setting setting)
        {
            #region Contracts

            if (componentContext == null) throw new ArgumentException(nameof(componentContext));
            if (setting == null) throw new ArgumentException(nameof(setting));

            #endregion

            // ComponentContext
            componentContext = componentContext.Resolve<IComponentContext>();
            if (componentContext == null) throw new InvalidOperationException($"{nameof(componentContext)}=null");

            // Create
            return new RedisCacheCustomerRepository
            (
                componentContext.Resolve<CustomerRepository>(setting.CustomerRepository),
                componentContext.Resolve<RedisCacheClient>(),
                componentContext.Resolve<MQTTnetClientFactory>(),
                componentContext.Resolve<ILogger<RedisCacheCustomerRepository>>()
            );
        }


        // Class
        public class Setting
        {
            public string CustomerRepository { get; set; }
        }
    }
}
