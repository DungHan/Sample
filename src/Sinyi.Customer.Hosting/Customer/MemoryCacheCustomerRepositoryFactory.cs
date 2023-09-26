using Autofac;
using MDP.Hosting;
using Sinyi.Caching;
using Sinyi.Caching.Memory;
using Sinyi.Customers.Accesses;
using Sinyi.MessageQueue.MQTTnet;
using System;

namespace Sinyi.Customers.Hosting
{
    public class MemoryCacheCustomerRepositoryFactory : ServiceFactory<CustomerRepository, MemoryCacheCustomerRepository, MemoryCacheCustomerRepositoryFactory.Setting>
    {
        // Methods
        protected override MemoryCacheCustomerRepository CreateService(IComponentContext componentContext, Setting setting)
        {
            #region Contracts

            if (componentContext == null) throw new ArgumentException(nameof(componentContext));
            if (setting == null) throw new ArgumentException(nameof(setting));

            #endregion

            // ComponentContext
            componentContext = componentContext.Resolve<IComponentContext>();
            if (componentContext == null) throw new InvalidOperationException($"{nameof(componentContext)}=null");

            // Create
            return new MemoryCacheCustomerRepository
            (
                componentContext.Resolve<CustomerRepository>(setting.CustomerRepository),
                componentContext.Resolve<MemoryCacheClient>()
            );
        }


        // Class
        public class Setting
        {
            public string CustomerRepository { get; set; }
        }
    }
}
