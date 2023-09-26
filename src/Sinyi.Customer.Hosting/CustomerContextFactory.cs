using Autofac;
using MDP.Hosting;
using Microsoft.Extensions.Logging;
using Sinyi.Data.MsSqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sinyi.Customers.Hosting
{
    public class CustomerContextFactory : ServiceFactory<CustomerContext, CustomerContext, CustomerContextFactory.Setting>
    {
        // Constructor
        public CustomerContextFactory()
        {
            // Default
            this.ServiceSingleton = true;
        }


        // Methods
        protected override CustomerContext CreateService(IComponentContext componentContext, Setting setting)
        {
            #region Contracts

            if (componentContext == null) throw new ArgumentException(nameof(componentContext));
            if (setting == null) throw new ArgumentException(nameof(setting));

            #endregion

            // Create
            return new CustomerContext
            (
                componentContext.Resolve<CustomerRepository>(setting.CustomerRepository)
            );
        }


        // Class
        public class Setting
        {
            // Properties
            public string CustomerRepository { get; set; }
        }
    }
}
